﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Artemis.Core.Events;
using Artemis.Core.Models.Profile;
using Artemis.Core.Models.Surface;
using Artemis.Core.Plugins.Models;
using Artemis.Core.Services;
using Artemis.Core.Services.Storage.Interfaces;
using Artemis.UI.Events;
using Artemis.UI.Screens.Module.ProfileEditor.Visualization.Tools;
using Artemis.UI.Screens.Shared;
using Artemis.UI.Services.Interfaces;
using RGB.NET.Core;
using Stylet;

namespace Artemis.UI.Screens.Module.ProfileEditor.Visualization
{
    public class ProfileViewModel : ProfileEditorPanelViewModel, IHandle<MainWindowFocusChangedEvent>, IHandle<MainWindowKeyEvent>
    {
        private readonly IProfileEditorService _profileEditorService;
        private readonly ISettingsService _settingsService;
        private readonly ISurfaceService _surfaceService;
        private int _activeToolIndex;
        private VisualizationToolViewModel _activeToolViewModel;
        private int _previousTool;
        private TimerUpdateTrigger _updateTrigger;

        public ProfileViewModel(IProfileEditorService profileEditorService, ISurfaceService surfaceService, ISettingsService settingsService, IEventAggregator eventAggregator)
        {
            _profileEditorService = profileEditorService;
            _surfaceService = surfaceService;
            _settingsService = settingsService;

            Execute.OnUIThreadSync(() =>
            {
                CanvasViewModels = new ObservableCollection<CanvasViewModel>();
                PanZoomViewModel = new PanZoomViewModel();
            });

            ApplySurfaceConfiguration(surfaceService.ActiveSurface);
            ApplyActiveProfile();
            CreateUpdateTrigger();
            ActivateToolByIndex(0);

            _profileEditorService.SelectedProfileChanged += OnSelectedProfileChanged;
            _profileEditorService.SelectedProfileElementChanged += OnSelectedProfileElementChanged;
            _profileEditorService.SelectedProfileElementUpdated += OnSelectedProfileElementUpdated;
            eventAggregator.Subscribe(this);
        }


        public bool IsInitializing { get; private set; }
        public ObservableCollection<CanvasViewModel> CanvasViewModels { get; set; }
        public PanZoomViewModel PanZoomViewModel { get; set; }
        public PluginSetting<bool> HighlightSelectedLayer { get; set; }
        public PluginSetting<bool> PauseRenderingOnFocusLoss { get; set; }

        public ReadOnlyCollection<ProfileDeviceViewModel> Devices => CanvasViewModels
            .Where(vm => vm is ProfileDeviceViewModel)
            .Cast<ProfileDeviceViewModel>()
            .ToList()
            .AsReadOnly();

        public VisualizationToolViewModel ActiveToolViewModel
        {
            get => _activeToolViewModel;
            set
            {
                // Remove the tool from the canvas
                if (_activeToolViewModel != null)
                {
                    CanvasViewModels.Remove(_activeToolViewModel);
                    NotifyOfPropertyChange(() => CanvasViewModels);
                }

                // Set the new tool
                _activeToolViewModel = value;
                // Add the new tool to the canvas
                if (_activeToolViewModel != null)
                {
                    CanvasViewModels.Add(_activeToolViewModel);
                    NotifyOfPropertyChange(() => CanvasViewModels);
                }
            }
        }

        public int ActiveToolIndex
        {
            get => _activeToolIndex;
            set
            {
                if (_activeToolIndex != value)
                {
                    _activeToolIndex = value;
                    ActivateToolByIndex(value);
                    NotifyOfPropertyChange(() => ActiveToolIndex);
                }
            }
        }

        private void CreateUpdateTrigger()
        {
            // Borrow RGB.NET's update trigger but limit the FPS
            var targetFpsSetting = _settingsService.GetSetting("Core.TargetFrameRate", 25);
            var editorTargetFpsSetting = _settingsService.GetSetting("ProfileEditor.TargetFrameRate", 15);
            var targetFps = Math.Min(targetFpsSetting.Value, editorTargetFpsSetting.Value);
            _updateTrigger = new TimerUpdateTrigger {UpdateFrequency = 1.0 / targetFps};
            _updateTrigger.Update += UpdateLeds;

            _surfaceService.ActiveSurfaceConfigurationChanged += OnActiveSurfaceConfigurationChanged;
        }

        private void OnActiveSurfaceConfigurationChanged(object sender, SurfaceConfigurationEventArgs e)
        {
            ApplySurfaceConfiguration(e.Surface);
        }

        private void ApplyActiveProfile()
        {
            Execute.PostToUIThread(() =>
            {
                lock (CanvasViewModels)
                {
                    var layerViewModels = CanvasViewModels.Where(vm => vm is ProfileLayerViewModel).Cast<ProfileLayerViewModel>().ToList();
                    var layers = _profileEditorService.SelectedProfile?.GetAllLayers() ?? new List<Layer>();


                    // Add new layers missing a VM
                    foreach (var layer in layers)
                    {
                        if (layerViewModels.All(vm => vm.Layer != layer))
                            CanvasViewModels.Add(new ProfileLayerViewModel(layer));
                    }

                    // Remove layers that no longer exist
                    var toRemove = layerViewModels.Where(vm => !layers.Contains(vm.Layer));
                    foreach (var profileLayerViewModel in toRemove)
                    {
                        profileLayerViewModel.Dispose();
                        CanvasViewModels.Remove(profileLayerViewModel);
                    }
                }
            });
        }

        private void ApplySurfaceConfiguration(ArtemisSurface surface)
        {
            var devices = new List<ArtemisDevice>();
            devices.AddRange(surface.Devices);

            // Make sure all devices have an up-to-date VM
            foreach (var surfaceDeviceConfiguration in devices)
            {
                // Create VMs for missing devices
                var viewModel = Devices.FirstOrDefault(vm => vm.Device.RgbDevice == surfaceDeviceConfiguration.RgbDevice);
                if (viewModel == null)
                {
                    // Create outside the UI thread to avoid slowdowns as much as possible
                    var profileDeviceViewModel = new ProfileDeviceViewModel(surfaceDeviceConfiguration);
                    Execute.PostToUIThread(() =>
                    {
                        // Gotta call IsInitializing on the UI thread or its never gets picked up
                        IsInitializing = true;
                        lock (CanvasViewModels)
                        {
                            CanvasViewModels.Add(profileDeviceViewModel);
                        }
                    });
                }
                // Update existing devices
                else
                    viewModel.Device = surfaceDeviceConfiguration;
            }


            // Sort the devices by ZIndex
            Execute.PostToUIThread(() =>
            {
                lock (CanvasViewModels)
                {
                    foreach (var device in Devices.OrderBy(d => d.ZIndex).ToList())
                    {
                        var newIndex = Math.Max(device.ZIndex - 1, CanvasViewModels.Count - 1);
                        CanvasViewModels.Move(CanvasViewModels.IndexOf(device), newIndex);
                    }
                }
            });
        }

        private void UpdateLeds(object sender, CustomUpdateData customUpdateData)
        {
            lock (CanvasViewModels)
            {
                if (IsInitializing)
                    IsInitializing = Devices.Any(d => !d.AddedLeds);

                foreach (var profileDeviceViewModel in Devices)
                    profileDeviceViewModel.Update();
            }
        }

        private void UpdateLedsDimStatus()
        {
            if (HighlightSelectedLayer.Value && _profileEditorService.SelectedProfileElement is Layer layer)
            {
                foreach (var led in Devices.SelectMany(d => d.Leds))
                    led.IsDimmed = !layer.Leds.Contains(led.Led);
            }
            else
            {
                foreach (var led in Devices.SelectMany(d => d.Leds))
                    led.IsDimmed = false;
            }
        }

        protected override void OnActivate()
        {
            HighlightSelectedLayer = _settingsService.GetSetting("ProfileEditor.HighlightSelectedLayer", true);
            PauseRenderingOnFocusLoss = _settingsService.GetSetting("ProfileEditor.PauseRenderingOnFocusLoss", true);

            HighlightSelectedLayer.SettingChanged += HighlightSelectedLayerOnSettingChanged;

            _updateTrigger.Start();
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            HighlightSelectedLayer.Save();
            PauseRenderingOnFocusLoss.Save();

            try
            {
                _updateTrigger.Stop();
            }
            catch (NullReferenceException)
            {
                // TODO: Remove when fixed in RGB.NET, or avoid double stopping
            }

            base.OnDeactivate();
        }

        #region Buttons

        private void ActivateToolByIndex(int value)
        {
            switch (value)
            {
                case 0:
                    ActiveToolViewModel = new ViewpointMoveToolViewModel(this, _profileEditorService);
                    break;
                case 1:
                    ActiveToolViewModel = new SelectionToolViewModel(this, _profileEditorService);
                    break;
                case 2:
                    ActiveToolViewModel = new SelectionAddToolViewModel(this, _profileEditorService);
                    break;
                case 3:
                    ActiveToolViewModel = new SelectionRemoveToolViewModel(this, _profileEditorService);
                    break;
                case 5:
                    ActiveToolViewModel = new EllipseToolViewModel(this, _profileEditorService);
                    break;
                case 6:
                    ActiveToolViewModel = new RectangleToolViewModel(this, _profileEditorService);
                    break;
                case 7:
                    ActiveToolViewModel = new PolygonToolViewModel(this, _profileEditorService);
                    break;
                case 8:
                    ActiveToolViewModel = new FillToolViewModel(this, _profileEditorService);
                    break;
            }

            ActiveToolIndex = value;
        }

        public void ResetZoomAndPan()
        {
            PanZoomViewModel.Reset();
        }

        #endregion

        #region Mouse

        public void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            ActiveToolViewModel?.MouseDown(sender, e);
        }

        public void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            ActiveToolViewModel?.MouseUp(sender, e);
        }

        public void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            ActiveToolViewModel?.MouseMove(sender, e);
        }

        public void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PanZoomViewModel.ProcessMouseScroll(sender, e);
            ActiveToolViewModel?.MouseWheel(sender, e);
        }

        #endregion

        #region Context menu actions

        public bool CanApplyToLayer { get; set; }

        public void CreateLayer()
        {
        }

        public void ApplyToLayer()
        {
            if (!(_profileEditorService.SelectedProfileElement is Layer layer))
                return;

            layer.ClearLeds();
            layer.AddLeds(Devices.SelectMany(d => d.Leds).Where(vm => vm.IsSelected).Select(vm => vm.Led));

            _profileEditorService.UpdateSelectedProfileElement();
        }

        public void SelectAll()
        {
            foreach (var ledVm in Devices.SelectMany(d => d.Leds))
                ledVm.IsSelected = true;
        }

        public void InverseSelection()
        {
            foreach (var ledVm in Devices.SelectMany(d => d.Leds))
                ledVm.IsSelected = !ledVm.IsSelected;
        }

        public void ClearSelection()
        {
            foreach (var ledVm in Devices.SelectMany(d => d.Leds))
                ledVm.IsSelected = false;
        }

        #endregion

        #region Event handlers

        private void HighlightSelectedLayerOnSettingChanged(object sender, EventArgs e)
        {
            UpdateLedsDimStatus();
        }

        private void OnSelectedProfileChanged(object sender, EventArgs e)
        {
            ApplyActiveProfile();
        }

        private void OnSelectedProfileElementChanged(object sender, EventArgs e)
        {
            UpdateLedsDimStatus();
            CanApplyToLayer = _profileEditorService.SelectedProfileElement is Layer;
        }

        private void OnSelectedProfileElementUpdated(object sender, EventArgs e)
        {
            ApplyActiveProfile();
            UpdateLedsDimStatus();
            CanApplyToLayer = _profileEditorService.SelectedProfileElement is Layer;
        }

        public void Handle(MainWindowFocusChangedEvent message)
        {
            if (PauseRenderingOnFocusLoss == null || ScreenState != ScreenState.Active)
                return;

            try
            {
                if (PauseRenderingOnFocusLoss.Value && !message.IsFocused)
                    _updateTrigger.Stop();
                else if (PauseRenderingOnFocusLoss.Value && message.IsFocused)
                    _updateTrigger.Start();
            }
            catch (NullReferenceException)
            {
                // TODO: Remove when fixed in RGB.NET, or avoid double stopping
            }
        }

        public void Handle(MainWindowKeyEvent message)
        {
            Debug.WriteLine(message.KeyDown);
            if (message.KeyDown)
            {
                if (ActiveToolIndex != 0)
                {
                    _previousTool = ActiveToolIndex;
                    if ((message.EventArgs.Key == Key.LeftCtrl || message.EventArgs.Key == Key.RightCtrl) && message.EventArgs.IsDown)
                        ActivateToolByIndex(0);
                }

                ActiveToolViewModel?.KeyDown(message.EventArgs);
            }
            else
            {
                if ((message.EventArgs.Key == Key.LeftCtrl || message.EventArgs.Key == Key.RightCtrl) && message.EventArgs.IsUp)
                    ActivateToolByIndex(_previousTool);

                ActiveToolViewModel?.KeyUp(message.EventArgs);
            }
        }

        #endregion
    }
}
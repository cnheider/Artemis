﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Artemis.Core.Models.Profile;
using Artemis.UI.Services;
using Artemis.UI.Services.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Stylet;

namespace Artemis.UI.Screens.Module.ProfileEditor.Visualization.Tools
{
    public class EditToolViewModel : VisualizationToolViewModel
    {
        private readonly ILayerEditorService _layerEditorService;
        private bool _draggingHorizontally;
        private bool _draggingVertically;
        private double _dragOffsetX;
        private double _dragOffsetY;
        private Point _dragStart;
        private bool _isDragging;

        public EditToolViewModel(ProfileViewModel profileViewModel, IProfileEditorService profileEditorService, ILayerEditorService layerEditorService)
            : base(profileViewModel, profileEditorService)
        {
            _layerEditorService = layerEditorService;
            Cursor = Cursors.Arrow;
            Update();

            profileEditorService.SelectedProfileChanged += (sender, args) => Update();
            profileEditorService.SelectedProfileElementUpdated += (sender, args) => Update();
            profileEditorService.ProfilePreviewUpdated += (sender, args) => Update();
        }

        public SKRect ShapeRectangle { get; set; }
        public SKPoint ShapeAnchor { get; set; }
        public RectangleGeometry ShapeGeometry { get; set; }
        public TransformCollection ShapeTransformCollection { get; set; }

        private void Update()
        {
            if (ProfileEditorService.SelectedProfileElement is Layer layer)
            {
                if (layer.LayerShape != null)
                {
                    ShapeRectangle = _layerEditorService.GetShapeRenderRect(layer.LayerShape).ToSKRect();
                    ShapeAnchor = _layerEditorService.GetLayerAnchor(layer, true);

                    Execute.PostToUIThread(() =>
                    {
                        var shapeGeometry = new RectangleGeometry(_layerEditorService.GetShapeRenderRect(layer.LayerShape));
                        shapeGeometry.Transform = _layerEditorService.GetLayerTransformGroup(layer);
                        shapeGeometry.Freeze();
                        ShapeGeometry = shapeGeometry;
                        ShapeTransformCollection = _layerEditorService.GetLayerTransformGroup(layer).Children;

                        // Get a square path to use for mutation point placement
                        var path = _layerEditorService.GetLayerPath(layer);
                        TopLeft = path.Points[0];
                        TopRight = path.Points[1];
                        BottomRight = path.Points[2];
                        BottomLeft = path.Points[3];

                        TopCenter = new SKPoint((TopLeft.X + TopRight.X) / 2, (TopLeft.Y + TopRight.Y) / 2);
                        RightCenter = new SKPoint((TopRight.X + BottomRight.X) / 2, (TopRight.Y + BottomRight.Y) / 2);
                        BottomCenter = new SKPoint((BottomLeft.X + BottomRight.X) / 2, (BottomLeft.Y + BottomRight.Y) / 2);
                        LeftCenter = new SKPoint((TopLeft.X + BottomLeft.X) / 2, (TopLeft.Y + BottomLeft.Y) / 2);
                    });
                }
            }
        }

        public SKPoint TopLeft { get; set; }
        public SKPoint TopRight { get; set; }
        public SKPoint BottomRight { get; set; }
        public SKPoint BottomLeft { get; set; }
        public SKPoint TopCenter { get; set; }
        public SKPoint RightCenter { get; set; }
        public SKPoint BottomCenter { get; set; }
        public SKPoint LeftCenter { get; set; }

        public void ShapeEditMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
                return;

            ((IInputElement) sender).CaptureMouse();
            if (ProfileEditorService.SelectedProfileElement is Layer layer)
            {
                var dragStartPosition = GetRelativePosition(sender, e);
                var skRect = layer.LayerShape.RenderRectangle;
                _dragOffsetX = skRect.Left - dragStartPosition.X;
                _dragOffsetY = skRect.Top - dragStartPosition.Y;
                _dragStart = dragStartPosition;
            }

            _isDragging = true;
            _draggingHorizontally = false;
            _draggingVertically = false;

            e.Handled = true;
        }

        public void AnchorEditMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
                return;

            // Use the regular mousedown
            ShapeEditMouseDown(sender, e);
            // Override the drag offset
            var dragStartPosition = GetRelativePosition(sender, e);
//            _dragOffsetX = AnchorSkPoint.X - dragStartPosition.X;
//            _dragOffsetY = AnchorSkPoint.Y - dragStartPosition.Y;
            _dragStart = dragStartPosition;
        }

        public void ShapeEditMouseUp(object sender, MouseButtonEventArgs e)
        {
            ProfileEditorService.UpdateSelectedProfileElement();

            _isDragging = false;
            _draggingHorizontally = false;
            _draggingVertically = false;

            ((IInputElement) sender).ReleaseMouseCapture();
            e.Handled = true;
        }

        public void AnchorMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var x = (float) (position.X + _dragOffsetX);
            var y = (float) (position.Y + _dragOffsetY);
            layer.LayerShape.SetFromUnscaledAnchor(new SKPoint(x, y), ProfileEditorService.CurrentTime);
            ProfileEditorService.UpdateProfilePreview();
        }

        public void Move(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var skRect = _layerEditorService.GetShapeRenderRect(layer.LayerShape).ToSKRect();
            var x = (float) (position.X + _dragOffsetX);
            var y = (float) (position.Y + _dragOffsetY);

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (_draggingVertically)
                    x = skRect.Left;
                else if (_draggingHorizontally)
                    y = skRect.Top;
                else
                {
                    _draggingHorizontally = Math.Abs(position.X - _dragStart.X) > Math.Abs(position.Y - _dragStart.Y);
                    _draggingVertically = Math.Abs(position.X - _dragStart.X) < Math.Abs(position.Y - _dragStart.Y);
                    return;
                }
            }

            // Convert the desired X and Y position to a translation
            var layerRect = _layerEditorService.GetLayerRenderRect(layer).ToSKRect();
            var translation = layer.PositionProperty.CurrentValue;
            var scaled = _layerEditorService.GetScaledPoint(layer, new SKPoint(x , y));
            Console.WriteLine(scaled);
            layer.PositionProperty.SetCurrentValue(scaled, ProfileEditorService.CurrentTime);

            ProfileEditorService.UpdateProfilePreview();
        }

        public void TopLeftRotate(object sender, MouseEventArgs e)
        {
        }

        public void TopLeftResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var skRect = layer.LayerShape.RenderRectangle;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // Take the greatest difference
                // Base the smallest difference on the greatest difference, maintaining aspect ratio
            }
            else
            {
                skRect.Top = (float) Math.Min(position.Y, skRect.Bottom);
                skRect.Left = (float) Math.Min(position.X, skRect.Bottom);
            }

            ApplyShapeResize(skRect);
        }

        public void TopCenterResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);

            var skRect = layer.LayerShape.RenderRectangle;
            skRect.Top = (float) Math.Min(position.Y, skRect.Bottom);
            ApplyShapeResize(skRect);
        }

        public void TopRightRotate(object sender, MouseEventArgs e)
        {
        }

        public void TopRightResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var skRect = layer.LayerShape.RenderRectangle;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // Take the greatest difference
                // Base the smallest difference on the greatest difference, maintaining aspect ratio
            }
            else
            {
                skRect.Top = (float) Math.Min(position.Y, skRect.Bottom);
                skRect.Right = (float) Math.Max(position.X, skRect.Left);
            }

            ApplyShapeResize(skRect);
        }

        public void CenterRightResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);

            var skRect = layer.LayerShape.RenderRectangle;
            skRect.Right = (float) Math.Max(position.X, skRect.Left);
            ApplyShapeResize(skRect);
        }

        public void BottomRightRotate(object sender, MouseEventArgs e)
        {
        }

        public void BottomRightResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var skRect = layer.LayerShape.RenderRectangle;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // Take the greatest difference
                // Base the smallest difference on the greatest difference, maintaining aspect ratio
            }
            else
            {
                skRect.Bottom = (float) Math.Max(position.Y, skRect.Top);
                skRect.Right = (float) Math.Max(position.X, skRect.Left);
            }

            ApplyShapeResize(skRect);
        }

        public void BottomCenterResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);

            var skRect = layer.LayerShape.RenderRectangle;
            skRect.Bottom = (float) Math.Max(position.Y, skRect.Top);
            ApplyShapeResize(skRect);
        }

        public void BottomLeftRotate(object sender, MouseEventArgs e)
        {
        }

        public void BottomLeftResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);
            var skRect = layer.LayerShape.RenderRectangle;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // Take the greatest difference
                // Base the smallest difference on the greatest difference, maintaining aspect ratio
            }
            else
            {
                skRect.Bottom = (float) Math.Max(position.Y, skRect.Top);
                skRect.Left = (float) Math.Min(position.X, skRect.Right);
            }

            ApplyShapeResize(skRect);
        }

        public void CenterLeftResize(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            var position = GetRelativePosition(sender, e);

            var skRect = layer.LayerShape.RenderRectangle;
            skRect.Left = (float) Math.Min(position.X, skRect.Right);
            ApplyShapeResize(skRect);
        }

        private Point GetRelativePosition(object sender, MouseEventArgs mouseEventArgs)
        {
            var parent = VisualTreeHelper.GetParent((DependencyObject) sender);
            return mouseEventArgs.GetPosition((IInputElement) parent);
        }

        private void ApplyShapeResize(SKRect newRect)
        {
            if (!(ProfileEditorService.SelectedProfileElement is Layer layer))
                return;

            // TODO: Apply the translation
            // Store the original position to create an offset for the anchor
            // var original = layer.PositionProperty.CurrentValue;
            // layer.LayerShape.SetFromUnscaledRectangle(newRect, ProfileEditorService.CurrentTime);
            // var updated = layer.PositionProperty.CurrentValue;
            // // Apply the offset to the anchor so it stays in at same spot
            // layer.AnchorPointProperty.SetCurrentValue(new SKPoint(
            //     layer.AnchorPointProperty.CurrentValue.X + (original.X - updated.X),
            //     layer.AnchorPointProperty.CurrentValue.Y + (original.Y - updated.Y)
            // ), ProfileEditorService.CurrentTime);

            // Update the preview
            ProfileEditorService.UpdateProfilePreview();
        }
    }
}
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Games.R3E.API;
using Artemis.Plugins.Modules.Games.R3E.API.Data;
using SkiaSharp;
using Artemis.Plugins.Modules.Games.R3E.DataModels;

namespace Artemis.Plugins.Modules.Games.R3E
{
    public class R3EModule : ProfileModule<R3EDataModel>
    {
        #region Properties & Fields

        private bool IsMapped => _file != null;
        private MemoryMappedFile _file;
        private byte[] _buffer;

        #endregion

        #region Methods

        public override void EnablePlugin()
        {
            DisplayName = "Race Room Racing Experience";
            DisplayIcon = "CarSports";
            DefaultPriorityCategory = ModulePriorityCategory.Application;

            ActivationRequirementMode = ActivationRequirementType.Any;
            ActivationRequirements.Add(new ProcessActivationRequirement("RRRE"));
            ActivationRequirements.Add(new ProcessActivationRequirement("RRRE64"));
        }

        public override void DisablePlugin()
        {
            _file?.Dispose();
            _file = null;
        }

        public override void ModuleActivated(bool isOverride)
        { }

        public override void ModuleDeactivated(bool isOverride)
        { }

        public override void Update(double deltaTime)
        {
            if (IsActivatedOverride) return;

            try
            {
                if (!IsMapped)
                    if (Map())
                        _buffer = new byte[Marshal.SizeOf(typeof(Shared))];

                if (IsMapped)
                {
                    Shared data = Read();
                    DataModel.Data = data;
                }
            }
            catch
            {
                _file?.Dispose();
                _file = null;
            }
        }

        private Shared Read()
        {
            MemoryMappedViewStream view = _file.CreateViewStream();
            BinaryReader stream = new BinaryReader(view);
            _buffer = stream.ReadBytes(Marshal.SizeOf(typeof(Shared)));
            GCHandle handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            Shared data = (Shared)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Shared));
            handle.Free();

            return data;
        }

        private bool Map()
        {
            try
            {
                _file = MemoryMappedFile.OpenExisting(Constant.SharedMemoryName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public override void Render(double deltaTime, ArtemisSurface surface, SKCanvas canvas, SKImageInfo canvasInfo)
        { }

        #endregion
    }
}
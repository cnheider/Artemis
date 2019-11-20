﻿using System;
using System.Drawing;
using Artemis.Core.Models.Profile.Abstract;
using Artemis.Core.Plugins.Abstract;
using Artemis.Core.Plugins.Interfaces;
using Artemis.Core.Services.Interfaces;
using Artemis.Storage.Entities.Profile;

namespace Artemis.Core.Models.Profile
{
    public sealed class Layer : ProfileElement
    {
        internal Layer(Profile profile, Folder folder, string name)
        {
            LayerEntity = new LayerEntity();
            EntityId = Guid.NewGuid();

            Profile = profile;
            ParentFolder = folder;
            Name = name;
        }

        internal Layer(Profile profile, Folder folder, LayerEntity layerEntity, IPluginService pluginService)
        {
            LayerEntity = layerEntity;
            EntityId = layerEntity.Id;

            Profile = profile;
            ParentFolder = folder;
            LayerType = pluginService.GetLayerTypeByGuid(layerEntity.LayerTypeGuid);
        }

        internal LayerEntity LayerEntity { get; set; }
        internal Guid EntityId { get; set; }

        public Profile Profile { get; }
        public Folder ParentFolder { get; }

        public LayerType LayerType { get; private set; }
        public ILayerTypeConfiguration LayerTypeConfiguration { get; set; }

        public override void Update(double deltaTime)
        {
            if (LayerType == null)
                return;

            lock (LayerType)
            {
                LayerType.Update(this);
            }
        }

        public override void Render(double deltaTime, Surface.Surface surface, Graphics graphics)
        {
            if (LayerType == null)
                return;

            lock (LayerType)
            {
                LayerType.Render(this, surface, graphics);
            }
        }

        internal override void ApplyToEntity()
        {
            LayerEntity.Id = EntityId;
            LayerEntity.ParentId = ParentFolder?.EntityId ?? new Guid();
            LayerEntity.LayerTypeGuid = LayerType?.PluginInfo.Guid ?? new Guid();

            LayerEntity.Order = Order;
            LayerEntity.Name = Name;

            LayerEntity.ProfileId = Profile.EntityId;

            // TODO: LEDs, conditions, elements
        }

        public void UpdateLayerType(LayerType layerType)
        {
            if (LayerType != null)
            {
                lock (LayerType)
                {
                    LayerType.Dispose();
                }
            }

            LayerType = layerType;
        }

        public override string ToString()
        {
            return $"{nameof(Profile)}: {Profile}, {nameof(Order)}: {Order}, {nameof(Name)}: {Name}";
        }
    }
}
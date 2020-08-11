﻿using Artemis.Core.Models.Profile;
using Artemis.Core.Models.Profile.LayerProperties.Attributes;
using Artemis.Core.Models.Profile.LayerProperties.Types;
using Artemis.Core.Plugins.LayerEffect.Abstract;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class OpacityEffect : LayerEffect<OpacityEffectProperties>
    {
        public override void EnableLayerEffect()
        {
        }

        public override void DisableLayerEffect()
        {
        }

        public override void Update(double deltaTime)
        {
        }

        public override void PreProcess(SKCanvas canvas, SKImageInfo canvasInfo, SKPath renderBounds, SKPaint paint)
        {
            paint.Color = paint.Color.WithAlpha((byte) (Properties.Opacity.CurrentValue * 2.55f));
        }

        public override void PostProcess(SKCanvas canvas, SKImageInfo canvasInfo, SKPath renderBounds, SKPaint paint)
        {
        }
    }

    public class OpacityEffectProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The opacity of the shape", InputAffix = "%", MinInputValue = 0f, MaxInputValue = 100f)]
        public FloatLayerProperty Opacity { get; set; }

        protected override void PopulateDefaults()
        {
            Opacity.DefaultValue = 100f;
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }
}
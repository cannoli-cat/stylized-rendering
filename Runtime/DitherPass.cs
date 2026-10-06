using System;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace CannoliCat.Stylized {
    public class DitherPass : ScreenEffectPass<DitherSettings> {
        private const string UpscaleTextureName = "_DitherUpscaleTexture";

        public DitherPass(Material material, DitherSettings settings, ScriptableRenderPassInput requiredInputs) : base(material, settings, requiredInputs) { }

        protected override TextureHandle RenderEffect(RenderGraph renderGraph, TextureHandle source, TextureDesc desc) {
            var small = desc;
            var current = source;

            for (var i = 0; i < settings.DownSamples; i++) {
                small.width = Math.Max(1, small.width / 2);
                small.height = Math.Max(1, small.height / 2);

                var stepDesc = small;
                stepDesc.name = "_DitherDownsampleTexture";
                var stepTexture = renderGraph.CreateTexture(stepDesc);

                renderGraph.AddBlitPass(current, stepTexture, Vector2.one, Vector2.zero,
                    filterMode: RenderGraphUtils.BlitFilterMode.ClampBilinear,
                    passName: "DitherDownsample");

                current = stepTexture;
            }

            var dst = base.RenderEffect(renderGraph, current, small);
            if (small.width == desc.width && small.height == desc.height) return dst;

            var upscaleDesc = desc;
            upscaleDesc.name = UpscaleTextureName;
            var upscaled = renderGraph.CreateTexture(upscaleDesc);

            renderGraph.AddBlitPass(dst, upscaled, Vector2.one, Vector2.zero,
                filterMode: RenderGraphUtils.BlitFilterMode.ClampNearest,
                passName: "DitherUpscale");

            return upscaled;
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace CannoliCat.Stylized {
    public class ScreenEffectPass<TSettings> : ScriptableRenderPass where TSettings : IScreenEffectSettings {
        protected readonly Material material;
        protected readonly TSettings settings;
        
        private readonly string textureName = "_" + typeof(TSettings).Name.Replace("Settings", "Texture");
        
        public ScreenEffectPass(Material material, TSettings settings, ScriptableRenderPassInput requiredInputs) {
            this.material = material;
            this.settings = settings;
            profilingSampler = new ProfilingSampler(typeof(TSettings).Name.Replace("Settings", "Pass"));
            
            requiresIntermediateTexture = true;
            ConfigureInput(requiredInputs);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
            if (material == null) return;

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer || cameraData.renderType == CameraRenderType.Overlay) return;

            var srcCamColor = resourceData.activeColorTexture;
            if (!srcCamColor.IsValid()) return;

            var desc = renderGraph.GetTextureDesc(srcCamColor);
            desc.name = textureName;
            desc.clearBuffer = false;
            desc.depthBufferBits = DepthBits.None;
            desc.msaaSamples = MSAASamples.None;
            desc.filterMode = FilterMode.Point;

            settings.Apply(material);

            var dst = RenderEffect(renderGraph, srcCamColor, desc);
            if (!dst.IsValid()) return;

            resourceData.cameraColor = dst;
        }

        protected virtual TextureHandle RenderEffect(RenderGraph renderGraph, TextureHandle source, TextureDesc desc) {
            var dst = renderGraph.CreateTexture(desc);
            renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, dst, material, 0), passName);
            return dst;
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CannoliCat.Stylized {
    public abstract class ScreenEffectFeature<TSettings> : ScriptableRendererFeature where TSettings : IScreenEffectSettings {
        [SerializeField] private TSettings settings;
        [SerializeField] private Shader shader;
        [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
        private Material material;
        private ScreenEffectPass<TSettings> pass;
        
        protected virtual ScriptableRenderPassInput RequiredInputs => ScriptableRenderPassInput.None;

        public override void Create() {
            if (material) {
                CoreUtils.Destroy(material);
                material = null;
            }
            
            pass = null;
            if (shader == null) return;

            material = CoreUtils.CreateEngineMaterial(shader);
            pass = CreatePass(material, settings);
            pass.renderPassEvent = renderPassEvent;
        }

        protected virtual ScreenEffectPass<TSettings> CreatePass(Material material, TSettings settings) {
            return new ScreenEffectPass<TSettings>(material, settings, RequiredInputs);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (pass == null || material == null) return;

            if (renderingData.cameraData.cameraType == CameraType.Game) {
                renderer.EnqueuePass(pass);
            }
        }
        
        protected override void Dispose(bool disposing) {
            pass = null;
            CoreUtils.Destroy(material);
            material = null;
        }
    }
}
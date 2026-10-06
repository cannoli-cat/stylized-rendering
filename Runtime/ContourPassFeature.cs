using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CannoliCat.Stylized {
    public class ContourPassFeature : ScreenEffectFeature<ContourSettings> {
        protected override ScriptableRenderPassInput RequiredInputs => ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal;
    }
    
    [Serializable]
    public class ContourSettings : IScreenEffectSettings {
        private static readonly int Threshold = Shader.PropertyToID("_Threshold");
        private static readonly int Thickness = Shader.PropertyToID("_Thickness");
        private static readonly int NormalSensitivity = Shader.PropertyToID("_NormalSensitivity");
        private static readonly int DepthSensitivity = Shader.PropertyToID("_DepthSensitivity");
        private static readonly int PaperColor = Shader.PropertyToID("_PaperColor");
        private static readonly int LineColor = Shader.PropertyToID("_LineColor");
        private static readonly int PaperMix = Shader.PropertyToID("_PaperMix");
        private static readonly int ColorSensitivity = Shader.PropertyToID("_ColorSensitivity");
        private static readonly int ContourWidth = Shader.PropertyToID("_ContourWidth");
        private static readonly int BandCount = Shader.PropertyToID("_BandCount");
        private static readonly int WobbleAmount = Shader.PropertyToID("_WobbleAmount");
        private static readonly int WobbleFrequency = Shader.PropertyToID("_WobbleFrequency");
        private static readonly int WidthVariation = Shader.PropertyToID("_WidthVariation");
        private static readonly int WidthFrequency = Shader.PropertyToID("_WidthFrequency");
        private static readonly int BoilRate = Shader.PropertyToID("_BoilRate");
        private static readonly int PaperGrain = Shader.PropertyToID("_PaperGrain");
        
        [SerializeField] private Color paperColor;
        [SerializeField] private Color lineColor;
        [SerializeField] [Range(0f, 20f)] private float depthSensitivity = 2f;
        [SerializeField] [Range(0f, 20f)] private float normalSensitivity = 3.5f;
        [SerializeField] [Range(0.5f, 4f)] private float thickness = 1f;
        [SerializeField] [Range(0f, 1f)] private float threshold = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float paperMix = 0.5f;
        [SerializeField] [Range(0f, 20f)] private float colorSensitivity = 3f;
        [SerializeField] [Range(1f, 30f)] private float bandCount = 8f;
        [SerializeField] [Range(0.5f, 4f)] private float contourWidth = 1f;
        [SerializeField] [Range(0f, 8f)] private float wobbleAmount = 2f;
        [SerializeField] [Range(1f, 60f)] private float wobbleFrequency = 15f;
        [SerializeField] [Range(0f, 1f)] private float widthVariation = 0.5f;
        [SerializeField] [Range(1f, 40f)] private float widthFrequency = 8f;
        [SerializeField] [Range(0f, 24f)] private float boilRate = 8f;
        [SerializeField] [Range(0f, 1f)] private float paperGrain = 0.3f;
        [SerializeField] [Tooltip("Swap paper and line colors (light lines on dark paper)")] private bool invert;
        
        public void Apply(Material material) {
            material.SetFloat(DepthSensitivity, depthSensitivity);
            material.SetFloat(NormalSensitivity, normalSensitivity);
            material.SetFloat(Thickness, thickness);
            material.SetFloat(Threshold, threshold);
            material.SetColor(PaperColor, invert ? lineColor : paperColor);
            material.SetColor(LineColor, invert ? paperColor : lineColor);
            material.SetFloat(PaperMix, paperMix);
            material.SetFloat(ColorSensitivity, colorSensitivity);
            material.SetFloat(BandCount, bandCount);
            material.SetFloat(ContourWidth, contourWidth);
            material.SetFloat(WobbleAmount, wobbleAmount);
            material.SetFloat(WobbleFrequency, wobbleFrequency);
            material.SetFloat(WidthVariation, widthVariation);
            material.SetFloat(WidthFrequency, widthFrequency);
            material.SetFloat(BoilRate, boilRate);
            material.SetFloat(PaperGrain, paperGrain);
        }
    }
}
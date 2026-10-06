using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CannoliCat.Stylized {
    public class DitherPassFeature : ScreenEffectFeature<DitherSettings> {
        protected override ScreenEffectPass<DitherSettings> CreatePass(Material material, DitherSettings settings) {
            return new DitherPass(material, settings, RequiredInputs);
        }
    }
    
    [Serializable]
    public class DitherSettings : IScreenEffectSettings {
        private static readonly int Spread = Shader.PropertyToID("_Spread");
        private static readonly int RedColorCount = Shader.PropertyToID("_RedColorCount");
        private static readonly int GreenColorCount = Shader.PropertyToID("_GreenColorCount");
        private static readonly int BlueColorCount = Shader.PropertyToID("_BlueColorCount");
        private static readonly int BayerLevel = Shader.PropertyToID("_BayerLevel");
        private static readonly int QuantizationID = Shader.PropertyToID("_DitherQuantization");
        private static readonly int AnchorID = Shader.PropertyToID("_DitherAnchor");
        private static readonly int PatternID = Shader.PropertyToID("_DitherPattern");
        private static readonly int BlueNoiseTex = Shader.PropertyToID("_BlueNoiseTex");

        [SerializeField] [Range(0.0f, 1.0f)] private float spread = 0.5f;
        [SerializeField] [Range(2, 64)] private int redColorCount = 2;
        [SerializeField] [Range(2, 64)] private int greenColorCount = 2;
        [SerializeField] [Range(2, 64)] private int blueColorCount = 2;
        [SerializeField] [Range(0, 3)] private int bayerLevel = 0;
        [SerializeField] [Range(0, 8)] private int downSamples = 0;
        [SerializeField] private Texture2D blueNoiseTexture;
        [SerializeField] private DitherQuantization ditherQuantization = DitherQuantization.RgbLevels;
        [SerializeField] private DitherAnchor ditherAnchor = DitherAnchor.Screen;
        [SerializeField] private DitherPattern ditherPattern = DitherPattern.Bayer;
        
        public int DownSamples => downSamples;
        
        public void Apply(Material material) {
            material.SetFloat(Spread, spread);
            material.SetInteger(RedColorCount, redColorCount);
            material.SetInteger(GreenColorCount, greenColorCount);
            material.SetInteger(BlueColorCount, blueColorCount);
            material.SetInteger(BayerLevel, bayerLevel);
            material.SetInteger(QuantizationID, (int)ditherQuantization);
            material.SetInteger(AnchorID, (int)ditherAnchor);
            
            var pattern = ditherPattern == DitherPattern.BlueNoise && blueNoiseTexture == null
                ? DitherPattern.Bayer
                : ditherPattern;
            
            material.SetInteger(PatternID, (int)pattern);
            
            if (blueNoiseTexture != null) 
                material.SetTexture(BlueNoiseTex, blueNoiseTexture);
        }
    }

    public enum DitherQuantization {
        RgbLevels,
        None
    }

    public enum DitherAnchor {
        Screen,
        ViewDirection
    }

    public enum DitherPattern {
        Bayer,
        BlueNoise
    }
}
using UnityEngine;

namespace CannoliCat.Stylized {
    public class PaletteSwapperPassFeature : ScreenEffectFeature<PaletteSwapperSettings> { }

    [System.Serializable]
    public class PaletteSwapperSettings : IScreenEffectSettings {
        private static readonly int Mode = Shader.PropertyToID("_PaletteMode");
        private static readonly int Invert = Shader.PropertyToID("_Invert");
        private static readonly int LightnessWeight = Shader.PropertyToID("_LightnessWeight");
        private static readonly int PaletteCount = Shader.PropertyToID("_PaletteCount");
        private static readonly int PaletteLab = Shader.PropertyToID("_PaletteLab");
        private static readonly int PaletteColors = Shader.PropertyToID("_PaletteColors");

        private const int MaxPaletteSize = 64;

        [SerializeField] private Texture2D colorPalette;
        [SerializeField] private bool invert;
        [SerializeField] private PaletteMode paletteMode = PaletteMode.Nearest;
        [SerializeField] [Range(0f, 4f)] private float lightnessWeight = 1f;
        
        [System.NonSerialized] private readonly Vector4[] paletteColors = new Vector4[MaxPaletteSize];
        [System.NonSerialized] private readonly Vector4[] paletteLab = new Vector4[MaxPaletteSize];
        [System.NonSerialized] private int paletteCount;
        [System.NonSerialized] private Texture2D cachedPalette;
        [System.NonSerialized] private uint cachedUpdateCount;
        
        private bool PaletteChanged =>
            colorPalette != cachedPalette ||
            (colorPalette != null && colorPalette.updateCount != cachedUpdateCount);

        public void Apply(Material material) {
            if (PaletteChanged) RebuildPaletteCache();
            
            material.SetInteger(Invert, invert ? 1 : 0);
            material.SetInteger(Mode, (int)paletteMode);
            material.SetFloat(LightnessWeight, lightnessWeight);
            material.SetVectorArray(PaletteColors, paletteColors);
            material.SetVectorArray(PaletteLab, paletteLab);
            material.SetInteger(PaletteCount, paletteCount);
        }

        private void RebuildPaletteCache() {
            if (colorPalette == null) {
                paletteCount = 0;
                cachedPalette = null;
                return;
            }

            if (!colorPalette.isReadable) {
                Debug.LogWarning($"PaletteSwapper: Color palette {colorPalette.name} needs Read/Write enabled in its import settings.");
                paletteCount = 0;
                cachedPalette = colorPalette;
                cachedUpdateCount = colorPalette.updateCount;
                return;
            }
            
            var pixels = colorPalette.GetPixels();
            paletteCount = Mathf.Min(colorPalette.width, MaxPaletteSize);
            
            if (colorPalette.width > MaxPaletteSize) {
                Debug.LogWarning($"PaletteSwapper: Color palette {colorPalette.name} width exceeds maximum size of {MaxPaletteSize}. Only the first {MaxPaletteSize} colors will be used.");
            }
            
            for (var i = 0; i < paletteCount; i++) {
                var linear = colorPalette.isDataSRGB ? pixels[i].linear : pixels[i];
                paletteColors[i] = linear;
                paletteLab[i] = OklabUtility.LinearToOklab(linear);
            }
            
            // ramp modes index colors dark -> light; nearest mode doesn't depend on order.
            SortPaletteByLightness();
            
            cachedPalette = colorPalette;
            cachedUpdateCount = colorPalette.updateCount;
        }

        private void SortPaletteByLightness() {
            var lightness = new float[paletteCount];
            var order = new int[paletteCount];
            
            for (var i = 0; i < paletteCount; i++) {
                lightness[i] = paletteLab[i].x;
                order[i] = i;
            }
            
            System.Array.Sort(lightness, order);

            var sortedColors = new Vector4[paletteCount];
            var sortedLab = new Vector4[paletteCount];
            
            for (var i = 0; i < paletteCount; i++) {
                sortedColors[i] = paletteColors[order[i]];
                sortedLab[i] = paletteLab[order[i]];
            }
            
            System.Array.Copy(sortedColors, paletteColors, paletteCount);
            System.Array.Copy(sortedLab, paletteLab, paletteCount);
        }
    }

    public enum PaletteMode {
        Nearest,
        RampSmooth,
        RampStepped
    }
}
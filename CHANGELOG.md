# Changelog

All notable changes to this package are documented here.
The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and versions follow [Semantic Versioning](https://semver.org/).

## [1.0.0]

First release as a Unity package (`com.cannoli-cat.stylized-rendering`). Requires Unity 6 / URP RenderGraph.

### Added
- `ScreenEffectFeature<TSettings>` / `ScreenEffectPass<TSettings>` base classes: new screen effects only need a settings class and a shader.
- **Contour**: pen/hand-drawn lines from depth, normals and luminance, with wobble, line boil, width variation and paper grain.
- **Dither**: Bayer or blue-noise patterns; screen or view-direction anchoring (pattern follows camera rotation); RGB-level quantization or offset-only mode for stacking with the Palette Swapper; stepwise bilinear downsampling.
- **Palette Swapper**: OKLab nearest-color matching with lightness weight, smooth and stepped ramp modes, palettes sorted by lightness automatically.
- **Sharpness**: sharpen/blur filter.
- Shared HLSL includes (`StylizedCommon`, `StylizedColor`, `StylizedNoise`, `StylizedDepth`).
- Blue-noise textures by Christoph Peters (CC0).

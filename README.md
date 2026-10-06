# Stylized Rendering

Stylized screen effects for Unity (URP) focused on pixel-art, retro and hand-drawn aesthetics:
contour lines, dithering, palette swapping and sharpness.

## Installation

Requires **Unity 6** with **URP** (RenderGraph).

In Unity, open **Window → Package Manager**, click **+ → Install package from git URL…**, and enter:

```
https://github.com/cannoli-cat/stylized-rendering.git#v1.1.0
```

The `#v1.1.0` pins a release. Remove it to track the latest `main`.

## Features

- **Screen effects** (added as Renderer Features on your URP renderer)
  - **Contour**: pen-style lines from depth, normals and luminance, with wobble, line boil and paper grain
  - **Dither**: Bayer or blue-noise patterns, optional downsampling, and a view-direction anchor that keeps the pattern stable as the camera turns
  - **Palette Swapper**: OKLab nearest-color matching, plus smooth and stepped color ramps
  - **Sharpness**: simple sharpen/blur filter

### Stacking effects

Effects run in the order they are listed on the renderer. A good 1-bit / limited-palette setup:

1. **Dither**: Quantization `None`, Spread ≈ `1 / (palette colors − 1)`
2. **Palette Swapper**: `Nearest` or `RampStepped`

Set both to **After Rendering Post Processing** so they see the final tonemapped image.

## License and attribution

**CC BY 4.0.** You may use, modify and redistribute this, including in commercial games, **as long as you give credit.**
In your game's credits (or documentation / about screen), include:

> Stylized Rendering by Tyler Wolfe (cannoli-cat) — https://github.com/cannoli-cat/stylized-rendering

See [LICENSE.md](LICENSE.md) for the full license text.

Blue-noise textures in `Textures/BlueNoise` are by Christoph Peters, dedicated to the public domain (CC0); see the license files in that folder.

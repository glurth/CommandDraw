# CommandDraw

A Unity package by **Glurth** for high-performance, GPU-based 2D shape rendering directly to textures using Signed Distance Fields (SDFs).

**CommandDraw** is an alternative to traditional CPU-based or immediate-mode rendering, allowing you to define complex 2D shapes (lines, arcs, rectangles, etc.) using high-level C# components which are automatically converted into GPU-ready command buffers and rasterized via a fragment shader. This enables complex, anti-aliased 2D graphics with minimal CPU overhead, suitable for procedural textures, UI backgrounds, or custom visual feedback.

| Status | Unity Version | Author |
| :--- | :--- | :--- |
| **Release** | 2022.3+ | Glurth |

---

## Key Features

* **GPU-Accelerated:** Rendering is performed entirely in a fragment shader using **Signed Distance Fields (SDFs)**, ensuring high performance and resolution-independent anti-aliasing.
* **Editor Preview (`[ExecuteAlways]`):** Draw commands update in real-time in the Unity Editor and Inspector via the `DrawCommandListRenderer`.
* **Flexible Coordinate Systems:** Supports drawing using dynamic screen-space coordinates or fixed, normalized **[0, 1] UV space** for pixel-perfect procedural texture generation.
* **Easy Abstraction:** High-level C# classes (e.g., `CircleDrawCommand`, `OrientedRectDrawCommand`) manage the complexity of packing data for the GPU.
* **Advanced Compositing:** Uses **pre-multiplied alpha blending** to correctly composite overlapping shapes within the draw list.

---

## Installation

This package uses the Unity Package Manager (UPM) format.

1.  Open the **Package Manager** in Unity (`Window > Package Manager`).
2.  Click the **+** button in the top-left corner.
3.  Select **"Add package from git URL..."**
4.  Paste the repository URL:
    ```
    # https://github.com/glurth/CommandDraw.git
    ```

---

## Usage

### 1. The Renderer Component

The core of the system is the `DrawCommandListRenderer` component.

1.  Create a new empty **GameObject** in your scene.
2.  Add the `DrawCommandListRenderer` component to it.
3.  Assign a **Display Material** to the `textureBasedDisplayMaterial` field (e.g., a standard Unity UI material that uses the `_MainTex` property).

The renderer will automatically:
* Find the required fragment shader (`Unlit/DrawListFragShader`).
* Allocate a target `RenderTexture` (defaulting to screen size or a custom size).
* Run the full command pipeline on enable and whenever a property changes (via `OnValidate`).

### 2. Defining Commands

Draw commands are defined as C# classes that inherit from `DrawCommandBase`. They are exposed in the Inspector via a `[SerializeReference]` list.

1.  In the Inspector on your `DrawCommandListRenderer` component, expand the **Draw Commands** list.
2.  Click the **+** button.
3.  Select the desired command type (e.g., `LineDrawCommand`, `DiskDrawCommand`).
4.  Configure its properties (Position, Color, Radius, Thickness, etc.) in the Inspector. The renderer will instantly update the output texture.

### 3. Pixel-Based Rendering

To render into a fixed-size texture (e.g., $256 \times 256$ pixels for a game asset):

* Use the **`DrawCommandListRendererPixelBased`** component instead of the base class.
* Set the desired resolution using the **`Texture Size`** field.
* This renderer automatically normalizes your high-level commands into [0, 1] UV space before sending them to the GPU, ensuring consistent scaling regardless of the size you choose.

---

## Supported Draw Commands (SDF Primitives)

| High-Level Class | Target Primitive | Description |
| :--- | :--- | :--- |
| **`LineDrawCommand`** | `CMD_LINE` | A single straight line segment with thickness. |
| **`PolyLineDrawCommand`** | `CMD_LINE` (multi) | A connected series of line segments (a chain). |
| **`RadialLine`** | `CMD_LINE` | A line segment defined by a center, angle, and inner/outer radius fractions. |
| **`CircleDrawCommand`** | `CMD_ARC` | A complete circle outline using the ARC primitive. |
| **`ArcDrawCommand`** | `CMD_ARC` | A partial circle outline defined by a center, radius, and start/end angles in turns. |
| **`DiskDrawCommand`** | `CMD_DISK` | A filled circle. |
| **`CapsuleDrawCommand`** | `CMD_CAPSULE` | A line segment with two rounded endpoints (a filled stadium shape). |
| **`RectDrawCommand`** | `CMD_RECT` | A filled, axis-aligned rectangle. |
| **`OrientedRectDrawCommand`** | `CMD_ORECT` | A filled rectangle that can be rotated by a specified angle. |
| **`RoundedRectDrawCommand`** | `CMD_RRECT` | A filled rectangle with independent corner radii for smooth corners. |
| **`TriangleDrawCommand`** | `CMD_TRIANGLE` | A filled triangle defined by three vertices. |
| **`CurveDrawCommand`** | `CMD_LINE` (multi) | A quadratic Bézier curve that is adaptively subdivided into multiple `CMD_LINE` segments for GPU rendering. |

---

## Technical Details

The package relies on two core, synchronized components:

1.  **`PackedDrawCommand` (C# Struct):** A memory-aligned struct used to pack spatial, color, and size data efficiently for the GPU.
2.  **`DrawCommand` (HLSL Struct):** The corresponding struct defined within the `Unlit/DrawListFragShader` to interpret the data passed via the `StructuredBuffer` (`ComputeBuffer`).

The fragment shader iterates through all commands and uses the calculated Signed Distance (d) to blend the color of the shape onto the pixel, using:

$$
\text{alpha} = \text{smoothstep}(\text{pxAA}, 0, d - \frac{\text{thickness}}{2})
$$

where $\text{pxAA}$ is the screen-space anti-aliasing kernel size derived from $\text{fwidth}$.

---

## Contributions

Contributions welcome.

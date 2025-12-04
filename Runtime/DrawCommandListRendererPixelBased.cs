using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specialized version of <see cref="DrawCommandListRenderer"/> that renders draw commands
/// into a fixed-size pixel-based render texture. Overrides resolution and provides a
/// post-generation scaling step to map commands into normalized pixel space.
/// </summary>
public class DrawCommandListRendererPixelBased : DrawCommandListRenderer
{
    /// <summary>
    /// The fixed dimensions of the render texture in pixels.
    /// Overrides the base dynamic screen resolution.
    /// </summary>
    public Vector2Int textureSize = new Vector2Int(100, 100);

    /// <summary>
    /// Returns the pixel-based render texture size.
    /// Overrides <see cref="DrawCommandListRenderer.GetRenderTextureSize"/>.
    /// </summary>
    /// <returns>Fixed texture size in pixels.</returns>
    protected override Vector2Int GetRenderTextureSize()
    {
        return textureSize;
    }

    /// <summary>
    /// The shader used to render the draw commands.
    /// Can be overridden if a different shader is needed for pixel-based rendering.
    /// </summary>
    protected override string ShaderName => "Unlit/DrawListFragShader";

    /// <summary>
    /// Post-processes generated draw commands by scaling all positions and radii
    /// into normalized pixel coordinates [0,1] based on <see cref="textureSize"/>.
    /// This allows the base shader to operate in a consistent coordinate space
    /// regardless of actual pixel dimensions.
    /// </summary>
    protected override void PostGenPackedDrawCommands()
    {
        Vector2 texSizeReciprocal = new Vector2(1f / (float)textureSize.x, 1f / (float)textureSize.y);
        for (int i = 0; i < generatedDrawList.Count; i++)
        {
            PackedDrawCommand cmd = generatedDrawList[i];

            /* 
            Legacy code: manual scaling of individual components
            cmd.start *= texSizeReciprocal;
            cmd.end *= texSizeReciprocal;
            cmd.thirdCorner *= texSizeReciprocal;
            cmd.radius *= scalarReciprocal;
            cmd.thickness *= scalarReciprocal;
            */

            // Use the helper Scale method for clarity
            generatedDrawList[i] = cmd.Scale(texSizeReciprocal);
        }
    }
}

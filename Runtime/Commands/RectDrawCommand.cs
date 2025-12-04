using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Axis-aligned rectangle defined by center + half-extents.
/// Packs into a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_RECT"/>.
/// Packed fields:
/// <list type="bullet">
/// <item>packed.start = center</item>
/// <item>packed.end   = halfSize (half-width, half-height)</item>
/// <item>packed.thirdCorner = Vector2.zero (unused)</item>
/// <item>packed.radius = 0</item>
/// <item>packed.thickness = 0</item>
/// </list>
/// </summary>
[System.Serializable]
public class RectDrawCommand : DrawCommandBase
{
    /// <summary>Rectangle center.</summary>
    public Vector2 center;

    /// <summary>Half-width and half-height.</summary>
    public Vector2 halfSize;

    /// <summary>Fill color.</summary>
    public Color color;

    public RectDrawCommand()
    {
        center = Vector2.one * 0.5f;
        halfSize = center;
        color = Color.black;
    }

    public RectDrawCommand(Vector2 center, Vector2 halfSize, Color color)
    {
        this.center = center;
        this.halfSize = halfSize;
        this.color = color;
    }

    /// <summary>
    /// Initializes to a centered square covering the full UV domain.
    /// Scales to pixel coordinates if a texture size is provided.
    /// </summary>
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one * 0.5f;
        halfSize = center;
        color = Color.black;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            center *= pixelSize;
            halfSize *= pixelSize;
        }
    }

    /// <summary>
    /// Produces a single CMD_RECT command.
    /// <c>packed.start = center</c>
    /// <c>packed.end   = halfSize</c>
    /// </summary>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(
            PackedDrawCommand.CMD_RECT,
            center,
            halfSize,
            Vector2.zero,
            color,
            0.0f,
            0.0f,
            objID
        );

        cmd.objectID = objID;
        return new List<PackedDrawCommand> { cmd };
    }
}


/*using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectDrawCommand : DrawCommandBase
{
    public Vector2 center;
    public Vector2 halfSize; // half-width, half-height
    public Color color;

    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one / 2f;
        halfSize = center;
        color = Color.black;
        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            center *= pixelSize;
            halfSize *= pixelSize;
        }
    }

    public RectDrawCommand() {
        center = Vector2.one / 2f;
        halfSize = center;
        color = Color.black;
    }

    public RectDrawCommand(Vector2 center, Vector2 halfSize, Color color)
    {
        this.center = center;
        this.halfSize = halfSize;
        this.color = color;
    }

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(PackedDrawCommand.CMD_RECT, center, halfSize, Vector2.zero, color, 0.0f, 0.0f, objID);
        cmd.objectID = objID;
        List<PackedDrawCommand> list = new List<PackedDrawCommand> { cmd };
        return list;
    }
}
*/
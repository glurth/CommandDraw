using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rounded rectangle defined by center + half-extents + corner radii.
/// Packs into a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_RRECT"/>.
/// Packed fields:
/// <list type="bullet">
/// <item>packed.start  = center</item>
/// <item>packed.end    = halfSize</item>
/// <item>packed.thirdCorner = cornerRadii (x,y radii)</item>
/// <item>packed.radius = 0</item>
/// <item>packed.thickness = 0</item>
/// </list>
/// </summary>
[System.Serializable]
public class RoundedRectDrawCommand : DrawCommandBase
{
    /// <summary>Rectangle center.</summary>
    public Vector2 center;

    /// <summary>Half-width and half-height.</summary>
    public Vector2 halfSize;

    /// <summary>Corner radius per axis.</summary>
    public Vector2 cornerRadii;

    /// <summary>Fill color.</summary>
    public Color color;

    public RoundedRectDrawCommand()
    {
        center = new Vector2(0.25f, 0.5f);
        halfSize = new Vector2(0.5f, 0.5f);
        cornerRadii = new Vector2(0.1f, 0.1f);
        color = Color.black;
    }

    public RoundedRectDrawCommand(Vector2 center, Vector2 halfSize, Vector2 cornerRadii, Color color)
    {
        this.center = center;
        this.halfSize = halfSize;
        this.cornerRadii = cornerRadii;
        this.color = color;
    }

    /// <summary>
    /// Initializes with a fixed test shape.
    /// Scales center + halfSize to pixel coordinates when available.
    /// Radii remain in UV scale for now (matches shader expectations).
    /// </summary>
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = new Vector2(0.25f, 0.5f);
        halfSize = new Vector2(0.5f, 0.5f);
        cornerRadii = new Vector2(0.1f, 0.1f);
        color = Color.black;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            center *= pixelSize;
            halfSize *= pixelSize;
        }
    }

    /// <summary>
    /// Produces a single CMD_RRECT command.
    /// <c>packed.start  = center</c>,
    /// <c>packed.end = halfSize</c>,
    /// <c>packed.thirdCorner = cornerRadii</c>
    /// </summary>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(
            PackedDrawCommand.CMD_RRECT,
            center,
            halfSize,
            cornerRadii,
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
public class RoundedRectDrawCommand : DrawCommandBase
{
    public Vector2 center;
    public Vector2 halfSize;
    public Vector2 cornerRadii; // uniform
    public Color color;

    public override void InitializeData(Vector2Int pixelSize)
    {
        center = new Vector2(0.25f, 0.5f);
        halfSize = new Vector2(0.5f, 0.5f);
        cornerRadii = new Vector2(0.1f, 0.1f);
        color = Color.black;
        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            center *= pixelSize;
            halfSize *= pixelSize;
        }
    }

    public RoundedRectDrawCommand() {
        center = new Vector2(0.25f, 0.5f);
        halfSize = new Vector2(0.5f, 0.5f);
        cornerRadii = new Vector2(0.1f, 0.1f);
        color = Color.black;
    }

    public RoundedRectDrawCommand(Vector2 center, Vector2 halfSize, Vector2 cornerRadii, Color color)
    {
        this.center = center;
        this.halfSize = halfSize;
        this.cornerRadii = cornerRadii;
        this.color = color;
    }

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        // reuse thirdCorner to carry corner radii: x = uniform radius, y = 0
        PackedDrawCommand cmd = new PackedDrawCommand(PackedDrawCommand.CMD_RRECT, center, halfSize, cornerRadii, color, 0.0f, 0.0f, objID);
        cmd.objectID = objID;
        List<PackedDrawCommand> list = new List<PackedDrawCommand> { cmd };
        return list;
    }
}*/

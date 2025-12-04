using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a radial line: a single line segment emitted from a center point along a direction.
/// Produces exactly one <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_LINE"/> where:
/// <list type="bullet">
/// <item>packed.start = center + dir * (startEndRadialFraction.x * radius)</item>
/// <item>packed.end   = center + dir * (startEndRadialFraction.y * radius)</item>
/// <item>packed.thirdCorner = Vector2.zero (unused)</item>
/// <item>packed.radius = 0 (unused for lines)</item>
/// <item>packed.thickness = thickness</item>
/// </list>
/// </summary>
[System.Serializable]
public class RadialLine : DrawCommandBase
{
    /// <summary>
    /// Base point for radial evaluation.
    /// Maps to components of <see cref="PackedDrawCommand.start"/> and <see cref="PackedDrawCommand.end"/> after computation.
    /// </summary>
    public Vector2 center;

    /// <summary>
    /// Fractional inner/outer multipliers applied to <see cref="radius"/>.
    /// Example: (0.5, 0.9) => start at 50% of radius, end at 90% of radius.
    /// Not packed directly; used to compute start/end before packing.
    /// </summary>
    public Vector2 startEndRadialFraction;

    /// <summary>
    /// Distance scale from <see cref="center"/>. Used when computing start/end positions.
    /// Not packed directly into the command (radius field is set to 0 for line commands).
    /// </summary>
    public float radius;

    /// <summary>
    /// Direction expressed in turns (1.0 = full rotation). Converted to radians for computation.
    /// Not packed directly; direction is resolved before packing.
    /// </summary>
    public float angleInTurns;

    /// <summary>
    /// Stroke color. Packed into <see cref="PackedDrawCommand.color"/>.
    /// </summary>
    public Color color;

    /// <summary>
    /// Stroke thickness. Packed into <see cref="PackedDrawCommand.thickness"/>.
    /// </summary>
    public float thickness;

    /// <summary>
    /// Default constructor. Initializes a centered radial line pointing to 0 turns.
    /// </summary>
    public RadialLine()
    {
        center = new Vector2(0.5f, 0.5f);
        startEndRadialFraction = new Vector2(0.5f, 0.9f);
        radius = 0.25f;
        angleInTurns = 0f;
        color = Color.black;
        thickness = 0.1f;
    }

    /// <summary>
    /// Constructs a radial line with explicit parameters.
    /// </summary>
    /// <param name="center">Center point.</param>
    /// <param name="startEndRadialFraction">Inner and outer radial fractions.</param>
    /// <param name="radius">Radial distance scale.</param>
    /// <param name="angle">Direction in turns.</param>
    /// <param name="color">Stroke color.</param>
    /// <param name="thickness">Stroke thickness.</param>
    public RadialLine(Vector2 center, Vector2 startEndRadialFraction, float radius, float angle, Color color, float thickness)
    {
        this.center = center;
        this.startEndRadialFraction = startEndRadialFraction;
        this.radius = radius;
        this.angleInTurns = angle;
        this.color = color;
        this.thickness = thickness;
    }

    /// <summary>
    /// Initializes the radial line with sensible defaults and scales to pixel coordinates if <paramref name="pixelSize"/> indicates a real texture.
    /// </summary>
    /// <param name="pixelSize">Target texture size. Use Vector2Int.zero or Vector2Int.one for normalized/UV units.</param>
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = new Vector2(0.5f, 0.5f);
        startEndRadialFraction = new Vector2(0.5f, 0.9f);
        radius = 0.25f;
        angleInTurns = 0f;
        color = Color.black;
        thickness = 0.1f;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            center *= pixelSize;
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            radius *= shorterPixelSize;
            thickness *= shorterPixelSize;
        }
    }

    /// <summary>
    /// Converts this radial definition into a single <see cref="PackedDrawCommand"/> (CMD_LINE).
    /// Computation:
    /// <code>
    /// float rads = angleInTurns * (2 * PI);
    /// Vector2 dir = new Vector2(cos(rads), sin(rads));
    /// startPos = center + dir * (startEndRadialFraction.x * radius);
    /// endPos   = center + dir * (startEndRadialFraction.y * radius);
    /// </code>
    /// </summary>
    /// <param name="objID">Object ID assigned to the packed command.</param>
    /// <param name="tolerance">Ignored for radial lines; present for API compatibility.</param>
    /// <returns>List containing one packed line command.</returns>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        float rads = angleInTurns * Mathf.PI * 2f;
        Vector2 dir = new Vector2(Mathf.Cos(rads), Mathf.Sin(rads));

        Vector2 startPos = center + dir * (startEndRadialFraction.x * radius);
        Vector2 endPos = center + dir * (startEndRadialFraction.y * radius);

        PackedDrawCommand cmd = new PackedDrawCommand(
            PackedDrawCommand.CMD_LINE,
            startPos,
            endPos,
            Vector2.zero, // thirdCorner unused
            color,
            0.0f,         // radius unused for lines
            thickness,
            objID
        );

        cmd.objectID = objID;
        return new List<PackedDrawCommand> { cmd };
    }
}

/*using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RadialLine : DrawCommandBase
{
    public Vector2 center;
    public Vector2 startEndRadialFraction;
    public float radius;
    public float angleInTurns;
    public Color color;
    public float thickness;

    public RadialLine()
    {
        center = new Vector2(0.5f, 0.5f);
        startEndRadialFraction = new Vector2(0.5f, 0.9f);
        radius = 0.25f;
        angleInTurns = 0;
        color = Color.black;
        thickness = 0.1f;
    }

    public RadialLine(Vector2 center, Vector2 startEndRadialFraction, float radius, float angle, Color color, float thickness)
    {
        this.center = center;
        this.startEndRadialFraction = startEndRadialFraction;
        this.radius = radius;
        this.angleInTurns = angle;
        this.color = color;
        this.thickness = thickness;
    }

    public override void InitializeData(Vector2Int pixelSize)
    {
        center = new Vector2(0.5f, 0.5f);
        startEndRadialFraction = new Vector2(0.5f, 0.9f);
        radius = 0.25f;
        angleInTurns = 0;
        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            center *= pixelSize;
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            radius *= shorterPixelSize;
        }
    }



    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        float rads = angleInTurns * Mathf.PI * 2f;
        Vector2 dir = new Vector2(Mathf.Cos(rads), Mathf.Sin(rads));

        Vector2 startPos = center + dir * startEndRadialFraction.x* radius;
        Vector2 endPos = center + dir * startEndRadialFraction.y * radius;
        PackedDrawCommand cmd = new PackedDrawCommand(PackedDrawCommand.CMD_LINE, startPos, endPos, Vector2.zero, color, 0.0f, thickness, objID);
        cmd.objectID = objID;
        List<PackedDrawCommand> list = new List<PackedDrawCommand> { cmd };
        return list;
    }
}*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a circular arc draw command. Converts center, start/end angles,
/// radius, color, and thickness into a <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_ARC"/>.
/// </summary>
[System.Serializable]
public class ArcDrawCommand : DrawCommandBase
{
    /// <summary>Center point of the arc. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
    public Vector2 center;

    /// <summary>
    /// Start angle in turns (0–1). Mapped to <see cref="PackedDrawCommand.end"/>.x in radians.
    /// </summary>
    public float startAngleInTurns;

    /// <summary>
    /// End angle in turns (0–1). Mapped to <see cref="PackedDrawCommand.end"/>.y in radians.
    /// </summary>
    public float endAngleInTurns;

    /// <summary>Radius of the arc. Maps to <see cref="PackedDrawCommand.radius"/>.</summary>
    public float radius;

    /// <summary>Arc color. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
    public Color color;

    /// <summary>Thickness of the stroke. Maps to <see cref="PackedDrawCommand.thickness"/>.</summary>
    public float thickness;

    /// <summary>
    /// Converts this arc into a list of packed GPU commands.
    /// Generates a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_ARC"/>.
    /// </summary>
    /// <param name="objID">Object ID assigned to this arc in the draw list.</param>
    /// <param name="tolerance">Ignored for arcs; present for compatibility.</param>
    /// <returns>List containing one <see cref="PackedDrawCommand"/> representing this arc.</returns>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        const float turnToRad = Mathf.PI * 2f;
        PackedDrawCommand c = new PackedDrawCommand(
            PackedDrawCommand.CMD_ARC,
            center,
            new Vector2(startAngleInTurns * turnToRad, endAngleInTurns * turnToRad),
            Vector2.zero, // thirdCorner unused
            color,
            radius,
            thickness,
            objID
        );

        return new List<PackedDrawCommand>() { c };
    }

    /// <summary>
    /// Initializes arc with default values. Scales center, radius, and thickness if a pixelSize is provided.
    /// </summary>
    /// <param name="pixelSize">
    /// Target texture size. Use Vector2Int.zero or Vector2Int.one for normalized coordinates.
    /// </param>
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one / 2f;
        startAngleInTurns = 0;
        endAngleInTurns = 0.5f;
        radius = 0.5f;
        color = Color.black;
        thickness = 0.1f;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            center *= pixelSize;
            radius *= shorterPixelSize;
            thickness *= shorterPixelSize;
        }
    }

    /// <summary>
    /// Default constructor. Initializes an arc centered at (0.5,0.5) with a 180° sweep, radius 0.5, black color, and thickness 0.5.
    /// </summary>
    public ArcDrawCommand()
    {
        center = Vector2.one / 2f;
        startAngleInTurns = 0;
        endAngleInTurns = 0.5f;
        radius = 0.5f;
        color = Color.black;
        thickness = 0.5f;
    }

    /// <summary>
    /// Constructs an arc with explicit center, start/end angles, radius, color, and thickness.
    /// </summary>
    /// <param name="center">Center of the arc.</param>
    /// <param name="startAngle">Start angle in turns (0–1).</param>
    /// <param name="endAngle">End angle in turns (0–1).</param>
    /// <param name="radius">Arc radius.</param>
    /// <param name="color">Arc color.</param>
    /// <param name="thickness">Stroke thickness.</param>
    public ArcDrawCommand(Vector2 center, float startAngle, float endAngle, float radius, Color color, float thickness)
    {
        this.center = center;
        this.startAngleInTurns = startAngle;
        this.endAngleInTurns = endAngle;
        this.radius = radius;
        this.color = color;
        this.thickness = thickness;
    }
}

/*using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArcDrawCommand : DrawCommandBase
{
    public Vector2 center;
    public float startAngleInTurns;
    public float endAngleInTurns;

    public float radius;
    public Color color;
    public float thickness; // in pixels or UV units

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        const float turnToRad = Mathf.PI * 2f;
        PackedDrawCommand c = new PackedDrawCommand(PackedDrawCommand.CMD_ARC, center, new Vector2(startAngleInTurns* turnToRad, endAngleInTurns* turnToRad), Vector2.zero, color, radius, thickness, objID);
        List<PackedDrawCommand> commandList = new List<PackedDrawCommand>() { c };
        return commandList;
    }
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one / 2f;
        startAngleInTurns = 0;
        endAngleInTurns = 0.5f;// Mathf.PI;
        radius = 0.5f;
        color = Color.black;
        thickness = 0.1f;
        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            center *= pixelSize;
            radius *= shorterPixelSize;
            thickness *= shorterPixelSize;
        }
    }

    public ArcDrawCommand() {
        center = Vector2.one / 2f;
        startAngleInTurns = 0;
        endAngleInTurns = 0.5f;// Mathf.PI;
        radius = 0.5f;
        color = Color.black;
        thickness = 0.5f;
    }

    public ArcDrawCommand(Vector2 center, float startAngle, float endAngle, float radius, Color color, float thickness)
    {
        this.center = center;
        this.startAngleInTurns = startAngle;
        this.endAngleInTurns = endAngle;
        this.radius = radius;
        this.color = color;
        this.thickness = thickness;
    }
}*/

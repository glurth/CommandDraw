using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a connected series of line segments (polyline). Each consecutive pair of points produces a <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_LINE"/>.
/// </summary>
[System.Serializable]
public class PolyLineDrawCommand : DrawCommandBase
{
    /// <summary>Array of points defining the polyline. Each segment is created between positions[i] and positions[i+1]. Maps to <see cref="PackedDrawCommand.start"/> and <see cref="PackedDrawCommand.end"/> for each segment.</summary>
    public Vector2[] positions;

    /// <summary>Stroke color for all segments. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
    public Color color;

    /// <summary>Stroke thickness for all segments. Maps to <see cref="PackedDrawCommand.thickness"/>.</summary>
    public float thickness;

    /// <summary>
    /// Initializes default positions, color, and thickness. Scales to pixel units if pixelSize > 1.
    /// </summary>
    /// <param name="pixelSize">Target texture size. Use Vector2Int.zero/one for normalized units.</param>
    public override void InitializeData(Vector2Int pixelSize)
    {
        positions = new Vector2[]{
            new Vector2(0.1f,0.1f), new Vector2(0.1f,0.5f), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.1f)
        };
        color = Color.black;
        thickness = 0.1f;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            thickness *= shorterPixelSize;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(positions[i].x * pixelSize.x, positions[i].y * pixelSize.y);
            }
        }
    }

    /// <summary>Default constructor: sets up a 4-point polyline with black color and default thickness.</summary>
    public PolyLineDrawCommand()
    {
        positions = new Vector2[]{
            new Vector2(0.1f,0.1f), new Vector2(0.1f,0.5f), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.1f)
        };
        color = Color.black;
        thickness = 0.1f;
    }

    /// <summary>Constructs a polyline with specified positions, color, and thickness.</summary>
    public PolyLineDrawCommand(Vector2[] positions, Color color, float thickness)
    {
        this.positions = positions;
        this.color = color;
        this.thickness = thickness;
    }

    /// <summary>
    /// Converts this polyline into multiple <see cref="PackedDrawCommand"/>s, one per line segment.
    /// <para>start = positions[i]</para>
    /// <para>end = positions[i+1]</para>
    /// <para>thirdCorner = ignored</para>
    /// <para>radius = 0</para>
    /// <para>thickness = thickness</para>
    /// </summary>
    /// <param name="objID">Object ID assigned to all segments.</param>
    /// <param name="tolerance">Unused for polylines.</param>
    /// <returns>List of packed line commands.</returns>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        List<PackedDrawCommand> lines = new List<PackedDrawCommand>();
        for (int i = 0; i < positions.Length - 1; i++)
        {
            PackedDrawCommand lineSegment = new PackedDrawCommand(
                PackedDrawCommand.CMD_LINE,
                positions[i],
                positions[i + 1],
                Vector2.zero,
                color,
                0,
                thickness,
                objID
            );
            lines.Add(lineSegment);
        }
        return lines;
    }
}

/*using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PolyLineDrawCommand : DrawCommandBase
{
    public Vector2[] positions;
    public Color color;
    public float thickness; // in pixels or UV units


    public override void InitializeData(Vector2Int pixelSize)
    {
        positions = new Vector2[]{
            new Vector2(0.1f,0.1f), new Vector2(0.1f,0.5f), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.1f)
        };
        color = Color.black;
        thickness = 0.1f;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            Debug.Log("PolyLineDrawCommand  pixelSize: " + pixelSize);
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            thickness *= shorterPixelSize;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(positions[i].x * pixelSize.x, positions[i].y*pixelSize.y);
            }
        }
        else
            Debug.Log("PolyLineDrawCommand  pixelSize for UV");

    }

    public PolyLineDrawCommand() {
        this.positions = new Vector2[]{
            new Vector2(0.1f,0.1f), new Vector2(0.1f,0.5f), new Vector2(0.5f,0.5f), new Vector2(0.5f,0.1f)
        };
        this.color = Color.black;
        this.thickness = 0.1f;

    }
    public PolyLineDrawCommand(Vector2[] positions, Color color, float thickness)
    {
        this.positions = positions;
        this.color = color;
        this.thickness = thickness;
    }

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        List<PackedDrawCommand> lines = new List<PackedDrawCommand>();
        for(int i=0; i< positions.Length-1; i++)
        {
            PackedDrawCommand lineSegement = new PackedDrawCommand(PackedDrawCommand.CMD_LINE, positions[i],positions[i+1],Vector2.zero,color,0,thickness, objID);
            lines.Add(lineSegement);
        }
        return lines;
    }
}*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triangle primitive defined by three 2D vertices (a,b,c).
/// Packs into a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_TRIANGLE"/>.
/// Packed fields:
/// <list type="bullet">
/// <item>packed.start  = a</item>
/// <item>packed.end    = b</item>
/// <item>packed.thirdCorner = c</item>
/// <item>packed.radius = 0</item>
/// <item>packed.thickness = 0</item>
/// </list>
/// </summary>
[System.Serializable]
public class TriangleDrawCommand : DrawCommandBase
{
    /// <summary>Triangle vertex A.</summary>
    public Vector2 a;

    /// <summary>Triangle vertex B.</summary>
    public Vector2 b;

    /// <summary>Triangle vertex C.</summary>
    public Vector2 c;

    /// <summary>Fill color.</summary>
    public Color color;

    public TriangleDrawCommand()
    {
        a = new Vector2(0.1f, 0.1f);
        b = new Vector2(0.5f, 0.9f);
        c = new Vector2(0.9f, 0.1f);
        color = Color.black;
    }

    public TriangleDrawCommand(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.color = color;
    }

    /// <summary>
    /// Initializes with a fixed triangle.  
    /// Scales vertices to pixel coordinates when pixelSize > 1.
    /// </summary>
    public override void InitializeData(Vector2Int pixelSize)
    {
        a = new Vector2(0.1f, 0.1f);
        b = new Vector2(0.5f, 0.9f);
        c = new Vector2(0.9f, 0.1f);
        color = Color.black;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            a *= pixelSize;
            b *= pixelSize;
            c *= pixelSize;
        }
    }

    /// <summary>
    /// Packs A,B,C directly into the command.
    /// </summary>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(
            PackedDrawCommand.CMD_TRIANGLE,
            a,
            b,
            c,
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
public class TriangleDrawCommand : DrawCommandBase
{
    public Vector2 a;
    public Vector2 b;
    public Vector2 c;
    public Color color;


    public override void InitializeData(Vector2Int pixelSize)
    {
        a = new Vector2(0.1f, 0.1f);
        b = new Vector2(0.5f, 0.9f);
        c = new Vector2(0.9f, 0.1f);
        color = Color.black;
        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            a *= pixelSize;
            b *= pixelSize;
            c *= pixelSize;
        }
    }

    public TriangleDrawCommand() {
        a = new Vector2(0.1f, 0.1f);
        b = new Vector2(0.5f, 0.9f);
        c = new Vector2(0.9f, 0.1f);
        color = Color.black;
    }

    public TriangleDrawCommand(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.color = color;
    }

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(PackedDrawCommand.CMD_TRIANGLE, a, b, c, color, 0.0f, 0.0f, objID);
        cmd.objectID = objID;
        List<PackedDrawCommand> list = new List<PackedDrawCommand> { cmd };
        return list;
    }
}
*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a filled disk or circle. Produces a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_DISK"/>.
/// </summary>
[System.Serializable]
public class DiskDrawCommand : DrawCommandBase
{
    /// <summary>Center of the disk. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
    public Vector2 center;

    /// <summary>Radius of the disk. Maps to <see cref="PackedDrawCommand.radius"/>.</summary>
    public float radius;

    /// <summary>Color of the disk. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
    public Color color;

    /// <summary>
    /// Initializes the disk with default values. Scales to pixel units if pixelSize > 1.
    /// </summary>
    /// <param name="pixelSize">Target texture size. Use Vector2Int.zero/one for normalized units.</param>
    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one / 2f;
        radius = 0.5f;
        color = Color.black;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            center *= pixelSize;
            radius *= shorterPixelSize;
        }
    }

    /// <summary>Default constructor: sets center to (0.5,0.5), radius 0.5, color black.</summary>
    public DiskDrawCommand()
    {
        center = Vector2.one / 2f;
        radius = 0.5f;
        color = Color.black;
    }

    /// <summary>Constructs a disk with specified center, radius, and color.</summary>
    public DiskDrawCommand(Vector2 center, float radius, Color color)
    {
        this.center = center;
        this.radius = radius;
        this.color = color;
    }

    /// <summary>
    /// Converts this disk into a single <see cref="PackedDrawCommand"/>.
    /// <para>start = center</para>
    /// <para>end = ignored</para>
    /// <para>thirdCorner = ignored</para>
    /// <para>radius = radius</para>
    /// <para>thickness = 0 (filled)</para>
    /// </summary>
    /// <param name="objID">Object ID assigned to the command.</param>
    /// <param name="tolerance">Unused for disks.</param>
    /// <returns>List containing a single packed disk command.</returns>
    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(
            PackedDrawCommand.CMD_DISK,
            center,
            Vector2.zero,
            Vector2.zero,
            color,
            radius,
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
public class DiskDrawCommand : DrawCommandBase
{
    public Vector2 center;
    public float radius;
    public Color color;

    public override void InitializeData(Vector2Int pixelSize)
    {
        center = Vector2.one / 2f;
        radius = 0.5f;
        color = Color.black;

        if (pixelSize.x > 1 || pixelSize.y > 1)
        {
            int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
            center *= pixelSize;
            radius *= shorterPixelSize;

        }
    }

    public DiskDrawCommand() {
        center = Vector2.one / 2f;
        radius = 0.5f;
        color = Color.black;
    }

    public DiskDrawCommand(Vector2 center, float radius, Color color)
    {
        this.center = center;
        this.radius = radius;
        this.color = color;
    }

    public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
    {
        PackedDrawCommand cmd = new PackedDrawCommand(PackedDrawCommand.CMD_DISK, center, Vector2.zero, Vector2.zero, color, radius, 0.0f,objID);
        cmd.objectID = objID;
        List<PackedDrawCommand> list = new List<PackedDrawCommand> { cmd };
        return list;
    }
}
*/
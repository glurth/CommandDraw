using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a single GPU-ready draw command with all data packed into simple fields.
    /// Each field’s interpretation depends on <see cref="commandType"/>. Used by
    /// <see cref="DrawCommandListRenderer"/> to send draw instructions to a shader.
    /// </summary>
    [System.Serializable]
    public struct PackedDrawCommand
    {
        /// <summary>
        /// Total byte size of this struct when sent to a ComputeBuffer.
        /// </summary>
        public const int STRIDE = sizeof(float) * 2 * 3 // start + end + thirdCorner
            + sizeof(float) * 4 // color
            + sizeof(float) * 2 // radius + thickness
            + sizeof(int) * 2; // commandType + objID

        /// <summary>Command type: simple line.</summary>
        public const int CMD_LINE = 0;
        /// <summary>Command type: arc.</summary>
        public const int CMD_ARC = 1;
        /// <summary>Command type: filled disk.</summary>
        public const int CMD_DISK = 2;
        /// <summary>Command type: axis-aligned filled rectangle.</summary>
        public const int CMD_RECT = 3;
        /// <summary>Command type: oriented filled rectangle.</summary>
        public const int CMD_ORECT = 4;
        /// <summary>Command type: rounded filled rectangle.</summary>
        public const int CMD_RRECT = 5;
        /// <summary>Command type: capsule.</summary>
        public const int CMD_CAPSULE = 6;
        /// <summary>Command type: triangle.</summary>
        public const int CMD_TRIANGLE = 7;

        /// <summary>
        /// Start position or primary reference point.
        /// <list type="bullet">
        /// <item>LINE: endpoint A</item>
        /// <item>ARC: center</item>
        /// <item>DISK: center</item>
        /// <item>RECT: center</item>
        /// <item>ORECT: center</item>
        /// <item>RRECT: center</item>
        /// <item>CAPSULE: endpoint A</item>
        /// <item>TRIANGLE: point A</item>
        /// </list>
        /// </summary>
        public Vector2 start;

        /// <summary>
        /// End position or secondary reference.
        /// <list type="bullet">
        /// <item>LINE: endpoint B</item>
        /// <item>ARC: start/end angles packed (x=startAngle, y=endAngle)</item>
        /// <item>DISK: ignored</item>
        /// <item>RECT: halfSize (x=halfWidth, y=halfHeight)</item>
        /// <item>ORECT: halfSize</item>
        /// <item>RRECT: halfSize</item>
        /// <item>CAPSULE: endpoint B</item>
        /// <item>TRIANGLE: point B</item>
        /// </list>
        /// </summary>
        public Vector2 end;

        /// <summary>
        /// Tertiary point or auxiliary data.
        /// <list type="bullet">
        /// <item>LINE: ignored</item>
        /// <item>ARC: ignored</item>
        /// <item>DISK: ignored</item>
        /// <item>RECT: ignored</item>
        /// <item>ORECT: ignored</item>
        /// <item>RRECT: cornerRadii (x,y)</item>
        /// <item>CAPSULE: ignored</item>
        /// <item>TRIANGLE: point C</item>
        /// </list>
        /// </summary>
        public Vector2 thirdCorner;

        /// <summary>
        /// Radius or scalar parameter.
        /// <list type="bullet">
        /// <item>LINE: ignored</item>
        /// <item>ARC: radius</item>
        /// <item>DISK: radius</item>
        /// <item>RECT: ignored</item>
        /// <item>ORECT: rotation angle (radians)</item>
        /// <item>RRECT: ignored</item>
        /// <item>CAPSULE: radius</item>
        /// <item>TRIANGLE: ignored</item>
        /// </list>
        /// </summary>
        public float radius;

        /// <summary>Shape color.</summary>
        public Color color;

        /// <summary>
        /// Stroke thickness for stroked shapes; ignored for filled shapes.
        /// </summary>
        public float thickness;

        /// <summary>Command type identifier (use the CMD_* constants).</summary>
        public int commandType;

        /// <summary>Unique object identifier for tracking or selection.</summary>
        public int objectID;

        /// <summary>
        /// Constructs a packed draw command with all fields specified.
        /// </summary>
        public PackedDrawCommand(int commandType, Vector2 start, Vector2 end, Vector2 thirdCorner, Color color, float radius, float thickness, int objID)
        {
            this.commandType = commandType;
            this.start = start;
            this.end = end;
            this.thirdCorner = thirdCorner;
            this.color = color;
            this.radius = radius;
            this.thickness = thickness;
            this.objectID = objID;
        }

        /// <summary>
        /// Returns a new <see cref="PackedDrawCommand"/> scaled by the given vector.
        /// Positions and thirdCorner are multiplied component-wise.
        /// Radius and thickness are scaled by the largest component.
        /// </summary>
        /// <param name="scale2d">2D scale vector.</param>
        /// <returns>Scaled draw command.</returns>
        public PackedDrawCommand Scale(Vector2 scale2d)
        {
            PackedDrawCommand cmd = this;
            cmd.start *= scale2d;
            if (cmd.commandType != CMD_ARC)
                cmd.end *= scale2d;
            cmd.thirdCorner *= scale2d;
            float scalar = Mathf.Max(scale2d.x, scale2d.y);
            cmd.radius *= scalar;
            cmd.thickness *= scalar;
            return cmd;
        }
    }
    /// <summary>
    /// Base class for user-facing draw commands.
    /// Provides interface for initialization and conversion to <see cref="PackedDrawCommand"/>.
    /// </summary>
    [System.Serializable]
    public abstract class DrawCommandBase
    {
        /// <summary>
        /// Initialize any internal data or buffers needed for this command.
        /// </summary>
        /// <param name="pixelSize">
        /// Pixel size of the target render texture. Use Vector2Int.zero or Vector2Int.one
        /// for normalized UV-based coordinates.
        /// </param>
        public abstract void InitializeData(Vector2Int pixelSize);

        /// <summary>
        /// Converts this high-level command into one or more packed GPU commands.
        /// </summary>
        /// <param name="objID">Object ID used for identification in the draw list.</param>
        /// <param name="tolerance">Optional tolerance for curve approximation, etc.</param>
        /// <returns>List of <see cref="PackedDrawCommand"/> representing this command.</returns>
        public abstract List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f);
    }
}
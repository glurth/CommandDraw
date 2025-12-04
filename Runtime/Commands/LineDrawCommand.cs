using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a single straight line draw command. Converts user-defined start/end points,
    /// color, and thickness into a <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_LINE"/>.
    /// </summary>
    [System.Serializable]
    public class LineDrawCommand : DrawCommandBase
    {
        /// <summary>Start point of the line. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
        public Vector2 start;

        /// <summary>End point of the line. Maps to <see cref="PackedDrawCommand.end"/>.</summary>
        public Vector2 end;

        /// <summary>Thickness of the line. Maps to <see cref="PackedDrawCommand.thickness"/>.</summary>
        public float thickness;

        /// <summary>Line color. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
        public Color color;

        /// <summary>
        /// Initializes the line with default values.
        /// Scales positions and thickness if a pixelSize is provided.
        /// </summary>
        /// <param name="pixelSize">
        /// Target texture size. Use Vector2Int.zero or Vector2Int.one for normalized coordinates.
        /// </param>
        public override void InitializeData(Vector2Int pixelSize)
        {
            start = Vector2.zero;
            end = Vector2.one;
            thickness = 0.1f;
            color = Color.black;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                start *= pixelSize;
                end *= pixelSize;
                int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
                thickness *= shorterPixelSize;
            }
        }

        /// <summary>Default constructor. Initializes a black line from (0,0) to (1,1) with thickness 0.1.</summary>
        public LineDrawCommand()
        {
            start = Vector2.zero;
            end = Vector2.one;
            thickness = 0.1f;
            color = Color.black;
        }

        /// <summary>
        /// Constructs a line with explicit start/end, thickness, and color.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        /// <param name="thickness">Line thickness.</param>
        /// <param name="color">Line color.</param>
        public LineDrawCommand(Vector2 start, Vector2 end, float thickness, Color color)
        {
            this.start = start;
            this.end = end;
            this.thickness = thickness;
            this.color = color;
        }

        /// <summary>
        /// Converts this line command into a list of packed GPU commands.
        /// Generates a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_LINE"/>.
        /// </summary>
        /// <param name="objID">Object ID assigned to this line in the draw list.</param>
        /// <param name="tolerance">Ignored for lines; present for compatibility with base class.</param>
        /// <returns>List containing one <see cref="PackedDrawCommand"/> representing this line.</returns>
        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            PackedDrawCommand lineCmd = new PackedDrawCommand(
                PackedDrawCommand.CMD_LINE,
                start,
                end,
                Vector2.zero, // thirdCorner unused for line
                color,
                0,            // radius unused for line
                thickness,
                objID
            );

            lineCmd.objectID = objID;
            return new List<PackedDrawCommand> { lineCmd };
        }
    }
}
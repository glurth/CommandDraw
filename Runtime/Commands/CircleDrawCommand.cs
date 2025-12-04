using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a circle outline draw command. Converts center, radius, color, and thickness
    /// into a <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_ARC"/> covering a full 360° sweep.
    /// </summary>
    [System.Serializable]
    public class CircleDrawCommand : DrawCommandBase
    {
        /// <summary>Center of the circle. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
        public Vector2 center;

        /// <summary>Circle radius. Maps to <see cref="PackedDrawCommand.radius"/>.</summary>
        public float radius;

        /// <summary>Circle color. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
        public Color color;

        /// <summary>Stroke thickness. Maps to <see cref="PackedDrawCommand.thickness"/>.</summary>
        public float thickness;

        /// <summary>
        /// Converts this circle into a packed GPU command.
        /// Generates a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_ARC"/> with start=0 and end=2π radians.
        /// </summary>
        /// <param name="objID">Object ID for this circle.</param>
        /// <param name="tolerance">Ignored; included for compatibility with base class.</param>
        /// <returns>List containing one packed command representing this circle.</returns>
        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            PackedDrawCommand c = new PackedDrawCommand(
                PackedDrawCommand.CMD_ARC,
                center,
                new Vector2(0, Mathf.PI * 2f),
                Vector2.zero, // thirdCorner unused
                color,
                radius,
                thickness,
                objID
            );

            return new List<PackedDrawCommand>() { c };
        }

        /// <summary>
        /// Initializes circle with default center, radius, color, and thickness.
        /// Scales center, radius, and thickness if pixelSize indicates a real texture size.
        /// </summary>
        /// <param name="pixelSize">Target texture size. Vector2Int.zero/one keeps normalized units.</param>
        public override void InitializeData(Vector2Int pixelSize)
        {
            center = Vector2.one / 2f;
            radius = 0.25f;
            color = Color.black;
            thickness = 0.05f;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
                center *= pixelSize;
                radius *= shorterPixelSize;
                thickness *= shorterPixelSize;
            }
        }

        /// <summary>Default constructor. Sets center to (0.5,0.5), radius 0.25, black color, thickness 0.05.</summary>
        public CircleDrawCommand()
        {
            center = Vector2.one / 2f;
            radius = 0.25f;
            color = Color.black;
            thickness = 0.05f;
        }

        /// <summary>Constructs a circle with explicit center, radius, color, and thickness.</summary>
        /// <param name="center">Circle center.</param>
        /// <param name="radius">Circle radius.</param>
        /// <param name="color">Circle color.</param>
        /// <param name="thickness">Stroke thickness.</param>
        public CircleDrawCommand(Vector2 center, float radius, Color color, float thickness)
        {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.thickness = thickness;
        }
    }
}
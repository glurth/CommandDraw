using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a filled, oriented (rotated) rectangle. Produces a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_ORECT"/>.
    /// </summary>
    [System.Serializable]
    public class OrientedRectDrawCommand : DrawCommandBase
    {
        /// <summary>Center of the rectangle. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
        public Vector2 center;

        /// <summary>Half-size of the rectangle (half-width, half-height). Maps to <see cref="PackedDrawCommand.end"/>.</summary>
        public Vector2 halfSize;

        /// <summary>Rotation angle in radians. Maps to <see cref="PackedDrawCommand.radius"/>.</summary>
        public float rotationRadians;

        /// <summary>Fill color. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
        public Color color;

        /// <summary>
        /// Initializes the oriented rectangle with default values. Scales to pixel units if pixelSize > 1.
        /// </summary>
        /// <param name="pixelSize">Target texture size. Use Vector2Int.zero/one for normalized units.</param>
        public override void InitializeData(Vector2Int pixelSize)
        {
            center = new Vector2(0.25f, 0.5f);
            halfSize = new Vector2(0.5f, 0.5f);
            rotationRadians = Mathf.PI / 2;
            color = Color.black;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                center *= pixelSize;
                halfSize *= pixelSize;
            }
        }

        /// <summary>Default constructor: sets center, halfSize, rotation, and color to defaults.</summary>
        public OrientedRectDrawCommand()
        {
            center = new Vector2(0.25f, 0.5f);
            halfSize = new Vector2(0.5f, 0.5f);
            rotationRadians = Mathf.PI / 2;
            color = Color.black;
        }

        /// <summary>Constructs an oriented rectangle with specified center, halfSize, rotation, and color.</summary>
        public OrientedRectDrawCommand(Vector2 center, Vector2 halfSize, float rotationRadians, Color color)
        {
            this.center = center;
            this.halfSize = halfSize;
            this.rotationRadians = rotationRadians;
            this.color = color;
        }

        /// <summary>
        /// Converts this oriented rectangle into a single <see cref="PackedDrawCommand"/>.
        /// <para>start = center</para>
        /// <para>end = halfSize</para>
        /// <para>thirdCorner = ignored</para>
        /// <para>radius = rotationRadians (radians)</para>
        /// <para>thickness = 0 (filled)</para>
        /// </summary>
        /// <param name="objID">Object ID assigned to the command.</param>
        /// <param name="tolerance">Unused for oriented rectangles.</param>
        /// <returns>List containing a single packed oriented rectangle command.</returns>
        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            PackedDrawCommand cmd = new PackedDrawCommand(
                PackedDrawCommand.CMD_ORECT,
                center,
                halfSize,
                Vector2.zero,
                color,
                rotationRadians,
                0.0f,
                objID
            );
            cmd.objectID = objID;
            return new List<PackedDrawCommand> { cmd };
        }
    }
}
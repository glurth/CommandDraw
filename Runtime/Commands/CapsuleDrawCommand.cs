using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a capsule (line with rounded ends) draw command. Converts two endpoints, radius, and color
    /// into a <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_CAPSULE"/>.
    /// </summary>
    [System.Serializable]
    public class CapsuleDrawCommand : DrawCommandBase
    {
        /// <summary>First endpoint of the capsule. Maps to <see cref="PackedDrawCommand.start"/>.</summary>
        public Vector2 endpointA;

        /// <summary>Second endpoint of the capsule. Maps to <see cref="PackedDrawCommand.end"/>.</summary>
        public Vector2 endpointB;

        /// <summary>Capsule radius. Maps to <see cref="PackedDrawCommand.radius"/>.</summary>
        public float radius;

        /// <summary>Capsule color. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
        public Color color;

        /// <summary>
        /// Initializes capsule with default positions, radius, and color.
        /// Scales endpoints and radius if a pixelSize is provided.
        /// </summary>
        /// <param name="pixelSize">
        /// Target texture size. Use Vector2Int.zero or Vector2Int.one for normalized coordinates.
        /// </param>
        public override void InitializeData(Vector2Int pixelSize)
        {
            endpointA = new Vector2(0.1f, 0.5f);
            endpointB = new Vector2(0.9f, 0.5f);
            radius = 0.2f;
            color = Color.black;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                endpointA *= pixelSize;
                endpointB *= pixelSize;
                int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
                radius *= shorterPixelSize;
            }
        }

        /// <summary>Default constructor. Sets endpoints to (0.1,0.5)–(0.9,0.5), radius 0.2, black color.</summary>
        public CapsuleDrawCommand()
        {
            endpointA = new Vector2(0.1f, 0.5f);
            endpointB = new Vector2(0.9f, 0.5f);
            radius = 0.2f;
            color = Color.black;
        }

        /// <summary>
        /// Constructs a capsule with explicit endpoints, radius, and color.
        /// </summary>
        /// <param name="a">First endpoint.</param>
        /// <param name="b">Second endpoint.</param>
        /// <param name="radius">Capsule radius.</param>
        /// <param name="color">Capsule color.</param>
        public CapsuleDrawCommand(Vector2 a, Vector2 b, float radius, Color color)
        {
            this.endpointA = a;
            this.endpointB = b;
            this.radius = radius;
            this.color = color;
        }

        /// <summary>
        /// Converts this capsule into a list of packed GPU commands.
        /// Generates a single <see cref="PackedDrawCommand"/> of type <see cref="PackedDrawCommand.CMD_CAPSULE"/>.
        /// <para>Thickness is ignored; the shader treats it as a filled capsule.</para>
        /// </summary>
        /// <param name="objID">Object ID assigned to this capsule in the draw list.</param>
        /// <param name="tolerance">Ignored for capsules; present for base class compatibility.</param>
        /// <returns>List containing one <see cref="PackedDrawCommand"/> representing this capsule.</returns>
        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            PackedDrawCommand cmd = new PackedDrawCommand(
                PackedDrawCommand.CMD_CAPSULE,
                endpointA,
                endpointB,
                Vector2.zero, // thirdCorner unused
                color,
                radius,
                0.0f,        // thickness ignored
                objID
            );

            cmd.objectID = objID;
            return new List<PackedDrawCommand> { cmd };
        }
    }
}
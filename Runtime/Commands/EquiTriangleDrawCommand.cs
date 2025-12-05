using System.Collections.Generic;
using UnityEngine;

namespace EyE.Graphics
{
    /// <summary>
    /// Oriented equilateral triangle centered at a point, with size and rotation.
    /// Packs into a CMD_TRIANGLE.
    /// </summary>
    [System.Serializable]
    public class EquiTriangleDrawCommand : DrawCommandBase
    {
        /// <summary>Triangle center position.</summary>
        public Vector2 center;

        /// <summary>
        /// Triangle size. Represents distance from center to each vertex.
        /// </summary>
        public float size;

        /// <summary>
        /// Rotation angle in radians.
        /// </summary>
        public float angle;

        /// <summary>Fill color.</summary>
        public Color color;

        public EquiTriangleDrawCommand()
        {
            center = new Vector2(0.5f, 0.5f);
            size = 0.05f;
            angle = 0.0f;
            color = Color.black;
        }

        public EquiTriangleDrawCommand(Vector2 center, float size, float angle, Color color)
        {
            this.center = center;
            this.size = size;
            this.angle = angle;
            this.color = color;
        }

        public override void InitializeData(Vector2Int pixelSize)
        {
            // Keep normalized by default
            center = new Vector2(0.5f, 0.5f);
            size = 0.05f;
            angle = 0.0f;
            color = Color.black;

            // If pixelSize > 1, scale center+size into pixel domain
            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                center *= pixelSize;
                int shortSide = pixelSize.x < pixelSize.y ? pixelSize.x : pixelSize.y;
                size *= shortSide;
            }
        }

        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            // Precompute the 3 vertices
            float a0 = angle;
            float a1 = angle + (2.0f * Mathf.PI / 3.0f);
            float a2 = angle + (4.0f * Mathf.PI / 3.0f);

            Vector2 v0 = new Vector2(
                center.x + size * Mathf.Cos(a0),
                center.y + size * Mathf.Sin(a0)
            );

            Vector2 v1 = new Vector2(
                center.x + size * Mathf.Cos(a1),
                center.y + size * Mathf.Sin(a1)
            );

            Vector2 v2 = new Vector2(
                center.x + size * Mathf.Cos(a2),
                center.y + size * Mathf.Sin(a2)
            );

            PackedDrawCommand cmd = new PackedDrawCommand(
                PackedDrawCommand.CMD_TRIANGLE,
                v0,
                v1,
                v2,
                color,
                0.0f,
                0.0f,
                objID
            );

            cmd.objectID = objID;

            List<PackedDrawCommand> list = new List<PackedDrawCommand>();
            list.Add(cmd);
            return list;
        }
    }
}

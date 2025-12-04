using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    /// <summary>
    /// Represents a quadratic Bézier curve. Automatically subdivides into adaptive line segments
    /// that are converted into <see cref="PackedDrawCommand"/>s of type <see cref="PackedDrawCommand.CMD_LINE"/>.
    /// </summary>
    [System.Serializable]
    public class CurveDrawCommand : DrawCommandBase
    {
        /// <summary>Start point of the curve. Maps to <see cref="PackedDrawCommand.start"/> of line segments.</summary>
        public Vector2 p0;

        /// <summary>Control point of the curve, used for subdivision. Not directly in <see cref="PackedDrawCommand"/>.</summary>
        public Vector2 p1;

        /// <summary>End point of the curve. Maps to <see cref="PackedDrawCommand.end"/> of line segments.</summary>
        public Vector2 p2;

        /// <summary>Color of the curve segments. Maps to <see cref="PackedDrawCommand.color"/>.</summary>
        public Color color;

        /// <summary>Stroke thickness. Maps to <see cref="PackedDrawCommand.thickness"/>.</summary>
        public float thickness;

        /// <summary>
        /// Converts the quadratic Bézier curve into adaptive line segments as <see cref="PackedDrawCommand"/>s.
        /// </summary>
        /// <param name="objID">Object ID assigned to each segment.</param>
        /// <param name="tolerance">Maximum allowed flatness for subdivision; smaller values produce more segments.</param>
        /// <returns>List of packed line commands approximating the curve.</returns>
        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            List<PackedDrawCommand> segments = new List<PackedDrawCommand>();
            SubdivideQuad(p0, p1, p2, color, thickness, tolerance, segments);
            return segments;

            void SubdivideQuad(Vector2 p0, Vector2 p1, Vector2 p2, Color color, float thickness, float tolerance, List<PackedDrawCommand> segments)
            {
                if (Flatness(p0, p1, p2) <= tolerance)
                {
                    segments.Add(new PackedDrawCommand(
                        PackedDrawCommand.CMD_LINE,
                        p0,
                        p2,
                        Vector2.zero,
                        color,
                        0f,
                        thickness,
                        objID
                    ));
                    return;
                }

                // Subdivide using de Casteljau
                Vector2 p01 = (p0 + p1) * 0.5f;
                Vector2 p12 = (p1 + p2) * 0.5f;
                Vector2 p012 = (p01 + p12) * 0.5f;

                SubdivideQuad(p0, p01, p012, color, thickness, tolerance, segments);
                SubdivideQuad(p012, p12, p2, color, thickness, tolerance, segments);
            }

            float Flatness(Vector2 p0, Vector2 p1, Vector2 p2)
            {
                Vector2 d = p2 - p0;
                if (d.sqrMagnitude < Mathf.Epsilon) return (p1 - p0).magnitude;
                float t = Vector2.Dot(p1 - p0, d) / d.sqrMagnitude;
                Vector2 proj = p0 + Mathf.Clamp01(t) * d;
                return (p1 - proj).magnitude;
            }
        }

        /// <summary>
        /// Initializes curve with default points, color, and thickness.
        /// Scales points and thickness if pixelSize > 1.
        /// </summary>
        /// <param name="pixelSize">Target texture size. Use Vector2Int.zero/one for normalized units.</param>
        public override void InitializeData(Vector2Int pixelSize)
        {
            p0 = Vector2.zero;
            p1 = new Vector2(0.5f, 0.5f);
            p2 = new Vector2(0.9f, 0.5f);
            color = Color.black;
            thickness = 0.1f;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                p0 *= pixelSize;
                p1 *= pixelSize;
                p2 *= pixelSize;
                int shorterPixelSize = Mathf.Min(pixelSize.x, pixelSize.y);
                thickness *= shorterPixelSize;
            }
        }

        /// <summary>Default constructor: sets default points, color black, thickness 0.1.</summary>
        public CurveDrawCommand()
        {
            p0 = Vector2.zero;
            p1 = new Vector2(0.5f, 0.5f);
            p2 = new Vector2(0.9f, 0.5f);
            color = Color.black;
            thickness = 0.1f;
        }

        /// <summary>Constructs curve with explicit points, color, and thickness.</summary>
        public CurveDrawCommand(Vector2 p0, Vector2 p1, Vector2 p2, Color color, float thickness)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.color = color;
            this.thickness = thickness;
        }
    }

}
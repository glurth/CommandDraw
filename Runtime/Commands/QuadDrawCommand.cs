using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyE.Graphics
{
    [System.Serializable]
    public class QuadDrawCommand : DrawCommandBase
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;
        public Color color;

        public QuadDrawCommand()
        {
            a = new Vector2(0.1f, 0.1f);
            b = new Vector2(0.1f, 0.9f);
            c = new Vector2(0.9f, 0.9f);
            d = new Vector2(0.9f, 0.1f);
            color = Color.black;
        }

        public QuadDrawCommand(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.color = color;
        }

        public override void InitializeData(Vector2Int pixelSize)
        {
            a = new Vector2(0.1f, 0.1f);
            b = new Vector2(0.1f, 0.9f);
            c = new Vector2(0.9f, 0.9f);
            d = new Vector2(0.9f, 0.1f);
            color = Color.black;

            if (pixelSize.x > 1 || pixelSize.y > 1)
            {
                a *= pixelSize;
                b *= pixelSize;
                c *= pixelSize;
                d *= pixelSize;
            }
        }

        public override List<PackedDrawCommand> ToDrawCommands(
            int objID, float tolerance = 0.001f)
        {
            PackedDrawCommand cmd1 = new PackedDrawCommand(
                PackedDrawCommand.CMD_TRIANGLE,
                a,
                b,
                c,
                color,
                0.0f,
                0.0f,
                objID, textureReference, blendMode
            );

            PackedDrawCommand cmd2 = new PackedDrawCommand(
                PackedDrawCommand.CMD_TRIANGLE,
                a,
                c,
                d,
                color,
                0.0f,
                0.0f,
                objID, textureReference, blendMode
            );

            cmd1.objectID = objID;
            cmd2.objectID = objID;

            return new List<PackedDrawCommand> { cmd1, cmd2 };
        }
    }
}

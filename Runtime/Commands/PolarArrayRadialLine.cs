using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EyE.Graphics
{
    public class PolarRadialLineArray : DrawCommandBase
    {
        public RadialLine prototype;
        public float angleStepTurns;
        public int count;

        public PolarRadialLineArray()
        {
            prototype = new RadialLine();
            angleStepTurns = 0.1f;
            count = 5;
        }

        public override void InitializeData(Vector2Int pixelSize)
        {
            prototype.InitializeData(pixelSize);
        }

        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            List<PackedDrawCommand> list = new List<PackedDrawCommand>(count);
            RadialLine workingPrototype = new RadialLine(
                    prototype.center,
                    prototype.startEndRadialFraction,
                    prototype.radius,
                    prototype.angleInTurns,
                    prototype.color,
                    prototype.thickness
                );

            for (int i = 0; i < count; i++)
            {
                List<PackedDrawCommand> tmp = workingPrototype.ToDrawCommands(objID, tolerance);
                list.AddRange(tmp);
                workingPrototype.angleInTurns += angleStepTurns;
            }

            return list;
        }
    }
    public class PolarAngleSpanRadialLineArray : DrawCommandBase
    {
        public RadialLine prototype;
        public float totalAngleSpanTurns;
        public int count;

        public PolarAngleSpanRadialLineArray()
        {
            prototype = new RadialLine();
            totalAngleSpanTurns = 1f;
            count = 12;
        }

        public override void InitializeData(Vector2Int pixelSize)
        {
            prototype.InitializeData(pixelSize);
        }

        public override List<PackedDrawCommand> ToDrawCommands(int objID, float tolerance = 0.001f)
        {
            List<PackedDrawCommand> list = new List<PackedDrawCommand>(count);
            RadialLine workingPrototype = new RadialLine(
                    prototype.center,
                    prototype.startEndRadialFraction,
                    prototype.radius,
                    prototype.angleInTurns,
                    prototype.color,
                    prototype.thickness
                );

            float angleStepTurns = totalAngleSpanTurns / count;
            for (int i = 0; i < count; i++)
            {
                List<PackedDrawCommand> tmp = workingPrototype.ToDrawCommands(objID, tolerance);
                list.AddRange(tmp);
                workingPrototype.angleInTurns += angleStepTurns;
            }

            return list;
        }
    }
}
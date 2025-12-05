using System;
using System.Collections.Generic;
using UnityEngine;

namespace EyE.Graphics
{


    [ExecuteAlways]
    public class GrandfatherClockRenderer : MonoBehaviour
    {
        public DrawCommandListRenderer rendererRef;

        private CircleDrawCommand outerBezel;
        private CircleDrawCommand innerBezel;

        private RadialLine hourHand;
        private RadialLine minuteHand;
        private RadialLine secondHand;

        private CapsuleDrawCommand pendulumRod;
        private DiskDrawCommand pendulumBob;

        private List<EquiTriangleDrawCommand> hourSymbols;

        private bool initialized = false;
        private float pendulumPhase = 0.0f;

        private void Start()
        {
            if (rendererRef == null)
            {
                Debug.LogError("GrandfatherClockRenderer: rendererRef missing.");
                return;
            }

            BuildClock();
        }

        private void BuildClock()
        {
            rendererRef.drawCommands.Clear();

            // -----------------------------
            // CLOCK FRAME
            // -----------------------------
            outerBezel = new CircleDrawCommand();
            outerBezel.center = new Vector2(0.5f, 0.5f);
            outerBezel.radius = 0.47f;
            outerBezel.color = new Color(0.15f, 0.08f, 0.02f, 1.0f); // wood-dark
            outerBezel.thickness = 0.05f;
            rendererRef.drawCommands.Add(outerBezel);

            innerBezel = new CircleDrawCommand();
            innerBezel.center = new Vector2(0.5f, 0.5f);
            innerBezel.radius = 0.40f;
            innerBezel.color = new Color(0.9f, 0.85f, 0.7f, 1.0f); // old-cream
            innerBezel.thickness = 0.015f;
            rendererRef.drawCommands.Add(innerBezel);

            // -----------------------------
            // HOUR SYMBOLS (12)
            // -----------------------------
            hourSymbols = new List<EquiTriangleDrawCommand>();

            int i = 0;
            while (i < 12)
            {
                float turn = (float)i / 12.0f;
                float ang = turn * Mathf.PI * 2.0f;

                float r = 0.33f;
                Vector2 p = new Vector2(
                    innerBezel.center.x + r * Mathf.Cos(ang),
                    innerBezel.center.y + r * Mathf.Sin(ang)
                );

                EquiTriangleDrawCommand tri = new EquiTriangleDrawCommand();
                tri.center = p;
                tri.size = 0.03f;
                tri.angle = ang + Mathf.PI / 2.0f;
                tri.color = new Color(0.1f, 0.08f, 0.05f, 1.0f);
                hourSymbols.Add(tri);
                rendererRef.drawCommands.Add(tri);

                i = i + 1;
            }

            // -----------------------------
            // HANDS
            // -----------------------------
            hourHand = new RadialLine();
            hourHand.center = innerBezel.center;
            hourHand.radius = innerBezel.radius;
            hourHand.startEndRadialFraction = new Vector2(0.0f, 0.45f);
            hourHand.color = Color.black;
            hourHand.thickness = 0.03f;
            rendererRef.drawCommands.Add(hourHand);

            minuteHand = new RadialLine();
            minuteHand.center = innerBezel.center;
            minuteHand.radius = innerBezel.radius;
            minuteHand.startEndRadialFraction = new Vector2(0.0f, 0.60f);
            minuteHand.color = Color.black;
            minuteHand.thickness = 0.02f;
            rendererRef.drawCommands.Add(minuteHand);

            secondHand = new RadialLine();
            secondHand.center = innerBezel.center;
            secondHand.radius = innerBezel.radius;
            secondHand.startEndRadialFraction = new Vector2(0.0f, 0.70f);
            secondHand.color = new Color(0.7f, 0.1f, 0.1f, 1.0f);
            secondHand.thickness = 0.01f;
            rendererRef.drawCommands.Add(secondHand);

            // -----------------------------
            // PENDULUM
            // -----------------------------
            pendulumRod = new CapsuleDrawCommand();
            pendulumRod.endpointA = new Vector2(innerBezel.center.x, innerBezel.center.y - 0.05f);
            pendulumRod.endpointB = new Vector2(innerBezel.center.x, 0.10f);
            pendulumRod.radius = 0.006f;
            pendulumRod.color = new Color(0.8f, 0.65f, 0.4f, 1.0f);
            rendererRef.drawCommands.Add(pendulumRod);

            pendulumBob = new DiskDrawCommand();
            pendulumBob.center = new Vector2(innerBezel.center.x, 0.10f);
            pendulumBob.radius = 0.05f;
            pendulumBob.color = new Color(0.9f, 0.75f, 0.45f, 1.0f);
            rendererRef.drawCommands.Add(pendulumBob);

            initialized = true;
            rendererRef.SetDirty();
        }

        private const float INV_12 = 1f / 12f;
        private const float INV_720 = 1f / 720f;
        private const float INV_60 = 1f / 60f;
        private const float INV_3600 = 1f / 3600f;
        private const float INV_60000 = 1f / 60000f;
        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            DateTime t = DateTime.Now;

            // --------------------------------
            // HAND ANGLES
            // --------------------------------
            float hourTurns = (t.Hour % 12) * INV_12 + t.Minute * INV_720;
            float minuteTurns = t.Minute * INV_60 + t.Second * INV_3600;
            float secondTurns = t.Second * INV_60 + t.Millisecond * INV_60000;
            hourHand.angleInTurns = .25f - hourTurns;
            minuteHand.angleInTurns = .25f - minuteTurns;
            secondHand.angleInTurns = .25f - secondTurns;


            // --------------------------------
            // PENDULUM SWING
            // --------------------------------
            pendulumPhase = pendulumPhase + Time.deltaTime;

            float swing = Mathf.Sin(pendulumPhase * 2.0f) * 0.1f;

            RotatePendulum(swing);

            rendererRef.SetDirty();
        }

        private void RotatePendulum(float offsetX)
        {
            // pivot = top of rod
            Vector2 pivot = pendulumRod.endpointA;

            // rod bottom
            float length = pendulumRod.endpointB.y - pivot.y;

            pendulumRod.endpointB = new Vector2(
                pivot.x + offsetX,
                pivot.y + length
            );

            pendulumBob.center = new Vector2(
                pendulumRod.endpointB.x,
                pendulumRod.endpointB.y
            );
        }
    }
}

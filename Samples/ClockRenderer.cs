using System;
using System.Collections.Generic;
using UnityEngine;

namespace EyE.Graphics
{
    [ExecuteAlways]
    public class ClockRenderer : MonoBehaviour
    {
        public DrawCommandListRenderer rendererRef;

        private CircleDrawCommand faceCircle;
        private PolarAngleSpanRadialLineArray majorTicks;
        private PolarAngleSpanRadialLineArray minorTicks;

        private RadialLine hourHand;
        private RadialLine minuteHand;
        private RadialLine secondHand;

        private bool initialized = false;

        private void Start()
        {
            if (rendererRef == null)
            {
                Debug.LogError("ClockRenderer: rendererRef missing.");
                return;
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            rendererRef.drawCommands.Clear();

            // ----- CLOCK FACE -----
            faceCircle = new CircleDrawCommand();
            faceCircle.center = new Vector2(0.5f, 0.5f);
            faceCircle.radius = 0.48f;
            faceCircle.color = new Color(1f, 1f, 1f, 1f);
            faceCircle.thickness = 0.015f;
            rendererRef.drawCommands.Add(faceCircle);

            // ----- MAJOR TICKS (12) -----
            majorTicks = new PolarAngleSpanRadialLineArray();
            majorTicks.prototype.center = new Vector2(0.5f, 0.5f);
            majorTicks.prototype.radius = 0.48f;
            majorTicks.prototype.startEndRadialFraction = new Vector2(0.80f, 1.00f);
            majorTicks.prototype.thickness = 0.015f;
            majorTicks.prototype.color = Color.black;
            majorTicks.totalAngleSpanTurns = 1.0f;   // full circle
            majorTicks.count = 12;
            rendererRef.drawCommands.Add(majorTicks);

            // ----- MINOR TICKS (60) -----
            minorTicks = new PolarAngleSpanRadialLineArray();
            minorTicks.prototype.center = new Vector2(0.5f, 0.5f);
            minorTicks.prototype.radius = 0.48f;
            minorTicks.prototype.startEndRadialFraction = new Vector2(0.90f, 1.00f);
            minorTicks.prototype.thickness = 0.006f;
            minorTicks.prototype.color = new Color(0, 0, 0, 0.5f);
            minorTicks.totalAngleSpanTurns = 1.0f;
            minorTicks.count = 60;
            rendererRef.drawCommands.Add(minorTicks);

            // ----- HOUR HAND -----
            hourHand = new RadialLine();
            hourHand.center = new Vector2(0.5f, 0.5f);
            hourHand.startEndRadialFraction = new Vector2(0.0f, 0.45f);
            hourHand.radius = 0.48f;
            hourHand.thickness = 0.03f;
            hourHand.color = Color.black;
            rendererRef.drawCommands.Add(hourHand);

            // ----- MINUTE HAND -----
            minuteHand = new RadialLine();
            minuteHand.center = new Vector2(0.5f, 0.5f);
            minuteHand.startEndRadialFraction = new Vector2(0.0f, 0.60f);
            minuteHand.radius = 0.48f;
            minuteHand.thickness = 0.02f;
            minuteHand.color = Color.black;
            rendererRef.drawCommands.Add(minuteHand);

            // ----- SECOND HAND -----
            secondHand = new RadialLine();
            secondHand.center = new Vector2(0.5f, 0.5f);
            secondHand.startEndRadialFraction = new Vector2(0.0f, 0.70f);
            secondHand.radius = 0.48f;
            secondHand.thickness = 0.01f;
            secondHand.color = Color.red;
            rendererRef.drawCommands.Add(secondHand);

            initialized = true;
            rendererRef.SetDirty();
        }

        private float nextUpdateTime = 0f;
        private const float UPDATE_INTERVAL = 0.05f; // 50ms (~20 FPS)
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

            if (Time.time < nextUpdateTime) return;
            nextUpdateTime = Time.time + UPDATE_INTERVAL;
            DateTime t = DateTime.Now;
            float hourTurns = (t.Hour % 12) * INV_12 + t.Minute * INV_720;
            float minuteTurns = t.Minute * INV_60 + t.Second * INV_3600;
            float secondTurns = t.Second * INV_60 + t.Millisecond * INV_60000;
            hourHand.angleInTurns = .25f-hourTurns;
            minuteHand.angleInTurns = .25f - minuteTurns;
            secondHand.angleInTurns = .25f - secondTurns;




            // simple day/night color swap
            if (t.Hour >= 18 || t.Hour < 6)
            {
                faceCircle.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            }
            else
            {
                faceCircle.color = new Color(1f, 1f, 1f, 1f);
            }

            rendererRef.SetDirty();
        }
    }
}

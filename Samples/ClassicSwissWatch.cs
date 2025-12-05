using System;
using UnityEngine;

namespace EyE.Graphics
{
    [ExecuteAlways]
    public class ClassicSwissWatch : MonoBehaviour
    {
        public DrawCommandListRenderer rendererRef;

        private DiskDrawCommand faceFill;
        private CircleDrawCommand outerRing;
        private DiskDrawCommand centerCap;

        private PolarAngleSpanRadialLineArray majorTicks;
        private PolarAngleSpanRadialLineArray minorTicks;

        private CapsuleDrawCommand hourBody;
        private CapsuleDrawCommand minuteBody;
        private CapsuleDrawCommand secondBody;
        private DiskDrawCommand secondCounterweight;

        private bool initialized = false;

        private void Start()
        {
            if (rendererRef == null)
            {
                Debug.LogError("ClassicSwissWatch: rendererRef missing.");
                return;
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            rendererRef.drawCommands.Clear();

            // ---- Face Fill ----
            faceFill = new DiskDrawCommand();
            faceFill.center = new Vector2(0.5f, 0.5f);
            faceFill.radius = 0.48f;
            faceFill.color = new Color(0.98f, 0.98f, 0.98f, 1f);
            rendererRef.drawCommands.Add(faceFill);

            // ---- Outer Metal Ring ----
            outerRing = new CircleDrawCommand();
            outerRing.center = new Vector2(0.5f, 0.5f);
            outerRing.radius = 0.48f;
            outerRing.thickness = 0.03f;
            outerRing.color = new Color(0.85f, 0.85f, 0.90f, 1f);
            rendererRef.drawCommands.Add(outerRing);

            // ---- Major Ticks (12) ----
            majorTicks = new PolarAngleSpanRadialLineArray();
            majorTicks.prototype.center = new Vector2(0.5f, 0.5f);
            majorTicks.prototype.radius = 0.48f;
            majorTicks.prototype.startEndRadialFraction = new Vector2(0.74f, 0.98f);
            majorTicks.prototype.thickness = 0.018f;
            majorTicks.prototype.color = Color.black;
            majorTicks.totalAngleSpanTurns = 1.0f;
            majorTicks.count = 12;
            rendererRef.drawCommands.Add(majorTicks);

            // ---- Minor Ticks (60) ----
            minorTicks = new PolarAngleSpanRadialLineArray();
            minorTicks.prototype.center = new Vector2(0.5f, 0.5f);
            minorTicks.prototype.radius = 0.48f;
            minorTicks.prototype.startEndRadialFraction = new Vector2(0.88f, 0.98f);
            minorTicks.prototype.thickness = 0.007f;
            minorTicks.prototype.color = new Color(0, 0, 0, 0.55f);
            minorTicks.totalAngleSpanTurns = 1.0f;
            minorTicks.count = 60;
            rendererRef.drawCommands.Add(minorTicks);

            // ---- Hour Hand ----
            hourBody = new CapsuleDrawCommand();
            hourBody.endpointA = new Vector2(0.5f, 0.5f);
            hourBody.endpointB = new Vector2(0.5f, 0.78f);
            hourBody.radius = 0.018f;
            hourBody.color = Color.black;
            rendererRef.drawCommands.Add(hourBody);

            // ---- Minute Hand ----
            minuteBody = new CapsuleDrawCommand();
            minuteBody.endpointA = new Vector2(0.5f, 0.5f);
            minuteBody.endpointB = new Vector2(0.5f, 0.88f);
            minuteBody.radius = 0.012f;
            minuteBody.color = Color.black;
            rendererRef.drawCommands.Add(minuteBody);

            // ---- Second Hand ----
            secondBody = new CapsuleDrawCommand();
            secondBody.endpointA = new Vector2(0.5f, 0.5f);
            secondBody.endpointB = new Vector2(0.5f, 0.94f);
            secondBody.radius = 0.006f;
            secondBody.color = Color.red;
            rendererRef.drawCommands.Add(secondBody);

            secondCounterweight = new DiskDrawCommand();
            secondCounterweight.center = new Vector2(0.5f, 0.5f) - (secondBody.endpointB- new Vector2(0.5f, 0.5f) )  * 0.4f;
            secondCounterweight.radius = 0.02f;
            secondCounterweight.color = Color.red;
            rendererRef.drawCommands.Add(secondCounterweight);

            // ---- Center Cap ----
            centerCap = new DiskDrawCommand();
            centerCap.center = new Vector2(0.5f, 0.5f);
            centerCap.radius = 0.025f;
            centerCap.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            rendererRef.drawCommands.Add(centerCap);

            initialized = true;
            rendererRef.SetDirty();
        }

        private static readonly float TWO_PI = Mathf.PI * 2f;
        private static readonly Vector2 CENTER = new Vector2(0.5f, 0.5f);
        private static readonly float HOUR_SCALE = 0.28f;
        private static readonly float MINUTE_SCALE = 0.38f;
        private static readonly float SECOND_SCALE = 0.44f;
        private static readonly float QUARTER = 0.25f;
        private float nextUpdateTime = 0f;
        private const float UPDATE_INTERVAL = 0.05f; // 50ms (~20 FPS)
        private const float INV_12 = 1f / 12f;
        private const float INV_720 = 1f / 720f;
        private const float INV_60 = 1f / 60f;
        private const float INV_3600 = 1f / 3600f;
        private const float INV_60000 = 1f / 60000f;
        private void Update()
        {
            if (!initialized) return;

            if (Time.time < nextUpdateTime) return;
            nextUpdateTime = Time.time + UPDATE_INTERVAL;

            DateTime t = DateTime.Now;

            float hourTurns = (t.Hour % 12) * INV_12 + t.Minute * INV_720;
            float minuteTurns = t.Minute * INV_60 + t.Second * INV_3600;
            float secondTurns = t.Second * INV_60 + t.Millisecond * INV_60000;



            float hourAngle = (QUARTER - hourTurns) * TWO_PI;
            float minuteAngle = (QUARTER - minuteTurns) * TWO_PI;
            float secondAngle = (QUARTER - secondTurns) * TWO_PI;

            // ---- Hour Hand ----
            hourBody.endpointA = CENTER;
            hourBody.endpointB = CENTER + new Vector2(Mathf.Cos(hourAngle), Mathf.Sin(hourAngle)) * HOUR_SCALE;

            // ---- Minute Hand ----
            minuteBody.endpointA = CENTER;
            minuteBody.endpointB = CENTER + new Vector2(Mathf.Cos(minuteAngle), Mathf.Sin(minuteAngle)) * MINUTE_SCALE;

            // ---- Second Hand ----
            Vector2 secondDir = new Vector2(Mathf.Cos(secondAngle), Mathf.Sin(secondAngle));
            secondBody.endpointA = CENTER - secondDir * (SECOND_SCALE * 0.5f);
            secondBody.endpointB = CENTER + secondDir * SECOND_SCALE;
            secondCounterweight.center = secondBody.endpointA;

            // ---- Face colors ----
            bool night = t.Hour >= 18 || t.Hour < 6;
            faceFill.color = night ? new Color(0.12f, 0.12f, 0.16f, 1f) : new Color(0.98f, 0.98f, 0.98f, 1f);
            outerRing.color = night ? new Color(0.55f, 0.55f, 0.60f, 1f) : new Color(0.85f, 0.85f, 0.90f, 1f);

            rendererRef.SetDirty();
        }
    }
}

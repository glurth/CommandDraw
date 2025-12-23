Shader "Unlit/DrawListFragShader"
{
    Properties
    {
        _BackgroundColor("BackgroundColor", Color) = (0,0,0,1)
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // ------------------------------------------------------------------
            // Command Type Constants
            // ------------------------------------------------------------------
            static const int CMD_LINE = 0;
            static const int CMD_ARC = 1;
            static const int CMD_DISK = 2;
            static const int CMD_RECT = 3; // axis-aligned filled rect
            static const int CMD_ORECT = 4; // oriented filled rect
            static const int CMD_RRECT = 5; // rounded filled rect
            static const int CMD_CAPSULE = 6;
            static const int CMD_TRIANGLE = 7;
            // ------------------------------------------------------------------
            // Blend Mode Constants
            // ------------------------------------------------------------------
            static const int BLEND_NORMAL = 0;
            static const int BLEND_ADDITIVE = 1;
            static const int BLEND_MULTIPLY = 2;


            // ------------------------------------------------------------------
            // Unified DrawCommand - every field documented per command
            // ------------------------------------------------------------------
            struct DrawCommand
            {
                float2 positionA;   // LINE:   endpoint A
                                    // ARC:    center
                                    // DISK:   center
                                    // RECT:   center
                                    // ORECT:  center
                                    // RRECT:  center
                                    // CAPSULE:endpoint A
                                    // TRIANGLE: point A

                float2 positionB;   // LINE:   endpoint B
                                    // ARC:    start/end angles packed: x = startAngle, y = endAngle
                                    // DISK:   (ignored)
                                    // RECT:   halfSize (x=halfWidth,y=halfHeight)
                                    // ORECT:  halfSize
                                    // RRECT:  halfSize
                                    // CAPSULE: endpoint B
                                    // TRIANGLE: point B

                float2 positionC;   // LINE:   (ignored)
                                    // ARC:    (ignored)
                                    // DISK:   (ignored)
                                    // RECT:   (ignored)
                                    // ORECT:  (ignored)
                                    // RRECT:  cornerRadii (x,y) 
                                    // CAPSULE:(ignored)
                                    // TRIANGLE: point C

                float  scalarA;     // ARC:    radius
                                    // DISK:   radius
                                    // ORECT:  rotation angle (radians)
                                    // CAPSULE: radius
                                    // others: ignored

                float4 color;
                float  thickness;   // stroke thickness for stroked shapes; ignored for filled
                int    commandType;
                int    objectID;
            };

            StructuredBuffer<DrawCommand> _Commands;
            float _AntiAliasingScalar;
            int _CommandCount;
            float4 _BackgroundColor;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float2 uv:TEXCOORD0; float4 vertex:SV_POSITION; };

            // ------------------------------------------------------------------
            // Utility SDF functions
            // ------------------------------------------------------------------
            float dist_Line(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float denom = dot(ba, ba);
                // avoid divide by zero
                if (denom == 0.0) return length(pa);
                float h = saturate(dot(pa, ba) / denom);
                return length(pa - ba * h);
            }

            float dist_Arc(float2 p, float2 center, float innerRadius, float outerRadius, float startAngle, float endAngle)
            {
                float2 d = p - center;
                float ang = atan2(d.y, d.x);
                if (ang < 0.0) ang += 6.28318530718;

                float s = startAngle;
                float e = endAngle;
                if (s < 0.0) s += 6.28318530718;
                if (e < 0.0) e += 6.28318530718;

                float pRadius = length(d);
                float radial=0;
                if (pRadius < innerRadius) radial = innerRadius - pRadius;
                else if (pRadius > outerRadius)radial = pRadius - outerRadius;
                


                bool insideSpan = false;
                if (s <= e)
                {
                    if (ang >= s && ang <= e) insideSpan = true;
                }
                else
                {
                    if (ang >= s || ang <= e) insideSpan = true;
                }

                if (insideSpan)
                {
                    return radial;
                }
                else
                {
                    return 1;
                    /*float midRadius = (innerRadius + outerRadius) / 2.0;
                    float2 e0 = center + midRadius * float2(cos(s), sin(s));
                    float2 e1 = center + midRadius * float2(cos(e), sin(e));
                    float d0 = length(p - e0);
                    float d1 = length(p - e1);
                    return min(d0, d1);*/
                }
            }

            float dist_Disk(float2 p, float2 center, float r)
            {
                return length(p - center) - r;
            }

            float dist_Rect(float2 p, float2 c, float2 half)
            {
                float2 d = abs(p - c) - half;
                float2 dpos = max(d, 0.0);
                float inner = min(max(d.x, d.y), 0.0);
                return max(0, inner + length(dpos));
            }


            float2 rotate2(float2 v, float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float2(c * v.x - s * v.y, s * v.x + c * v.y);
            }

            float dist_OrientedRect(float2 p, float2 c, float2 half, float angle)
            {
                float2 rp = rotate2(p - c, -angle);
                return dist_Rect(rp, float2(0.0, 0.0), half);
            }

            // Signed rounded-rect SDF where radius.x/radius.y are independent
            float dist_RoundedRect(float2 p, float2 c, float2 half, float2 rad)
            {
                float2 r = min(rad, half);            // clamp radii
                float2 ap = abs(p - c);               // first-quadrant position
                float2 inner = half - r;              // corner centers offset from rect corners
                float2 q = ap - inner;                // >0 means in corner region on that axis
                float2 outside = max(q, 0.0);

                // Corner region: compute distance to normalized ellipse (q / r)
                if (outside.x > 0.0 || outside.y > 0.0)
                {
                    // normalized offset inside ellipse-space (ellipse eqn: (x/rx)^2 + (y/ry)^2 == 1)
                    float2 en = outside / r;
                    float ne = length(en);            // ne == 1 on ellipse boundary

                    // normalized signed distance: ne - 1
                    // convert back to world units by scaling by a representative radius.
                    // min(r.x,r.y) is a cheap, stable choice that gives correct sign and magnitude order.
                    return (ne - 1.0) * min(r.x, r.y);
                }

                // Flat-edge / interior region: signed distance to inner box (<=0 inside)
                return max(q.x, q.y);
            }
            float dist_Capsule(float2 p, float2 a, float2 b, float r)
            {
                return dist_Line(p, a, b) - r;
            }

            float dist_Triangle(float2 p, float2 a, float2 b, float2 c)
            {
                float2 ba = b - a; float2 pa = p - a;
                float2 cb = c - b; float2 pb = p - b;
                float2 ac = a - c; float2 pc = p - c;

                float s1 = sign(dot(float2(ba.y, -ba.x), pa));
                float s2 = sign(dot(float2(cb.y, -cb.x), pb));
                float s3 = sign(dot(float2(ac.y, -ac.x), pc));

                float d1 = dist_Line(p, a, b);
                float d2 = dist_Line(p, b, c);
                float d3 = dist_Line(p, c, a);

                bool outside = (s1 + s2 + s3) < 2.0;

                if (outside)
                {
                    return min(min(d1, d2), d3);
                }
                else
                {
                    return -min(min(d1, d2), d3);
                }
            }


            // ------------------------------------------------------------------
            // Blend functions
            // ------------------------------------------------------------------

            float4 BlendNormal(float4 dst, float4 src, float alpha)
            {
                float finalAlpha = src.a * alpha;
                float3 srcRGB = src.rgb * finalAlpha;
                float srcA = finalAlpha;
                float3 outRGB = srcRGB + dst.rgb * (1.0 - srcA);
                float outA = srcA + dst.a * (1.0 - srcA);
                return float4(outRGB, outA);
            }

            float4 BlendAdditive(float4 dst, float4 src, float alpha)
            {
                float finalAlpha = src.a * alpha;
                return dst + src * finalAlpha;
            }

            float4 BlendMultiply(float4 dst, float4 src, float alpha)
            {
                float finalAlpha = src.a * alpha;
                return dst * (1.0 - finalAlpha) + src * finalAlpha;
            }

            // ------------------------------------------------------------------
            // Draw loop
            // ------------------------------------------------------------------

            // cmdInt contains both shape type (lower 4 bits) and blend mode (upper 4 bits)
            void UnpackCommandType(int cmdInt, out int shapeType, out int blendMode)
            {
                shapeType = cmdInt & 0xF;         // lower 4 bits
                blendMode = (cmdInt >> 4) & 0xF; // next 4 bits
            }
            float4 DrawCommands(float2 uv)
            {
                float4 outColor = float4(_BackgroundColor.rgb * _BackgroundColor.a, _BackgroundColor.a);
                float minD = 1e20;
                int lastID = -1;

                float pxAA = max(
                    max(abs(ddx(uv.x)), abs(ddy(uv.x))),
                    max(abs(ddx(uv.y)), abs(ddy(uv.y)))
                );
                pxAA *= _AntiAliasingScalar;
                pxAA = max(pxAA,  1e-7);

                for (int i = 0; i < _CommandCount; i++)
                {
                    DrawCommand cmd = _Commands[i];
                    float d = 0.0;
                    float halfT = cmd.thickness * 0.5;

                    int shapeType;// = cmd.commandType;
                    int blendMode;
                    UnpackCommandType(cmd.commandType, shapeType, blendMode);

                    if (shapeType == CMD_LINE)
                    {
                        d = dist_Line(uv, cmd.positionA, cmd.positionB);
                    }
                    else if (shapeType == CMD_ARC)
                    {
                        // start/end angles packed into positionB.x/y
                        d = dist_Arc(uv, cmd.positionA, cmd.scalarA-halfT, cmd.scalarA + halfT, cmd.positionB.x, cmd.positionB.y);
                    }
                    else if (shapeType == CMD_DISK)
                    {
                        d = dist_Disk(uv, cmd.positionA, cmd.scalarA);
                    }
                    else if (shapeType == CMD_RECT)
                    {
                        d = dist_Rect(uv, cmd.positionA, cmd.positionB);
                    }
                    else if (shapeType == CMD_ORECT)
                    {
                        d = dist_OrientedRect(uv, cmd.positionA, cmd.positionB, cmd.scalarA);
                    }
                    else if (shapeType == CMD_RRECT)
                    {
                        d = dist_RoundedRect(uv, cmd.positionA, cmd.positionB, cmd.positionC);
                    }
                    else if (shapeType == CMD_CAPSULE)
                    {
                        d = dist_Capsule(uv, cmd.positionA, cmd.positionB, cmd.scalarA);
                    }
                    else // CMD_TRIANGLE
                    {
                        d = dist_Triangle(uv, cmd.positionA, cmd.positionB, cmd.positionC);
                    }

                    int nextID = -999;
                    if (i + 1 < _CommandCount)
                    {
                        nextID = _Commands[i + 1].objectID;
                    }

                    d -= halfT;
                    if (cmd.objectID == lastID)
                    {
                        if (d < minD) minD = d;
                    }
                    else
                    {
                        minD = d;
                    }

                    if (cmd.objectID != nextID)
                    {
                        float alpha;
                        alpha = smoothstep(pxAA, 0, minD);
                        /*float finalAlpha = cmd.color.a * alpha;

                        // premultiplied stroke
                        float3 srcRGB = cmd.color.rgb * finalAlpha;
                        float  srcA = finalAlpha;

                        // premultiplied over existing outColor
                        outColor.rgb = srcRGB + outColor.rgb * (1.0 - srcA);
                        outColor.a = srcA + outColor.a * (1.0 - srcA);
                        */
                        if (blendMode == BLEND_NORMAL)
                            outColor = BlendNormal(outColor, cmd.color, alpha);
                        else if (blendMode == BLEND_ADDITIVE)
                            outColor = BlendAdditive(outColor, cmd.color, alpha);
                        else if (blendMode == BLEND_MULTIPLY)
                            outColor = BlendMultiply(outColor, cmd.color, alpha);

                        minD = 1e20;
                    }

                    lastID = cmd.objectID;
                }

                return outColor;
            }

            // ------------------------------------------------------------------
            // Vertex / Fragment
            // ------------------------------------------------------------------
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return DrawCommands(i.uv);
            }

            ENDCG
        }
    }
}

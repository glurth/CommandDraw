using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Graphics;

public sealed class NormalizedGraph<T> where T : struct, IConvertible
{
    private readonly List<float> values = new List<float>();

    private float min = float.PositiveInfinity;
    private float max = float.NegativeInfinity;

    public int Count
    {
        get { return values.Count; }
    }

    public void Append(T value)
    {
        float v = Convert.ToSingle(value);

        values.Add(v);

        if (v < min) min = v;
        if (v > max) max = v;
    }

    public List<DrawCommandBase> DrawGraph(float thickness, Color color)
    {
        int count = values.Count;
        List<DrawCommandBase> lines = new List<DrawCommandBase>(Mathf.Max(0, count - 1));

        if (count < 2)
        {
            return lines;
        }

        float range = max - min;
        if (range <= 0f)
        {
            range = 1f; // flat line safeguard
        }
        float oneOveRange = 1f / range;
        float invCountMinusOne = 1f / (count - 1);

        for (int i = 0; i < count - 1; i++)
        {
            float x0 = i * invCountMinusOne;
            float x1 = (i + 1) * invCountMinusOne;

            float y0 = (values[i] - min) * oneOveRange;
            float y1 = (values[i + 1] - min) * oneOveRange;

            lines.Add(new LineDrawCommand(new Vector2(x0, y0), new Vector2(x1, y1), thickness, color));
        }

        return lines;
    }
}
[ExecuteAlways]
public class NormlizedGraphDisplay : MonoBehaviour
{
    NormalizedGraph<float> graph;
    public DrawCommandListRenderer rendererRef;
    public float lineThickness = 0.01f;
    // Start is called before the first frame update
    void InitGraph()
    {
        graph = new NormalizedGraph<float>();
        for (float x = 0; x <= 5; x += 0.1f)
            graph.Append(Mathf.Sin(x));
    }
    void DrawGraph()
    {
        rendererRef.drawCommands = graph.DrawGraph(lineThickness, Color.red);
        rendererRef.SetDirty();
    }
    private void Start()
    {
        InitGraph();
        DrawGraph();
        if(Application.isPlaying)
            StartCoroutine(Animate());
    }
    private void OnValidate()
    {
        if (graph == null) InitGraph();
        DrawGraph();
    }
    bool loop = true;
    IEnumerator Animate()
    {
        float ticks = 0f;
        while (loop)
        {   
            graph.Append(Mathf.Sin(ticks));
            ticks += 0.1f;
            DrawGraph();
            yield return new WaitForSeconds(0.2f);
        }
    }
}

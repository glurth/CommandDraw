using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
/// <summary>
/// Central renderer that converts high-level <see cref="DrawCommandBase"/> definitions into
/// packed GPU-ready draw commands, uploads them to a shader, and renders the result into a
/// managed <see cref="RenderTexture"/> for display. Handles texture allocation, command
/// expansion, buffer lifecycle, shader setup, and edit-mode reactivity under
/// <see cref="ExecuteAlways"/>.
/// </summary>
public class DrawCommandListRenderer : MonoBehaviour
{


    /// <summary>
    /// Ordered list of user-authored draw command definitions. Each item expands into one or more packed GPU-ready commands.
    /// </summary>
    [SerializeReference]
    public List<DrawCommandBase> drawCommands = new List<DrawCommandBase>();

    /// <summary>
    /// Flattened list of packed draw commands generated from <see cref="drawCommands"/>. Sent directly to the shader.
    /// </summary>
    protected List<PackedDrawCommand> generatedDrawList = new List<PackedDrawCommand>();

    /// <summary>
    /// Background color passed to the fragment shader.
    /// </summary>
    public Color backgroundColor = Color.white;

    public float antiAliasingScalar=1f;

    /// <summary>
    /// Material instance created from <see cref="ShaderName"/>. Used during blit to render all commands.
    /// </summary>
    Material drawListFragShaderBasedMaterial;

    #region renderTextureStuff

    /// <summary>
    /// Material whose <c>_MainTex</c> is updated with the generated render texture for display.
    /// </summary>
    public Material textureBasedDisplayMaterial;

    /// <summary>
    /// The render target into which the draw commands are rasterized.
    /// </summary>
    RenderTexture targetRenderTexture;

    /// <summary>
    /// Returns the texture size used for the render target. Override to control resolution.
    /// </summary>
    protected virtual Vector2Int GetRenderTextureSize()
    {
        return new Vector2Int(Screen.width, Screen.height);
    }

    /// <summary>
    /// Allocates or resizes <see cref="targetRenderTexture"/> when needed.
    /// </summary>
    private void SetupRenderTexture()
    {
        Vector2Int rtSize = GetRenderTextureSize();
        if (targetRenderTexture == null || targetRenderTexture.width != rtSize.x || targetRenderTexture.height != rtSize.y)
        {
            if (targetRenderTexture != null) targetRenderTexture.Release();

            targetRenderTexture = new RenderTexture(rtSize.x, rtSize.y, 0, RenderTextureFormat.ARGB32);
            targetRenderTexture.enableRandomWrite = false;
            targetRenderTexture.Create();
        }
    }

    /// <summary>
    /// Renders the draw command list into <see cref="targetRenderTexture"/>.
    /// </summary>
    private void RenderToTexture()
    {
        SetupRenderTexture();
        Graphics.Blit(null, targetRenderTexture, drawListFragShaderBasedMaterial);
    }

    /// <summary>
    /// Assigns <see cref="targetRenderTexture"/> to the display material.
    /// </summary>
    private void UpdateDisplayTexture()
    {
        textureBasedDisplayMaterial.SetTexture("_MainTex", targetRenderTexture);
    }

    #endregion

    /// <summary>
    /// GPU buffer holding packed draw commands for the shader.
    /// </summary>
    ComputeBuffer drawCommandBuffer;

    /// <summary>
    /// The name of the shader used to render the draw list. Override to substitute a custom shader.
    /// </summary>
    protected virtual string ShaderName => "Unlit/DrawListFragShader";

    /// <summary>
    /// Finds the shader and constructs the working material.
    /// </summary>
    private void Awake()
    {
        #if UNITY_EDITOR
        Cleanup(); // free old buffer if present
        if (drawListFragShaderBasedMaterial != null)
            DestroyImmediate(drawListFragShaderBasedMaterial);
        #endif
        CreateDrawShaderMaterialIfNeeded();
    }

    private void CreateDrawShaderMaterialIfNeeded()
    {
        if (drawListFragShaderBasedMaterial != null) return;
        Shader drawListFragShader = Shader.Find(ShaderName);
        if (drawListFragShader == null)
            Debug.LogError("Unable to find shader `" + ShaderName + "`");
        else
            Debug.Log("Found shader `" + ShaderName + "`");

        drawListFragShaderBasedMaterial = new Material(drawListFragShader);
    }

    /// <summary>
    /// Converts all <see cref="drawCommands"/> into GPU-ready <see cref="PackedDrawCommand"/> items.
    /// Logs expanded detail for diagnostic visibility.
    /// </summary>
    private void GenPackedDrawCommands()
    {
        generatedDrawList.Clear();
        int i = 0;

        foreach (DrawCommandBase command in drawCommands)
        {
            List<PackedDrawCommand> commands = command.ToDrawCommands(i);

           /* Debug.Log("Command " + i + " type:" + command.GetType().Name + "   num sub-commands: " + commands.Count);

            int commandIndex = 0;
            foreach (PackedDrawCommand drawCommand in commands)
            {
                Debug.LogFormat(
                    "   Command {0} | Type:{1} | Start:{2} | End:{3} | ThirdCorner:{4} | Color:{5} | Radius:{6} | Thickness:{7} | ObjectID:{8}",
                    commandIndex++,
                    drawCommand.commandType,
                    drawCommand.start,
                    drawCommand.end,
                    drawCommand.thirdCorner,
                    drawCommand.color,
                    drawCommand.radius,
                    drawCommand.thickness,
                    drawCommand.objectID
                );
            }
           */
            generatedDrawList.AddRange(commands);
            i++;
        }
    }

    /// <summary>
    /// Optional extension point invoked after <see cref="GenPackedDrawCommands"/> completes.
    /// Override for additional processing.
    /// </summary>
    protected virtual void PostGenPackedDrawCommands() { }

    /// <summary>
    /// Uploads <see cref="generatedDrawList"/> to the shader via <see cref="ComputeBuffer"/> and sets uniforms.
    /// </summary>
    private void SendDataToShader()
    {
        if (generatedDrawList != null && generatedDrawList.Count > 0)
        {
            PackedDrawCommand[] commands = generatedDrawList.ToArray();
            drawCommandBuffer = new ComputeBuffer(commands.Length, PackedDrawCommand.STRIDE);
            drawCommandBuffer.SetData(commands);

            drawListFragShaderBasedMaterial.SetColor("_BackgroundColor", backgroundColor);
            drawListFragShaderBasedMaterial.SetBuffer("_Commands", drawCommandBuffer);
            drawListFragShaderBasedMaterial.SetInt("_CommandCount", commands.Length);
            drawListFragShaderBasedMaterial.SetFloat("_AntiAliasingScalar", antiAliasingScalar);
        }
        else
        {
            Debug.LogWarning("Zero draw commands detected: aborting send to shader operation");
        }
    }


    private bool doRenderOnUpdate = false;

    /// <summary>
    /// Automatically renders when the object becomes enabled.
    /// </summary>
    private void OnEnable()
    {
        //DoRender();
        doRenderOnUpdate = true;
    }

    /// <summary>
    /// Full render pipeline: expand commands, post-process, upload, draw, display, cleanup.
    /// </summary>
    private void DoRender()
    {
        CreateDrawShaderMaterialIfNeeded();
        GenPackedDrawCommands();
        PostGenPackedDrawCommands();
        SendDataToShader();
        RenderToTexture();
        UpdateDisplayTexture();
        Cleanup();
    }

    private void Update()
    {
        if (doRenderOnUpdate)
            DoRender();
        doRenderOnUpdate = false;
    }

    /// <summary>
    /// Releases the compute buffer. Keeps GPU resources clean in edit and play mode.
    /// </summary>
    private void Cleanup()
    {
        if (drawCommandBuffer != null)
        {
            drawCommandBuffer.Release();
            drawCommandBuffer = null;
        }
    }

    /// <summary>
    /// In edit mode, re-renders when fields change.
    /// </summary>
    private void OnValidate()
    {
        if (gameObject.activeInHierarchy && enabled) //&& Application.isPlaying)
            doRenderOnUpdate = true;// DoRender();
    }

    private void OnDisable()
    {
        Cleanup();
    }

    private void OnDestroy()
    {
        Cleanup();
        #if UNITY_EDITOR
                if (drawListFragShaderBasedMaterial != null)
                    DestroyImmediate(drawListFragShaderBasedMaterial);
        #else
            if (drawListFragShaderBasedMaterial != null)
                Destroy(drawListFragShaderBasedMaterial);
        #endif
    }
}
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using EyE.Graphics;

namespace EyE.Editor.Graphics
{
    [CustomEditor(typeof(DrawCommandListRendererPixelBased))]
    public class DrawCommanderPixelBasedEditor : DrawCommanderEditor
    {
        SerializedProperty textureSizeProp;
        override protected void OnEnable()
        {
            base.OnEnable();
            textureSizeProp = serializedObject.FindProperty("textureSize");
        }
        protected override void DrawPreListProperties()
        {
            EditorGUILayout.PropertyField(textureSizeProp);
        }
        protected override Vector2Int TexSize => ((DrawCommandListRendererPixelBased)target).textureSize;
    }
}
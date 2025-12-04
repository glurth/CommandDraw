using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DrawCommandListRenderer))]
public class DrawCommanderEditor : Editor
{
    ReorderableList list;
    SerializedProperty commandsProp;
    SerializedProperty backgroundColorProp;
    SerializedProperty textueBasedMaterialProp;
    SerializedProperty antiAliasingScalarProp;
    private static readonly System.Type[] commandTypes = new System.Type[]
    {
        typeof(LineDrawCommand),
        typeof(CurveDrawCommand),
        typeof(PolyLineDrawCommand),
        typeof(CircleDrawCommand),
        typeof(ArcDrawCommand),
        typeof(DiskDrawCommand),
        typeof(RectDrawCommand),  
        typeof(OrientedRectDrawCommand),  
        typeof(RoundedRectDrawCommand), 
        typeof(CapsuleDrawCommand), 
        typeof(TriangleDrawCommand),
        typeof(RadialLine)
    };

    virtual protected void OnEnable()
    {
        commandsProp = serializedObject.FindProperty("drawCommands");
        backgroundColorProp = serializedObject.FindProperty("backgroundColor");
        textueBasedMaterialProp = serializedObject.FindProperty("textureBasedDisplayMaterial");
        antiAliasingScalarProp = serializedObject.FindProperty("antiAliasingScalar");
        list = new ReorderableList(serializedObject, commandsProp, true, true, true, true);

        list.drawHeaderCallback = (Rect r) =>
        {
            EditorGUI.LabelField(r, "Draw Commands");
        };

        list.elementHeightCallback = (int index) =>
        {
            var element = commandsProp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + 6;
        };

        list.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
        {
            const float indent = 16f;              // shifts the object right
            Rect r = new Rect(
                rect.x + indent,
                rect.y,
                rect.width - indent,
                EditorGUIUtility.singleLineHeight
            );

            SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
            // Extract readable type name
            string fullName = element.managedReferenceFullTypename;

            string typeName = fullName;//.Substring(fullName.LastIndexOf('.') + 1);
            typeName = typeName.Replace("Assembly-CSharp ", "");
            typeName = typeName.Replace("DrawCommand", "");    // Curve, Line, PolyLine
            EditorGUI.PropertyField(r, element,new GUIContent(typeName), true);

        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            GenericMenu menu = new GenericMenu();

            foreach (var t in commandTypes)
            {
                string label = t.Name.Replace("DrawCommand", "");   // nicer menu labels
                menu.AddItem(new GUIContent(label), false, () => AddCommand(t));
            }

            menu.ShowAsContext();
        };
    }

    protected virtual Vector2Int TexSize=>Vector2Int.zero;
    void AddCommand(System.Type t)
    {
        serializedObject.Update();

        int index = commandsProp.arraySize;
        commandsProp.InsertArrayElementAtIndex(index);

        SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
        DrawCommandBase newElement = (DrawCommandBase)System.Activator.CreateInstance(t);
        newElement.InitializeData(TexSize);
        element.managedReferenceValue = newElement;// System.Activator.CreateInstance(t);
        /*if (t == typeof(LineDrawCommand))
        {
            element.managedReferenceValue = new LineDrawCommand();
        }
        else if (t == typeof(CurveDrawCommand))
        {
            element.managedReferenceValue = new CurveDrawCommand();
        }
        else if (t == typeof(PolyLineDrawCommand))
        {
            element.managedReferenceValue = new PolyLineDrawCommand();
        }
        else if (t == typeof(CircleDrawCommand))
        {
            element.managedReferenceValue = new CircleDrawCommand();
        }
        else
        {
            Debug.LogError("Unknown command type: " + t);
        }*/

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawPreListProperties()
    {
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(backgroundColorProp);
        EditorGUILayout.PropertyField(antiAliasingScalarProp);
        EditorGUILayout.PropertyField(textueBasedMaterialProp);

        DrawPreListProperties();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

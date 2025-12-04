using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using EyE.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EyE.Editor.Graphics
{
    [CustomEditor(typeof(DrawCommandListRenderer))]
    public class DrawCommanderEditor : UnityEditor.Editor
    {
        ReorderableList list;
        SerializedProperty commandsProp;
        SerializedProperty backgroundColorProp;
        SerializedProperty textueBasedMaterialProp;
        SerializedProperty antiAliasingScalarProp;

        private static System.Type[] commandTypes=null;

        System.Type[] CommandTypes
        {
            get
            {
                if (commandTypes != null)
                    return commandTypes;

                List<Type> types = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] assemblyTypes;
                    try
                    {
                        assemblyTypes = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        assemblyTypes = e.Types;
                    }

                    if (assemblyTypes == null)
                        continue;

                    foreach (Type t in assemblyTypes)
                    {
                        if (t == null)
                            continue;

                        if (typeof(DrawCommandBase).IsAssignableFrom(t) && !t.IsAbstract)
                            types.Add(t);
                    }
                }

                commandTypes = types.ToArray();
                return commandTypes;
            }
        }
        virtual protected void OnEnable()
        {
            commandsProp = serializedObject.FindProperty("drawCommands");
            backgroundColorProp = serializedObject.FindProperty("backgroundColor");
            textueBasedMaterialProp = serializedObject.FindProperty("textureBasedDisplayMaterial");
            antiAliasingScalarProp = serializedObject.FindProperty("antiAliasingScalar");

            list = new ReorderableList(serializedObject, commandsProp, true, true, true, true);

            list.drawHeaderCallback = r => EditorGUI.LabelField(r, "Draw Commands");

            list.elementHeightCallback = delegate (int index)
            {
                SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 6;
            };


            list.drawElementCallback = delegate (Rect rect, int index, bool active, bool focused)
            {
                const float indent = 16f;
                Rect r = new Rect(rect.x + indent, rect.y, rect.width - indent, EditorGUIUtility.singleLineHeight);

                SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
                string fullName = element.managedReferenceFullTypename ?? "";
                int lastDot = fullName.LastIndexOf('.');
                string typeName = lastDot >= 0 ? fullName.Substring(lastDot + 1) : fullName;
                typeName = typeName.Replace("DrawCommand", "");
                EditorGUI.PropertyField(r, element, new GUIContent(typeName), true);
            };


            list.onAddDropdownCallback = delegate (Rect buttonRect, ReorderableList l)
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < CommandTypes.Length; i++)
                {
                    Type t = CommandTypes[i];
                    string label = t.Name.Replace("DrawCommand", "");
                    menu.AddItem(new GUIContent(label), false, () => AddCommand(t));
                }
                menu.ShowAsContext();
            };
        }

        protected virtual Vector2Int TexSize => Vector2Int.zero;

        void AddCommand(Type t)
        {
            serializedObject.Update();

            int index = commandsProp.arraySize;
            commandsProp.InsertArrayElementAtIndex(index);

            SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
            DrawCommandBase newElement = (DrawCommandBase)Activator.CreateInstance(t);
            newElement.InitializeData(TexSize);
            element.managedReferenceValue = newElement;

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawPreListProperties() { }

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
}


/*using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using EyE.Graphics;

namespace EyE.Editor.Graphics
{
    [CustomEditor(typeof(DrawCommandListRenderer))]
    public class DrawCommanderEditor : UnityEditor.Editor
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
        typeof(RadialLine),
        typeof(PolarRadialLineArray)
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
            EditorGUI.PropertyField(r, element, new GUIContent(typeName), true);

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

        protected virtual Vector2Int TexSize => Vector2Int.zero;
        void AddCommand(System.Type t)
        {
            serializedObject.Update();

            int index = commandsProp.arraySize;
            commandsProp.InsertArrayElementAtIndex(index);

            SerializedProperty element = commandsProp.GetArrayElementAtIndex(index);
            DrawCommandBase newElement = (DrawCommandBase)System.Activator.CreateInstance(t);
            newElement.InitializeData(TexSize);
            element.managedReferenceValue = newElement;// System.Activator.CreateInstance(t);


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
}*/
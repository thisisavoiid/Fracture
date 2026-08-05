using System;
using System.Collections.Generic;
using ToolkitByJonathan;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryPropertyDrawer : PropertyDrawer
{
    private static GUIStyle _wrappedTextAreaStyle = new GUIStyle(EditorStyles.textArea)
    {
        wordWrap = true
    };
    private static readonly Dictionary<Type, Action<SerializedProperty>> _drawers =
    new()
    {
        { typeof(string), prop => prop.stringValue = EditorGUILayout.TextArea(prop.stringValue, _wrappedTextAreaStyle, GUILayout.Width(150))},
        { typeof(int),    prop => prop.intValue = EditorGUILayout.IntField(prop.intValue, GUILayout.Width(150)) },
        { typeof(float),  prop => prop.floatValue = EditorGUILayout.FloatField(prop.floatValue, GUILayout.Width(150)) },
        { typeof(bool),   prop => prop.boolValue = EditorGUILayout.Toggle(prop.boolValue, GUILayout.Width(150)) },
    };

    private bool _isExpanded = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var keys = property.FindPropertyRelative("Keys");
        var values = property.FindPropertyRelative("Values");

        if (keys == null || values == null)
        {
            EditorGUILayout.HelpBox("Keys or Values array is null!", MessageType.Error);
            return;
        }

        Type keysType = keys.arrayElementType.GetType();
        Type valuesType = values.arrayElementType.GetType();

        EditorGUI.BeginProperty(position, label, property);

        _isExpanded = EditorGUILayout.Foldout(_isExpanded, property.name);

        if (_isExpanded)
        {
            EditorGUILayout.BeginVertical("box");

            int size = Mathf.Min(values.arraySize, keys.arraySize);

            for (int i = 0; i < size; i++)
            {
                SerializedProperty currentValue = values.GetArrayElementAtIndex(i);
                SerializedProperty currentKey = keys.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Key", EditorStyles.miniLabel, GUILayout.Width(150));
                if (_drawers.TryGetValue(keysType, out var keyDrawer))
                {
                    keyDrawer(currentKey);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Value", EditorStyles.miniLabel, GUILayout.Width(150));
                if (_drawers.TryGetValue(valuesType, out var valueDrawer))
                {
                    valueDrawer(currentValue);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    keys.DeleteArrayElementAtIndex(i);
                    values.DeleteArrayElementAtIndex(i);
                    property.serializedObject.ApplyModifiedProperties();
                    break;
                }

                DrawHelper.DrawSeperator();
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("+"))
            {
                keys.InsertArrayElementAtIndex(keys.arraySize);
                values.InsertArrayElementAtIndex(values.arraySize);
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUI.EndProperty();
    }
}
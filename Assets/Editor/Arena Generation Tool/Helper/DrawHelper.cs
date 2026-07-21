using UnityEditor;
using UnityEngine;

public static class DrawHelper
{
    public static void DrawHeader(string text)
    {
        EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
    }

    public static void DrawSeperator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimation))]
public class UIAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIAnimation uiAnimation = (UIAnimation)target;

        // Draw the default inspector for MyScript.
        DrawDefaultInspector();

        // Add a button to the inspector.
        if (GUILayout.Button("Get Text Mesh Components"))
        {
            // Do something when the button is clicked.
            uiAnimation.GetTextMeshComponents();
        }
    }
}

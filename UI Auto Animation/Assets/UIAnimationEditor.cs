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

        if (GUILayout.Button("Fade In"))
        {
            // Do something when the button is clicked.
            uiAnimation.FadeIn();
        }

        // Add a button to the inspector.
        if (GUILayout.Button("Fade Out"))
        {
            // Do something when the button is clicked.
            uiAnimation.FadeOut();
        }
    }
}

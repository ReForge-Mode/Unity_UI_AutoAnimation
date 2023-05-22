using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This Editor just displays two buttons to trigger the animation on Play Mode for debugging purposes
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(UIAutoAnimation))]
public class UIAutoAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIAutoAnimation uiAnimation = (UIAutoAnimation)target;

        DrawDefaultInspector();


        EditorGUILayout.Space(20);
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Entrance Animation", GUILayout.Height(40)))
            {
                uiAnimation.EntranceAnimation();
            }

            if (GUILayout.Button("Exit Animation", GUILayout.Height(40)))
            {
                uiAnimation.ExitAnimation();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();




        //Hide this message on Play Mode
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("You must be in Play Mode to interact with these buttons", MessageType.Info);
        }
    }
}
#endif
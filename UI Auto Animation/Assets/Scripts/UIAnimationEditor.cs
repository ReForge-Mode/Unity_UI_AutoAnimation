using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(UIAnimation))]
public class UIAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIAnimation uiAnimation = (UIAnimation)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Entrance Animation"))
        {
            uiAnimation.EntranceAnimation();
        }

        if (GUILayout.Button("Exit Animation"))
        {
            uiAnimation.ExitAnimation();
        }
    }
}
#endif
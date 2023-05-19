using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(SOAnimationPresets))]
public class YourScriptNameEditor : Editor
{
    SerializedProperty durationProp;
    SerializedProperty delayPerElementProp;
    SerializedProperty curveAlphaProp;
    SerializedProperty offsetPositionProp;
    SerializedProperty curvePositionProp;
    SerializedProperty offsetScaleProp;
    SerializedProperty curveScaleProp;
    SerializedProperty offsetRotationProp;
    SerializedProperty curveRotationProp;

    bool showFadeInOutAlpha = true;
    bool showFloatInOutPosition = true;
    bool showZoomInOutScale = true;
    bool showSpinInOutRotation = true;

    void OnEnable()
    {
        durationProp = serializedObject.FindProperty("duration");
        delayPerElementProp = serializedObject.FindProperty("delayPerElement");
        curveAlphaProp = serializedObject.FindProperty("curveAlpha");
        offsetPositionProp = serializedObject.FindProperty("offsetPosition");
        curvePositionProp = serializedObject.FindProperty("curvePosition");
        offsetScaleProp = serializedObject.FindProperty("offsetScale");
        curveScaleProp = serializedObject.FindProperty("curveScale");
        offsetRotationProp = serializedObject.FindProperty("offsetRotation");
        curveRotationProp = serializedObject.FindProperty("curveRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        durationProp.floatValue = EditorGUILayout.FloatField("Duration", durationProp.floatValue);
        delayPerElementProp.floatValue = EditorGUILayout.FloatField("Delay Per Element", delayPerElementProp.floatValue);


        EditorGUILayout.Space(20);
        showFadeInOutAlpha = EditorGUILayout.Toggle("Use Alpha Transition", showFadeInOutAlpha);
        EditorGUI.BeginDisabledGroup(!showFadeInOutAlpha);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(curveAlphaProp, new GUIContent("Curve Alpha"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        showFloatInOutPosition = EditorGUILayout.Toggle("Use Position Transition", showFloatInOutPosition);
        EditorGUI.BeginDisabledGroup(!showFloatInOutPosition);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(offsetPositionProp, new GUIContent("Offset Position"));
            EditorGUILayout.PropertyField(curvePositionProp, new GUIContent("Curve Position"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        showZoomInOutScale = EditorGUILayout.Toggle("Use Scale Transition", showZoomInOutScale);
        EditorGUI.BeginDisabledGroup(!showZoomInOutScale);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(offsetScaleProp, new GUIContent("Offset Scale"));
            EditorGUILayout.PropertyField(curveScaleProp, new GUIContent("Curve Scale"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        showSpinInOutRotation = EditorGUILayout.Toggle("Use Rotation Transition", showSpinInOutRotation);
        EditorGUI.BeginDisabledGroup(!showSpinInOutRotation);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(offsetRotationProp, new GUIContent("Offset Rotation"));
            EditorGUILayout.PropertyField(curveRotationProp, new GUIContent("Curve Rotation"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
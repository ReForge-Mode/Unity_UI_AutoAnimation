using UnityEditor;
using UnityEngine;

/// <summary>
/// This Editor disable elements as the boolean is enabled or disabled
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(SOAnimationPresets))]
public class SOAnimationPresetsEditor : Editor
{
    private SerializedProperty duration;
    private SerializedProperty delayPerElement;

    private SerializedProperty useAlphaAnimation;
    private SerializedProperty alphaDuration;
    private SerializedProperty curveAlpha;

    private SerializedProperty usePositionAnimation;
    private SerializedProperty positionDuration;
    private SerializedProperty offsetPosition;
    private SerializedProperty curvePosition;

    private SerializedProperty useScaleAnimation;
    private SerializedProperty scaleDuration;
    private SerializedProperty offsetScale;
    private SerializedProperty curveScale;

    private SerializedProperty useRotationAnimation;
    private SerializedProperty rotationDuration;
    private SerializedProperty offsetRotation;
    private SerializedProperty curveRotation;

    private void OnEnable()
    {
        duration = serializedObject.FindProperty("duration");
        delayPerElement = serializedObject.FindProperty("delayPerElement");

        useAlphaAnimation = serializedObject.FindProperty("useAlphaAnimation");
        alphaDuration = serializedObject.FindProperty("alphaDuration");
        curveAlpha = serializedObject.FindProperty("curveAlpha");

        usePositionAnimation = serializedObject.FindProperty("usePositionAnimation");
        positionDuration = serializedObject.FindProperty("positionDuration");
        offsetPosition = serializedObject.FindProperty("offsetPosition");
        curvePosition = serializedObject.FindProperty("curvePosition");

        useScaleAnimation = serializedObject.FindProperty("useScaleAnimation");
        scaleDuration = serializedObject.FindProperty("scaleDuration");
        offsetScale = serializedObject.FindProperty("offsetScale");
        curveScale = serializedObject.FindProperty("curveScale");

        useRotationAnimation = serializedObject.FindProperty("useRotationAnimation");
        rotationDuration = serializedObject.FindProperty("rotationDuration");
        offsetRotation = serializedObject.FindProperty("offsetRotation");
        curveRotation = serializedObject.FindProperty("curveRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        duration.floatValue = EditorGUILayout.FloatField("Duration", duration.floatValue);
        delayPerElement.floatValue = EditorGUILayout.FloatField("Delay Per Element", delayPerElement.floatValue);


        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useAlphaAnimation, new GUIContent("Use Alpha Animation"));
        EditorGUI.BeginDisabledGroup(!useAlphaAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(alphaDuration);
            EditorGUILayout.PropertyField(curveAlpha);
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(usePositionAnimation, new GUIContent("Use Position Animation"));
        EditorGUI.BeginDisabledGroup(!usePositionAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(positionDuration);
            EditorGUILayout.PropertyField(offsetPosition);
            EditorGUILayout.PropertyField(curvePosition);
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useScaleAnimation, new GUIContent("Use Scale Animation"));
        EditorGUI.BeginDisabledGroup(!useScaleAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(scaleDuration);
            EditorGUILayout.PropertyField(offsetScale);
            EditorGUILayout.PropertyField(curveScale);
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useRotationAnimation, new GUIContent("Use Rotation Animation"));
        EditorGUI.BeginDisabledGroup(!useRotationAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationDuration);
            EditorGUILayout.PropertyField(offsetRotation);
            EditorGUILayout.PropertyField(curveRotation);
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(20);
        EditorGUILayout.HelpBox("All Animation Curves must start at value 0 and ends in value 1, " +
                                "even if it is used for fade out transition. " +
                                "It is used in interpolation, not for setting values", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
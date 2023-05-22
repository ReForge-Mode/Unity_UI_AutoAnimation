using UnityEditor;
using UnityEngine;

/// <summary>
/// This Editor disable elements as the boolean is enabled or disabled
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(SOAnimationPresets))]
public class SOAnimationPresetsEditor : Editor
{
    private SerializedProperty useAlphaAnimation;
    private SerializedProperty alphaDelay;
    private SerializedProperty alphaDuration;
    private SerializedProperty delayPerElementAlpha;
    private SerializedProperty curveAlpha;

    private SerializedProperty usePositionAnimation;
    private SerializedProperty positionDelay;
    private SerializedProperty positionDuration;
    private SerializedProperty delayPerElementPosition;
    private SerializedProperty offsetPosition;
    private SerializedProperty curvePosition;

    private SerializedProperty useScaleAnimation;
    private SerializedProperty scaleDelay;
    private SerializedProperty scaleDuration;
    private SerializedProperty delayPerElementScale;
    private SerializedProperty offsetScale;
    private SerializedProperty curveScale;

    private SerializedProperty useRotationAnimation;
    private SerializedProperty rotationDelay;
    private SerializedProperty rotationDuration;
    private SerializedProperty delayPerElementRotation;
    private SerializedProperty offsetRotation;
    private SerializedProperty curveRotation;

    private SerializedProperty autoTrigger;
    private SerializedProperty autoTriggerTimer;

    private void OnEnable()
    {
        useAlphaAnimation = serializedObject.FindProperty("useAlphaAnimation");
        alphaDelay = serializedObject.FindProperty("alphaDelay");
        alphaDuration = serializedObject.FindProperty("alphaDuration");
        delayPerElementAlpha = serializedObject.FindProperty("delayPerElementAlpha");
        curveAlpha = serializedObject.FindProperty("curveAlpha");

        usePositionAnimation = serializedObject.FindProperty("usePositionAnimation");
        positionDelay = serializedObject.FindProperty("positionDelay");
        positionDuration = serializedObject.FindProperty("positionDuration");
        delayPerElementPosition = serializedObject.FindProperty("delayPerElementPosition");
        offsetPosition = serializedObject.FindProperty("offsetPosition");
        curvePosition = serializedObject.FindProperty("curvePosition");

        useScaleAnimation = serializedObject.FindProperty("useScaleAnimation");
        scaleDelay = serializedObject.FindProperty("scaleDelay");
        scaleDuration = serializedObject.FindProperty("scaleDuration");
        delayPerElementScale = serializedObject.FindProperty("delayPerElementScale");
        offsetScale = serializedObject.FindProperty("offsetScale");
        curveScale = serializedObject.FindProperty("curveScale");

        useRotationAnimation = serializedObject.FindProperty("useRotationAnimation");
        rotationDelay = serializedObject.FindProperty("rotationDelay");
        rotationDuration = serializedObject.FindProperty("rotationDuration");
        delayPerElementRotation = serializedObject.FindProperty("delayPerElementRotation");
        offsetRotation = serializedObject.FindProperty("offsetRotation");
        curveRotation = serializedObject.FindProperty("curveRotation");

        autoTrigger = serializedObject.FindProperty("autoTrigger");
        autoTriggerTimer = serializedObject.FindProperty("autoTriggerTimer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        //USE ALPHA ANIMATION
        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useAlphaAnimation, new GUIContent("Use Alpha Animation"));
        EditorGUI.BeginDisabledGroup(!useAlphaAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(alphaDelay, new GUIContent("Delay"));
            EditorGUILayout.PropertyField(alphaDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(delayPerElementAlpha, new GUIContent("Delay Per Element"));
            EditorGUILayout.PropertyField(curveAlpha, new GUIContent("Interpolation Curve"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        //USE POSITION ANIMATION
        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(usePositionAnimation, new GUIContent("Use Position Animation"));
        EditorGUI.BeginDisabledGroup(!usePositionAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(positionDelay, new GUIContent("Delay"));
            EditorGUILayout.PropertyField(positionDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(delayPerElementPosition, new GUIContent("Delay Per Element"));
            EditorGUILayout.PropertyField(offsetPosition, new GUIContent("Offset Position"));
            EditorGUILayout.PropertyField(curvePosition, new GUIContent("Interpolation Curve"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        //USE SCALE ANIMATION
        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useScaleAnimation, new GUIContent("Use Scale Animation"));
        EditorGUI.BeginDisabledGroup(!useScaleAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(scaleDelay, new GUIContent("Delay"));
            EditorGUILayout.PropertyField(scaleDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(delayPerElementScale, new GUIContent("Delay Per Element"));
            EditorGUILayout.PropertyField(offsetScale, new GUIContent("Offset Scale"));
            EditorGUILayout.PropertyField(curveScale, new GUIContent("Interpolation Curve"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        //USE ROTATION ANIMATION
        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(useRotationAnimation, new GUIContent("Use Rotation Animation"));
        EditorGUI.BeginDisabledGroup(!useRotationAnimation.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationDelay, new GUIContent("Delay"));
            EditorGUILayout.PropertyField(rotationDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(delayPerElementRotation, new GUIContent("Delay Per Element"));
            EditorGUILayout.PropertyField(offsetRotation, new GUIContent("Offset Rotation"));
            EditorGUILayout.PropertyField(curveRotation, new GUIContent("Interpolation Curve"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();


        //AUTO-TRIGGER
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("When the animation is finished...");
        EditorGUILayout.PropertyField(autoTrigger, new GUIContent(""));
        EditorGUI.BeginDisabledGroup(autoTrigger.intValue == 0);
        {
            EditorGUILayout.PropertyField(autoTriggerTimer, new GUIContent("After Seconds"));
        }
        EditorGUI.EndDisabledGroup();


        //MESSAGE TYPE
        EditorGUILayout.Space(20);
        EditorGUILayout.HelpBox("All Animation Curves must start at value 0 and ends in value 1, " +
                                "even if it is used for fade out transition. " +
                                "It is used in interpolation, not for setting values", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
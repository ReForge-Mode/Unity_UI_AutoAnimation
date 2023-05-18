using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The animation presets for UI animations, detailing how the animation should be run
/// </summary>
[CreateAssetMenu(fileName = "Animation Entrance Preset", menuName = "Scriptable Objects/Animation Entrance Preset")]
public class SOAnimationEntrancePresets : ScriptableObject
{
    [Header("Entrance Animation")]
    //The duration of the animation in seconds
    public float duration         = 0.5f;
    public float delayPerElement  = 0.2f;

    [Header("Fade In Alpha")]
    public AnimationCurve curveAlpha;

    [Header("Float In Motion")]
    //The relative distance to slide in to when the UI element first appears
    //You can set this to zero if you don't want any slide in animation
    public Vector2 offsetPosition = new Vector2(0, -10f);

    //The animation curve for easing in and out
    public AnimationCurve curveMotion;

    [Header("Zoom In Sizing")]
    public Vector2 offsetScale;
    public AnimationCurve curveScale;
}

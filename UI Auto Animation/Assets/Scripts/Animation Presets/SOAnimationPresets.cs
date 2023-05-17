using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The animation presets for UI animations, detailing how the animation should be run
/// </summary>
[CreateAssetMenu(fileName = "Animation Settings", menuName = "Scriptable Objects/Animation Settings")]
public class SOAnimationPresets : ScriptableObject
{
    //The duration of the animation in seconds
    public float duration         = 0.5f;

    //The relative distance to slide in to when the UI element first appears
    //You can set this to zero if you don't want any slide in animation
    public Vector2 offsetPosition = new Vector2(0, -10f);

    //The animation curve for easing in and out
    public AnimationCurve curveAlphaFadeIn;
    public AnimationCurve curveMotionFadeIn;               
    public AnimationCurve curveAlphaFadeOut;
    public AnimationCurve curveMotionFadeOut;               
}

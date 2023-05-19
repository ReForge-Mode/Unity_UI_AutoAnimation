using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The animation presets for UI animations, detailing how the animation should be run
/// </summary>
[CreateAssetMenu(fileName = "Animation Preset", menuName = "Scriptable Objects/Animation Preset")]
public class SOAnimationPresets : ScriptableObject
{
    [Header("Entrance/Exit Animation")]
    public float duration         = 0.5f;       //The duration of the animation in seconds
    public float delayPerElement  = 0.2f;       

    //Fade In/Out Alpha
    public AnimationCurve curveAlpha;

    //Float In/Out Position
    public Vector2 offsetPosition = new Vector2(0, -10f);
    public AnimationCurve curvePosition;

    //Zoom In/Out Scale
    public Vector2 offsetScale;
    public AnimationCurve curveScale;

    //Spin In/Out Rotation
    public Vector3 offsetRotation;
    public AnimationCurve curveRotation;
}

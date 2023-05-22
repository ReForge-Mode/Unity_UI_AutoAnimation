using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The animation properties for UI animations. Put this in the UIAnimation Component to use it.
/// </summary>
[CreateAssetMenu(fileName = "Animation 1", menuName = "Scriptable Objects/Animation Preset")]
public class SOAnimationPresets : ScriptableObject
{
    //FADE IN/OUT ALPHA
    [Tooltip("If you don't use this property, it is recommended to turn it off to save performance.")]
    public bool useAlphaAnimation = true;

    [Tooltip("The delay before the animation begins")]
    public float alphaDelay = 0f;

    [Tooltip("The duration of each alpha animation in seconds. " +
             "Set it to zero to make all elements appear without any animation")]
    public float alphaDuration = 0.5f;

    [Tooltip("The delay between each element being animated in seconds. " +
             "Set it to zero to make all elements appear at once")]
    public float delayPerElementAlpha = 0.2f;

    [Tooltip("This animation curve must start at value 0 and ends in value 1, " +
             "even if it is used for fade out transition. " +
             "It is used as an interpolation, not for setting values.")]
    public AnimationCurve curveAlpha = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    //SLIDE IN/OUT POSITION
    [Tooltip("If you don't use this property, it is recommended to turn it off to save performance.")]
    public bool usePositionAnimation = true;

    [Tooltip("The delay before the animation begins")]
    public float positionDelay = 0f;

    [Tooltip("The duration of each position animation in seconds. " +
             "Set it to zero to make all elements appear without any animation")]
    public float positionDuration = 0.5f;

    [Tooltip("The delay between each element being animated in seconds. " +
             "Set it to zero to make all elements appear at once")]
    public float delayPerElementPosition = 0.2f;

    [Tooltip("Offset Position is relative, where (0, 0) means no movement. " +
             "Use positive for up and right, negative for left and down")]
    public Vector2 offsetPosition = Vector2.zero;

    [Tooltip("This animation curve must start at value 0 and ends in value 1, " +
             "even if it is used for fade out transition. " +
             "It is used as an interpolation, not for setting values")]
    public AnimationCurve curvePosition = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    //ZOOM IN/OUT SCALE
    [Tooltip("If you don't use this property, it is recommended to turn it off to save performance.")]
    public bool useScaleAnimation = true;

    [Tooltip("The delay before the animation begins")]
    public float scaleDelay = 0f;

    [Tooltip("The duration of each scale animation in seconds. " +
             "Set it to zero to make all elements appear without any animation")]
    public float scaleDuration = 0.5f;

    [Tooltip("The delay between each element being animated in seconds. " +
             "Set it to zero to make all elements appear at once")]
    public float delayPerElementScale = 0.2f;

    [Tooltip("Offset Scale is relative, where (1, 1) means no scaling.")]
    public Vector2 offsetScale = Vector2.one;

    [Tooltip("This animation curve must start at value 0 and ends in value 1, " +
             "even if it is used for fade out transition. " +
             "It is used as an interpolation, not for setting values")]
    public AnimationCurve curveScale = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    //SPIN IN/OUT ROTATION
    [Tooltip("If you don't use this property, it is recommended to turn it off to save performance.")]
    public bool useRotationAnimation = false;

    [Tooltip("The delay before the animation begins")]
    public float rotationDelay = 0f;

    [Tooltip("The duration of each rotational animation in seconds. " +
             "Set it to zero to make all elements appear without any animation")]
    public float rotationDuration = 0.5f;

    [Tooltip("The delay between each element being animated in seconds. " +
             "Set it to zero to make all elements appear at once")]
    public float delayPerElementRotation = 0.2f;

    [Tooltip("Offset Rotation uses euler angles (360 degree). " +
             "Rotation can also be 3D in any Vector3 axis. " +
             "It accepts negative values for counter-clockwise rotation.")]
    public Vector3 offsetRotation = Vector3.zero;

    [Tooltip("This animation curve must start at value 0 and ends in value 1, " +
             "even if it is used for fade out transition. " +
             "It is used as an interpolation, not for setting values")]
    public AnimationCurve curveRotation = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);



    //AT THE END
    [Tooltip("Trigger the Entrance/Exit Animation again, " +
             "which can be used to create a looping animation.")]
    public AutoTrigger autoTrigger = AutoTrigger.DoNothing;

    [Tooltip("The amount of time after the last animation is finished. ")]
    public float autoTriggerTimer = 5f;

    public enum AutoTrigger
    {
        DoNothing, TriggerEntranceAnimation, TriggerExitAnimation
    }
}

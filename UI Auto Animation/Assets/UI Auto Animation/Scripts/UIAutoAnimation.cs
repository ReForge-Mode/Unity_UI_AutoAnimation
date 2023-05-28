using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIAutoAnimation : MonoBehaviour
{
    public SOAnimationPresets animationEntrancePresets;     
    public SOAnimationPresets animationExitPresets;

    private List<Component> componentList;
    private RectTransform[] rectTransformList;
    private HorizontalOrVerticalLayoutGroup[] layoutGroupList;
    private float[] originalAlpha;
    private Vector2[] originalPosition;
    private Vector3[] originalScale;
    private Vector3[] originalRotation;

    public void Awake()
    {
        GetComponentsList();
    }

    public void EntranceAnimation()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();
        StartCoroutine(EntranceAlphaEnumeration());
        StartCoroutine(EntrancePositionEnumeration());
        StartCoroutine(EntranceScaleEnumeration());
        StartCoroutine(EntranceRotationEnumeration());

        AutoTriggerCalculation(animationEntrancePresets);
    }

    public void ExitAnimation()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();
        StartCoroutine(ExitAlphaEnumeration());
        StartCoroutine(ExitPositionEnumeration());
        StartCoroutine(ExitScaleEnumeration());
        StartCoroutine(ExitRotationEnumeration());

        AutoTriggerCalculation(animationExitPresets);
    }


    #region Enumeration Functions
    private IEnumerator EntranceAlphaEnumeration()
    {
        if (componentList.Count > 0 && animationEntrancePresets.useAlphaAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Zero();

            /// Delay the animation
            yield return new WaitForSeconds(animationEntrancePresets.alphaDelay);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.TopToBottom, animationEntrancePresets.delayPerElementAlpha);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationEntrancePresets.alphaDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] >= animationEntrancePresets.alphaDuration)
                    {
                        //Set its final state
                        SetColorAlpha(componentList[i], originalAlpha[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }


                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.alphaDuration;

                    //Calculate current alpha
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);
                    float currentAlpha = Mathf.Lerp(0, originalAlpha[i], curveValue);
                    SetColorAlpha(componentList[i], currentAlpha);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetAllColorAlpha_Original();
    }

    private IEnumerator ExitAlphaEnumeration()
    {
        if (componentList.Count > 0 && animationExitPresets.useAlphaAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Original();

            /// Delay the animation
            yield return new WaitForSeconds(animationExitPresets.alphaDelay);

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.BottomToTop, animationExitPresets.delayPerElementAlpha);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationExitPresets.alphaDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationExitPresets.alphaDuration)
                    {
                        //Set its final state
                        SetColorAlpha(componentList[i], 0);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.alphaDuration;

                    //Calculate current alpha
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);
                    float currentAlpha = Mathf.Lerp(originalAlpha[i], 0, curveValue);
                    SetColorAlpha(componentList[i], currentAlpha);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha_Zero();
        }
    }

    private IEnumerator EntrancePositionEnumeration()
    {
        if (componentList.Count > 0 && animationEntrancePresets.usePositionAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllPosition_Offset(animationEntrancePresets);

            /// Delay the animation
            yield return new WaitForSeconds(animationEntrancePresets.positionDelay);


            Vector2[] offsetPositionList = CreateOffsetPositionList(animationEntrancePresets);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.TopToBottom, animationEntrancePresets.delayPerElementPosition);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationEntrancePresets.positionDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationEntrancePresets.positionDuration)
                    {
                        //Set its final state
                        SetPosition(rectTransformList[i], originalPosition[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }


                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.positionDuration;

                    //Calculate current position
                    float curveValue = animationEntrancePresets.curvePosition.Evaluate(t);
                    Vector2 currentPosition = Vector2.Lerp(offsetPositionList[i], originalPosition[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    elapsedTime[i] += Time.deltaTime;
                }

                ResetAllLayoutGroup();
                yield return null;
            }
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetAllPosition_Original();
        ResetAllLayoutGroup();
    }

    private IEnumerator ExitPositionEnumeration()
    {
        if (componentList.Count > 0 && animationExitPresets.usePositionAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllPosition_Original();

            /// Delay the animation
            yield return new WaitForSeconds(animationExitPresets.positionDelay);


            Vector2[] offsetPositionList = CreateOffsetPositionList(animationExitPresets);

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.BottomToTop, animationExitPresets.delayPerElementPosition);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationExitPresets.positionDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationExitPresets.positionDuration)
                    {
                        //Set its final state
                        SetPosition(rectTransformList[i], offsetPositionList[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.positionDuration;

                    //Calculate current position
                    float curveValue = animationExitPresets.curvePosition.Evaluate(t);
                    Vector2 currentPosition = Vector2.Lerp(originalPosition[i], offsetPositionList[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    elapsedTime[i] += Time.deltaTime;
                }

                ResetAllLayoutGroup();
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllPosition_Offset(animationExitPresets);
            ResetAllLayoutGroup();
        }
    }

    private IEnumerator EntranceScaleEnumeration()
    {
        if (componentList.Count > 0 && animationEntrancePresets.useScaleAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllScale_Offset(animationEntrancePresets);

            /// Delay the animation
            yield return new WaitForSeconds(animationEntrancePresets.scaleDelay);


            Vector3[] offsetScaleList = CreateOffsetScaleList(animationEntrancePresets);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.TopToBottom, animationEntrancePresets.delayPerElementScale);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationEntrancePresets.scaleDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationEntrancePresets.scaleDuration)
                    {
                        //Set its final state
                        SetScale(rectTransformList[i], originalScale[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }


                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.scaleDuration;

                    //Calculate current scale
                    float curveValue = animationEntrancePresets.curveScale.Evaluate(t);
                    Vector3 currentScale = Vector3.Lerp(offsetScaleList[i], originalScale[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    elapsedTime[i] += Time.deltaTime;
                }

                yield return null;
            }
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetAllScale_Original();
    }

    private IEnumerator ExitScaleEnumeration()
    {
        if (componentList.Count > 0 && animationExitPresets.useScaleAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllScale_Original();

            /// Delay the animation
            yield return new WaitForSeconds(animationExitPresets.scaleDelay);


            Vector3[] offsetScaleList = CreateOffsetScaleList(animationExitPresets);

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.BottomToTop, animationExitPresets.delayPerElementScale);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationExitPresets.scaleDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationExitPresets.scaleDuration)
                    {
                        //Set its final state
                        SetScale(rectTransformList[i], offsetScaleList[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.scaleDuration;

                    //Calculate current scale
                    float curveValue = animationExitPresets.curveScale.Evaluate(t);
                    Vector3 currentScale = Vector3.Lerp(originalScale[i], offsetScaleList[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    elapsedTime[i] += Time.deltaTime;
                }

                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllScale_Offset(animationExitPresets);
        }
    }

    private IEnumerator EntranceRotationEnumeration()
    {
        if (componentList.Count > 0 && animationEntrancePresets.useRotationAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllRotation_Offset(animationEntrancePresets);

            /// Delay the animation
            yield return new WaitForSeconds(animationEntrancePresets.rotationDelay);


            Vector3[] offsetRotationList = CreateOffsetRotationList(animationEntrancePresets);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.TopToBottom, animationEntrancePresets.delayPerElementRotation);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationEntrancePresets.rotationDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationEntrancePresets.rotationDuration)
                    {
                        //Set its final state
                        SetRotation(rectTransformList[i], originalRotation[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }


                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.rotationDuration;

                    //Calculate current rotation
                    float curveValue = animationEntrancePresets.curveRotation.Evaluate(t);
                    Vector3 currentRotation = Vector3.Lerp(offsetRotationList[i], originalRotation[i], curveValue);
                    SetRotation(rectTransformList[i], currentRotation);

                    elapsedTime[i] += Time.deltaTime;
                }

                yield return null;
            }
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetAllRotation_Original();
    }

    private IEnumerator ExitRotationEnumeration()
    {
        if (componentList.Count > 0 && animationExitPresets.useRotationAnimation == true)
        {
            /// Set starting state where the UI is invisible
            SetAllRotation_Original();

            /// Delay the animation
            yield return new WaitForSeconds(animationExitPresets.rotationDelay);



            Vector3[] offsetRotationList = CreateOffsetRotationList(animationExitPresets);

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayPerItem = CalculateDelayPerItem(DelayPerItemType.BottomToTop, animationExitPresets.delayPerElementRotation);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationExitPresets.rotationDuration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationExitPresets.rotationDuration)
                    {
                        //Set its final state
                        SetRotation(rectTransformList[i], offsetRotationList[i]);
                        continue;
                    }

                    //Don't do anything until the delay timer reaches 0
                    if (delayPerItem[i] > 0f)
                    {
                        delayPerItem[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.rotationDuration;

                    //Calculate current rotation
                    float curveValue = animationExitPresets.curveRotation.Evaluate(t);
                    Vector3 currentRotation = Vector3.Lerp(originalRotation[i], offsetRotationList[i], curveValue);
                    SetRotation(rectTransformList[i], currentRotation);

                    elapsedTime[i] += Time.deltaTime;
                }

                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllRotation_Offset(animationExitPresets);
        }
    }
    #endregion

    #region Find Components and its Depth in Hierarchy
    private void GetComponentInHierarchy_DepthFirst(Transform current, params Type[] types)
    {
        componentList = new List<Component>();

        DepthFirstRecursion(current, 0, types);
    }

    private void DepthFirstRecursion(Transform current, int recursionLevel, params Type[] types)
    {
        //Get this object's T, if it exists and add it to the list
        Component component = null;
        foreach (Type type in types)
        {
            component = current.GetComponent(type);
            if (component != null)
            {
                componentList.Add(component);
            }
        }

        //Now, get all of its children transform and recursively use this function again.
        int childCount = current.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DepthFirstRecursion(current.GetChild(i), recursionLevel++, types);
        }
    }

    #endregion

    #region Helper Functions
    /// <summary>
    /// Search all components that will be used for animation
    /// and store their original alpha, position, scale, and rotation
    /// </summary>
    private void GetComponentsList()
    {
        //Get the component list, sorted either Depth-first or Breadth-first
        GetComponentInHierarchy_DepthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));

        //Now replace the component with either TextMeshProUGUI or Image
        for (int i = 0; i < componentList.Count; i++)
        {
            //Replace with Image UI
            var img = componentList[i].GetComponent<Image>();
            if (img != null)
            {
                //We don't care about UI images which are intentionally hidden
                if (img.isActiveAndEnabled && img.color.a > 0f)
                {
                    //Prevent adding duplicates
                    if (!componentList.Contains(img))
                    {
                        componentList[i] = img;
                    }
                }
            }

            //Replace with TextMeshPro
            var tmp = componentList[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                //Prevent adding duplicates
                if (!componentList.Contains(tmp))
                {
                    componentList[i] = tmp;
                }
            }
        }

        //Get their initial alpha transparency
        originalAlpha = new float[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            if (componentList[i] is TextMeshProUGUI)
            {
                float alpha = ((TextMeshProUGUI)componentList[i]).alpha;
                originalAlpha[i] = alpha;
            }
            else if (componentList[i] is Image)
            {
                float alpha = ((Image)componentList[i]).color.a;
                originalAlpha[i] = alpha;
            }
        }

        //Get their initial rectTransform, position, scale, and rotation
        rectTransformList = new RectTransform[componentList.Count];
        originalPosition = new Vector2[componentList.Count];
        originalScale = new Vector3[componentList.Count];
        originalRotation = new Vector3[componentList.Count];

        for (int i = 0; i < componentList.Count; i++)
        {
            RectTransform rect = componentList[i].GetComponent<RectTransform>();
            rectTransformList[i] = rect;

            Vector2 position = rect.anchoredPosition;
            originalPosition[i] = position;

            Vector3 scale = rect.localScale;
            originalScale[i] = scale;

            Vector3 rotation = rect.localRotation.eulerAngles;
            originalRotation[i] = rotation;
        }

        //Get the Vertical Layout Group if it exists.
        //If we set the position of the child object in the Layout Group,
        //their position will reset to zero. So we need to recalculate them.
        layoutGroupList = GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>();
    }

    /// <summary>
    /// Calculate the AutoTrigger if it's used
    /// </summary>
    /// <param name="animationPresets"></param>
    private void AutoTriggerCalculation(SOAnimationPresets animationPresets)
    {
        if (animationPresets.autoTrigger != SOAnimationPresets.AutoTrigger.DoNothing)
        {
            float alphaDuration = (componentList.Count * animationPresets.delayPerElementAlpha) + animationPresets.alphaDuration;
            float positionDuration = (componentList.Count * animationPresets.delayPerElementPosition) + animationPresets.positionDuration;
            float scaleDuration = (componentList.Count * animationPresets.delayPerElementScale) + animationPresets.scaleDuration;
            float rotationDuration = (componentList.Count * animationPresets.delayPerElementRotation) + animationPresets.rotationDuration;
            float maxEntranceDuration = Mathf.Max(alphaDuration, positionDuration, scaleDuration, rotationDuration);

            if (animationPresets.autoTrigger == SOAnimationPresets.AutoTrigger.TriggerExitAnimation)
            {
                Invoke("ExitAnimation", maxEntranceDuration + animationPresets.autoTriggerTimer);
            }
            else if (animationPresets.autoTrigger == SOAnimationPresets.AutoTrigger.TriggerEntranceAnimation)
            {
                Invoke("EntranceAnimation", maxEntranceDuration + animationPresets.autoTriggerTimer);
            }
        }
    }

    /// <summary>
    /// Returns a list of timing for each element in the list,
    /// based on the animation presets scriptable object.
    /// </summary>
    /// <param name="delayPerItemType">TopToBottom is used in entrance animation. BottomToTop is used in exit animation</param>
    /// <param name="animationPresets">Specify to get the delayPerElement value from entrance or exit presets</param>
    /// <returns>Array of delayPerItem to count the animation delay in each element on the list</returns>
    private float[] CalculateDelayPerItem(DelayPerItemType delayPerItemType, float delayPerElement)
    {
        float[] delayPerItem = new float[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            //For "Top to Bottom" used in Entrance Animation, it gives the first item on the list
            //value timer 0 second, and increment each item by delayPerItem in the AnimationPresets.
            //For "Bottom to Top" used in Exit Animation, it does the opposite, where the last item is given 0
            //and increment each item backwards.

            int increment = (delayPerItemType == DelayPerItemType.TopToBottom) ? i : (componentList.Count - 1 - i);
            delayPerItem[i] = delayPerElement * increment;
        }
        return delayPerItem;
    }

    /// <summary>
    /// Set the component's alpha.
    /// It handles if the component is TextMeshProUGUI or an Image UI.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="value"></param>
    private void SetColorAlpha(Component component, float value)
    {
        if (component is TextMeshProUGUI)
        {
            float alpha = value;
            ((TextMeshProUGUI)component).alpha = alpha;
        }
        else if (component is Image)
        {
            float alpha = value;
            Color imageColor = ((Image)component).color;
            imageColor.a = alpha;
            ((Image)component).color = imageColor;
        }
    }

    /// <summary>
    /// Set all components' alpha to zero. 
    /// This is used at the start of the entrance animation and at the end of exit animation.
    /// </summary>
    private void SetAllColorAlpha_Zero()
    {
        if (componentList.Count > 0)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                SetColorAlpha(componentList[i], 0);
            }
        }
    }

    /// <summary>
    /// Set all components' alpha to its original value. 
    /// This is used at the end of the entrance animation and at the start of exit animation.
    /// </summary>
    private void SetAllColorAlpha_Original()
    {
        if (componentList.Count > 0)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                SetColorAlpha(componentList[i], originalAlpha[i]);
            }
        }
    }

    /// <summary>
    /// Helper function to set the position of rectTransform without ever seeing rect.localPosition
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="value"></param>
    private void SetPosition(RectTransform rect, Vector2 value)
    {
        rect.anchoredPosition = value;
    }

    /// <summary>
    /// Set all components' position back to its original position. 
    /// This is used at the end of the entrance animation and at the start of exit animation.
    /// </summary>
    private void SetAllPosition_Original()
    {
        if (rectTransformList.Length > 0)
        {
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetPosition(rectTransformList[i], originalPosition[i]);
            }
        }
    }

    /// <summary>
    /// Set all components' position to its animation-ready offset position. 
    /// This is used at the start of the entrance animation and at the end of exit animation.
    /// </summary>
    public void SetAllPosition_Offset(SOAnimationPresets animationPresets)
    {
        if (rectTransformList.Length > 0)
        {
            Vector2[] offsetPositionList = CreateOffsetPositionList(animationPresets);
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetPosition(rectTransformList[i], offsetPositionList[i]);
            }
        }
    }

    /// <summary>
    /// Helper function to set the scale of rectTransform without ever seeing rect.localScale
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="value"></param>
    private void SetScale(RectTransform rect, Vector3 value)
    {
        rect.localScale = value;
    }

    /// <summary>
    /// Set all components' scale back to its original scale. 
    /// This is used at the end of the entrance animation and at the start of exit animation.
    /// </summary>
    private void SetAllScale_Original()
    {
        if (rectTransformList.Length > 0)
        {
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetScale(rectTransformList[i], originalScale[i]);
            }
        }
    }

    /// <summary>
    /// Set all components' scale to its animation-ready offset scale. 
    /// This is used at the start of the entrance animation and at the end of exit animation. 
    /// </summary>
    /// <param name="offsetScaleList"></param>
    private void SetAllScale_Offset(SOAnimationPresets animationPresets)
    {
        if (rectTransformList.Length > 0)
        {
            Vector3[] offsetScaleList = CreateOffsetScaleList(animationPresets);
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetScale(rectTransformList[i], offsetScaleList[i]);
            }
        }
    }

    /// <summary>
    /// Helper function to set the rotation of rectTransform without ever seeing rect.localRotation 
    /// or dealing with Quaternion-Vector3 conversion mess
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="rotation"></param>
    private void SetRotation(RectTransform rect, Vector3 rotation)
    {
        rect.localRotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// Set all components' rotation back to its original rotation. 
    /// This is used at the end of the entrance animation and at the start of exit animation.
    /// </summary>
    private void SetAllRotation_Original()
    {
        if (rectTransformList.Length > 0)
        {
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetRotation(rectTransformList[i], originalRotation[i]);
            }
        }
    }

    /// <summary>
    /// Set all components' rotation to its animation-ready offset rotation. 
    /// This is used at the start of the entrance animation and at the end of exit animation. 
    /// </summary>
    /// <param name="offsetRotationList"></param>
    private void SetAllRotation_Offset(SOAnimationPresets animationPresets)
    {
        if (rectTransformList.Length > 0)
        {
            Vector3[] offsetRotationList = CreateOffsetRotationList(animationPresets);
            for (int i = 0; i < rectTransformList.Length; i++)
            {
                SetRotation(rectTransformList[i], offsetRotationList[i]);
            }
        }
    }

    /// <summary>
    /// Create a list of offsetPosition to animate to and from. 
    /// </summary>
    /// <param name="animationPresets">
    /// Specify which animationPresets to get the right offsetPosition value (entrance or exit animation)
    /// </param>
    /// <returns>Vector2 array detailing each offset position for interpolation</returns>
    private Vector2[] CreateOffsetPositionList(SOAnimationPresets animationPresets)
    {
        Vector2[] offsetPositionList = new Vector2[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector2 offset = originalPosition[i] + animationPresets.offsetPosition;
            offsetPositionList[i] = offset;
        }
        return offsetPositionList;
    }

    /// <summary>
    /// Create a list of offsetScale to animate to and from. 
    /// </summary>
    /// <param name="animationPresets">
    /// Specify which animationPresets to get the right offsetScale value (entrance or exit animation)
    /// </param>
    /// <returns>Vector2 array detailing each offset scale for interpolation</returns>
    private Vector3[] CreateOffsetScaleList(SOAnimationPresets animationPresets)
    {
        Vector3[] offsetScaleList = new Vector3[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector3 offset = originalScale[i] * animationPresets.offsetScale;
            offsetScaleList[i] = offset;
        }
        return offsetScaleList;
    }

    /// <summary>
    /// Create a list of offsetRotation to animate to and from. 
    /// </summary>
    /// <param name="animationPresets">
    /// Specify which animationPresets to get the right offsetRotation value (entrance or exit animation)
    /// </param>
    /// <returns>Vector2 array detailing each offset rotation for interpolation</returns>
    private Vector3[] CreateOffsetRotationList(SOAnimationPresets animationPresets)
    {
        Vector3[] offsetRotationList = new Vector3[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector3 offset = originalRotation[i] + animationPresets.offsetRotation;
            offsetRotationList[i] = offset;
        }
        return offsetRotationList;
    }

    /// <summary>
    /// Reset the layout group position. 
    /// If we set the position of the child object in the Layout Group through script,
    /// their position will reset to zero. So we need to recalculate them.
    /// </summary>
    private void ResetAllLayoutGroup()
    {
        if (layoutGroupList.Length > 0)
        {
            for (int i = 0; i < layoutGroupList.Length; i++)
            {
                layoutGroupList[i].CalculateLayoutInputVertical();
                layoutGroupList[i].SetLayoutVertical();

                layoutGroupList[i].CalculateLayoutInputHorizontal();
                layoutGroupList[i].SetLayoutHorizontal();
            }
        }
    }
    #endregion


    /// <summary>
    /// The iteration type when defining the delayPerItem. 
    /// TopToBottom is used in entrance animation. BottomToTop is used in exit animation
    /// </summary>
    public enum DelayPerItemType
    {
        TopToBottom, BottomToTop
    }

    /// <summary>
    /// Determines to order the elements in the UI Depth-First or Breadth-First
    /// </summary>
    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}
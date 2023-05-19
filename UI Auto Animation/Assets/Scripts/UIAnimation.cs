using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public SOAnimationPresets animationEntrancePresets;     
    public SOAnimationPresets animationExitPresets;            

    public SearchMode searchMode;

    [Space]
    [Header("Debug")]
    public List<Component> componentList;
    public List<RectTransform> rectTransformList;
    public List<float> originalAlpha;
    public List<Vector2> originalPosition;
    public List<Vector3> originalScale;
    public List<Vector3> originalRotation;

    public void Awake()
    {
        GetComponentsList();
    }

    public void EntranceAnimation()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();
        StartCoroutine(EntranceEnumeration());
    }

    public void ExitAnimation()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();
        StartCoroutine(ExitEnumeration());
    }

    private IEnumerator EntranceEnumeration()
    {
        if (componentList.Count > 0)
        {
            List<Vector2> offsetPositionList = CreateOffsetPositionList(animationEntrancePresets.offsetPosition);
            List<Vector3> offsetScaleList = CreateOffsetScaleList(animationEntrancePresets.offsetScale);
            List<Vector3> offsetRotationList = CreateOffsetRotationList(animationEntrancePresets.offsetRotation);

            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Zero();
            SetAllPosition_Offset(animationEntrancePresets.offsetPosition);
            SetAllScale_Offset(offsetScaleList);
            SetAllRotation_Offset(offsetRotationList);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = CalculateDelayTimer(DelayTimerType.TopToBottom);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationEntrancePresets.duration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationEntrancePresets.duration) continue;

                    //Don't do anything until the delay timer reaches 0
                    if (delayTimer[i] > 0f)
                    {
                        delayTimer[i] -= Time.deltaTime;
                        continue;
                    }


                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.duration;

                    //Calculate current alpha
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);
                    float currentAlpha = Mathf.Lerp(0, originalAlpha[i], curveValue);
                    SetColorAlpha(componentList[i], currentAlpha);

                    //Calculate current position
                    curveValue = animationEntrancePresets.curvePosition.Evaluate(t);
                    Vector2 currentPosition = Vector2.Lerp(offsetPositionList[i], originalPosition[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    //Calculate current scale
                    curveValue = animationEntrancePresets.curveScale.Evaluate(t);
                    Vector3 currentScale = Vector3.Lerp(offsetScaleList[i], originalScale[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    //Calculate current rotation
                    curveValue = animationEntrancePresets.curveRotation.Evaluate(t);
                    Vector3 currentRotation = Vector3.Lerp(offsetRotationList[i], originalRotation[i], curveValue);
                    SetRotation(rectTransformList[i], currentRotation);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha_Original();
            SetAllPosition_Original();
            SetAllScale_Original();
            SetAllRotation_Original();
        }
    }

    private IEnumerator ExitEnumeration()
    {
        if (componentList.Count > 0)
        {
            List<Vector2> offsetPositionList = CreateOffsetPositionList(animationExitPresets.offsetPosition);
            List<Vector3> offsetScaleList = CreateOffsetScaleList(animationExitPresets.offsetScale);
            List<Vector3> offsetRotationList = CreateOffsetRotationList(animationExitPresets.offsetRotation);

            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Original();
            SetAllPosition_Original();
            SetAllScale_Original();
            SetAllRotation_Original();

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = CalculateDelayTimer(DelayTimerType.BottomToTop);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationExitPresets.duration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationExitPresets.duration) continue;

                    //Don't do anything until the delay timer reaches 0
                    if (delayTimer[i] > 0f)
                    {
                        delayTimer[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the interpolated value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.duration;

                    //Calculate current alpha
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);
                    float currentAlpha = Mathf.Lerp(originalAlpha[i], 0, curveValue);
                    SetColorAlpha(componentList[i], currentAlpha);

                    //Calculate current position
                    curveValue = animationExitPresets.curvePosition.Evaluate(t);
                    Vector2 currentPosition = Vector2.Lerp(originalPosition[i], offsetPositionList[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    //Calculate current scale
                    curveValue = animationExitPresets.curveScale.Evaluate(t);
                    Vector3 currentScale = Vector3.Lerp(originalScale[i], offsetScaleList[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    //Calculate current rotation
                    curveValue = animationExitPresets.curveRotation.Evaluate(t);
                    Vector3 currentRotation = Vector3.Lerp(originalRotation[i], offsetRotationList[i], curveValue);
                    SetRotation(rectTransformList[i], currentRotation);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha_Zero();
            SetAllPosition_Offset(animationExitPresets.offsetPosition);
            SetAllScale_Offset(offsetScaleList);
            SetAllRotation_Offset(offsetRotationList);
        }
    }
    
    private List<Vector2> CreateOffsetPositionList(Vector2 offsetPosition)
    {
        List<Vector2> offsetPositionList = new List<Vector2>();
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector2 currentPosition = rectTransformList[i].anchoredPosition;
            Vector2 offset = currentPosition + offsetPosition;
            offsetPositionList.Add(offset);
        }
        return offsetPositionList;
    }

    private List<Vector3> CreateOffsetScaleList(Vector2 offsetScale)
    {
        List<Vector3> offsetScaleList = new List<Vector3>();
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector3 offset = originalScale[i] * offsetScale;
            offsetScaleList.Add(offset);
        }
        return offsetScaleList;
    }

    private List<Vector3> CreateOffsetRotationList(Vector3 offsetRotation)
    {
        List<Vector3> offsetRotationList = new List<Vector3>();
        for (int i = 0; i < componentList.Count; i++)
        {
            Vector3 offset = originalRotation[i] + offsetRotation;
            offsetRotationList.Add(offset);
        }
        return offsetRotationList;
    }

    #region Helper Functions
    private void GetComponentsList()
    {
        //Get the component list, sorted either Depth-first or Breadth-first
        componentList = new List<Component>();
        if (searchMode == SearchMode.DepthFirst)
        {
            componentList = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));
        }
        else if (searchMode == SearchMode.BreadthFirst)
        {
            componentList = ComponentSearch.GetComponentInHierarchy_BreadthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));
        }

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
        originalAlpha.Clear();
        for (int i = 0; i < componentList.Count; i++)
        {
            if (componentList[i] is TextMeshProUGUI)
            {
                float alpha = ((TextMeshProUGUI)componentList[i]).alpha;
                originalAlpha.Add(alpha);
            }
            else if (componentList[i] is Image)
            {
                float alpha = ((Image)componentList[i]).color.a;
                originalAlpha.Add(alpha);
            }
        }

        //Get their initial rectTransform, position, scale, and rotation
        originalPosition.Clear();
        for (int i = 0; i < componentList.Count; i++)
        {
            RectTransform rect = componentList[i].GetComponent<RectTransform>();
            rectTransformList.Add(rect);

            Vector2 position = rect.anchoredPosition;
            originalPosition.Add(position);

            Vector3 scale = rect.localScale;
            originalScale.Add(scale);

            Vector3 rotation = rect.localRotation.eulerAngles;
            originalRotation.Add(rotation);
        }
    }

    private float[] CalculateDelayTimer(DelayTimerType delayTimerType)
    {
        float[] delayTimer = new float[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            int increment = (delayTimerType == DelayTimerType.TopToBottom) ? i : (componentList.Count - 1 - i);
            delayTimer[i] = animationEntrancePresets.delayPerElement * increment;
        }
        return delayTimer;
    }

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

    private void SetPosition(RectTransform rect, Vector2 value)
    {
        rect.anchoredPosition = value;
    }

    private void SetAllPosition_Original()
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetPosition(rectTransformList[i], originalPosition[i]);
            }
        }
    }

    private void SetAllPosition_Offset(Vector2 offsetPosition)
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                Vector2 position = originalPosition[i] + offsetPosition;
                SetPosition(rectTransformList[i], position);
            }
        }
    }

    private void SetScale(RectTransform rect, Vector3 value)
    {
        rect.localScale = value;
    }

    private void SetAllScale_Original()
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetScale(rectTransformList[i], originalScale[i]);
            }
        }
    }

    private void SetAllScale_Offset(List<Vector3> offsetScaleList)
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetScale(rectTransformList[i], offsetScaleList[i]);
            }
        }
    }

    private void SetRotation(RectTransform rect, Vector3 rotation)
    {
        rect.localRotation = Quaternion.Euler(rotation);
    }

    private void SetAllRotation_Original()
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetRotation(rectTransformList[i], originalRotation[i]);
            }
        }
    }

    private void SetAllRotation_Offset(List<Vector3> offsetRotationList)
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetRotation(rectTransformList[i], offsetRotationList[i]);
            }
        }
    }

    #endregion

    public enum DelayTimerType
    {
        TopToBottom, BottomToTop
    }

    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}
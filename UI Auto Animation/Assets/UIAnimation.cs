using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public SOAnimationEntrancePresets animationEntrancePresets;     
    public SOAnimationExitPresets animationExitPresets;            

    public SearchMode searchMode;

    //For animating text color (fade in/fade out)
    public List<Component> componentList;
    public List<RectTransform> rectTransformList;
    public List<float> originalAlpha;

    //For animating positions (float in/float out)
    public List<Vector2> originalPosition;
    public List<Vector3> originalScale;

    public Coroutine fadeCoroutine;
    public Coroutine floatCoroutine;
    public Coroutine scaleCoroutine;

    [Header("Debug")]
    public List<Vector3> offsetScale;

    public void Awake()
    {
        GetComponentsList();
    }

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

        //Get their initial rectTransform, position, and scale
        originalPosition.Clear();
        for (int i = 0; i < componentList.Count; i++)
        {
            RectTransform rect = componentList[i].GetComponent<RectTransform>();
            rectTransformList.Add(rect);

            Vector2 position = rect.anchoredPosition;
            originalPosition.Add(position);

            Vector3 scale = rect.localScale;
            originalScale.Add(scale);
        }
    }

    public void FadeIn()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();

        fadeCoroutine  = StartCoroutine(FadeInEnumerator());
        floatCoroutine = StartCoroutine(FloatInEnumerator());
        scaleCoroutine = StartCoroutine(ZoomInEnumerator());
    }

    public void FadeOut()
    {
        //Stop any running animation before triggering a new one
        StopAllCoroutines();

        fadeCoroutine = StartCoroutine(FadeOutEnumerator());
        fadeCoroutine = StartCoroutine(FloatOutEnumerator());
        scaleCoroutine = StartCoroutine(ZoomOutEnumerator());
    }

    #region List of All Animations
    private IEnumerator FadeInEnumerator()
    {
        if (componentList.Count > 0)
        {
            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Zero();

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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.duration;
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);

                    //Fade in the alpha
                    SetColorAlpha(componentList[i], curveValue * originalAlpha[i]);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha_Original();
        }
    }

    private IEnumerator FadeOutEnumerator()
    {
        if (componentList.Count > 0)
        {
            /// Set starting state where the UI is invisible
            SetAllColorAlpha_Original();

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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.duration;
                    float curveValue = animationExitPresets.curveMotion.Evaluate(t);

                    //Fade in the alpha
                    SetColorAlpha(componentList[i], curveValue * originalAlpha[i]);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha_Zero();
        }
    }

    private IEnumerator FloatInEnumerator()
    {
        if (componentList.Count > 0)
        {
            //Calculate Offset Position
            List<Vector2> offsetPosition = new List<Vector2>();
            for (int i = 0; i < componentList.Count; i++)
            {
                Vector2 currentPosition = rectTransformList[i].anchoredPosition;
                Vector2 offset = currentPosition + animationEntrancePresets.offsetPosition;
                offsetPosition.Add(offset);
            }

            /// Set starting state where the UI is invisible
            SetAllPosition_Offset(animationEntrancePresets.offsetPosition);

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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.duration;
                    float curveValue = animationEntrancePresets.curveMotion.Evaluate(t);

                    //Fade in the alpha
                    Vector2 currentPosition = Vector2.Lerp(offsetPosition[i], originalPosition[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllPosition_Original();
        }
    }

    private IEnumerator FloatOutEnumerator()
    {
        if (componentList.Count > 0)
        {
            //Calculate Offset Position
            List<Vector2> offsetPosition = new List<Vector2>();
            for (int i = 0; i < componentList.Count; i++)
            {
                Vector2 currentPosition = rectTransformList[i].anchoredPosition;
                Vector2 offset = currentPosition + animationExitPresets.offsetPosition;
                offsetPosition.Add(offset);
            }

            /// Set starting state where the UI is invisible
            SetAllPosition_Original();

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = CalculateDelayTimer(DelayTimerType.BottomToTop);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.duration;
                    float curveValue = animationExitPresets.curveMotion.Evaluate(t);

                    //Fade in the alpha
                    Vector2 currentPosition = Vector2.Lerp(offsetPosition[i], originalPosition[i], curveValue);
                    SetPosition(rectTransformList[i], currentPosition);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllPosition_Offset(animationExitPresets.offsetScale);
        }
    }

    private IEnumerator ZoomInEnumerator()
    {
        if (componentList.Count > 0)
        {
            //Calculate offsetScale
            offsetScale = new List<Vector3>();
            for (int i = 0; i < componentList.Count; i++)
            {
                Vector3 offset = originalScale[i];
                offset.x *= animationEntrancePresets.offsetScale.x;
                offset.y *= animationEntrancePresets.offsetScale.y;
                offset.z = 1;
                offsetScale.Add(offset);
            }

            /// Set starting state where the UI is invisible
            SetAllScale_Offset(offsetScale);

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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationEntrancePresets.duration;
                    float curveValue = animationEntrancePresets.curveAlpha.Evaluate(t);

                    //Fade in the alpha
                    Vector3 currentScale = Vector3.Lerp(offsetScale[i], originalScale[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllScale_Original();
        }
    }

    private IEnumerator ZoomOutEnumerator()
    {
        if (componentList.Count > 0)
        {
            //Calculate offset Scale
            offsetScale = new List<Vector3>();
            for (int i = 0; i < componentList.Count; i++)
            {
                Vector3 offset = originalScale[i];
                offset.x *= animationExitPresets.offsetScale.x;
                offset.y *= animationExitPresets.offsetScale.y;
                offset.z = 1;
                offsetScale.Add(offset);
            }

            /// Set starting state where the UI is invisible
            SetAllScale_Original();

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = CalculateDelayTimer(DelayTimerType.BottomToTop);

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade in animation
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

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationExitPresets.duration;
                    float curveValue = animationExitPresets.curveScale.Evaluate(t);

                    //Fade in the alpha
                    Vector3 currentScale = Vector3.Lerp(originalScale[i], offsetScale[i], curveValue);
                    SetScale(rectTransformList[i], currentScale);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllScale_Offset(offsetScale);
        }
    }
    #endregion

    #region Helper Functions
    //private float FindCurveTime(AnimationCurve animationCurve, float curveValue)
    //{
    //    float time = animationCurve.Evaluate(curveValue);
    //    return time;
    //}

    //private float GetColorAlpha(Component component)
    //{
    //    if (component is TextMeshProUGUI)
    //    {
    //        return ((TextMeshProUGUI)component).alpha;
    //    }
    //    else if (component is Image)
    //    {
    //        return ((Image)component).color.a;
    //    }
    //    return -1;
    //}

    public enum DelayTimerType
    {
        TopToBottom, BottomToTop
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

    private void SetAllScale_Offset(List<Vector3> offsetScale)
    {
        if (rectTransformList.Count > 0)
        {
            for (int i = 0; i < rectTransformList.Count; i++)
            {
                SetScale(rectTransformList[i], offsetScale[i]);
            }
        }
    }
    #endregion

    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public SOAnimationPresets animationPresets;     //Reference to the ScriptableObject

    public SearchMode searchMode;
    public float delayPerElement = 0.2f;

    //For animating text color (fade in/fade out)
    public List<Component> componentList;
    public List<float> originalAlpha;

    public Coroutine fadeCoroutine;
    public Coroutine floatCoroutine;

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
    }

    
    public void FadeIn()
    {
        //Stop any running animation before triggering a new one
        if (fadeCoroutine != null) StopAllCoroutines();
        fadeCoroutine = StartCoroutine(FadeInEnumerator());
    }

    public void FadeOut()
    {
        //Stop any running animation before triggering a new one
        if (fadeCoroutine != null) StopAllCoroutines();
        fadeCoroutine = StartCoroutine(FadeOutEnumerator());
    }

    #region List of All Animations
    private IEnumerator FloatInEnumerator()
    {
        if (componentList.Count > 0)
        {
            /// Set starting state where the UI is invisible
            //SetAllColorAlpha(0);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = new float[componentList.Count];
            for (int i = 0; i < componentList.Count; i++)
            {
                delayTimer[i] = delayPerElement * i;
            }

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// If the animation is stopped abruptly, continue the last animation
            /// If you want more precise timing, find the element that is not with alpha 0 or 1
            for (int i = 0; i < componentList.Count; i++)
            {
                elapsedTime[i] = FindCurveTime(animationPresets.curveAlphaFadeIn, GetColorAlpha(componentList[i]));
            }

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationPresets.duration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationPresets.duration) continue;

                    //Don't do anything until the delay timer reaches 0
                    if (delayTimer[i] > 0f)
                    {
                        delayTimer[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationPresets.duration;
                    float curveValue = animationPresets.curveAlphaFadeIn.Evaluate(t);

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

        floatCoroutine = null;
    }

    private IEnumerator FadeInEnumerator()
    {
        if (componentList.Count > 0)
        {
            /// Set starting state where the UI is invisible
            //SetAllColorAlpha(0);

            /// Calculate delay per element
            /// The top-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = new float[componentList.Count];
            for (int i = 0; i < componentList.Count; i++)
            {
                delayTimer[i] = delayPerElement * i;
            }

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// If the animation is stopped abruptly, continue the last animation
            /// If you want more precise timing, find the element that is not with alpha 0 or 1
            for (int i = 0; i < componentList.Count; i++)
            {
                elapsedTime[i] = FindCurveTime(animationPresets.curveAlphaFadeIn, GetColorAlpha(componentList[i]));
            }

            /// Animate the fade in animation
            while (elapsedTime[componentList.Count - 1] < animationPresets.duration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationPresets.duration) continue;

                    //Don't do anything until the delay timer reaches 0
                    if (delayTimer[i] > 0f)
                    {
                        delayTimer[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationPresets.duration;
                    float curveValue = animationPresets.curveAlphaFadeIn.Evaluate(t);

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

        fadeCoroutine = null;
    }

    private IEnumerator FadeOutEnumerator()
    {
        if (componentList.Count > 0)
        {
            /// Set starting state where the UI is invisible
            //SetAllColorAlpha_Original();

            /// Calculate delay per element
            /// The bottom-most UI on the list gets 0 second delay.
            /// Each subsequent element gets delayed by the amount.
            float[] delayTimer = new float[componentList.Count];
            for (int i = 0; i < componentList.Count; i++)
            {
                delayTimer[i] = delayPerElement * (componentList.Count - 1 - i);
            }

            /// Keep track of the time for each UI on the list
            /// because each UI has a different starting time based on the delay
            float[] elapsedTime = new float[componentList.Count];

            /// Animate the fade out animation
            while (elapsedTime[0] < animationPresets.duration)
            {
                for (int i = 0; i < componentList.Count; i++)
                {
                    //Don't do anything when the animation for this item is already finished.
                    if (elapsedTime[i] > animationPresets.duration) continue;

                    //Don't do anything until the delay timer reaches 0
                    if (delayTimer[i] > 0f)
                    {
                        delayTimer[i] -= Time.deltaTime;
                        continue;
                    }

                    //Count the alpha value from the animation curve
                    float t = elapsedTime[i] / animationPresets.duration;
                    float curveValue = animationPresets.curveMotionFadeOut.Evaluate(t);

                    //Fade in the alpha
                    SetColorAlpha(componentList[i], curveValue * originalAlpha[i]);

                    elapsedTime[i] += Time.deltaTime;
                }
                yield return null;
            }

            //There are always inaccuracies when dealing with float values
            //So to keep it safe, when the final loop is done, set everything to its final state.
            SetAllColorAlpha(0);
        }

        fadeCoroutine = null;
    }
    #endregion

    #region Helper Functions
    private float FindCurveTime(AnimationCurve animationCurve, float curveValue)
    {
        float time = animationCurve.Evaluate(curveValue);
        return time;
    }

    private float GetColorAlpha(Component component)
    {
        if (component is TextMeshProUGUI)
        {
            return ((TextMeshProUGUI)component).alpha;
        }
        else if (component is Image)
        {
            return ((Image)component).color.a;
        }
        return -1;
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

    private void SetAllColorAlpha(float value)
    {
        if (componentList.Count > 0)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                SetColorAlpha(componentList[i], value);
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
    #endregion

    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}
using System.Collections;
using System.Collections.Generic;
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

    public Coroutine coroutine;

    public void Awake()
    {
        GetComponents();
    }

    public void OnEnable()
    {
        //Stop any running animation before triggering a new one
        if (coroutine != null) StopAllCoroutines();
        coroutine = StartCoroutine(FadeIn());
    }

    public void GetComponents()
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

    private IEnumerator FadeIn()
    {
        //Set starting state
        if (componentList.Count > 0)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                if (componentList[i] is TextMeshProUGUI)
                {
                    float alpha = 0;
                    ((TextMeshProUGUI)componentList[i]).alpha = alpha;
                }
                else if (componentList[i] is Image)
                {
                    float alpha = 0;
                    Color imageColor = ((Image)componentList[i]).color;
                    imageColor.a = alpha;
                    ((Image)componentList[i]).color = imageColor;
                }
            }
        }

        //Calculate delay per element. The top UI on the list gets 0 delay.
        //Each subsequent element gets delayed by the amount.
        float[] delayTimer = new float[componentList.Count];
        for (int i = 0; i < componentList.Count; i++)
        {
            delayTimer[i] = delayPerElement * i;
        }


        //Create the timer
        float[] elapsedTime = new float[componentList.Count];
        while (elapsedTime[componentList.Count - 1] < animationPresets.duration)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                //Don't do anything when the animation already finished.
                if (elapsedTime[i] > animationPresets.duration)
                {
                    continue;
                }

                //Don't do anything until the delay timer reaches 0
                if(delayTimer[i] > 0f) 
                {
                    delayTimer[i] -= Time.deltaTime;
                    continue;
                }

                float t = elapsedTime[i] / animationPresets.duration;
                float curveValue = animationPresets.curveFadeIn.Evaluate(t);

                //Fade in the alpha
                if (componentList[i] is TextMeshProUGUI)
                {
                    float alpha = curveValue * originalAlpha[i];
                    ((TextMeshProUGUI)componentList[i]).alpha = alpha;
                }
                else if (componentList[i] is Image)
                {
                    float alpha = curveValue * originalAlpha[i];
                    Color imageColor = ((Image)componentList[i]).color;
                    imageColor.a = alpha;
                    ((Image)componentList[i]).color = imageColor;
                }

                elapsedTime[i] += Time.deltaTime;
            }
            yield return null;
        }

        ////The process
        //while (elapsedTime < animationPresets.duration)
        //{
        //    //Calculate the "t" interpolation value based on the animation curve
        //    float t = elapsedTime / animationPresets.duration;
        //    float curveValue = animationPresets.curveFadeIn.Evaluate(t);

        //    ////Animate the position
        //    //Vector3 currentPosition = Vector3.Lerp(initialPosition, targetPosition, curveValue);
        //    //rectTransform.anchoredPosition = currentPosition;

        //    //Fade in the alpha color in TextMeshPro components
        //    if (componentList.Count > 0)
        //    {
        //        for (int i = 0; i < componentList.Count; i++)
        //        {
        //            if (delayTimer[i] > 0)
        //            {
        //                delayTimer[i] -= Time.deltaTime;
        //                continue;
        //            }

        //            if (componentList[i] is TextMeshProUGUI)
        //            {
        //                float alpha = curveValue * originalAlpha[i];
        //                ((TextMeshProUGUI)componentList[i]).alpha = alpha;
        //            }
        //            else if (componentList[i] is Image)
        //            {
        //                float alpha = curveValue * originalAlpha[i];
        //                Color imageColor = ((Image)componentList[i]).color;
        //                imageColor.a = alpha;
        //                ((Image)componentList[i]).color = imageColor;
        //            }
        //        }
        //    }

        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        //Set the final text color alpha value
        if (componentList.Count > 0)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                if (componentList[i] is TextMeshProUGUI)
                {
                    float alpha = originalAlpha[i];
                    ((TextMeshProUGUI)componentList[i]).alpha = alpha;
                }
                else if (componentList[i] is Image)
                {
                    float alpha = originalAlpha[i];
                    Color imageColor = ((Image)componentList[i]).color;
                    imageColor.a = alpha;
                    ((Image)componentList[i]).color = imageColor;
                }
            }
        }

        coroutine = null;
    }

    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}

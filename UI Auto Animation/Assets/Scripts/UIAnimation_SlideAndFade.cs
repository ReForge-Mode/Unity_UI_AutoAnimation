using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script implements Fade In/Fade Out along with Float In/Float Out animation in UI.
/// This script must be added as a component in any UI gameobject that will be animated..
/// Usually, this requires the animation to be triggered by UIPageDirector, by calling SetEnable function.
/// </summary>
public class UIAnimation_SlideAndFade : MonoBehaviour
{
    public SOAnimationPresets animationPresets;     //Reference to the ScriptableObject

    public float fadeInDelay = 0f;                  //The delay to fade in seconds
    public float fadeOutDelay = 0f;                 //These are used to make objects fade in one by one as the page is loaded


    [Header("Debug")]
    private Coroutine coroutine;                    //Storing Coroutine so we can check if there are any animation is currently running

    //These are for Fade In/Fade Out Animation
    public TextMeshProUGUI[] tmpList;               //Reference to the TextMeshPro component
    public float[] tmpOriginalAlphaList;            //Store the original value of the text alpha
    public Image[] imageList;                       //Reference to the Image component
    public float[] imageOriginalAlphaList;          //Store the original value of the image alpha

    //These are for Float In/Float Out Animation
    private RectTransform rectTransform;            //Reference to the rectTransform component
    private Vector2 initialPosition;                //The initial position of the UI element
    private Vector2 targetPosition;                 //The target position for the UI element to animate to


    private void Awake()
    {
        GetReferences();

        //Get the initial locations
        targetPosition  = rectTransform.anchoredPosition;
        initialPosition = targetPosition + animationPresets.offsetPosition;
        rectTransform.anchoredPosition = initialPosition;

        //Set the initial state as hidden by default
        SetFinalFadeOutState();
    }

    /// <summary>
    /// This is the function needed to trigger the Fade In/Fade Out animation
    /// </summary>
    /// <param name="value"></param>
    public void SetEnable(bool value)
    {
        //Stop any running animation before triggering a new one
        if (coroutine != null) StopAllCoroutines();

        if (value == true)
        {
            coroutine = StartCoroutine(FadeIn());
        }
        else
        {
            coroutine = StartCoroutine(FadeOut());
        }
    }

    /// <summary>
    /// The animation function for Fade In
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(fadeInDelay);

        float elapsedTime = 0f;
        while (elapsedTime < animationPresets.duration)
        {
            //Calculate the "t" interpolation value based on the animation curve
            float t = elapsedTime / animationPresets.duration;
            float curveValue = animationPresets.curveMotionFadeIn.Evaluate(t);

            //Animate the position
            Vector3 currentPosition = Vector3.Lerp(initialPosition, targetPosition, curveValue);
            rectTransform.anchoredPosition = currentPosition;

            //Fade in the text alpha color in TextMeshPro components
            if (tmpList.Length > 0)
            {
                for (int i = 0; i < tmpList.Length; i++)
                {
                    tmpList[i].alpha = curveValue * tmpOriginalAlphaList[i];
                }
            }

            //Fade in the Image alpha color in Image components
            if (imageList.Length > 0)
            {
                for (int i = 0; i < imageList.Length; i++)
                {
                    Color imageColor = imageList[i].color;
                    imageColor.a = curveValue * imageOriginalAlphaList[i];
                    imageList[i].color = imageColor;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetFinalFadeInState();

        coroutine = null;
    }

    /// <summary>
    /// The animation function for FadeOut
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        float elapsedTime = animationPresets.duration;
        while (elapsedTime > 0f)
        {
            //Calculate the "t" interpolation value based on the animation curve
            float t = elapsedTime / animationPresets.duration;
            float curveValue = animationPresets.curveMotionFadeOut.Evaluate(t);

            //Animate the position
            Vector3 currentPosition = Vector3.Lerp(initialPosition, targetPosition, curveValue);
            rectTransform.anchoredPosition = currentPosition;

            //Fade out the text color in TextMeshPro components
            if (tmpList.Length > 0)
            {
                for (int i = 0; i < tmpList.Length; i++)
                {
                    tmpList[i].alpha = curveValue * tmpOriginalAlphaList[i];
                }
            }

            //Fade out the Image color in Image components
            if (imageList.Length > 0)
            {
                for (int i = 0; i < imageList.Length; i++)
                {
                    Color imageColor = imageList[i].color;
                    imageColor.a = curveValue * imageOriginalAlphaList[i];
                    imageList[i].color = imageColor;
                }
            }

            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        //There are always inaccuracies when dealing with float values
        //So to keep it safe, when the final loop is done, set everything to its final state.
        SetFinalFadeOutState();

        coroutine = null;
    }

    /// <summary>
    /// Helper function for setting the final position and alpha values
    /// </summary>
    private void SetFinalFadeInState()
    {
        //Set the final UI position
        rectTransform.anchoredPosition = targetPosition;

        //Set the final text color alpha value
        if (tmpList.Length > 0)
        {
            for (int i = 0; i < tmpList.Length; i++)
            {
                tmpList[i].alpha = tmpOriginalAlphaList[i];
            }
        }

        //Set the final image color alpha value
        if (imageList.Length > 0)
        {
            for (int i = 0; i < imageList.Length; i++)
            {
                Color imageColor = imageList[i].color;
                imageColor.a = imageOriginalAlphaList[i];
                imageList[i].color = imageColor;
            }
        }
    }

    /// <summary>
    /// Helper function for setting the final position and alpha values
    /// </summary>
    private void SetFinalFadeOutState()
    {
        //Set the final UI position
        rectTransform.anchoredPosition = initialPosition;

        //Set the final text color alpha value
        if (tmpList.Length > 0)
        {
            for (int i = 0; i < tmpList.Length; i++)
            {
                tmpList[i].alpha = 0f;
            }
        }

        //Set the final image color alpha value
        if (imageList.Length > 0)
        {
            for (int i = 0; i < imageList.Length; i++)
            {
                Color imageColor = imageList[i].color;
                imageColor.a = 0;
                imageList[i].color = imageColor;
            }
        }
    }

    /// <summary>
    /// Gets all references of UI TextMeshPro and UI Image
    /// in this gameobject and its children in the hierarchy.
    /// Store the reference 
    /// </summary>
    private void GetReferences()
    {
        rectTransform = GetComponent<RectTransform>();

        //Find TMP in parent and children
        if (tmpOriginalAlphaList.Length == 0)
        {
            var tmpInParent = GetComponents<TextMeshProUGUI>();
            var tmpInChildren = GetComponentsInChildren<TextMeshProUGUI>();
            tmpList = tmpInParent.Concat(tmpInChildren).ToArray();

            //Find the original alpha for the TMP
            tmpOriginalAlphaList = new float[tmpList.Length];
            for (int i = 0; i < tmpList.Length; i++)
            {
                tmpOriginalAlphaList[i] = tmpList[i].alpha;
            }
        }

        //Find Image in parent and children
        if (imageOriginalAlphaList.Length == 0)
        {
            var imageInParent = GetComponents<Image>();
            var imageInChildren = GetComponentsInChildren<Image>();
            imageList = imageInParent.Concat(imageInChildren).ToArray();

            //Find the original alpha for Images
            imageOriginalAlphaList = new float[imageList.Length];
            for (int i = 0; i < imageList.Length; i++)
            {
                imageOriginalAlphaList[i] = imageList[i].color.a;
            }
        }
    }
}

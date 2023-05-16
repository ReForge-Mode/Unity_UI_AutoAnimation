using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public SOAnimationPresets animationPresets;     //Reference to the ScriptableObject

    public SearchMode searchMode;

    //For animating text color (fade in/fade out)
    public List<Component> componentList;

    public void Awake()
    {
        componentList = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));
    }

    public void GetTextMeshComponents()
    {
        if (searchMode == SearchMode.DepthFirst)
        {
            componentList = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));
        }
        else if (searchMode == SearchMode.BreadthFirst)
        {
            componentList = ComponentSearch.GetComponentInHierarchy_BreadthFirst<Component>(transform, typeof(TextMeshProUGUI), typeof(Image));
        }
    }

    public enum SearchMode
    {
        DepthFirst, BreadthFirst
    }
}

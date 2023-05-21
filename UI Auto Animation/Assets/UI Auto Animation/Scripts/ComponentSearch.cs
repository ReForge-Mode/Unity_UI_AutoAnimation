using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// This class contains functions to list specific or multiple components type in the child objects. 
/// Unlike GetComponentsInChildren, you can define to search them Depth-First or Breadth-First. 
/// Plus, you can search different types of components in a single list, like UI objects
/// </summary>
public static class ComponentSearch
{
    /// Example on how to use these functions:
    /// var components = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(Transform), typeof(Rigidbody), typeof(BoxCollider));
    /// var components = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(TextMeshProUGUI), typeof(Image));

    /// <summary>
    /// Gets all of the components of the specified types on the current object and its children, in a depth-first traversal.
    /// </summary>
    /// <param name="current">The current object.</param>
    /// <param name="types">The types of components to search for.</param>
    /// <returns>A list of all of the found components.</returns>
    public static List<Component> GetComponentInHierarchy_DepthFirst(Transform current, params Type[] types)
    {
        List<Component> componentList = new List<Component>();

        DepthFirstRecursion(current, ref componentList, types);

        return componentList;
    }

    private static void DepthFirstRecursion(Transform current, ref List<Component> componentList, params Type[] types)
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
            DepthFirstRecursion(current.GetChild(i), ref componentList, types);
        }
    }

    public static List<Component> GetComponentInHierarchy_BreadthFirst(Transform root, params Type[] types)
    {
        List<Component> componentList = new List<Component>();

        //Create a queue to store the nodes that have not yet been visited.
        Queue<Transform> queue = new Queue<Transform>();

        //Add the current node to the queue.
        queue.Enqueue(root);

        //While the queue is not empty, do the following:
        while (queue.Count > 0)
        {
            //Get the next node from the queue.
            Transform node = queue.Dequeue();

            Component component = null;
            foreach (Type type in types)
            {
                component = node.GetComponent(type);
                if (component != null)
                {
                    componentList.Add(component);
                    Debug.Log(component + " added!");
                }
            }

            //Now, get all of its children transform and add them to the queue.
            int childCount = node.childCount;
            for (int i = 0; i < childCount; i++)
            {
                queue.Enqueue(node.GetChild(i));
            }
        }

        return componentList;
    }
}
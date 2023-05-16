using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class ComponentSearch
{
    /// Example on how to use these functions:
    /// var components = ComponentSearch.GetComponentInHierarchy_DepthFirst(transform, typeof(Transform), typeof(Rigidbody), typeof(BoxCollider));


    /// <summary>
    /// Gets all of the components of the specified types on the current object and its children, in a depth-first traversal.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
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
                Debug.Log(component + " added!");
            }
        }

        //Now, get all of its children transform and recursively use this function again.
        int childCount = current.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DepthFirstRecursion(current.GetChild(i), ref componentList, types);
        }
    }

    /// <summary>
    /// Gets all of the components of the specified types on the current object and its children, in a breadth-first traversal.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
    /// <param name="root">The root object.</param>
    /// <param name="types">The types of components to search for.</param>
    /// <returns>A list of all of the found components.</returns>
    public static List<T> GetComponentInHierarchy_BreadthFirst<T>(Transform root, params Type[] types)
    {
        List<T> componentList = new List<T>();

        //Create a queue to store the nodes that have not yet been visited.
        Queue<Transform> queue = new Queue<Transform>();

        //Add the current node to the queue.
        queue.Enqueue(root);

        //While the queue is not empty, do the following:
        while (queue.Count > 0)
        {
            //Get the next node from the queue.
            Transform node = queue.Dequeue();

            //Get this object's T, if it exists and add it to the list
            T rect = node.GetComponent<T>();
            if (rect != null)
            {
                componentList.Add(rect);
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
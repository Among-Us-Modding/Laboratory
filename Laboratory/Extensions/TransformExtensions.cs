using System;
using UnityEngine;

namespace Laboratory.Extensions;

public static class TransformExtensions
{
    /// <summary>
    /// Sets the world X position of a transform
    /// </summary>
    public static Transform SetX(this Transform transform, float x)
    {
        Vector3 vector = transform.position;
        vector.x = x;
        transform.position = vector;

        return transform;
    }

    /// <summary>
    /// Sets the world Y position of a transform
    /// </summary>
    public static Transform SetY(this Transform transform, float y)
    {
        Vector3 vector = transform.position;
        vector.y = y;
        transform.position = vector;

        return transform;
    }

    /// <summary>
    /// Sets the world Z position of a transform
    /// </summary>
    public static Transform SetZ(this Transform transform, float z)
    {
        Vector3 vector = transform.position;
        vector.z = z;
        transform.position = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local X position of a transform
    /// </summary>
    public static Transform SetLocalX(this Transform transform, float x)
    {
        Vector3 vector = transform.localPosition;
        vector.x = x;
        transform.localPosition = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local Y position of a transform
    /// </summary>
    public static Transform SetLocalY(this Transform transform, float y)
    {
        Vector3 vector = transform.localPosition;
        vector.y = y;
        transform.localPosition = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local Z position of a transform
    /// </summary>
    public static Transform SetLocalZ(this Transform transform, float z)
    {
        Vector3 vector = transform.localPosition;
        vector.z = z;
        transform.localPosition = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local X scale of a transform
    /// </summary>
    public static Transform SetScaleX(this Transform transform, float x)
    {
        Vector3 vector = transform.localScale;
        vector.x = x;
        transform.localScale = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local Y scale of a transform
    /// </summary>
    public static Transform SetScaleY(this Transform transform, float y)
    {
        Vector3 vector = transform.localScale;
        vector.y = y;
        transform.localScale = vector;

        return transform;
    }

    /// <summary>
    /// Sets the local Z scale of a transform
    /// </summary>
    public static Transform SetScaleZ(this Transform transform, float z)
    {
        Vector3 vector = transform.localScale;
        vector.z = z;
        transform.localScale = vector;

        return transform;
    }

    /// <summary>
    /// Performs an action on every child of a transform with its sibling index
    /// </summary>
    public static void ForeachChildIdx(this Transform transform, Action<int, Transform> action)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            action(i, transform.GetChild(i));
        }
    }

    /// <summary>
    /// Get all the children of a transform
    /// </summary>
    public static Transform[] GetChildren(this Transform transform)
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }

        return children;
    }
}
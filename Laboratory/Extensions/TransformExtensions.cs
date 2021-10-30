using System;
using UnityEngine;

namespace Laboratory.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Sets the world X position of a transform
        /// </summary>
        public static Transform SetXPos(this Transform transform, float x)
        {
            Vector3 vector = transform.position;
            vector.x = x;
            transform.position = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the world Y position of a transform
        /// </summary>
        public static Transform SetYPos(this Transform transform, float y)
        {
            Vector3 vector = transform.position;
            vector.y = y;
            transform.position = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the world Z position of a transform
        /// </summary>
        public static Transform SetZPos(this Transform transform, float z)
        {
            Vector3 vector = transform.position;
            vector.z = z;
            transform.position = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the local X position of a transform
        /// </summary>
        public static Transform SetXLocalPos(this Transform transform, float x)
        {
            Vector3 vector = transform.localPosition;
            vector.x = x;
            transform.localPosition = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the local Y position of a transform
        /// </summary>
        public static Transform SetYLocalPos(this Transform transform, float y)
        {
            Vector3 vector = transform.localPosition;
            vector.y = y;
            transform.localPosition = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the local Z position of a transform
        /// </summary>
        public static Transform SetZLocalPos(this Transform transform, float z)
        {
            Vector3 vector = transform.localPosition;
            vector.z = z;
            transform.localPosition = vector;
            
            return transform;
        }

        /// <summary>
        /// Sets the local X scale of a transform
        /// </summary>
        public static Transform SetXScale(this Transform transform, float x)
        {
            Vector3 vector = transform.localScale;
            vector.x = x;
            transform.localScale = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the local Y scale of a transform
        /// </summary>
        public static Transform SetYScale(this Transform transform, float y)
        {
            Vector3 vector = transform.localScale;
            vector.y = y;
            transform.localScale = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Sets the local Z scale of a transform
        /// </summary>
        public static Transform SetZScale(this Transform transform, float z)
        {
            Vector3 vector = transform.localScale;
            vector.z = z;
            transform.localScale = vector;
            
            return transform;
        }
        
        /// <summary>
        /// Performs an action on every child of a transform
        /// </summary>
        public static void ForeachChild(this Transform transform, Action<Transform> action)
        {
            foreach (var child in transform.GetChildren())
            {
                action(child);
            }
        }
    
        /// <summary>
        /// Performs an action on every child of a transform with its sibling index
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="action"></param>
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
            int childCount = transform.childCount;
            Transform[] children = new Transform[childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }
    }
}
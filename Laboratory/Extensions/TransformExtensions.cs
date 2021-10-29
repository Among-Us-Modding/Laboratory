using System;
using UnityEngine;

namespace Laboratory.Extensions
{
    public static class TransformExtensions
    {
        public static Transform SetXPos(this Transform transform, float x)
        {
            Vector3 vector = transform.position;
            vector.x = x;
            transform.position = vector;
            
            return transform;
        }
        
        public static Transform SetYPos(this Transform transform, float y)
        {
            Vector3 vector = transform.position;
            vector.y = y;
            transform.position = vector;
            
            return transform;
        }
        
        public static Transform SetZPos(this Transform transform, float z)
        {
            Vector3 vector = transform.position;
            vector.z = z;
            transform.position = vector;
            
            return transform;
        }
        
        public static Transform SetXLocalPos(this Transform transform, float x)
        {
            Vector3 vector = transform.localPosition;
            vector.x = x;
            transform.localPosition = vector;
            
            return transform;
        }
        
        public static Transform SetYLocalPos(this Transform transform, float y)
        {
            Vector3 vector = transform.localPosition;
            vector.y = y;
            transform.localPosition = vector;
            
            return transform;
        }
        
        public static Transform SetZLocalPos(this Transform transform, float z)
        {
            Vector3 vector = transform.localPosition;
            vector.z = z;
            transform.localPosition = vector;
            
            return transform;
        }
        
        public static void SetLocalZ(this Transform self, float z)
        {
            Vector3 localPosition = self.localPosition;
            localPosition.z = z;
            self.localPosition = localPosition;
        }
        
        public static void SetWorldZ(this Transform self, float z)
        {
            Vector3 position = self.position;
            position.z = z;
            self.position = position;
        }

        
        public static Transform SetXScale(this Transform transform, float x)
        {
            Vector3 vector = transform.localScale;
            vector.x = x;
            transform.localScale = vector;
            
            return transform;
        }
        
        public static Transform SetYScale(this Transform transform, float y)
        {
            Vector3 vector = transform.localScale;
            vector.y = y;
            transform.localScale = vector;
            
            return transform;
        }
        
        public static Transform SetZScale(this Transform transform, float z)
        {
            Vector3 vector = transform.localScale;
            vector.z = z;
            transform.localScale = vector;
            
            return transform;
        }
        
        public static void ForeachChild(this Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action(transform.GetChild(i));
            }
        }
    
        public static void ForeachChildIdx(this Transform transform, Action<int, Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action(i, transform.GetChild(i));
            }
        }

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
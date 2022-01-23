using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Laboratory.Utils;

/// <summary>
/// Clone of <see cref="Effects"/> so you can use <see cref="System.Action"/>
/// </summary>
public static class MEffects
{
    private static readonly HashSet<Transform> _activeShakes = new();

    public static IEnumerator Action(Action todo)
    {
        todo();
        yield break;
    }

    public static IEnumerator Wait(float duration)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            yield return null;
        }
    }

    public static IEnumerator Sequence(params IEnumerator[] items)
    {
        var i = 0;
        while (i < items.Length)
        {
            yield return items[i];
            var num = i + 1;
            i = num;
        }
    }

    public static IEnumerator All(params IEnumerator[] items)
    {
        var enums = new Stack<IEnumerator>[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            enums[i] = new Stack<IEnumerator>();
            enums[i].Push(items[i]);
        }

        var cap = 0;
        while (cap < 100000)
        {
            var flag = false;
            foreach (var t in enums)
            {
                if (t.Count <= 0)
                {
                    continue;
                }

                flag = true;
                var enumerator = t.Peek();
                if (enumerator.MoveNext())
                {
                    if (enumerator.Current is IEnumerator current)
                    {
                        t.Push(current);
                    }
                }
                else
                {
                    t.Pop();
                }
            }

            if (flag)
            {
                yield return null;
                var num = cap + 1;
                cap = num;
                continue;
            }

            break;
        }
    }

    public static IEnumerator Lerp(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return null;
        }

        action(1f);
    }

    public static IEnumerator Overlerp(float duration, Action<float> action, float overextend = 0.05f)
    {
        var d1 = duration * 0.95f;
        for (var t2 = 0f; t2 < d1; t2 += Time.deltaTime)
        {
            action(Mathf.Lerp(0f, 1f + overextend, t2 / d1));
            yield return null;
        }

        var d2 = duration * 0.050000012f;
        for (var t2 = 0f; t2 < d2; t2 += Time.deltaTime)
        {
            action(Mathf.Lerp(1f + overextend, 1f, t2 / d2));
            yield return null;
        }

        action(1f);
    }

    public static IEnumerator ScaleIn(Transform self, float source, float target, float duration)
    {
        if ((bool)self)
        {
            var localScale = default(Vector3);
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                localScale.x = (localScale.y = (localScale.z = Mathf.SmoothStep(source, target, t / duration)));
                self.localScale = localScale;
                yield return null;
            }

            localScale.x = (localScale.y = (localScale.z = target));
            self.localScale = localScale;
        }
    }

    public static IEnumerator CycleColors(SpriteRenderer self, Color source, Color target, float rate, float duration)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                var t2 = Mathf.Sin(t * (float)Math.PI / rate) / 2f + 0.5f;
                self.color = Color.Lerp(source, target, t2);
                yield return null;
            }

            self.color = source;
        }
    }

    public static IEnumerator PulseColor(SpriteRenderer self, Color source, Color target, float duration = 0.5f)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(target, source, t / duration);
                yield return null;
            }

            self.color = source;
        }
    }

    public static IEnumerator PulseColor(TextMeshPro self, Color source, Color target, float duration = 0.5f)
    {
        if ((bool)self)
        {
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(target, source, t / duration);
                yield return null;
            }

            self.color = source;
        }
    }

    public static IEnumerator ColorFade(TextMeshPro self, Color source, Color target, float duration)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(source, target, t / duration);
                yield return null;
            }

            self.color = target;
        }
    }

    public static IEnumerator ColorFade(SpriteRenderer self, Color source, Color target, float duration)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(source, target, t / duration);
                yield return null;
            }

            self.color = target;
        }
    }

    public static IEnumerator Rotate2D(Transform target, float source, float dest, float duration = 0.75f)
    {
        var temp = target.localEulerAngles;
        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            temp.z = Mathf.SmoothStep(source, dest, t);
            target.localEulerAngles = temp;
            yield return null;
        }

        temp.z = dest;
        target.localEulerAngles = temp;
    }

    public static IEnumerator Slide3D(Transform target, Vector3 source, Vector3 dest, float duration = 0.75f)
    {
        var localPosition = default(Vector3);
        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            localPosition.x = Mathf.SmoothStep(source.x, dest.x, t);
            localPosition.y = Mathf.SmoothStep(source.y, dest.y, t);
            localPosition.z = Mathf.Lerp(source.z, dest.z, t);
            target.localPosition = localPosition;
            yield return null;
        }

        target.localPosition = dest;
    }

    public static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f)
    {
        var temp = default(Vector3);
        temp.z = target.localPosition.z;
        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            temp.x = Mathf.SmoothStep(source.x, dest.x, t);
            temp.y = Mathf.SmoothStep(source.y, dest.y, t);
            target.localPosition = temp;
            yield return null;
        }

        temp.x = dest.x;
        temp.y = dest.y;
        target.localPosition = temp;
    }

    public static IEnumerator Slide2DWorld(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f)
    {
        var temp = default(Vector3);
        temp.z = target.position.z;
        for (var time = 0f; time < duration; time += Time.deltaTime)
        {
            var t = time / duration;
            temp.x = Mathf.SmoothStep(source.x, dest.x, t);
            temp.y = Mathf.SmoothStep(source.y, dest.y, t);
            target.position = temp;
            yield return null;
        }

        temp.x = dest.x;
        temp.y = dest.y;
        target.position = temp;
    }

    public static IEnumerator Bounce(Transform target, float duration = 0.3f, float height = 0.15f)
    {
        if (!target)
        {
            yield break;
        }

        var origin = target.localPosition;
        var temp = origin;
        for (var timer = 0f; timer < duration; timer += Time.deltaTime)
        {
            var num = timer / duration;
            var num2 = 1f - num;
            temp.y = origin.y + height * Mathf.Abs(Mathf.Sin(num * (float)Math.PI * 3f)) * num2;
            if (!target)
            {
                yield break;
            }

            target.localPosition = temp;
            yield return null;
        }

        if ((bool)target)
        {
            target.transform.localPosition = origin;
        }
    }

    public static IEnumerator Shake(Transform target, float duration, float halfWidth, bool taper)
    {
        _ = target.localPosition;
        for (var timer = 0f; timer < duration; timer += Time.deltaTime)
        {
            var num = timer / duration;
            var vector = (Vector3)UnityEngine.Random.insideUnitCircle * halfWidth;
            if (taper)
            {
                vector *= 1f - num;
            }

            target.localPosition += vector;
            yield return null;
        }
    }

    public static IEnumerator SwayX(Transform target, float duration = 0.75f, float halfWidth = 0.25f)
    {
        if (_activeShakes.Add(target))
        {
            var origin = target.localPosition;
            for (var timer = 0f; timer < duration; timer += Time.deltaTime)
            {
                var num = timer / duration;
                target.localPosition = origin + Vector3.right * (halfWidth * Mathf.Sin(num * 30f) * (1f - num));
                yield return null;
            }

            target.transform.localPosition = origin;
            _activeShakes.Remove(target);
        }
    }

    public static IEnumerator Bloop(float delay, Transform target, float finalSize = 1f, float duration = 0.5f)
    {
        for (var t = 0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }

        var localScale = default(Vector3);
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            var z = ElasticOut(t, duration) * finalSize;
            localScale.x = (localScale.y = (localScale.z = z));
            target.localScale = localScale;
            yield return null;
        }

        localScale.x = (localScale.y = (localScale.z = finalSize));
        target.localScale = localScale;
    }

    public static IEnumerator ArcSlide(float duration, Transform target, Vector2 sourcePos, Vector2 targetPos, float anchorDistance)
    {
        var vector = (targetPos - sourcePos) / 2f;
        var anchor = sourcePos + vector + vector.Rotate(90f).normalized * anchorDistance;
        var z = target.localPosition.z;
        for (var timer = 0f; timer < duration; timer += Time.deltaTime)
        {
            Vector3 localPosition = Bezier(timer / duration, sourcePos, targetPos, anchor);
            localPosition.z = z;
            target.localPosition = localPosition;
            yield return null;
        }

        target.transform.localPosition = targetPos;
    }

    public static Vector3 Bezier(float t, Vector3 src, Vector3 dest, Vector3 anchor)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        var num = 1f - t;
        return num * num * src + 2f * num * t * anchor + t * t * dest;
    }

    public static Vector2 Bezier(float t, Vector2 src, Vector2 dest, Vector2 anchor)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        var num = 1f - t;
        return num * num * src + 2f * num * t * anchor + t * t * dest;
    }

    private static float ElasticOut(float time, float duration)
    {
        time /= duration;
        var num = time * time;
        var num2 = num * time;
        return 33f * num2 * num + -106f * num * num + 126f * num2 + -67f * num + 15f * time;
    }

    public static float ExpOut(float t)
    {
        return Mathf.Clamp(1f - Mathf.Pow(2f, -10f * t), 0f, 1f);
    }
}

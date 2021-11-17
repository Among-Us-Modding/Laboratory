using System;
using System.Collections;
using PowerTools;
using UnityEngine;

namespace Laboratory.Extensions;

public static class AnimationExtensions
{
    public static IEnumerator Wait(this AnimationClip clip, Action<AnimationEvent>? onEvent = null)
    {
        var length = clip.length;

        foreach (var animationEvent in clip.events)
        {
            var time = animationEvent.time;
            length -= time;

            yield return new WaitForSeconds(time);

            onEvent?.Invoke(animationEvent);
        }

        yield return new WaitForSeconds(length);
    }

    public static IEnumerator PlayAndWait(this SpriteAnim animator, AnimationClip clip, Action<AnimationEvent>? onEvent = null)
    {
        animator.Play(clip);
        yield return clip.Wait(onEvent);
    }
}

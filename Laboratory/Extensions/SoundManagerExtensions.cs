using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Laboratory.Extensions;

public static class SoundManagerExtensions
{
    public static AudioSource PlaySoundAtPoint(this SoundManager soundManager, AudioClip audioClip, Vector2 point, float volume = 1f)
    {
        void PointSoundFunc(AudioSource source, float dt)
        {
            var distance = Vector2.Distance(point, PlayerControl.LocalPlayer.GetTruePosition());
            var maxDist = 4;
            if (distance > maxDist)
            {
                source.volume = 0f;
            }

            var modVol = 1f - Mathf.Clamp01(distance / maxDist);
            source.volume = Mathf.Lerp(source.volume, volume * modVol, dt);
        }

        DynamicSound.GetDynamicsFunction func = (Action<AudioSource, float>)PointSoundFunc;
        return soundManager.PlayDynamicSound(Random.RandomRangeInt(0, int.MaxValue).ToString(), audioClip, false, func, true);
    }

    public static AudioSource PlaySoundAtTransform(this SoundManager soundManager, AudioClip audioClip, Transform trns, float volume = 1f)
    {
        void PointSoundFunc(AudioSource source, float dt)
        {
            var distance = Vector2.Distance(trns.position, PlayerControl.LocalPlayer.GetTruePosition());
            var maxDist = 4;
            if (distance > maxDist)
            {
                source.volume = 0f;
            }

            var modVol = 1f - Mathf.Clamp01(distance / maxDist);
            source.volume = Mathf.Lerp(source.volume, volume * modVol, dt);
        }

        DynamicSound.GetDynamicsFunction func = (Action<AudioSource, float>)PointSoundFunc;
        return soundManager.PlayDynamicSound(Random.RandomRangeInt(0, int.MaxValue).ToString(), audioClip, false, func, true);
    }
}

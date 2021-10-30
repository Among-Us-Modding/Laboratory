using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Laboratory.Mods.Utils
{
    public class AudioHelpers
    {
        public static AudioSource PlaySoundAtPoint(AudioClip audioClip, Vector2 point, float volume)
        {
            void PointSoundFunc(AudioSource source, float dt)
            {
                float distance = Vector2.Distance(point, PlayerControl.LocalPlayer.GetTruePosition());
                int maxDist = 4;
                if (distance > maxDist)
                {
                    source.volume = 0f;
                }
                float modVol = 1f - Mathf.Clamp01(distance / maxDist);
                source.volume = Mathf.Lerp(source.volume, volume * modVol, dt);
            }

            DynamicSound.GetDynamicsFunction func = (Action<AudioSource, float>) PointSoundFunc;
            return SoundManager.Instance.PlayDynamicSound(Random.RandomRangeInt(0, Int32.MaxValue).ToString(), audioClip, false, func, true);
        }
        
        public static AudioSource PlaySoundAtTransform(AudioClip audioClip, Transform trns, float volume)
        {
            void PointSoundFunc(AudioSource source, float dt)
            {
                float distance = Vector2.Distance(trns.position, PlayerControl.LocalPlayer.GetTruePosition());
                int maxDist = 4;
                if (distance > maxDist)
                {
                    source.volume = 0f;
                }
                float modVol = 1f - Mathf.Clamp01(distance / maxDist);
                source.volume = Mathf.Lerp(source.volume, volume * modVol, dt);
            }

            DynamicSound.GetDynamicsFunction func = (Action<AudioSource, float>) PointSoundFunc;
            return SoundManager.Instance.PlayDynamicSound(Random.RandomRangeInt(0, Int32.MaxValue).ToString(), audioClip, false, func, true);
        }
    }
}
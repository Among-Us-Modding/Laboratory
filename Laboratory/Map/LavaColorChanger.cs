using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Laboratory.Map;

public static class LavaColorChanger
{
    /// <summary>
    /// Default color scheme for lava colors
    /// </summary>
    public static Color32[] Default { get; } =
    {
        new Color32(255, 98, 0, 255),
        new Color32(255, 103, 1, 255),
        new Color32(255, 42, 0, 255),
        new Color32(255, 225, 21, 255),
        new Color32(255, 98, 0, 255),
    };

    /// <summary>
    /// Blue / Water color scheme for lava colors
    /// </summary>
    public static Color32[] Blue { get; } =
    {
        new Color32(17, 111, 137, 255),
        new Color32(33, 143, 173, 255),
        new Color32(24, 79, 94, 255),
        new Color32(105, 198, 227, 255),
        new Color32(17, 111, 137, 255),
    };

    /// <summary>
    /// Reference color scheme for lava colors
    /// </summary>
    public static Color32[] Reference { get; } =
    {
        new Color32(255, 0, 0, 255),
        new Color32(255, 170, 170, 255),
        new Color32(0, 255, 0, 255),
        new Color32(0, 0, 255, 255),
        new Color32(0, 0, 0, 255),
    };

    /// <summary>
    /// This allows you to replace the colors of the lava in polus
    /// </summary>
    /// <param name="colors">The colors that should be provided to replace the lava colors with.</param>
    /// <remarks>https://media.discordapp.net/attachments/885556508866265098/890628223862448149/Lava_Recolor_Guide.png</remarks>
    public static void ChangeColors(Color32[] colors)
    {
        if (colors.Length != 5) throw new ArgumentException("You need exactly 5 replacement colors", nameof(colors));

        if (colors != Default)
        {
            GameObject.Find("BubbleParent")?.SetActive(false);

            MeshAnimator meshAnimator = GameObject.Find("LavaOrange").GetComponent<MeshAnimator>();

            foreach (Mesh mesh in meshAnimator.Frames)
            {
                List<Color32> currentColors = new();
                mesh.GetColors(currentColors);

                Il2CppStructArray<Color32> replacedColors = new Il2CppStructArray<Color32>(currentColors.Count);
                for (int i = 0; i < currentColors.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        // DNF is hard clearing this sus line // But it's not even throwing a warning anymore...
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        if (currentColors[(Index) i].Equals(Default[j]))
                        {
                            replacedColors[i] = colors[j];
                        }
                    }
                }

                mesh.SetColors(replacedColors);
            }
        }

        // Makes a sprite to go under the clear part of the lava
        SpriteRenderer blankSprite = new GameObject { layer = 9 }.AddComponent<SpriteRenderer>();
        blankSprite.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
        blankSprite.drawMode = SpriteDrawMode.Sliced;
        blankSprite.size = new Vector2(8.63f, 1.28f);
        blankSprite.transform.position = new Vector3(40.038f, -15.413f, 15);
        blankSprite.color = colors[4];
    }
}

using System;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.CustomMap;

public static class LavaColorChanger
{
    /// <summary>
    /// This allows you to replace the colors of the lava in polus
    /// </summary>
    /// <param name="replacedColors">The colors that should be provided to replace the lava colors with.
    /// See reference colors in method for an example of how this should look</param>
    public static void ChangeColor(Color32[] replacedColors)
    {
        // Argument checking
        if (replacedColors.Length != 5) throw new InvalidOperationException("Bro u need a replacement color for all of them");
            
        var blankSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            
        // Not actually needed, just for reference back to image provided to artists
        var artistReferenceColors = new[]
        {
            new Color32(255, 0, 0, 255),
            new Color32(255, 170, 170, 255),
            new Color32(0, 255, 0, 255),
            new Color32(0, 0, 255, 255),
            new Color32(0, 0, 0, 255),
        };

        // Hide the bubbles
        GameObject.Find("BubbleParent")?.SetActive(false);

        // Get lava
        var meshAnimator = GameObject.Find("LavaOrange").GetComponent<MeshAnimator>();
            
        List<Color32> color32s = new();
        System.Collections.Generic.List<Color32> newColors = new();
        // Swap colors in frames
        foreach (var mesh in meshAnimator.Frames)
        {
            color32s.Clear();
            mesh.GetColors(color32s);
            var newColor32s = new Il2CppStructArray<Color32>(color32s.Count);

            var i = 0;
            // Replace colors with ones from replacement colors array
            foreach (var current in color32s)
            {
                if (!newColors.Contains(current)) newColors.Add(current);

                // we only check green here as all colors are unique here and is faster
                newColor32s[i] = current.g switch
                {
                    98 => replacedColors[0],
                    103 => replacedColors[1],
                    42 => replacedColors[2],
                    225 => replacedColors[3],
                    _ => current
                };

                ++i;
            }
                
            // Set the mesh colors
            mesh.SetColors(newColor32s);
        }
            
        // Makes a sprite to go under the clear part of the lava
        var blankSpriteObj = new GameObject{layer = 9}.AddComponent<SpriteRenderer>();
        blankSpriteObj.sprite = blankSprite;
        blankSpriteObj.drawMode = SpriteDrawMode.Sliced;
        blankSpriteObj.size = new Vector2(8.63f, 1.28f);
        blankSpriteObj.transform.position = new Vector3(40.038f, -15.413f, 15);
        blankSpriteObj.color = replacedColors[4];
    }
}
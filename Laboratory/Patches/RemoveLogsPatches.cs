using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Text;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class LanguageUnitLogPatch
{
    public static MethodBase TargetMethod()
    {
        return typeof(LanguageUnit).GetConstructor(new [] {typeof(TranslatedImageSet)})!;
    }

    [HarmonyPrefix]
    public static bool Prefix(LanguageUnit __instance, TranslatedImageSet locSet)
    {
        // The changes here are just adding what would automaticly be in the constructor and removing logs 
        __instance.AllStrings = new Dictionary<StringNames, string>();
        __instance.AllImages = new Dictionary<ImageNames, Sprite>();
        __instance.AllQuickChatVariantSets = new Dictionary<StringNames, QuickChatSentenceVariantSet>();
        __instance.builder = new StringBuilder(512);
        
        __instance.languageID = locSet.languageID;
        ImageData[] images = locSet.Images;
        for (int i = 0; i < images.Length; i++)
        {
            ImageData imageData = images[i];
            __instance.AllImages.Add(imageData.Name, imageData.Sprite);
        }
        using (StringReader stringReader = new StringReader(locSet.Data.text))
        {
            for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
            {
                if (text.Length != 0)
                {
                    int num = text.IndexOf('\t');
                    if (num < 0)
                    {
                        // Debug.LogWarning("Couldn't parse: " + text);
                    }
                    else
                    {
                        string text2 = text.Substring(0, num);
                        string text3 = __instance.UnescapeCodes(text, num + 1);
                        if (text2.Length != 0 || text3.Length != 0)
                        {
                            if (!Enum.TryParse<StringNames>(text2, out StringNames result) && !string.IsNullOrEmpty(text2))
                            {
                                if (text2[0] == 'Q' && text2[1] == 'C')
                                {
                                    __instance.HandleQCSentenceVariant(text2, text3);
                                }
                            }
                            else if (__instance.AllStrings.ContainsKey(result))
                            {
                                // Debug.LogWarning($"Duplicate translation for {result}: '{text3}' and '{AllStrings[result]}'");
                            }
                            else
                            {
                                __instance.AllStrings.Add(result, text3);
                            }
                        }
                    }
                }
            }
        }
        foreach (StringNames value in Enum.GetValues(typeof(StringNames)))
        {
            if (!__instance.AllStrings.ContainsKey(value))
            {
                // if (locSet.name == "English")
                // {
                //     string[] obj = new string[6]
                //     {
                //         "No Translation for ",
                //         value.ToString(),
                //         " (",
                //         null,
                //         null,
                //         null
                //     };
                //     int i = (int)value;
                //     obj[3] = i.ToString();
                //     obj[4] = ") in language: ";
                //     obj[5] = locSet.name;
                //     Debug.LogWarning(string.Concat(obj));
                // }
                // else
                // {
                //     Debug.LogWarning("No Translation for " + value.ToString() + " in language: " + locSet.name);
                // }
            }
        }
        return false;
    }
}

[HarmonyPatch]
internal static class RemoveLogsPatches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    [HarmonyPrefix]
    public static bool NoValidMenuActivePatch()
    {
        return false;
    }
}
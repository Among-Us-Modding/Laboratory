using System.Collections.Generic;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Debugging.Unstripping;

[HarmonyPatch]
internal static class UnstrippingPatches
{
    [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.DoTextField), typeof(string), typeof(int), typeof(bool), typeof(GUIStyle), typeof(Il2CppReferenceArray<GUILayoutOption>))]
    [HarmonyPrefix]
    public static bool GUILayoutDoTextFieldPatch(GUILayout __instance, ref string __result, string text, int maxLength, bool multiline, GUIStyle style, Il2CppReferenceArray<GUILayoutOption> options)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
        GUIContent.Temp(text);
        GUIContent content = GUIUtility.keyboardControl == controlId ? GUIContent.Temp(text + GUIUtility.compositionString) : GUIContent.Temp(text);
        Rect rect = GUILayoutUtility.GetRect(content, style, options);
        if (GUIUtility.keyboardControl == controlId) content = GUIContent.Temp(text);
        GUI.DoTextField(rect, controlId, content, multiline, maxLength, style);
        __result = content.text;
        return false;
    }

    [HarmonyPatch(typeof(GUIUtility), nameof(GUIUtility.GetControlID), typeof(FocusType))]
    [HarmonyPrefix]
    public static bool GUIUtilityGetControlIDPatch(GUIUtility __instance, ref int __result, FocusType focus)
    {
        Rect rect = new(0, 0, 0, 0);
        __result = GUIUtility.GetControlID_Injected(0, focus, ref rect);
        return false;
    }

    private static Dictionary<int, TextEditor_Reimpl> StateObjects = new();

    private static TextEditor_Reimpl GetStateObject(int controlID)
    {
        if (!StateObjects.TryGetValue(controlID, out TextEditor_Reimpl instance))
        {
            instance = new TextEditor_Reimpl();
            StateObjects[controlID] = instance;
        }

        return instance;
    }

    private static void HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor_Reimpl editor)
    {
        Event current = Event.current;
        bool flag = false;
        switch (current.type)
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition))
                {
                    GUIUtility.hotControl = id;
                    GUIUtility.keyboardControl = id;
                    editor.m_HasFocus = true;
                    editor.MoveCursorToPosition(Event.current.mousePosition);
                    if (Event.current.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
                    {
                        editor.SelectCurrentWord();
                        editor.DblClickSnap(TextEditor_Reimpl.DblClickSnapping.WORDS);
                        editor.MouseDragSelectsWholeWords(true);
                    }

                    if (Event.current.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
                    {
                        editor.SelectCurrentParagraph();
                        editor.MouseDragSelectsWholeWords(true);
                        editor.DblClickSnap(TextEditor_Reimpl.DblClickSnapping.PARAGRAPHS);
                    }

                    current.Use();
                }

                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id)
                {
                    editor.MouseDragSelectsWholeWords(false);
                    GUIUtility.hotControl = 0;
                    current.Use();
                }

                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == id)
                {
                    if (current.shift)
                        editor.MoveCursorToPosition(Event.current.mousePosition);
                    else
                        editor.SelectToPosition(Event.current.mousePosition);
                    current.Use();
                }

                break;
            case EventType.KeyDown:
                if (GUIUtility.keyboardControl != id) return;
                if (editor.HandleKeyEvent(current))
                {
                    current.Use();
                    flag = true;
                    content.text = editor.text;
                    break;
                }

                if (current.keyCode == KeyCode.Tab || current.character == '\t') return;
                char character = current.character;
                if (character == '\n' && !multiline && !current.alt) return;
                Font font = style.font;
                if (!(bool)(Object)font) font = GUI.skin.font;
                if (font.HasCharacter(character) || character == '\n')
                {
                    editor.Insert(character);
                    flag = true;
                    break;
                }

                if (character == char.MinValue)
                {
                    if (GUIUtility.compositionString.Length > 0)
                    {
                        editor.ReplaceSelection("");
                        flag = true;
                    }

                    current.Use();
                }

                break;
            case EventType.Repaint:
                if (GUIUtility.keyboardControl != id)
                {
                    style.Draw(position, content, id, false);
                    break;
                }

                editor.DrawCursor(content.text);
                break;
        }

        if (GUIUtility.keyboardControl == id) GUIUtility.textFieldInput = true;
        if (!flag) return;
        GUI.changed = true;
        content.text = editor.text;
        if (maxLength >= 0 && content.text.Length > maxLength) content.text = content.text.Substring(0, maxLength);
        current.Use();
    }

    [HarmonyPatch(typeof(GUI), nameof(GUI.DoTextField), typeof(Rect), typeof(int), typeof(GUIContent), typeof(bool), typeof(int), typeof(GUIStyle), typeof(string), typeof(char))]
    [HarmonyPrefix]
    public static bool GUIDoTextFieldPatch(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar)
    {
        GUIUtility.CheckOnGUI();
        if (maxLength >= 0 && content.text.Length > maxLength) content.text = content.text.Substring(0, maxLength);
        TextEditor_Reimpl editor = GetStateObject(id);
        editor.text = content.text;
        editor.SaveBackup();
        editor.position = position;
        editor.style = style;
        editor.multiline = multiline;
        editor.controlID = id;

        editor.DetectFocusChange();
        HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, editor);
        editor.UpdateScrollOffsetIfNeeded(Event.current);
        return false;
    }
}
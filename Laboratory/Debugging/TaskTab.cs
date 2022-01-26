using Il2CppSystem.Text;
using UnityEngine;

namespace Laboratory.Debugging;

public class TaskTab : BaseDebugTab
{
    public override string Name => "Tasks";

    public override void BuildUI()
    {
        if (!PlayerControl.LocalPlayer) return;

        foreach (var dataTask in PlayerControl.LocalPlayer.myTasks)
        {
            if (!dataTask.IsComplete && dataTask.TryCast<NormalPlayerTask>() is { } task)
            {
                var sb = new StringBuilder();
                dataTask.AppendTaskText(sb);
                if (GUILayout.Button($"{sb.ToString().Replace('\n', ' ')}"))
                {
                    task.NextStep();
                }
            }
        }
    }
}
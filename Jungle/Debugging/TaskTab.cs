using Il2CppSystem.Text;
using UnityEngine;

namespace Jungle.Debugging;

public class TaskTab : BaseDebugTab
{
    public override string Name => "Tasks";

    public override void BuildUI()
    {
        if (!PlayerControl.LocalPlayer) return;

        foreach (PlayerTask dataTask in PlayerControl.LocalPlayer.myTasks)
        {
            if (!dataTask.IsComplete && dataTask.TryCast<NormalPlayerTask>() is { } task)
            {
                StringBuilder sb = new();
                dataTask.AppendTaskText(sb);
                if (GUILayout.Button($"{sb.ToString().Replace('\n', ' ')}"))
                {
                    task.NextStep();
                }
            }
        }
    }
}
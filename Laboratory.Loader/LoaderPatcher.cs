using System;
using BepInEx.Preloader.Core.Patching;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;

namespace Laboratory.Loader;

[PatcherPluginInfo("Laboratory.Loader", "Laboratory.Loader", "1.0.0")]
public class LoaderPatcher : BasePatcher
{
    public override void Initialize()
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(Array.Empty<string>(), ShutdownMode.OnExplicitShutdown);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}

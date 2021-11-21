using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Laboratory.Loader.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private bool _starting;

    private void OnClosed(object sender, EventArgs e)
    {
        if (_starting)
            return;

        Environment.Exit(0);
    }

    private void StartGame(object sender, RoutedEventArgs e)
    {
        _starting = true;
        ((ClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Shutdown();
    }
}

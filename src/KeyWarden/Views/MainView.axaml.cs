using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using KeyWarden.ViewModels;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden.Views;

public partial class MainView : UserControl
{
	public MainView()
	{
		InitializeComponent();

		//AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
		//TitlebarRight.AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
		TitlebarLeft.PointerPressed += OnPointerPressed;
		TitlebarRight.PointerPressed += OnPointerPressed;
		TitlebarLeft.DoubleTapped += OnDoubleClicked;
		TitlebarRight.DoubleTapped += OnDoubleClicked;
		SyncButton.Click += SyncButton_Click;
		AboutButton.Click += AboutButton_Click;

		Loaded += (e, s) =>
		{
			if (DataContext is null)
				return;
			if (DataContext is not MainViewModel model)
				throw new Exception("DataContext has an unexpected type!");
			model.HandleAuthPrompt = OnHandleAuthPrompt;
		};
	}

	private async Task<bool> OnHandleAuthPrompt(SshKey key, ClientInfo info, CancellationToken ct)
	{
		var window = new AuthPrompt(key, info, ct);
		window.Show();

		return await window.Result;
	}

	private void AboutButton_Click(object? sender, RoutedEventArgs e)
	{
		if (VisualRoot is not Window window)
			return;

		new AboutWindow().ShowDialog(window);
	}

	private void SyncButton_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not MainViewModel model)
			throw new Exception("DataContext has an unexpected type!");

		model.SyncKeys();
	}

	private void OnDoubleClicked(object? sender, TappedEventArgs e)
	{
		if (VisualRoot is not Window window)
			return;

		if (window.WindowState == WindowState.Maximized)
			window.WindowState = WindowState.Normal;
		else if (window.WindowState == WindowState.Normal)
			window.WindowState = WindowState.Maximized;
	}

	private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (VisualRoot is not Window window)
			return;

		window.BeginMoveDrag(e);
	}
}

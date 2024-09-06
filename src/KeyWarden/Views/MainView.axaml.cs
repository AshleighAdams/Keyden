using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;

using KeyWarden.ViewModels;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden.Views;

public partial class MainView : UserControl
{
	public MainView()
	{
		DataContext = App.GetService<MainViewModel>();

		InitializeComponent();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			ThreeDots.IsVisible = false;
			TitleText.IsVisible = false;
		}

		SyncButton.Click += SyncButton_Click;
		AboutButton.Click += AboutButton_Click;
		KeysListBox.SelectionChanged += KeysListBox_SelectionChanged;
		KeysListBox.GotFocus += KeysListBox_GotFocus;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);

		if (e.Root is not Window window)
			return;

		window.PropertyChanged += Window_PropertyChanged;
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (e.Root is not Window window)
			return;

		TitleText.IsVisible = false;
		window.PropertyChanged -= Window_PropertyChanged;
	}

	private void Window_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == Window.IsExtendedIntoWindowDecorationsProperty)
		{
			if (e.NewValue is not bool newValue)
				return;

			if (newValue)
			{
				TitlebarLeft.PointerPressed += OnPointerPressed;
				TitlebarRight.PointerPressed += OnPointerPressed;
				TitlebarLeft.DoubleTapped += OnDoubleClicked;
				TitlebarRight.DoubleTapped += OnDoubleClicked;

				TitleText.IsVisible = true;
			}
			else
			{
				TitlebarLeft.PointerPressed -= OnPointerPressed;
				TitlebarRight.PointerPressed -= OnPointerPressed;
				TitlebarLeft.DoubleTapped -= OnDoubleClicked;
				TitlebarRight.DoubleTapped -= OnDoubleClicked;

				TitleText.IsVisible = false;
			}
		}
	}

	private void KeysListBox_GotFocus(object? sender, GotFocusEventArgs e)
	{
		if (DataContext is not MainViewModel model)
			return;
		model.TabIndexSelected = 0;
	}

	private void KeysListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (KeyOptions.DataContext is not KeyOptionsViewModel vm)
			throw new InvalidCastException($"MainView: KeyOptions view model is not a KeyOptionsViewModel");
		
		vm.Key = KeysListBox.SelectedItem as ObservableSshKey;
	}

	internal void AboutButton_Click(object? sender, RoutedEventArgs e)
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

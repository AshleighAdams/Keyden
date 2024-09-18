using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

using Keyden.ViewModels;

using System.ComponentModel;

namespace Keyden.Views;

public partial class KeyOptionsView : UserControl
{
	private readonly KeyOptionsViewModel ViewModel;
	public KeyOptionsView()
	{
		DataContext = ViewModel = App.GetService<KeyOptionsViewModel>();
		InitializeComponent();

		EditButton.Click += EditButton_Click;
	}

	private void EditButton_Click(object? sender, RoutedEventArgs e)
	{
		ViewModel.Unlock(VisualRoot as Window);
	}
}

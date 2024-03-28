using Avalonia;
using Avalonia.Controls;

using KeyWarden.ViewModels;

namespace KeyWarden.Views;

public partial class KeyOptionsView : UserControl
{
	public KeyOptionsView()
	{
		DataContext = App.GetService<KeyOptionsViewModel>();
		InitializeComponent();
	}
}

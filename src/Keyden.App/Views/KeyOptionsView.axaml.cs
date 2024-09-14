using Avalonia;
using Avalonia.Controls;

using Keyden.ViewModels;

namespace Keyden.Views;

public partial class KeyOptionsView : UserControl
{
	public KeyOptionsView()
	{
		DataContext = App.GetService<KeyOptionsViewModel>();
		InitializeComponent();
	}
}

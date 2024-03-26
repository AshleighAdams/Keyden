using Avalonia.Controls;

using KeyWarden.ViewModels;

namespace KeyWarden.Views
{
	public partial class ActivityView : UserControl
	{
		public ActivityView()
		{
			DataContext = App.GetService<ActivityViewModel>();
			InitializeComponent();
		}
	}
}

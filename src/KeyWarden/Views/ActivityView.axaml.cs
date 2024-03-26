using System;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;

using KeyWarden.ViewModels;

namespace KeyWarden.Views;

public partial class ActivityView : UserControl
{
	private bool SuppressScrollChanged { get; set; }
	private bool AtBottom { get; set; } = true;

	public ActivityView()
	{
		DataContext = App.GetService<ActivityViewModel>();
		InitializeComponent();

		ActivitiesListBox.TemplateApplied += ActivitiesListBox_TemplateApplied;
	}

	private void ActivitiesListBox_TemplateApplied(object? sender, TemplateAppliedEventArgs e)
	{
		var sv = e.NameScope.Get<ScrollViewer>("PART_ScrollViewer");
		sv.ScrollChanged += ScrollViewer_ScrollChanged;
	}

	private void ScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
	{
		if (SuppressScrollChanged)
			return;
		if (e.Source is not ScrollViewer sv)
			return;

		if (Math.Abs(e.ExtentDelta.Y) > double.Epsilon && AtBottom)
		{
			SuppressScrollChanged = true;
			ActivitiesListBox.ScrollIntoView(ActivitiesListBox.Items.Count - 1);
			SuppressScrollChanged = false;
			return;
		}

		var bottom = sv.Viewport.Height + sv.Offset.Y;
		var bottomGap = sv.Extent.Height - bottom;

		AtBottom = Math.Abs(bottomGap) < double.Epsilon;
	}
}

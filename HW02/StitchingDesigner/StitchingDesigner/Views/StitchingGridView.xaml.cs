using StitchingDesigner.ViewModels;

namespace StitchingDesigner.Views;

public partial class StitchingGridView : ContentPage
{
	public StitchingGridView()
	{
		InitializeComponent();
	}

	private void OnGridViewportSizeChanged(object? sender, EventArgs e)
	{
		if (BindingContext is GridViewModel vm)
		{
			vm.UpdateAvailableSpace(GridViewport.Width, GridViewport.Height);
		}
	}
}
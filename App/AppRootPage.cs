namespace App;

public class AppRootPage : ContentPage
{
    private object? _previousBindingContext;

    public AppRootPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
        SetDynamicResource(BackgroundColorProperty, "BkgAbyss");
    }

    public void SetCurrentView(View view, object? bindingContext, string? title)
    {
        // Dispose the previous binding context if it implements IDisposable
        if (_previousBindingContext is IDisposable disposableVM)
        {
            try
            {
                disposableVM.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error disposing previous view model: {ex.Message}");
            }
        }

        BindingContext = bindingContext;
        Title = title ?? string.Empty;
        Content = view;
        _previousBindingContext = bindingContext;
    }
}

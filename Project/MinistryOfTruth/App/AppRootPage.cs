namespace App;

public class AppRootPage : ContentPage
{
    public AppRootPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
        SetDynamicResource(BackgroundColorProperty, "BkgAbyss");
    }

    public void SetCurrentView(View view, object? bindingContext, string? title)
    {
        BindingContext = bindingContext;
        Title = title ?? string.Empty;
        Content = view;
    }
}

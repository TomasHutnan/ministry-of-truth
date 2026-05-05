using CommunityToolkit.Maui.Views;

namespace App.Views.Popups;

public partial class NoticePopup : Popup<bool>
{
    public NoticePopup(string message, bool isError = false)
    {
        InitializeComponent();
        MessageLabel.Text = message;
        if (isError && Application.Current != null &&
            Application.Current.Resources.TryGetValue("ViolationRed", out var colorvalue))
        {
            BackgroundColor = (Color)colorvalue;
        }
    }

    private async void OnOkClicked(object sender, EventArgs e) => await CloseAsync(true);
}
using CommunityToolkit.Maui.Views;

namespace App.Views.Popups;

public partial class ConfirmPopup : Popup<bool>
{
    public ConfirmPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    private async void OnYesClicked(object sender, EventArgs e) => await CloseAsync(true);
    private async void OnNoClicked(object sender, EventArgs e) => await CloseAsync(false);
}
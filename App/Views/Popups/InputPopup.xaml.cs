using CommunityToolkit.Maui.Views;

namespace App.Views.Popups;

public partial class InputPopup : Popup<string?>
{
    public InputPopup(string prompt)
    {
        InitializeComponent();
        PromptLabel.Text = prompt;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
        => await CloseAsync(InputEntry.Text);

    private async void OnCancelClicked(object sender, EventArgs e)
        => await CloseAsync(null);
}

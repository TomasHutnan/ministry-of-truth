using App.Views.Popups;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using MinistryOfTruth.ViewModels.Interfaces;

namespace App.Services;

public class PopupService : IPopupService
{
    public async Task<bool> ShowConfirmationAsync(string message)
    {
        var popup = new ConfirmPopup(message);
        var page = GetCurrentPage();
        IPopupResult result = await page.ShowPopupAsync(popup);

        if (result.WasDismissedByTappingOutsideOfPopup)
            return false;

        if (result is IPopupResult<bool> typedResult)
            return typedResult.Result;

        return false;
    }

    public async Task<string?> ShowInputAsync(string prompt)
    {
        var popup = new InputPopup(prompt);
        var page = GetCurrentPage();
        IPopupResult result = await page.ShowPopupAsync(popup);

        if (result.WasDismissedByTappingOutsideOfPopup)
            return null;

        if (result is IPopupResult<string> typedResult)
            return typedResult.Result;

        return null;
    }

    public async Task<bool> ShowNoticeAsync(string message)
    {
        var popup = new NoticePopup(message, false);
        return await ShowBoolPopupAsync(popup);
    }

    public async Task<bool> ShowErrorAsync(string message)
    {
        var popup = new NoticePopup(message, true);
        return await ShowBoolPopupAsync(popup);
    }

    private async Task<bool> ShowBoolPopupAsync(Popup<bool> popup)
    {
        var page = GetCurrentPage();
        IPopupResult result = await page.ShowPopupAsync(popup);

        if (result.WasDismissedByTappingOutsideOfPopup)
            return false;

        if (result is IPopupResult<bool> typedResult)
            return typedResult.Result;

        return false;
    }

    private static Page GetCurrentPage() =>
        Application.Current?.Windows[0]?.Page
        ?? throw new InvalidOperationException("No current page");
}
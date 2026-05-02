using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using App.Views.Popups;
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

    private static Page GetCurrentPage() =>
        Application.Current?.Windows[0]?.Page
        ?? throw new InvalidOperationException("No current page");
}
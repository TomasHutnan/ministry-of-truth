namespace MinistryOfTruth.ViewModels.Interfaces;

public interface IPopupService
{
    Task<bool> ShowConfirmationAsync(string message);
    Task<string?> ShowInputAsync(string prompt);
}

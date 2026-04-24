using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace MinistryOfTruth.ViewModels;

public partial class StartViewModel(INavigationService navigationService, IGameEngine gameEngine) : ViewModelBase(navigationService, gameEngine)
{
    [RelayCommand]
    public async Task Start()
    {
        await _navigationService.GoToMenuAsync();
    }
}

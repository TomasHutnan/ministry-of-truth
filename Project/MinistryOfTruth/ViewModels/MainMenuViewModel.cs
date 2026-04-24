using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public class MainMenuViewModel : ViewModelBase
{
    public MainMenuViewModel(INavigationService navigationService, IGameEngine gameEngine) : base(navigationService, gameEngine) { }
}

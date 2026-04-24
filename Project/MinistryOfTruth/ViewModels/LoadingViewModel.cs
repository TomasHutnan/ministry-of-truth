using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public class LoadingViewModel : ViewModelBase
{
    public LoadingViewModel(INavigationService navigationService, IGameEngine gameEngine) : base(navigationService, gameEngine) { }
}

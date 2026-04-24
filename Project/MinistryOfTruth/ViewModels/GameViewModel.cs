using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public class GameViewModel : ViewModelBase
{
    public GameViewModel(INavigationService navigationService, IGameEngine gameEngine) : base(navigationService, gameEngine) { }
}

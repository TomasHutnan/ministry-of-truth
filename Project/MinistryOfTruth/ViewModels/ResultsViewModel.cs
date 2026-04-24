using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    public ResultsViewModel(INavigationService navigationService, IGameEngine gameEngine) : base(navigationService, gameEngine) { }
}

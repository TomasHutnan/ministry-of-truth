using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public class StartViewModel : ViewModelBase
{
    public StartViewModel(INavigationService navigationService, IGameEngine gameEngine) : base(navigationService, gameEngine) { }
}

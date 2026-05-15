using CommunityToolkit.Mvvm.ComponentModel;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Interfaces;

namespace MinistryOfTruth.ViewModels.Base;

public abstract partial class ViewModelBase(INavigationService navigationService, IGameEngine gameEngine) : ObservableObject
{
    protected readonly INavigationService _navigationService = navigationService;
    protected readonly IGameEngine _gameEngine = gameEngine;
}
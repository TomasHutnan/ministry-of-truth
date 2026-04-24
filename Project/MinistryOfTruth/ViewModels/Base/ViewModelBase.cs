using MinistryOfTruth.Domain.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinistryOfTruth.ViewModels.Base;

public abstract class ViewModelBase(INavigationService navigationService, IGameEngine gameEngine) : INotifyPropertyChanged
{
    protected readonly INavigationService _navigationService = navigationService;
    protected readonly IGameEngine _gameEngine = gameEngine;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(
        ref T backingField,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
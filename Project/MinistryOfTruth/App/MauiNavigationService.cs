using MinistryOfTruth.Domain.Interfaces;

namespace App
{
    public class MauiNavigationService : INavigationService
    {
        public async Task GoToStartAsync() => await Shell.Current.GoToAsync("//StartPage");

        public async Task GoToMenuAsync() => await Shell.Current.GoToAsync("//MainMenuPage");

        public async Task GoToGameAsync() => await Shell.Current.GoToAsync("//GamePage");

        public async Task GoToResultsAsync() => await Shell.Current.GoToAsync("//ResultsPage");
    }
}

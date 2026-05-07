using App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.ViewModels;

namespace App
{
    public class MauiNavigationService : INavigationService
    {
        private const string StartRoute = "StartPage";
        private const string MainMenuRoute = "MainMenuPage";
        private const string GameRoute = "GamePage";
        private const string ResultsRoute = "ResultsPage";

        private readonly IServiceProvider _serviceProvider;

        public MauiNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task GoToStartAsync() => NavigateToRouteAsync(StartRoute);

        public Task GoToMenuAsync() => NavigateToRouteAsync(MainMenuRoute);

        public Task GoToGameAsync() => NavigateToRouteAsync(GameRoute);

        public Task GoToResultsAsync(GameState gameState, ScoreResult scoreResult) => NavigateToResultsAsync(gameState, scoreResult);

        private async Task NavigateToRouteAsync(string route)
        {
            var rootPage = _serviceProvider.GetRequiredService<AppRootPage>();

            ContentView view = route switch
            {
                StartRoute => _serviceProvider.GetRequiredService<StartView>(),
                MainMenuRoute => _serviceProvider.GetRequiredService<MainMenuView>(),
                GameRoute => _serviceProvider.GetRequiredService<GameView>(),
                _ => throw new InvalidOperationException($"Unknown route '{route}'.")
            };

            await MainThread.InvokeOnMainThreadAsync(() => Show(rootPage, view));
        }

        private async Task NavigateToResultsAsync(GameState gameState, ScoreResult scoreResult)
        {
            var rootPage = _serviceProvider.GetRequiredService<AppRootPage>();
            var resultsViewModel = ActivatorUtilities.CreateInstance<ResultsViewModel>(_serviceProvider, gameState, scoreResult);
            var view = new ResultsView(resultsViewModel);

            await MainThread.InvokeOnMainThreadAsync(() => Show(rootPage, view));
        }

        private static void Show(AppRootPage rootPage, ContentView view)
        {
            rootPage.SetCurrentView(view, view.BindingContext, rootPage.Title);
        }
    }
}

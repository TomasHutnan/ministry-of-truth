using App.Views;
using MinistryOfTruth.Domain.Interfaces;

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

        public Task GoToResultsAsync() => NavigateToRouteAsync(ResultsRoute);

        private Task NavigateToRouteAsync(string route)
        {
            var rootPage = _serviceProvider.GetRequiredService<AppRootPage>();

            switch (route)
            {
                case StartRoute:
                    Show(rootPage, _serviceProvider.GetRequiredService<StartView>());
                    break;
                case MainMenuRoute:
                    Show(rootPage, _serviceProvider.GetRequiredService<MainMenuView>());
                    break;
                case GameRoute:
                    Show(rootPage, _serviceProvider.GetRequiredService<GameView>());
                    break;
                case ResultsRoute:
                    Show(rootPage, _serviceProvider.GetRequiredService<ResultsView>());
                    break;
                default:
                    throw new InvalidOperationException($"Unknown route '{route}'.");
            }

            return Task.CompletedTask;
        }

        private static void Show(AppRootPage rootPage, ContentView view)
        {
            rootPage.SetCurrentView(view, view.BindingContext, rootPage.Title);
        }
    }
}

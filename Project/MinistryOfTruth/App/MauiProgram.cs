using Microsoft.Extensions.Logging;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels;

namespace App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<IGameEngine, GameEngine>();

            builder.Services.AddTransient<StartViewModel>();
            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<MainMenuViewModel>();
            builder.Services.AddTransient<GameViewModel>();
            builder.Services.AddTransient<ResultsViewModel>();

            return builder.Build();
        }
    }
}

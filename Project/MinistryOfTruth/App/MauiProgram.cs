using App.Views;
using Microsoft.Extensions.Logging;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Data.Files;
using MinistryOfTruth.ViewModels;
using MinistryOfTruth.ViewModels.Interfaces;
using MinistryOfTruth.Data.Csv;

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
                    fonts.AddFont("Anton-Regular.ttf", "Titles");
                    fonts.AddFont("JetBrainsMono-VariableFont_wght.ttf", "Labels");
                    fonts.AddFont("SpecialElite-Regular.ttf", "Typewriter");
                });

#if DEBUG
			builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IHighScoreStore, FileHighScoreStore>();
            builder.Services.AddSingleton<IRuleRepository, CsvRuleRepository>();
            builder.Services.AddSingleton<ITextRepository, CsvTextRepository>();
            builder.Services.AddSingleton<IViolationRepository, CsvViolationRepository>();

            builder.Services.AddSingleton<ComplexityCalculator>();

            builder.Services.AddSingleton<IGameEngine, GameEngine>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<IMenuTextSource, MenuTextSource>();

            builder.Services.AddSingleton<AppRootPage>();

            builder.Services.AddTransient<StartView>();
            builder.Services.AddTransient<StartViewModel>();

            builder.Services.AddTransient<LoadingView>();
            builder.Services.AddTransient<LoadingViewModel>();

            builder.Services.AddTransient<MainMenuView>();
            builder.Services.AddTransient<MainMenuViewModel>();

            builder.Services.AddTransient<GameView>();
            builder.Services.AddTransient<GameViewModel>();

            builder.Services.AddTransient<ResultsView>();
            builder.Services.AddTransient<ResultsViewModel>();

            return builder.Build();
        }
    }
}

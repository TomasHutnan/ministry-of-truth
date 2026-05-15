using App.Views;
using Microsoft.Extensions.Logging;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Data.Files;
using MinistryOfTruth.Data.Services;
using MinistryOfTruth.ViewModels;
using MinistryOfTruth.Data.Csv;
using MinistryOfTruth.Domain.Presentation;
using CommunityToolkit.Maui;
using App.Services;
using MinistryOfTruth.ViewModels.Interfaces;

namespace App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
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
            builder.Services.AddSingleton<IDocumentGenerator, DocumentGenerator>();
            builder.Services.AddSingleton<IAppAssetProvider, MauiAppAssetProvider>();
            builder.Services.AddSingleton<ITextSetLoader, TextSetService>();

            builder.Services.AddSingleton<ComplexityCalculator>();
            builder.Services.AddSingleton<RuleTextFormatter>();

            builder.Services.AddSingleton<IGameEngine, GameEngine>();
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<MinistryOfTruth.ViewModels.Interfaces.IPopupService, PopupService>();
            builder.Services.AddSingleton<ITickerTextSource, MenuTickerTextSource>();
            builder.Services.AddSingleton<ITextTicker, TextTicker>();

            builder.Services.AddSingleton<AppRootPage>();

            builder.Services.AddTransient<StartView>();
            builder.Services.AddTransient<StartViewModel>();

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

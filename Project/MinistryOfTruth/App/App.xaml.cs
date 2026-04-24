using Microsoft.Extensions.DependencyInjection;
using MinistryOfTruth.Domain.Interfaces;

namespace App
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var rootPage = _serviceProvider.GetRequiredService<AppRootPage>();
            _ = _serviceProvider.GetRequiredService<INavigationService>().GoToStartAsync();
            return new Window(rootPage)
            {
                Title = "Ministry of Truth"
            };
        }
    }
}
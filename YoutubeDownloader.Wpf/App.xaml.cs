using Autofac;
using System.Reflection;
using System.Windows;
using YoutubeDownloader.Service;
using YoutubeDownloader.Wpf.Views.Windows;

namespace YoutubeDownloader.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            IContainer container = SetupDependencyInjection();

            using (var scope = container.BeginLifetimeScope())
            {
                var environmentService = scope.Resolve<ApplicationService>();
                environmentService.DownloadYoutubeDlIfNotExists();
            }

            using (var scope = container.BeginLifetimeScope())
            {
                var mainWindow = scope.Resolve<MainWindow>();
                MainWindow.Show();
            }
        }

        private static IContainer SetupDependencyInjection()
        {
            var builder = new ContainerBuilder();

            var wpfAssembly = Assembly.GetExecutingAssembly();
            var serviceAssembly = typeof(YoutubeDownloaderService).Assembly;
            builder.RegisterAssemblyTypes(wpfAssembly);
            builder.RegisterAssemblyTypes(serviceAssembly);

            var container = builder.Build();
            return container;
        }
    }
}

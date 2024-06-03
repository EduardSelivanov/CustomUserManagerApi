using AndroidClient.Services;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AndroidClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var getAssembly = Assembly.GetExecutingAssembly();
            using var stream = getAssembly.GetManifestResourceStream("AndroidClient.AppSet.json");
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            

            var mainPage = new MainPage(config);
            MainPage = new NavigationPage(mainPage);
        }
    }
}

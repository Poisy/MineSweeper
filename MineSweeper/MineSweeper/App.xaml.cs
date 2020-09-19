using MineSweeper.Models;
using Xamarin.Forms;

namespace MineSweeper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RecordsDatabase.GetInstance();

            NavigationPage navigation = new NavigationPage();
            navigation.PushAsync(new MainPage(), true);
            NavigationPage.SetHasNavigationBar(navigation, false);
            MainPage = navigation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Settings.GetSettings().Save();
        }

        protected override void OnResume()
        {
        }
    }
}

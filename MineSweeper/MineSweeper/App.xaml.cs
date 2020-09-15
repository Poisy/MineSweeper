using Xamarin.Forms;

namespace MineSweeper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
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

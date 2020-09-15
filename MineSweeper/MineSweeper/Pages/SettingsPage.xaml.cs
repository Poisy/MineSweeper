using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MineSweeper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : TabbedPage
    {
        public static bool DoMainPageNeedRestart { get; set; }

        public SettingsPage(MainPage page)
        {
            DoMainPageNeedRestart = false;

            Title = "Settings";
            Children.Add(CreateSettingsTab(new GamePlaySettings(), "Gameplay"));
            Children.Add(CreateSettingsTab(new ThemeSettings(), "Theme"));

            Disappearing += (sender, e) =>
            {
                if (DoMainPageNeedRestart) page.Restart();
            };
        }

        NavigationPage CreateSettingsTab(SettingsPageTemplate settings, string name)
        {
            NavigationPage tab = new NavigationPage(settings)
            {
                Title = name
            };

            return tab;
        }
    }
}
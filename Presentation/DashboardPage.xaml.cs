using MyPersonalGuardian.Presentation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WiFiConnect
{
 
    public sealed partial class DashboardPage : Page
    {

        private User _user;
        public DashboardPage()
        {
            this.InitializeComponent();
            _user = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _user = e.Parameter as User;
            _frmContent.Navigate(typeof(AlertsPage), _user);
        }
            private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {

            if (e.SourcePageType == typeof(AlertsPage))
            {

                _txtPageTitle.Text = "Alerts";
                _lstAppNavigation.SelectedItem = _uiNavAlerts;
                _navSplitView.IsPaneOpen = false;
                _navSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }


            else if (e.SourcePageType == typeof(TagSetupPage))
            {

                _txtPageTitle.Text = "Tag Setup";
                _lstAppNavigation.SelectedItem = _uiNavTagSetup;
                _navSplitView.IsPaneOpen = false;
                _navSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
        }

        private void OnNavigationItemClicked(object sender, ItemClickEventArgs e)
        {

            NavMenuItem navMenuItem = e.ClickedItem as NavMenuItem;

            if (navMenuItem == _uiNavAlerts)
            {
                _frmContent.Navigate(typeof(AlertsPage), _user);
            }
            else if (navMenuItem == _uiNavTagSetup)
            {
                _frmContent.Navigate(typeof(TagSetupPage));
            }
        }
    }
}

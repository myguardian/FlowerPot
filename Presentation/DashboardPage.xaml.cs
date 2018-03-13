using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WiFiConnect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TODO: Maybe send just first name as a parameter (unless other properties of the user are used)
            User user = e.Parameter as User;

            string day = DateTime.Now.DayOfWeek.ToString();
            string firstName = user.FirstName;

            txtWelcome.Text = String.Format("Happy {0} {1}", day, firstName);
        }

        private void OnTagSetupClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TagSetupPage));
        }
    }
}

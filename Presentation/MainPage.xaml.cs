// Copyright (c) Microsoft. All rights reserved.

using MyPersonalGuardian.Presentation;
using System;
using System.Collections.Generic;
using WiFiConnect;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            //TODO: Determine if user has setup their device before, go right to wifi connection if they have (and then skip
            //the user information form. Give them an option to go to the main screen and setup as a new device if they wish - 
            //maybe do that in the dashboard page (a log out or reset feature?)

        }

        private void OnSetupClick(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(UserInfo)); //use for testing
            this.Frame.Navigate(typeof(AlertsPage));
        }
    }
}

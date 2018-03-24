using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WiFiConnect.BusinessLogic;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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

        private User _user;
        private Alert[] _alerts;
        private List<Alert> _snoozedAlerts;

        public DashboardPage()
        {
            this.InitializeComponent();

            //initialized when the page is navigated to - may be trouble when incorporating other pages - move this to navigated to?
            _user = null;
            _alerts = null;
            _snoozedAlerts = new List<Alert>();

            lstAlerts.SelectedIndex = 0;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TODO: Maybe send just first name as a parameter (unless other properties of the user are used)
            _user = e.Parameter as User;

            LoadJson();
            DisplayAlerts();

            string day = DateTime.Now.DayOfWeek.ToString();
            string firstName = _user.FirstName;

            txtWelcome.Text = String.Format("Happy {0} {1}", day, firstName);            
        }

        private void DisplayAlerts()
        {
            bool highPriority = false;

            //orders the alerts by the alert level
            var query = _alerts.OrderByDescending(alert => alert.AlertLevel).ToArray();
            _alerts = query;

            lstAlerts.Items.Clear();
            foreach (Alert alert in _alerts)
            {
                lstAlerts.Items.Add(alert);
                if (alert.AlertLevel == 3)
                {
                    highPriority = true;
                }
            }

            if (highPriority == true)
            {
                ellRed.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }


        }

        private void LoadJson()
        {
            //TODO: Temporary web server
            string url = String.Format("https://hanifso.dev.fast.sheridanc.on.ca/Pi/getAlerts.php?flowerpotID={0}", _user.FlowerPotID);
            string alertString;

            using (var httpClient = new HttpClient())
            {

                using (HttpResponseMessage response = httpClient.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        alertString = content.ReadAsStringAsync().Result;
                    }
                }

            }

            JsonObject jsonObj = JsonObject.Parse(alertString);
            //stores all of the alerts into one array
            JsonArray alertArray = jsonObj.GetNamedArray("Alerts");

            _alerts = new Alert[alertArray.Count];

            //loops through all of the alerts in the JSON object
            for (uint iAlert = 0; iAlert < alertArray.Count; iAlert++)
            {
                //create a new JSON Object for each object in the JSON array
                JsonObject alertObject = alertArray.GetObjectAt(iAlert);

                //extract information from each alert
                int alertID = Int32.Parse(alertObject.GetNamedString("Alert ID"));
                string flowerpotID = alertObject.GetNamedString("Flowerpot ID");
                DateTime alertDateTime = DateTime.Parse(alertObject.GetNamedString("Alert Timestamp"));
                string shortDescription = alertObject.GetNamedString("Short Description");
                string longDescription = alertObject.GetNamedString("Long Description");
                //NOTE: Don;t need to get Acknowledged TimeStamp (only useful for database purposes
                int alertLevel = Int32.Parse(alertObject.GetNamedString("Level"));
                //TODO: GET IMAGE - set to null right now
                //TODO: GET SOUND - set to null right now

                //add the alert into an array of alerts
                //TODO: Temporarily adding the alert date time in place of the acknowledge date time
                Alert alert = new Alert(alertID, flowerpotID, alertDateTime, shortDescription, longDescription, alertLevel);
                _alerts[iAlert] = alert;
            }
        }

        private void OnTagSetupClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TagSetupPage));
        }

        private void OnRedButtonClick(object sender, RoutedEventArgs e)
        {
            AcknowledgeAlert();
        }

        private async void AcknowledgeAlert()
        {
            //TODO: acknowledge the selected alert - ask for confirmation with dialogue and wav file?
            ContentDialog removeDialog = new ContentDialog()
            {
                Title = "Acknowledgement Confirmation",
                FontFamily = new Windows.UI.Xaml.Media.FontFamily("Agency FB"),

                MaxWidth = this.ActualWidth,
                PrimaryButtonText = "Acknowledge Alert",
                SecondaryButtonText = "Cancel",

                Content = new TextBlock
                {
                    Text = "Are you sure you would like to acknowledge this alert?",
                    FontSize = 18,
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("Agency FB"),
                }
            };

            ContentDialogResult result = await removeDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

                Alert selectedAlert = (Alert)lstAlerts.SelectedItem;
                string url = String.Format("https://hanifso.dev.fast.sheridanc.on.ca/Pi/acknowledgeAlert.php?alertID={0}", selectedAlert.AlertID);

                using (var httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = httpClient.GetAsync(url).Result)
                    {
                    }
                }

                LoadJson();
                DisplayAlerts();
            }
        }

        private void OnYellowButtonClick(object sender, RoutedEventArgs e)
        {
            SnoozeAlert();
        }

        private void SnoozeAlert()
        {
            Alert alert = (Alert)lstAlerts.SelectedItem;
            alert.AcknowledgeDateTime = DateTime.Now;

            _snoozedAlerts.Add(alert);
            lstAlerts.Items.RemoveAt(lstAlerts.SelectedIndex);
            DisplayAlerts();
        }
        private void OnGreenButonClick(object sender, RoutedEventArgs e)
        {
            //TODO: Play wav file
        }

        /// <summary>
        /// Event Handler for up button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (lstAlerts.SelectedIndex > 0)
            {
                lstAlerts.SelectedIndex--;
                lstAlerts.ScrollIntoView(lstAlerts.SelectedItem);
            }
        }

        /// <summary>
        /// Event Handler for down button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (lstAlerts.SelectedIndex < lstAlerts.Items.Count - 1)
            {
                lstAlerts.SelectedIndex++;
                lstAlerts.ScrollIntoView(lstAlerts.SelectedItem);
            }
        }
    }
}

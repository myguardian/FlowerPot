using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WiFiConnect.BusinessLogic;
using Windows.Data.Json;
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

        private User _user;
        private Alert[] _alerts;

        public DashboardPage()
        {
            this.InitializeComponent();

            //initialized when the page is navigated to
            _user = null;
            _alerts = null;

            DisplayAlerts();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TODO: Maybe send just first name as a parameter (unless other properties of the user are used)
            _user = e.Parameter as User;

            string day = DateTime.Now.DayOfWeek.ToString();
            string firstName = _user.FirstName;

            txtWelcome.Text = String.Format("Happy {0} {1}", day, firstName);
            btnAcknowledge.IsEnabled = false;
            btnRemove.IsEnabled = false;
        }

        private async void DisplayAlerts()
        {
            await LoadJson();

            lstAlerts.Items.Clear();
            foreach (Alert alert in _alerts)
            {
                lstAlerts.Items.Add(alert);
            }
        }

        public async Task LoadJson()
        {
            string url = "https://hanifso.dev.fast.sheridanc.on.ca/Pi/alerts.json";
            JsonObject jsonObj = new JsonObject();

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var stream =  await httpClient.GetStreamAsync(url);
                StreamReader reader = new StreamReader(stream);

                //stores the entire json file into one object
                jsonObj = JsonObject.Parse(reader.ReadLine());
            }

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
                //TODO: GET ACKNOWLEDGE DATE TIME - not a valid date right now ------------CHANGE ME BELOW-----

               // if (alertObject.GetNamedString("Acknowledged Timestamp") == null)
                //{

                //}
                //DateTime acknowledgeDateTime = DateTime.Parse(alertObject.GetNamedString("Acknowledged Timestamp"));
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

        private void OnAlertSelected(object sender, SelectionChangedEventArgs e)
        {
            btnAcknowledge.IsEnabled = true;
            btnRemove.IsEnabled = true;
        }

        private void OnAcknowledgeAlertClick(object sender, RoutedEventArgs e)
        {
            Alert alert = (Alert)lstAlerts.SelectedItem;
            alert.AcknowledgeDateTime = DateTime.Now;

            //----TESTING
            lstAlerts.Items.Clear();
            foreach (Alert a in _alerts)
            {
                lstAlerts.Items.Add(a);
            }
        }

        private void OnListViewLostFocus(object sender, RoutedEventArgs e)
        {
            //btnAcknowledge.IsEnabled = false;
            //btnRemove.IsEnabled = false;
        }

        private void OnRemoveAlertClick(object sender, RoutedEventArgs e)
        {
            lstAlerts.Items.Remove(lstAlerts.SelectedItem);

            btnAcknowledge.IsEnabled = false;
            btnRemove.IsEnabled = false;
        }
    }
}

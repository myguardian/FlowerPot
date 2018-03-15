using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

            LoadJson();
        }

        public async void LoadJson()
        {
            string url = "https://hanifso.dev.fast.sheridanc.on.ca/Pi/alerts.json";

            string jsonString;
            JsonObject jsonObj = new JsonObject();
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(url);
                StreamReader reader = new StreamReader(stream);

                //stores the entire json file into one object
                jsonObj = JsonObject.Parse(reader.ReadLine());
    
            }

            //stores all of the alerts into one array
            JsonArray alertArray = jsonObj.GetNamedArray("Alerts");

            //create a string array of the same size as the alert array -----------------------TEMP - maybe?
            string[] importantAlertInfo = new string[alertArray.Count];

            //loops through all of the alerts in the JSON object
            for (uint iAlert = 0; iAlert < alertArray.Count; iAlert++)
            {
                //create a new JSON Object for each object in the JSON array
                JsonObject alertObject = alertArray.GetObjectAt(iAlert);

                //extract certain information from each alert
                importantAlertInfo[iAlert] = String.Format("Alert ID: {0} Flowerpot ID: {1} Alert Level: {2}", alertObject.GetNamedString("Alert ID"),
                    alertObject.GetNamedString("Flowerpot ID"), alertObject.GetNamedString("Level"));
            }
            //gets the first alert

            //gets the first alert from the first alertObject
            txtNoTags.Text = "";
            for (int i = 0; i < importantAlertInfo.Count(); i++)
            {
                txtNoTags.Text += String.Format("-----{0}-----", i);
                txtNoTags.Text += importantAlertInfo[i];
            }
            
         }

        private void OnTagSetupClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TagSetupPage));
        }
    }
}

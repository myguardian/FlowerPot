﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WiFiConnect;
using WiFiConnect.BusinessLogic;
using Windows.Data.Json;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyPersonalGuardian.Presentation
{
    public sealed partial class AlertsPage : Page
    {

        //--------------------------
        //                await Task.Delay(5000);
        //--------------------------
        private const int GREEN_BUTTON_PIN = 5;
        private const int YELLOW_BUTTON_PIN = 6;
        private const int RED_BUTTON_PIN = 13;
        private const int GREEN_LED_PIN = 16;
        private const int YELOW_LED_PIN = 20;
        private const int RED_LED_PIN = 21;
        private const int UP_BUTTON_PIN = 27;
        private const int DOWN_BUTTON_PIN = 17;

        private User _user;
        private List<Alert> _alerts;
        private List<Alert> _snoozedAlerts;
        private MediaElement _sound;

        private GpioPin _greenButtonPin;
        private GpioPin _yellowButtonPin;
        private GpioPin _redButtonPin;
        private GpioPin _greenLedPin;
        private GpioPin _yellowLedPin;
        private GpioPin _redLedPin;
        private GpioPin _upButtonPin;
        private GpioPin _downButtonPin;
        private GpioPinValue _ledPinValue = GpioPinValue.High;

        public AlertsPage()
        {
            this.InitializeComponent();

            //TODO: initialized when the page is navigated to - may be trouble when incorporating other pages - move this to navigated to?
            _user = null;
            _alerts = null;
            _snoozedAlerts = new List<Alert>();
            _sound = new MediaElement();

            txtNoAlerts.Visibility = Visibility.Collapsed;
            lstAlerts.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TODO: Maybe send just first name as a parameter (unless other properties of the user are used)
            //_user = e.Parameter as User;

            LoadJson();
            DisplayAlerts();
            InitGPIO();

            lstAlerts.SelectedIndex = 0;

            string day = DateTime.Now.DayOfWeek.ToString();
            string firstName = "User";

            txtWelcome.Text = String.Format("Happy {0} {1}", day, firstName);
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // if there is no GPIO controller
            if (gpio == null)
            {
                return;
            }

            _greenButtonPin = gpio.OpenPin(GREEN_BUTTON_PIN);
            _yellowButtonPin = gpio.OpenPin(YELLOW_BUTTON_PIN);
            _redButtonPin = gpio.OpenPin(RED_BUTTON_PIN);

            _greenLedPin = gpio.OpenPin(GREEN_LED_PIN);
            _yellowLedPin = gpio.OpenPin(YELOW_LED_PIN);
            _redLedPin = gpio.OpenPin(RED_LED_PIN);

            _upButtonPin = gpio.OpenPin(UP_BUTTON_PIN);
            _downButtonPin = gpio.OpenPin(DOWN_BUTTON_PIN);

            // Initialize LED to the OFF state by first writing a HIGH value
            // We write HIGH because the LED is wired in a active LOW configuration
            _greenLedPin.Write(GpioPinValue.Low);
            _greenLedPin.SetDriveMode(GpioPinDriveMode.Output);
            _yellowLedPin.Write(GpioPinValue.Low);
            _yellowLedPin.SetDriveMode(GpioPinDriveMode.Output);
            _redLedPin.Write(GpioPinValue.Low);
            _redLedPin.SetDriveMode(GpioPinDriveMode.Output);

            // Check if input pull-up resistors are supported ------------------------------------REFACTOR ME LATER - check for uneccessary code
            if (_greenButtonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _greenButtonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _greenButtonPin.SetDriveMode(GpioPinDriveMode.Input);

            if (_yellowButtonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _yellowButtonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _yellowButtonPin.SetDriveMode(GpioPinDriveMode.Input);

            if (_redButtonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _redButtonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _redButtonPin.SetDriveMode(GpioPinDriveMode.Input);

            if (_upButtonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _upButtonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _upButtonPin.SetDriveMode(GpioPinDriveMode.Input);

            if (_downButtonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                _downButtonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                _downButtonPin.SetDriveMode(GpioPinDriveMode.Input);

            _greenButtonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _yellowButtonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _redButtonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _upButtonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _downButtonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            _greenButtonPin.ValueChanged += OnButtonPress;
            _yellowButtonPin.ValueChanged += OnButtonPress;
            _redButtonPin.ValueChanged += OnButtonPress;
            _upButtonPin.ValueChanged += OnButtonPress;
            _downButtonPin.ValueChanged += OnButtonPress;
        }

        private void OnButtonPress(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _ledPinValue = (_ledPinValue == GpioPinValue.Low) ? GpioPinValue.High : GpioPinValue.Low;

                    if (sender == _greenButtonPin)
                    {
                        txtWelcome.Text = "Green Pressed";
                        _greenLedPin.Write();
                        PlayAlert();
                    }
                    else if (sender == _yellowButtonPin)
                    {
                        txtWelcome.Text = "Yellow Pressed";
                        _yellowLedPin.Write(_ledPinValue);
                        SnoozeAlert();
                    }
                    else if (sender == _redButtonPin)
                    {
                        txtWelcome.Text = "Red Pressed";
                        _redLedPin.Write(_ledPinValue);
                        AcknowledgeAlert();
                    }
                    else if (sender == _upButtonPin)
                    {
                        GoUp();
                    }
                    else if (sender == _downButtonPin)
                    {
                        GoDown();
                    }
                }
            });
        }
        private void DisplayAlerts()
        {

            if (_alerts != null && _alerts.Count > 0)
            {
                lstAlerts.Visibility = Visibility.Visible;
                txtNoAlerts.Visibility = Visibility.Collapsed;

                bool highPriority = false;

                //orders the alerts by the alert level
                var query = _alerts.OrderByDescending(alert => alert.AlertLevel).ToList();
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
                    //ellRed.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                }
            } else
            {
                lstAlerts.Visibility = Visibility.Collapsed;
                txtNoAlerts.Visibility = Visibility.Visible;
            }
        }

        private void LoadJson()
        {
            //TODO: Temporary web server
            string url = String.Format("https://hanifso.dev.fast.sheridanc.on.ca/Pi/getAlerts.php?flowerpotID={0}", "0HLFXXL972UO"/*_user.FlowerPotID*/);
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
            try
            {
                JsonObject jsonObj = JsonObject.Parse(alertString);
                //stores all of the alerts into one array
                JsonArray alertArray = jsonObj.GetNamedArray("Alerts");

                _alerts = new List<Alert>();

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

                    DateTime acknowledgeDateTime;
                    if (alertObject.GetNamedString("Acknowledged Timestamp").ToString().Equals("0000-00-00 00:00:00"))
                    {
                        acknowledgeDateTime = DateTime.Parse("1001-01-01 00:00:00");
                    }
                    else
                    {
                        acknowledgeDateTime = DateTime.Parse(alertObject.GetNamedString("Acknowledged Timestamp"));
                    }

                    int alertLevel = Int32.Parse(alertObject.GetNamedString("Level"));
                    //TODO: GET IMAGE - set to null right now
                    //TODO: GET SOUND - set to null right now

                    //add the alert into an array of alerts
                    //TODO: Temporarily adding the alert date time in place of the acknowledge date time
                    Alert alert = new Alert(alertID, flowerpotID, alertDateTime, shortDescription, longDescription, acknowledgeDateTime, alertLevel);
                    _alerts.Add(alert);
                }
            } catch (Exception e)
            {
               // lstAlerts.Visibility = Visibility.Collapsed;
                
            }
        }

        private void OnTagSetupClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TagSetupPage));
        }

        private async void AcknowledgeAlert()
        {

            _sound = await PlaySound("DeleteConfirmSample");
            _sound.Play();

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

                //if it is the last alert, do not increment
                if (lstAlerts.Items.Count > 0)
                {
                        LoadJson();
                        DisplayAlerts();
                        lstAlerts.SelectedIndex++;
                }

            }

            _sound.Stop();
        }

        private async void SnoozeAlert()
        {
            Alert alert = (Alert)lstAlerts.SelectedItem;
            alert.AcknowledgeDateTime = DateTime.Now;

            _snoozedAlerts.Add(alert);
            _alerts.Remove(alert);

            //if it is the last alert, dont increment
            if (lstAlerts.Items.Count > 0)
            {
                    DisplayAlerts();
                    lstAlerts.SelectedIndex++;
            }


            _sound = await PlaySound("SnoozeSample");
            _sound.Play();
        }

        private async void PlayAlert()
        {
            _sound = await PlaySound("AlertSample");
            _sound.Play();
        }

        private async Task<MediaElement> PlaySound(string wavResource)
        {
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets\\Sound");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(String.Format("{0}.wav", wavResource));
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            _sound.SetSource(stream, file.ContentType);

            return _sound;
        }
        
        private void GoUp()
        {
            if (lstAlerts.SelectedIndex > 0)
            {
                lstAlerts.SelectedIndex--;
                lstAlerts.ScrollIntoView(lstAlerts.SelectedItem);
            }
        }

        private void GoDown()
        {
            if (lstAlerts.SelectedIndex < lstAlerts.Items.Count - 1)
            {
                lstAlerts.SelectedIndex++;
                lstAlerts.ScrollIntoView(lstAlerts.SelectedItem);
            }
        }
}
}

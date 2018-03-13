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
    public sealed partial class UserInfo : Page
    {
        public UserInfo()
        {
            this.InitializeComponent();
        }

        private async void OnConfirmInformation(object sender, RoutedEventArgs e)
        {
            string errorMsg = CheckUserInput();

            if (errorMsg.Equals(""))
            {

                //TODO: Fill out FlowerPot ID, Tag Manager Serial Number and MAC Address for user? 
                string fpID = txtFPID.Text;
                string tagSerNum = txtTagMgr.Text;

                string fName = txtFirstName.Text;
                string lName = txtLastName.Text;
                string emailAddr = txtEmail.Text;
                int age = Int32.Parse(txtAge.Text);
                char gender = txtGender.Text[0];
                long pNumber = Convert.ToInt64(txtPhoneNum.Text);

                User newUser = new User(fpID, tagSerNum, fName, lName, emailAddr, age, gender, pNumber);

                //TODO: Create new record in the database

                this.Frame.Navigate(typeof(DashboardPage), newUser);
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Input Error",
                    MaxWidth = this.ActualWidth,
                    PrimaryButtonText = "OK",
                    Content = new TextBlock { Text = errorMsg, FontSize = 18,}
                };

                await dialog.ShowAsync();
            }
        }

        private string CheckUserInput()
        {
            //ensure all fields have values
            if (txtFPID.Text.Equals("") || txtTagMgr.Text.Equals("") || txtFirstName.Text.Equals("")
                || txtLastName.Equals("") || txtEmail.Text.Equals("") || txtAge.Text.Equals("")
                || txtGender.Text.Equals("") || txtPhoneNum.Text.Equals(""))
            {
                return "Please complete all of the fields";
            }

            //TODO: Validate Name Input

            //TODO: Vaildate Email Input

            //validate age input
            try 
            {
                int age = Int32.Parse(txtAge.Text);
                if (age > 150)
                {
                    return "Please enter a valid age";
                }  
            }
            catch
            {
                return "Age must be a number";
            }
            
            //TODO: Make gender a dropdown
            //validate gender input
            if (!Char.IsLetter(txtGender.Text[0]) && txtGender.Text[0] != 'M' 
                && txtGender.Text[0] != 'F' && txtGender.Text[0] != 'O')
            {
                return "Please enter a valid option for gender";
            }

            //TODO: Validate Phone Number Input (Reg expression) - maybe include dashes?
            //validate phone number input
            try
            {
                long pNum = Convert.ToInt64(txtPhoneNum.Text);
            }
            catch
            {
                return "Invalid phone number";
            }

            //return an empty string if there are no errors with the input
            return "";
        }
    }
}

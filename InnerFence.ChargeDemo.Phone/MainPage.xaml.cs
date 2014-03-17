using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using InnerFence.ChargeDemo.Phone.Resources;
using InnerFence.ChargeAPI;
using Microsoft.Xna.Framework.GamerServices;
using Windows.System;

namespace InnerFence.ChargeDemo.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void ChargeButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the ChargeRequest using the default constructor
            ChargeRequest chargeRequest = new ChargeRequest();

            // 2-way Integration
            //
            // By supplying the ReturnURL parameter, we give Credit Card
            // Terminal a way to invoke us when the transaction is
            // complete. If you don't give a ReturnURL, Credit Card Terminal
            // will still launch and pre-fill the form values supplied, but
            // there will be no way for the user to return to your
            // application.
            //
            // The simplest way to do this is just to set the property:
            // chargeRequest.ReturnURL = @"com-innerfence-chargedemo://chargeresponse";
            //
            // But since it's so common to include app-specific parameters in
            // the return URL, you can use the SetReturnURL()
            // method to provide a dictionary of app-specific parameters which
            // are automatically encoded into the query string of the
            // ReturnURL. Those parameters will be available in the
            // ExtraParams dictionary of the ChargeResponse when the request
            // is completed.
            //
            // In this sample, we include an app-specific "record_id"
            // parameter set to the value 123. You may call extra parameters
            // anything you like, but to avoid collision with charge-related
            // parameters, the names may not begin with "ifcc_".
            Dictionary<string, string> extraParams =
               new Dictionary<string, string>() { { "record_id", "123" } };
            chargeRequest.SetReturnURL(
                @"com-innerfence-chargedemo://chargeresponse",
                extraParams
            );

            // Finally, we can supply customer and transaction data so that it
            // will be pre-filled for submission with the charge.
            chargeRequest.Address = "123 Test St";
            chargeRequest.Amount = "50.00";
            chargeRequest.Currency = "USD";
            chargeRequest.City = "Nowhereville";
            chargeRequest.Company = "Company Inc";
            chargeRequest.Country = "US";
            chargeRequest.Description = "Test transaction";
            chargeRequest.Email = "john@example.com";
            chargeRequest.FirstName = "John";
            chargeRequest.InvoiceNumber = "321";
            chargeRequest.LastName = "Doe";
            chargeRequest.Phone = "555-1212";
            chargeRequest.State = "HI";
            chargeRequest.Zip = "98021";

            // Submitting the request will launch Credit Card Terminal
            try
            {
                ChargeUtils.SubmitChargeRequest(chargeRequest);
            }
            catch (ChargeException)
            {
                // An Exception is thrown when we are unable to launch
                // Credit Card Terminal. This usually means the app is
                // not installed.
                HandleCCTerminalNotInstalled();
            }
        }

        private async void HandleCCTerminalNotInstalled()
        {
            // We suggest showing the user an error with a easy way
            // to download the app by showing a message dialog similar
            // to the one below.
            IAsyncResult result = Guide.BeginShowMessageBox(
                "App Not Installed",
                "You'll need to install Credit Card Terminal before you can use this feature. " +
                "Click Install below to begin the installation process.",
                new string[] { "Install", "Close" },
                0, // Set the command that will be invoked by default
                Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.Alert,
                null,
                null);

            // Make message box synchronous
            result.AsyncWaitHandle.WaitOne();

            int? choice = Microsoft.Xna.Framework.GamerServices.Guide.EndShowMessageBox(result);
            if (choice.HasValue)
            {
                if (choice.Value == 0)
                {
                    // User clicks on the Install button
                    // Open the windows store link to Credit Card Terminal
                    await Launcher.LaunchUriAsync(new Uri(ChargeRequest.CCTERMINAL_WP8_STORE_LINK));

                }
            }
        }
    }
}
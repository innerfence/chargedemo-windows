using InnerFence.ChargeAPI;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InnerFence.ChargeDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
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
            // chargeRequest.ReturnURL = @"com-innerfence-chargedemo://chargeResponse";
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
                @"com-innerfence-chargedemo://chargeResponse",
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
            chargeRequest.Email = "johnexample.com";
            chargeRequest.FirstName = "John";
            chargeRequest.InvoiceNumber = "321";
            chargeRequest.LastName = "Doe";
            chargeRequest.Phone = "555-1212";
            chargeRequest.State = "HI";
            chargeRequest.Zip = "98021";

            // Submit request
            Utils.SubmitChargeRequest(chargeRequest);
        }
    }
}

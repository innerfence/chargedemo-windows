using InnerFence.ChargeAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace InnerFence.ChargeDemo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static readonly Regex RecordIdPattern = new Regex("^[0-9]+$");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            // When the transaction is complete, we'll launch the returnUrl
            // if you provided one to us.

            var protocolArgs = args as ProtocolActivatedEventArgs;
            if (args.Kind != ActivationKind.Protocol || protocolArgs == null)
            {
                // There are several ways your app can be activated.
                // We are only concern with the case where we're activated.
                // In your app, this might mean you should handle this
                // differently.
                return;
            }

            Uri uri = protocolArgs.Uri;

            // This sample always uses com-innerfence-chargedemo://chargeresponse
            // as the base return URL.
            if (!uri.Host.Equals("chargeresponse"))
            {
                // In your app, this might mean that you should handle this as
                // a normal URL request instead of a charge response.
                return;
            }

            ChargeResponse response;
            try
            {
                // Creating the ChargeResponse object will throw an exception
                // if there's a problem with the response URL parameters
                response = new ChargeResponse(uri);
            }
            catch (Exception ex)
            {
                // In the event the parsing fails, we will throw an exception
                // and you should handle the error.
                ShowMessage(ex.Message);
                return;
            }
            this.HandleResponse(response);
        }

        private void HandleResponse(ChargeResponse response)
        {
            // You may want to perform different actions based on the
            // response code. This example shows an message dialog with
            // the response data when the charge is approved.
            if (response.ResponseCode == ChargeResponse.Code.APPROVED)
            {
                // Any extra params we included with the return URL can be
                // queried from the ExtraParams dictionary.
                string recordId;
                response.ExtraParams.TryGetValue("record_id", out recordId);

                // The URL is a public attack vector for the app, so it's
                // important to validate any parameters.
                if (!this.IsValidRecordId(recordId))
                {
                    ShowMessage("Invalid Record ID");
                    return;
                }

                string message = String.Format(
                    "Charged!\n" +
                    "Record: {0}\n" +
                    "Transaction ID: {1}\n" +
                    "Amount: {2} {3}\n" +
                    "Card Type: {4}\n" +
                    "Redacted Number: {5}",
                    recordId,
                    response.TransactionId,
                    response.Amount,
                    response.Currency,
                    response.CardType,
                    response.RedactedCardNumber);

                // Generally you would do something app-specific here,
                // like load the record specified by recordId, record the
                // success or failure, etc. Since this sample doesn't
                // actually do much, we'll just pop a message dialog.
                ShowMessage(message);
            }
            else // other response code values are documented in ChargeResponse.cs
            {
                string recordId;
                response.ExtraParams.TryGetValue("record_id", out recordId);

                string message = String.Format(
                    "Not Charged!\n" +
                    "Record: {0}",
                    recordId);
                ShowMessage(message);
            }
        }

        private bool IsValidRecordId(string recordId)
        {
            return null != recordId && RecordIdPattern.Match(recordId).Success;
        }

        private async void ShowMessage(string message)
        {
            // Show message dialog
            var messageDialog = new MessageDialog(message);
            await messageDialog.ShowAsync();
        }
    }
}

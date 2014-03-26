using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using InnerFence.ChargeDemo.Phone.Resources;
using InnerFence.ChargeAPI;
using Microsoft.Xna.Framework.GamerServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace InnerFence.ChargeDemo.Phone
{
    public partial class App : Application
    {
        private static readonly Regex RecordIdPattern = new Regex("^[0-9]+$");

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Gets the current application.
        /// </summary>
        public new static App Current
        {
            get
            {
                return (App)Application.Current;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // *** NOTICE! ***
            // Assign the URI-mapper class to the application frame.
            // This is required if you want to handle incoming responses from Credit Card Terminal
            RootFrame.UriMapper = new ProtocolUriMapper();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        public void HandleResponse(Uri responseUri)
        {
            ChargeResponse response;
            try
            {
                // Creating the ChargeResponse object will throw an exception
                // if there's a problem with the response URL parameters
                response = new ChargeResponse(responseUri);
            }
            catch (ChargeException ex)
            {
                // In the event the parsing fails, we will throw an exception
                // and you should handle the error.
                ShowMessage(ex.Message);
                return;
            }

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
                    CultureInfo.CurrentCulture,
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
                    CultureInfo.CurrentCulture,
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

        private void ShowMessage(string message)
        {
            if (!Guide.IsVisible)
            {
                // Show message dialog
                Guide.BeginShowMessageBox(
                    "Credit Card Terminal",
                    message,
                    new string[] { "ok" },
                    0,
                    MessageBoxIcon.None,
                    null,
                    null);
            }
        }
    }
}
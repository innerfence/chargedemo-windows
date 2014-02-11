using InnerFence.ChargeAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            // TODO: generate test request
            ChargeRequest req = new ChargeRequest();

            // Submit request
            req.Submit();
        }

        private async void HandleResponse()
        {
            // Show the message dialog
            var messageDialog = new MessageDialog("Charge failed.");
            await messageDialog.ShowAsync();
        }
    }
}

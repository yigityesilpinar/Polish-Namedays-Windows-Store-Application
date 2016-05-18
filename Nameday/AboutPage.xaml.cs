using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nameday
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            var v = Package.Current.Id.Version;
            tbAppName.Text += $"{v.Major}.{v.Minor}.{v.Revision}.{v.Build}";
        }

        // Event handler which is called when the page is navigated to
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Put Back Button depending on the device
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            base.OnNavigatedTo(e);
        }

        // What happens when back button is pressed
        private void OnBackRequested(object sender, BackRequestedEventArgs backRequestedEventArgs){
            if (Frame.CanGoBack)
                Frame.GoBack();
            // mark the event as handled so that UI will not handle 
            backRequestedEventArgs.Handled = true;

        }
    }
}

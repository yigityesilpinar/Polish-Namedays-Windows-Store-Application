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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Nameday
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

        // Event Handler for email button
        private async void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            // btnEmail is inside the item template
            // cast button FrameworkElement get its DataContext property
            // cast DataContext as ContactEx object
            var contact = ((FrameworkElement)sender).DataContext as ContactEx;

            // contact is the contact which email button has clicked

            if (contact != null)
              await ((MainPageData)this.DataContext).SendEmailAsync(contact.Contact);
           

        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Nameday
{
    // Contact class contains info for Data-binding
    // Names is different to avoid confusino with  Windows.ApplicationModel.Contacts Contact class
    public class ContactEx
    {
        // Microsoft Contact class
        public Contact Contact { get; }

        public ContactEx(Contact contact)
        {
            Contact = contact;
        }

        // To facilitate design time data 
        public ContactEx(string firsName, string lastName)
        {
            Contact = new Contact
            {
                FirstName = firsName,
                LastName = lastName
            };
        }
        // New Property syntax C#
        // Returns the first chars of first and last name of the contact
        public string Initials => GetFirstCharacter(Contact.FirstName) +
            GetFirstCharacter(Contact.LastName);

        // To handle possible empty strings
        private string GetFirstCharacter(string s) =>
            string.IsNullOrEmpty(s) ? "" : s.Substring(0, 1);

        // Email Visibility Property
        // Controls email icon next to contacts visible or not
        // UWP controls visibality with Enum instead Bool
        public Visibility EmailVisibility =>
            DesignMode.DesignModeEnabled ||
            Contact.Emails.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Picture Property of the contact
        public ImageBrush Picture
        {
            get
            {
                if (Contact.SmallDisplayPicture == null) //No thumbnal
                    return null;

                // Create a bitmap image and set its source to the stream provided by Contact objects
                var image = new BitmapImage();
                
                // !!!! Every IO access in windows 10 is ASYNC
                // Property Getters can not be async!!

                //  .GetAwaiter().GetResult() nested, part of what await keyword does!! 
                image.SetSource(Contact.SmallDisplayPicture.OpenReadAsync()
                    .GetAwaiter().GetResult());

                return new ImageBrush { ImageSource = image };
            }
        }
    }
}

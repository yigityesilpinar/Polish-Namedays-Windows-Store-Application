using System;
using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;

namespace Nameday
{
    public class MainPageData : ObservableObject
    {

        public string _greeting = "Helo world!";

        public string Greeting
        {
            get
            {
                return _greeting;
            }

            set
            {
                //if (value == _greeting)
                //{
                //    return;
                //}
                //_greeting = value;
                ////if (PropertyChanged != null)
                ////{
                ////    PropertyChanged(this,new PropertyChangedEventArgs("Greeting"));
                ////}
                //// better way
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Greeting)));
                Set(ref _greeting, value);
            }
        }

        // For filtering we need to keep Unfiltered list somewhere PRIVATE 
        private List<NamedayModel> _allNamedays = new List<NamedayModel>();

        // --CHANGED to observable Namedays property stores namedays, generic list 
        // Collection SENDS EVERY CHANGE NOTIFICATION TO UI
        public ObservableCollection<NamedayModel> Namedays { get; set; }

        public ObservableCollection<ContactEx> Contacts { get; } = new ObservableCollection<ContactEx>();

        public Settings Settings { get; } = new Settings();

        public MainPageData()
        {
            AddReminderCommand = new AddReminderCommand(this);

            Namedays = new ObservableCollection<NamedayModel>();

            // to use only fake data in designing 
            // Whether the code is running in Run-time or Designer
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {

                // fake contact data
                Contacts = new ObservableCollection<ContactEx> {
                   new ContactEx("Contact","1"),
                   new ContactEx("Contact","2"),
                   new ContactEx("Contact","3")
                };


                // fake Nameday data
                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NamedayModel(
                        month, 1, new string[] { "Adam" }));
                    _allNamedays.Add(new NamedayModel(
                        month, 24, new string[] { "Eve", "Andrew" }));
                }

                PerformFiltering();
            }
            else
                LoadData(); // async from server
        }

        public async void LoadData()
        {
            try
            {
                _allNamedays = await NamedayRepository.GetAllNamedaysAsync();
                PerformFiltering();
            }
            catch (NullReferenceException)
            {
                
            }

        }

        // Added New property to store Currently Selected Name day 
        // Full property when set update Greeting message (get,set)
        //Field
        private NamedayModel _selectedNameday;

        //Property
        public NamedayModel SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                _selectedNameday = value;
                if (_selectedNameday == null)
                    Greeting = "Hello world!";
                else {
                    Greeting = "Hello " + value.NamesAsString;
                }

                // This property updates everytime User select day from calendar
                // Place to update contacts

                // Separete method because ASYNC
                // Invoking async operation without await like this is FIRE and FORGET OP
                // Any exception may happen will be swallowed (not thrown)
                UpdateContacts();

                // UWP framwerok to know things changed, Fire the event
                AddReminderCommand.FireCanExecuteChanged();
            }
        }

        private async void UpdateContacts()
        {

            Contacts.Clear();
            if (SelectedNameday != null)
            {
                var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

                foreach (var name in SelectedNameday.Names)
                {
                    foreach (var contact in await contactStore.FindContactsAsync(name))
                    {
                        Contacts.Add(new ContactEx(contact));
                    }
                }
            }

        }

        // Property for Filter text two way data bind
        private string _filter;

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                Set(ref _filter, value);
                PerformFiltering();
            }
        }

        private void PerformFiltering()
        {
            if (_filter == null)
            {
                _filter = "";
            }

            // Filter text lowercase and trimmed
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            // Filtered Elements (selected)
            var result = _allNamedays.Where(d => d.NamesAsString.ToLowerInvariant().Contains(lowerCaseFilter)).ToList();

            // The list needs to be removed from the Namedays 
            var toRemove = Namedays.Except(result).ToList();
            // Removal from Namedays list
            foreach (var x in toRemove)
            {
                Namedays.Remove(x);
            }

            var resultCount = result.Count;
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Namedays.Count || !Namedays[i].Equals(resultItem))
                    Namedays.Insert(i, resultItem);
            }

        }

        // based on the currently selected nameday
        public async Task SendEmailAsync(Contact contact)
        {

            if (contact == null || contact.Emails.Count == 0)
                return;
            var msg = new EmailMessage();
            msg.To.Add(new EmailRecipient(contact.Emails[0].Address));
            msg.Subject = "Bardzo Cie Kocham!";

            await EmailManager.ShowComposeNewEmailAsync(msg);

        }

        // Property for add reminder button to bind
        public AddReminderCommand AddReminderCommand { get; }

        // based on the currently selected nameday
        public async void AddReminderToCalendarAsync()
        {
            var appointment = new Appointment();
            appointment.Subject = "Nameday reminder for " + SelectedNameday.NamesAsString;
            appointment.AllDay = true;
            appointment.BusyStatus = AppointmentBusyStatus.Free;
            var dateThisYear = new DateTime(
                DateTime.Now.Year, SelectedNameday.Month, SelectedNameday.Day);

            // check if the nameday earlier, if so next year
            appointment.StartTime =
            dateThisYear < DateTime.Now ? dateThisYear.AddYears(1) : dateThisYear;

            //appointment.Recurrence = new AppointmentRecurrence
            //{
            //    Day = (uint)SelectedNameday.Day,
            //    Month = (uint)SelectedNameday.Month,
            //    Unit = AppointmentRecurrenceUnit.Yearly,
            //    Interval = 1
            //};


            await AppointmentManager.ShowEditNewAppointmentAsync(appointment);


        }


    }

    // TO Bind Button.Command property to a System.Windows.Input.ICommand object (Commanding)
    public class AddReminderCommand : System.Windows.Input.ICommand
    {
        private MainPageData _mpd;

        public AddReminderCommand(MainPageData mpd) {

            _mpd = mpd;
        }

        public event EventHandler CanExecuteChanged;

        // return true only if there is a selected nameday
        public bool CanExecute(object parameter) => _mpd.SelectedNameday != null;

        // on click add reminder to calendar
        public void Execute(object parameter) => _mpd.AddReminderToCalendarAsync();

        // fires  CanExecuteChanged event
        public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke(this,EventArgs.Empty);
      
    }

}

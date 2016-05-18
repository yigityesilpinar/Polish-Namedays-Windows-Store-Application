using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common
{
    // APP SETTINGS
    // Needed to store settings in local settings store to be available next time the app run
    public class Settings : ObservableObject
    {
        // Backing field is mandotary when simple set is not enough
        private bool _notificationsEnabled = false;

        public bool NotificationsEnabled
        {
            get { return _notificationsEnabled; }
            set
            {
                if (Set(ref _notificationsEnabled, value))
                    _localSettings.Values[nameof(NotificationsEnabled)] = value;
            }
        }

        private bool _updatingLiveTileEnabled = false;

        public bool UpdatingLiveTileEnabled
        {
            get { return _updatingLiveTileEnabled; }
            set
            {
                if (Set(ref _updatingLiveTileEnabled, value))
                    _localSettings.Values[nameof(UpdatingLiveTileEnabled)] = value;
            }
        }

        private DateTime _lastSuccessfulRun = DateTime.MinValue;


        public DateTime LasteSuccesfulRun
        {
            get { return _lastSuccessfulRun; }
            set {
                if (Set(ref _lastSuccessfulRun, value))
                    _localSettings.Values[nameof(LasteSuccesfulRun)] = value;
            }
        }
        public Settings()
        {
            LoadSettings();
        }

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private void LoadSettings()
        {
            var notificationsEnabled = _localSettings.Values[nameof(NotificationsEnabled)];
            if (notificationsEnabled != null)
                NotificationsEnabled = (bool)notificationsEnabled;

            var updatingLiveTileEnabled = _localSettings.Values[nameof(UpdatingLiveTileEnabled)];
            if (updatingLiveTileEnabled != null)
                UpdatingLiveTileEnabled = (bool)updatingLiveTileEnabled;

            var lastSuccessfulRun = _localSettings.Values[nameof(LasteSuccesfulRun)];
            if (lastSuccessfulRun != null)
                LasteSuccesfulRun = DateTime.Parse(lastSuccessfulRun.ToString(), CultureInfo.InvariantCulture);
        }



    }
}

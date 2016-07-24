using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Spico
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("SpeechConsent"))

            {

                var settings = IsolatedStorageSettings.ApplicationSettings;
                Boolean speechConsent = (Boolean)settings["SpeechConsent"];


                if (speechConsent == true)
                {
                    speechService.IsChecked = true;
                }
                else
                {
                    speechService.IsChecked = false;

                }
            } else
            {
                speechService.IsChecked = true;

            }


       
        }

        private void save_click(object sender, EventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }


        private void speechService_Checked(object sender, RoutedEventArgs e)
        {
           
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["SpeechConsent"] = true;
                settings.Save();

        }

        private void speechService_Unchecked(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["SpeechConsent"] = false;
            settings.Save();
        }
    }
}
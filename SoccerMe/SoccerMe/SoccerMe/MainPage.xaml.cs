using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace SoccerMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings settings;
        SmsComposeTask objSmsComposeTask = new SmsComposeTask();
        PhoneApplicationService objPhoneService = PhoneApplicationService.Current;
        // Constructor
        
        public MainPage()
        {
            InitializeComponent();
            settings = IsolatedStorageSettings.ApplicationSettings;
            
        }

        private object GetValue(string key)
        {
            if (settings.Contains(key))
                return settings[key];
            else
                return null;
        }

        private void AddUpdateSettings(string key, object value)
        {
            if (settings.Contains(key))
            {
                settings.Remove(key);
                settings.Add(key, value);
            }
            else
            {
                settings.Add(key, value);
            }
            settings.Save();
        }

        
        private void call(String rss,String cat) {
            AddUpdateSettings("rss", rss);
            AddUpdateSettings("cat", cat);
            NavigationService.Navigate(new Uri("/Viewer.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            call("http://www.soccernews.com/category/english-premier-league/feed","English Premier League"); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            call("http://www.soccernews.com/category/la-liga/feed/", "La Liga");            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            call("http://www.soccernews.com/category/serie-a/feed","Serie A");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            call("http://www.soccernews.com/category/ligue-1/feed", "Ligue 1");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            call("http://www.soccernews.com/category/uefa-champions-league/feed", "Champion League");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            call("http://sports.yahoo.com/soccer/rss.xml","World Soccer Updates");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            string msg = "Hi there,\nI know you are a fan of Soccer like me.\nI found this Great application called SoccerMe which gives updates all the time.\nYou should give it a try.";
            objSmsComposeTask.To = "";
            objSmsComposeTask.Body = msg;
            objSmsComposeTask.Show();
        }


    }
}
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;


using System.IO.IsolatedStorage;
using System.ComponentModel;

namespace SoccerMe
{
    public partial class Viewer : PhoneApplicationPage
    {
        IsolatedStorageSettings settings;
        public string link;
        public string cat;


        public Viewer()
        {
            InitializeComponent();
            settings = IsolatedStorageSettings.ApplicationSettings;
            Loaded += Viewer_Loaded;

        }

        void Viewer_Loaded(object sender, RoutedEventArgs e)
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
            link = GetValue("rss").ToString();
            cat = GetValue("cat").ToString();


            lblInfo.Text = cat;
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadStringAsync(new Uri(link));
        }

        protected override void OnBackKeyPress(CancelEventArgs e) {
            AddUpdateSettings("rss", "");
            AddUpdateSettings("cat", "");
            

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
        
        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                StringReader strReader = new StringReader(e.Result);
                XmlReader xmlReader = XmlReader.Create(strReader);
                SyndicationFeed feed;

                feed = SyndicationFeed.Load(xmlReader);
                var fixedFeed = (from item in feed.Items
                                 select new DataClass()
                                 {
                                     Title = item.Title.Text,
                                     SyndItem=item,
                                     Content = FixSummary(item.Summary.Text),
                                     PublishedDate = item.PublishDate.DateTime
                                 }).ToList();
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    feedListBox.ItemsSource = fixedFeed;
                });




            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Error.Message);
                });
            }
        }

        private string FixSummary(string summary)
        {
            string fixedString = "";
            int maxLength = 250;
            int strLength = 0;
            fixedString = Regex.Replace(summary, "<[^>]+>", string.Empty);
            fixedString = fixedString.Replace("\r", "").Replace("\n", "");
            fixedString = HttpUtility.HtmlDecode(fixedString);

            strLength = fixedString.ToString().Length;
            if (strLength == 0)
            {
                return "";
            }

            else if (strLength >= maxLength)
            {
                fixedString = fixedString.Substring(0, maxLength);
                fixedString = fixedString.Substring(0, fixedString.LastIndexOf(" "));
            }

            fixedString += "...";

            return fixedString;
        }

        private object GetValue(string key)
        {
            if (settings.Contains(key))
                return settings[key];
            else
                return null;
        }


        private void feedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                
                SyndicationItem sItem = ((DataClass)listBox.SelectedItem).SyndItem;

                if (sItem.Links.Count > 0)
                {
                    Uri uri = sItem.Links.FirstOrDefault().Uri;
                    
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = uri;
                    webBrowserTask.Show();
                }
            }
        }


    }

    public class DataClass
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public SyndicationItem SyndItem { get; set; }
    }
}
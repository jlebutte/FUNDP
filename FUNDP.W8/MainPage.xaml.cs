﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FUNDP.W8.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace FUNDP.W8
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : FUNDP.W8.Common.LayoutAwarePage
    {
        private List<Section> _sections = new List<Section>(); 
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("http://fundp.lebutte.net/horaires/GetSections");
            HttpWebResponse Response = (HttpWebResponse)await Request.GetResponseAsync();
            StreamReader ResponseDataStream = new StreamReader(Response.GetResponseStream());
            _sections =  JsonHelper.FromJson<List<Section>>(await ResponseDataStream.ReadToEndAsync());

            listSections.ItemsSource = _sections.Where(s => s.ParentId == 11);
            listSections.SelectedIndex = 1;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void listSections_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            Section item = (Section)listSections.SelectedItem;
            img.Source = new BitmapImage(new Uri("http://fundp.lebutte.net/horaires/GetImage?section="+item.Id)); ;
        }
    }
}
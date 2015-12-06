using PhoneAppTest.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PhoneAppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class myReports : Page
    {


        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        // class level instance of myReportslist
        private List<Report> myReportsList = new List<Report>();

        private int found;




        public myReports()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        }

        

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           this.navigationHelper.OnNavigatedTo(e);
            retrieveData();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region fill list from db and pass selected to singleReport page

        



        private async void retrieveData()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");

            var results = await connection.QueryAsync<Report>("Select * FROM Reports ");


            foreach (var row in results)
            {
                myReportsList.Add(row);
            }



          
            foreach(var item in myReportsList)
            {
                lvReports.Items.Add("Id: " + item.uid + "\nOrganisation: " + item.name + 
                                    "\nLatitude: " + item.lat + "\nLongitude: " + item.lng + 
                                    "\nMessage: " + item.message +  "\nPhoto: " + item.photo_id + 
                                    "\nEmail: " + item.organisation_email +  "\nPhone: " + item.organisation_email +  "\nDate: " + item.date);
            }

        }

    
        private void lvReports_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {


            

            found = lvReports.SelectedIndex;

            foreach (var item in myReportsList)
            {
                if (myReportsList.IndexOf(item) == found)
                {

                   
                    if (Frame != null)
                    {
                       
                        Frame.Navigate(typeof(SingleReport), new Report {uid = item.uid,
                                                                         name = item.name,
                                                                         lat = item.lat.ToString(),
                                                                         lng = item.lng.ToString(),
                                                                         message = item.message.ToString(),
                                                                         photo_id = item.photo_id.ToString(),
                                                                         organisation_email = item.organisation_email.ToString(),
                                                                         organisation_phone = item.organisation_phone.ToString(),
                                                                         date = item.date });

                    }

                }
            }

        }
        private async void deleteReport(int uid)
        {

            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");

            var Report = await connection.Table<Report>().Where(x => x.uid.Equals(uid)).FirstOrDefaultAsync();
            if (Report != null)
            {
                await connection.DeleteAsync(Report);

            }



        }






        private async void lvReports_Holding(object sender, HoldingRoutedEventArgs e)
        {


            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            if (element.DataContext != null && element.DataContext is StackPanel)
            {
                //ListViewItem selectedOne = ()element.DataContext;

                MessageDialog dialog = new MessageDialog("Are you sure you want to delete "  );
                await dialog.ShowAsync();
                // rest of the code
            }


            lvReports.Visibility = Visibility.Collapsed;





           

            // ListViewItem lv = (ListViewItem)sender;



            // SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");



            // MessageDialog dialog = new MessageDialog("Are you sure you want to delete " + lvReports);

            //dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            //dialog.Commands.Add(new UICommand("No") { Id = 1 });




            //switch (dialog.DefaultCommandIndex)
            //{
            //    case 0:
            //        //deleteReport(found);

            //        break;
            //    case 1:

            //        break;
            //    default:
            //        break;

            //}



            //if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            //{
            //    // Adding a 3rd command will crash the app when running on Mobile !!!
            //    dialog.Commands.Add(new Windows.UI.Popups.UICommand("Maybe later") { Id = 2 });
            //}

            //dialog.DefaultCommandIndex = 0;
            //dialog.CancelCommandIndex = 1;


            //return;

            //var btn = sender as Button;
            //btn.Content = $"Result: {result.Label} ({result.Id})";



            /// deleteReport(select);
            // md.ShowAsync();


        }

        
        }
    }

    #endregion


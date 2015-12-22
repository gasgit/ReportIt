using PhoneAppTest.Common;
using SQLite;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PhoneAppTest
{
   
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

        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

      
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

      
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

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
                lvReports.Items.Add("Id: " + item.uid + "\nSent To: " + item.name + 
                                    "\nLatitude: " + item.lat + "\nLongitude: " + item.lng + 
                                    "\nMessage: " + item.message +  "\nPhoto: " + item.photo_id + 
                                    "\nEmail: " + item.organisation_email +  "\nPhone: " + item.organisation_email +  "\nDate: " + item.date);
            }

        }

    
        private void lvReports_Tapped(object sender, TappedRoutedEventArgs e)
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

        
        }
    }

    #endregion


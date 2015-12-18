using PhoneAppTest.Common;
using SQLite;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PhoneAppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SingleReport : Page
    {

        
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        // set variable for uid to delete at class level
        private int idToDelete;

        private Report data;

        public StorageFile storageFile;

        private BitmapImage bitmap;

        private StorageFile stFile;

        public SingleReport()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            // Register the current page as a share source.
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        private  void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // gets the global state for subject declared in App.cs and set in button click mainpage.cs
            // var obj = App.Current as App;
            // args.Request.Data.Properties.Title = obj.subject;

            args.Request.Data.Properties.Title = data.name.ToString();
            args.Request.Data.SetText("Date:" + txtDate.Text + "\nLatitude: " + data.lat + "\nLongitude: " + data.lng + "\nMessage: " + txtMessage.Text +
                "\nGoogle Maps: " + "http://maps.google.com/maps?q=" + data.lat + "+" + data.lng + "\n" +
                "\nOpenStreetMaps: " + "http://www.openstreetmap.org/?mlat=" + data.lat + "&mlon=" + data.lng + "&zoom=16");



           var imageStreamRef = RandomAccessStreamReference.CreateFromUri(new Uri(data.photo_id));
           


            args.Request.Data.Properties.Thumbnail = imageStreamRef;



            args.Request.Data.SetBitmap(imageStreamRef);


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
            // passed  object fron myReports from selection on myReportsList
            data = e.Parameter as Report;

            idToDelete = data.uid;

            txtId.Text = data.uid.ToString();
            txtName.Text = data.name.ToString();
            txtLat.Text = data.lat.ToString();
            txtLng.Text = data.lng.ToString();
            txtMessage.Text = data.message.ToString();
            txtPhoto.Text = data.photo_id.ToString();
            txtPhone.Text = data.photo_id.ToString();
            txtEmail.Text = data.organisation_email.ToString();
            txtPhone.Text = data.organisation_phone.ToString();

            txtDate.Text = data.date.ToString();

            bitmap = new BitmapImage(new Uri(data.photo_id));

            sgImage.Source = bitmap;

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void deleteReport(int uid)
        {

            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");

            var Report = await connection.Table<Report>().Where(x => x.uid.Equals(uid)).FirstOrDefaultAsync();
            if (Report != null)
            {
                await connection.DeleteAsync(Report);

            }



        }







        private void btnResend_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();

        }

        private void btnDelete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            deleteReport(idToDelete);
        }

        private void btnMap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

           
            if (Frame != null)
            {
                // this.Frame.Navigate(typeof(Map));
                Frame.Navigate(typeof(Map), new Report
                {
                    uid = data.uid,
                    name = data.name,
                    lat = data.lat.ToString(),
                    lng = data.lng.ToString(),
                    message = data.message.ToString(),
                    photo_id = data.photo_id.ToString(),
                    organisation_email = data.organisation_email.ToString(),
                    organisation_phone = data.organisation_phone.ToString(),
                    date = data.date
                });
            }
        }
    }
}

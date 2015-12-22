using PhoneAppTest.Common;
using SQLite;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
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
        private RandomAccessStreamReference imageStreamRef;

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
            
            args.Request.Data.Properties.Title = data.name.ToString();
            args.Request.Data.SetText("Date:" + txtDate.Text + "\nLatitude: " + data.lat + "\nLongitude: " + data.lng + "\nMessage: " + txtMessage.Text +
                "\nGoogle Maps: " + "http://maps.google.com/maps?q=" + data.lat + "+" + data.lng + "\n" +
                "\nOpenStreetMaps: " + "http://www.openstreetmap.org/?mlat=" + data.lat + "&mlon=" + data.lng + "&zoom=16");

            imageStreamRef = RandomAccessStreamReference.CreateFromUri(new Uri(data.photo_id));
           
            args.Request.Data.Properties.Thumbnail = imageStreamRef;

            args.Request.Data.SetBitmap(imageStreamRef);

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
            // passed  object fron myReports from selection on myReportsList
            data = e.Parameter as Report;

            idToDelete = data.uid;

            txtId.Text = data.uid.ToString();
            txtName.Text = data.name;
            txtLat.Text = data.lat;
            txtLng.Text = data.lng;
            txtMessage.Text = data.message;
            txtPhoto.Text = data.photo_id;
            txtEmail.Text = data.organisation_email;
            txtPhone.Text = data.organisation_phone;

            txtDate.Text = data.date;

            bitmap = new BitmapImage(new Uri(data.photo_id));


            sgImage.Source = bitmap;

            navigationHelper.OnNavigatedTo(e);
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

        private async void btnResend_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // DataTransferManager.ShowShareUI();

            EmailRecipient sendTo = new EmailRecipient()
            {
                Address = data.organisation_email
            };

            //generate mail object
            EmailMessage mail = new EmailMessage();
            //add recipients to the mail object
            mail.To.Add(sendTo);
            // get subject from app.cs
            // var obj = App.Current as App;
            //mail.Subject = obj.subject;
            mail.Subject = "Resending";
            // check storagefile ( declared at class level, can also be used by  datatransfer)
            // 
            if (storageFile != null)
            {

                imageStreamRef = RandomAccessStreamReference.CreateFromFile(storageFile);

            }


            // get name from storagefile properties and save with image as attachment
            mail.Attachments.Add(new EmailAttachment(storageFile.Name.ToString(), imageStreamRef));
            // construct mail body 
            mail.Body = "Date:" + txtDate.Text + "\nLatitude: " + data.lat + "\nLongitude: " + data.lng + "\nMessage: " + txtMessage.Text +
                "\nGoogle Maps: " + "http://maps.google.com/maps?q=" + data.lat + "+" + data.lng + "\n" +
                "\nOpenStreetMaps: " + "http://www.openstreetmap.org/?mlat=" + data.lat + "&mlon=" + data.lng + "&zoom=16";





            await EmailManager.ShowComposeNewEmailAsync(mail);

        }

        private void btnDelete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            deleteReport(idToDelete);
        }

        private void btnMap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

           
            if (Frame != null)
            {

                // send new object to map page with details
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

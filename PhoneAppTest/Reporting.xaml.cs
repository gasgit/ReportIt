using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Data.Json;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PhoneAppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Reporting : Page
    {


        #region variable etc at class level


        //public image file byte array
        public byte[] fileBytes = null;

        public string emailed;
        CoreApplicationView view;
        string ImagePath;
        public BitmapImage bitmapImage = new BitmapImage();

        // for filepicker 
        public StorageFile storageFile, storageEmail;
        

        Report sendReportObj = new Report();

        public DateTime myDate = DateTime.Now;
        private Geolocator geo;

        #endregion variable etc at class level

        #region main

        public Reporting()
        {
            this.InitializeComponent();
            view = CoreApplication.GetCurrentView();
            // required to save the state if navigated away from
            this.NavigationCacheMode = NavigationCacheMode.Required;

        }


        #endregion main

        #region nav to and from with OnSharDataRequested

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            getLocation();
            await InitialiseListOfOrganisations();
            loadListView();
            
            txtDate.Text = myDate.ToString();

            // Register the current page as a share source.
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;

            base.OnNavigatedTo(e);
        }

        protected  override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            base.OnNavigatedFrom(e);

            // sets the cache to disabled when i want to nav from back to main menu in this case so that when user 
            // make choices again the form is new)
            if (e.NavigationMode == NavigationMode.Back)
                NavigationCacheMode = NavigationCacheMode.Disabled;

        }

        private  void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {

            // gets the global state for subject declared in App.cs and set in button click mainpage.cs
            var obj = App.Current as App;
            args.Request.Data.Properties.Title = obj.subject;

           
            args.Request.Data.SetText("Date:" + txtDate.Text + "\nLatitude: " + sendReportObj.lat + "\nLongitude: " + sendReportObj.lng + "\nMessage: " + txtMessage.Text +
                "\nGoogle Maps: " + "http://maps.google.com/maps?q=" + sendReportObj.lat + "+" + sendReportObj.lng + "\n" +
                "\nOpenStreetMaps: " + "http://www.openstreetmap.org/?mlat=" + sendReportObj.lat + "&mlon=" + sendReportObj.lng + "&zoom=16");

            var imageItems = new List<IStorageItem>();

            imageItems.Add(storageFile);

            args.Request.Data.SetStorageItems(imageItems);

            var imageStreamRef = RandomAccessStreamReference.CreateFromFile(storageFile);

            args.Request.Data.Properties.Thumbnail = imageStreamRef;

            args.Request.Data.SetBitmap(imageStreamRef);

           
        }

        //private StorageFile storageFile;






        private async void email()
        {
           

            if (storageFile == null)
            {
                MessageDialog message = new MessageDialog("Add photo to message ");
                await message.ShowAsync();
                return;
            }
            else if (cmbSelectOrg.SelectedIndex < 1)
            {
                MessageDialog message = new MessageDialog("Select Organisation ");
                await message.ShowAsync();
                return;

            }
            else if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageDialog message = new MessageDialog("Enter Message ");
                await message.ShowAsync();
                return;

            }
            else
            {

                EmailRecipient sendTo = new EmailRecipient()
                {
                    Address = sendReportObj.organisation_email
                };

                //generate mail object
                EmailMessage mail = new EmailMessage();
                //add recipients to the mail object
                mail.To.Add(sendTo);
                // get subject from app.cs
                var obj = App.Current as App;
                mail.Subject = obj.subject;
                // check storagefile ( declared at class level, can also be used by  datatransfer)
                // 
                if (storageFile != null)
                {

                    imageStreamRef = RandomAccessStreamReference.CreateFromFile(storageFile);

                }


                // get name from storagefile properties and save with image as attachment
                mail.Attachments.Add(new EmailAttachment(storageFile.Name.ToString(), imageStreamRef));
                // construct mail body 
                mail.Body = "Date:" + txtDate.Text + "\nLatitude: " + sendReportObj.lat + "\nLongitude: " + sendReportObj.lng + "\nMessage: " + txtMessage.Text +
                    "\nGoogle Maps: " + "http://maps.google.com/maps?q=" + sendReportObj.lat + "+" + sendReportObj.lng + "\n" +
                    "\nOpenStreetMaps: " + "http://www.openstreetmap.org/?mlat=" + sendReportObj.lat + "&mlon=" + sendReportObj.lng + "&zoom=16";





                await EmailManager.ShowComposeNewEmailAsync(mail);

            }



            //open the share contract with Mail only and recipent auto filled
            // add to db
            addReportDB();

        }



        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {
            // check if there is an image added to message as share contract expects it in datatransfer manager
            // if null show message dialog and return to app to add image



            loadObjectDetails();

            //email();


            if (storageFile == null)
            {
                MessageDialog message = new MessageDialog("Add photo to message ");
                await message.ShowAsync();
                return;
            }
            else if(cmbSelectOrg.SelectedIndex < 1)
            {
                MessageDialog message = new MessageDialog("Select Organisation ");
                await message.ShowAsync();
                return;

            }
            else if(string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageDialog message = new MessageDialog("Enter Message ");
                await message.ShowAsync();
                return;

            }
            else
            {

               

                DataTransferManager.ShowShareUI();

            }
            // add to db
            addReportDB();
           // retrieveData();

           
        }

     
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        #endregion nav to and from with OnSharDataRequested


        #region read json create list of organisations get email etc

        // read json file and parse to templist -- async task so doing in background
        // i need to retrieve list from server maybe so it can be used in different regions and types
        // i need to create a database to store user reports

        private async Task InitialiseListOfOrganisations()
        {
            // read file from location - data folder in solution 
            // using global App.Crrent in app.cs
            var objData = App.Current as App;

            var file = await Package.Current.InstalledLocation.GetFileAsync(objData.datafile);
            var result = await FileIO.ReadTextAsync(file);
            try
            {
                // parse the json file text to a json array
                var tempList = JsonArray.Parse(result);
                // call method to convert array to list
                convertArrayToList(tempList);

            }
            catch (Exception e)
            {
                string ex = e.Message;
            }

        }// end task

        // create list of organisations
        List<Organisation> newOrgList = new List<Organisation>();
        private IStorageFile fl;
        private RandomAccessStreamReference imageStreamRef;

        //private MediaCapture captureManager;


        // method to convert JsonArray templist to newList
        private void convertArrayToList(JsonArray tempList)
        {
            // iterate templist
            foreach (var item in tempList)
            {
                // create collection obj
                var obj = item.GetObject();
                // create organisation  object from organisation cls
                Organisation org = new Organisation();
                // get key values 
                foreach (var key in obj.Keys)
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value))
                        continue;

                    // add values to links object
                    if (key.Equals("name"))
                    {
                        org.name = value.GetString();
                    }
                    if (key.Equals("address"))
                    {
                        org.address = value.GetString();
                    }
                    if (key.Equals("phone"))
                    {
                        org.phone = value.GetString();
                    }
                    if (key.Equals("fax"))
                    {
                        org.fax = value.GetString();
                    }
                    if (key.Equals("email"))
                    {
                        org.email = value.GetString();
                    }



                } // end foreach objectKeys
                  
                // add links objects to newOrgList
                newOrgList.Add(org);

            }// end foreach templist

        }// end convert array

       

        public object NavigationService { get; private set; }

        // load combobox with organisation names 
        private void loadListView()
        {   // iterate newOrgList and add to cmbSelectOrg
            foreach (var item in newOrgList)
            {
                // get name from the item object
                cmbSelectOrg.Items.Add(item.name);


                var orgObj = App.Current as App;
                cmbSelectOrg.PlaceholderText = orgObj.orgName;


            }// end foreach newOrgList
        }




       
        private void loadObjectDetails()
        {

          

            if (cmbSelectOrg.SelectedIndex == -1)
            {
                MessageDialog msg = new MessageDialog("Select Organisation now!");
                
            }
            else
            {
                string selected = cmbSelectOrg.SelectedItem.ToString();

                foreach (Organisation listOrg in newOrgList)
                {

                    if (selected == listOrg.name)
                    {
                        
                        sendReportObj.name = listOrg.name;
                        sendReportObj.organisation_email = listOrg.email;
                        sendReportObj.organisation_phone = listOrg.phone;
                        sendReportObj.photo_id = ImagePath;
                        sendReportObj.message = txtMessage.Text;

                    }

                }
            }
          
        }



        // testing message box
        //private async void messageBox()
        //{
        //    MessageDialog message = new MessageDialog("Date: " + txtDate.Text
        //                                            + "\nOrganisation: " + sendReportObj.name
        //                                            + "\nEmail: " + sendReportObj.organisation_email 
        //                                            + "\nPhone: " + sendReportObj.organisation_phone
        //                                            + "\nLatitude: " + sendReportObj.lat
        //                                            + "\nLongitude: " + sendReportObj.lng
        //                                            + "\nMessage: " + sendReportObj.message
        //                                            + "\nPhoto: " + sendReportObj.photo_id);
        //    await message.ShowAsync();
        //}

        // add report to the db
        public async void addReportDB()
        {
            sendReportObj.date = txtDate.Text;


            // connection string to db
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");

            var Report = new Report()
            {
                name = sendReportObj.name,
                lat = sendReportObj.lat,
                lng = sendReportObj.lng,
                message = sendReportObj.message,
                photo_id = sendReportObj.photo_id,
                organisation_email = sendReportObj.organisation_email,
                organisation_phone = sendReportObj.organisation_phone,
                date = sendReportObj.date
                

            };

            await connection.InsertAsync(Report);


        }

       // private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel desiredCamera)
       // {
       //     DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
       //             .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredCamera);

       //     if (deviceID != null) return deviceID;
       //     else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desiredCamera));
       // }

       //private  async Task CapturePhoto()
       // {

       //     var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);
       //     captureManager = new MediaCapture();

       //     await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
       //     {
       //         StreamingCaptureMode = StreamingCaptureMode.Video,
       //         PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
       //         AudioDeviceId = string.Empty,
       //         VideoDeviceId = cameraID.Id
       //     });
       //     // Get resolutions
       //     var resolutions = captureManager.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Select(x => x as VideoEncodingProperties).ToList();
       //     // get width and height:
       //     uint width = resolutions[0].Width;
       //     uint height = resolutions[0].Height;


       //     // CameraCaptureUI camera = new CameraCaptureUI();

       // }

        #endregion read json create list of organisations get email etc


        #region location

        // need to check if location is enabled
        private async void getLocation()
        {

            try
            {

                // Geolocator is in the Windows.Devices.Geolocation namespace
                geo = new Geolocator();
                 //   geo.DesiredAccuracyInMeters = 10;
                geo.DesiredAccuracy = PositionAccuracy.High;
                
                // await this because we don't know hpw long it will take to complete and we don't want to block the UI
                Geoposition pos = await geo.GetGeopositionAsync(); // get the raw geoposition data
                double lat = pos.Coordinate.Point.Position.Latitude; // current latitude
                double lng = pos.Coordinate.Point.Position.Longitude; // current longitude

                txtLat.Text = lat.ToString();
                txtLng.Text = lng.ToString();
                sendReportObj.lat = lat.ToString();
                sendReportObj.lng = lng.ToString();

            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    MessageDialog msgLocation = new MessageDialog("Location disabled, go to phone settings.");

                    msgLocation.Commands.Add(new UICommand("Ok"));
                    msgLocation.Commands.Add(new UICommand("Cancel"));
                    var result = await msgLocation.ShowAsync();

                    if(result.Label == "Ok")
                    {
                        await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));

                    }
                    if(result.Label == "Cancel")
                    {
                        // 
                        MessageDialog msgLocation2 = new MessageDialog("Location not availible!, exit application?");
                        msgLocation2.Commands.Add(new UICommand("Ok"));
                        msgLocation2.Commands.Add(new UICommand("Cancel"));

                        var result2 = await msgLocation2.ShowAsync();
                        if(result2.Label == "Ok")
                        {
                            Application.Current.Exit();

                        }
                        if(result2.Label == "Cancel")
                        {
                            await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));

                        }
                    }

                }
                else
                {
                    MessageDialog msgLocation = new MessageDialog("Location not availible!, exit application?");


                    await msgLocation.ShowAsync();
                }
            }
        }

        private async void locationSettings()
        {
           await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings"));
        }


       

        #endregion location


        #region filePicker


        private void filePicker()
        {


            //empty string path for imagePath
            ImagePath = string.Empty;
            //create filePicker
            FileOpenPicker filePicker = new FileOpenPicker();
            // point to photo library
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // select view mode for images in library
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;

        }

      
        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {

            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                
                if (args.Files.Count == 0) return;
                
                view.Activated -= viewActivated;

                // storagefile declared at class level. 
                storageFile = args.Files[0];


                // get storagefile path for object
                ImagePath = storageFile.Path;

                var stream = await storageFile.OpenAsync(FileAccessMode.Read);


                await bitmapImage.SetSourceAsync(stream);

                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                myImage.Source = bitmapImage;

            }
        }


       

        // myImage has a tapped event that call the filePicker method to select an image 
        private void myImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            filePicker();
        }


        #endregion filePicker


        #region 



        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            loadObjectDetails();

            email();


          

        }

        // button to open telephony
        private void btnCall_Click(object sender, RoutedEventArgs e)
        {

            loadObjectDetails();
            try
            {
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(sendReportObj.organisation_phone, sendReportObj.name);
            }
            catch (Exception)
            {
                // MessageDialog msgTel = new MessageDialog("Select organisation for phone number"); 
                return;
            }
                       
        }

        #endregion  media capture ???
        // button to open mapview
        private  void btnMap_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null)
            {
                this.Frame.Navigate(typeof(Map));
            }

            //var message = new ChatMessage();
            //message.Recipients.Add("mobile-telephone-number");
            //message.Body = "This is a text message from an app!";
            //await ChatMessageManager.ShowComposeSmsMessageAsync(message);
        }
    }
}




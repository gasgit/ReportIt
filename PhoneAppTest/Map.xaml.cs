using PhoneAppTest.Common;
using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PhoneAppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Map : Page
    {
        private NavigationHelper navigationHelper;
        private string ex = "No connection to network";




        public Map()
        {
            this.InitializeComponent();
            myMap();

            navigationHelper = new NavigationHelper(this);

        }

        

        private async void myMap()
        {

            // create geolocator object 
            Geolocator geolocator = new Geolocator();
            // create geoposition
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
                MessageDialog emd = new MessageDialog(ex.ToString());
            }
            // my badly named map to center using geoposition
            myMaper.Center = geoposition.Coordinate.Point;
            // zoom level on the map
            myMaper.ZoomLevel = 15;

            // create new mapicon object to show on map
            MapIcon mapIcon = new MapIcon();
            // stream and set the icon with png from the assets folser
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/addPhoto.png"));
            // not sure here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            // set the location for the icon
            mapIcon.Location = geoposition.Coordinate.Point;
            // set the meaasge text to 'you are here'
            mapIcon.Title = "You are here";
            // add the icon to the map
            myMaper.MapElements.Add(mapIcon);
            // Set the Zoom Level of the Slider Control
            mySlider.Value = myMaper.ZoomLevel;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {

            this.navigationHelper.OnNavigatedTo(e);
 
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void mySlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (myMaper != null)
                myMaper.ZoomLevel = e.NewValue;
        }
    }
}

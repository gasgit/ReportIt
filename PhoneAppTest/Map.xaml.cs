using PhoneAppTest.Common;
using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
        // private string ex = "No connection to network";

        private Report dataFromSingleReport = new Report();
        Geolocator geolocator;

        public Map()
        {
            this.InitializeComponent();

            navigationHelper = new NavigationHelper(this);
    
        }

        // check which page navigated from
        private bool CheckLastPage(Type desiredPage)
        {
            var lastPage = Frame.BackStack.LastOrDefault();
            return (lastPage != null && lastPage.SourcePageType.Equals(desiredPage)) ? true : false;
        }

       
        protected  async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // passing values from singleReport
            dataFromSingleReport = e.Parameter as Report;

            MyMap.MapServiceToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

           // MyMap.ZoomLevel = 10;
            try
            {
                // Getting Current Location  
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                                            maximumAge: TimeSpan.FromMinutes(5),
                                            timeout: TimeSpan.FromSeconds(10));

                MapIcon mapIcon = new MapIcon();
                // Locate your MapIcon  
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin32.png"));
                // Show above the MapIcon  
               
               if (CheckLastPage(typeof(Reporting)))
                {

                    mapIcon.Title = "Current Location";
                    mapIcon.Location = new Geopoint(new BasicGeoposition()
                    {
                       
                         Latitude = geoposition.Coordinate.Point.Position.Latitude,
                         Longitude = geoposition.Coordinate.Point.Position.Longitude

                    });

                }

                else
                {
                    mapIcon.Title = dataFromSingleReport.message.ToString();

                    mapIcon.Location = new Geopoint(new BasicGeoposition()
                    {
                       
                        Latitude = Convert.ToDouble(dataFromSingleReport.lat),
                        Longitude = Convert.ToDouble(dataFromSingleReport.lng)

                    });

                }
               
                // Positon of the MapIcon  
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MyMap.MapElements.Add(mapIcon);
                // Showing in the Map  
                await MyMap.TrySetViewAsync(mapIcon.Location, 16D, 0, 0, MapAnimationKind.Bow);

                // Set the Zoom Level of the Slider Control  
                mySlider.Value = MyMap.ZoomLevel;
            }
            catch (UnauthorizedAccessException)
            {
                MessageDialog msg = new MessageDialog("Location service is turned off!");
               await msg.ShowAsync();
            }


             this.navigationHelper.OnNavigatedTo(e);
           

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void mySlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (MyMap != null)
                MyMap.ZoomLevel = e.NewValue;
        }
    }
}

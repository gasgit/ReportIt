using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;
using Windows.Graphics.Display;
using ReportIt;





// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PhoneAppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
      
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            createTable();




            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;




        }

        // check if the table already exists
        public async void tableCheck()
        {
            // conect to db
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");


            // try all from sqlite master, if return value 0 create table 
            var result = await connection.QueryAsync<Report>("SELECT name FROM sqlite_master WHERE type='table' AND name='table_name'");
            if (result.Count == 0)
            {

                createTable();

            }

        }

        // reused the layout for Repoting to use with any organisation


        // select council
        private void btnReportCouncil_Click(object sender, RoutedEventArgs e)
        {

            // set subject 
            var obj = App.Current as App;
            obj.subject = "Report to Council ";
            // select relevant file to parse
            var objData = App.Current as App;
            objData.datafile = "Data\\councilJSON.txt";
            // set name on combobox to council
            var org = App.Current as App;
            org.orgName = "Select Council";
            
            // navigate to reporting page
            if (Frame != null)
            {
                this.Frame.Navigate(typeof(Reporting));
            }
        }

        private void btnReportIspca_Click(object sender, RoutedEventArgs e)
        {
            // set subject to ISPCA
            var obj = App.Current as App;
            obj.subject = "Report to ISPCA \n";
            // select relevent file to parse
            var objData = App.Current as App;
            objData.datafile = "Data\\ispcaJSON.txt";

            // set text on combobox to ISPCA
            var org = App.Current as App;
            org.orgName = "Select ISPCA/SPCA";
            // navigate to Reporting
            if (Frame != null)
            {
                this.Frame.Navigate(typeof(Reporting));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");




        

        //public async Task<bool> DoesDbExist(string ReportIt)
        //{
        //    bool dbexist = true;
        //    try
        //    {
        //        StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(ReportIt);

        //    }
        //    catch
        //    {
        //        dbexist = false;
        //    }

        //    return dbexist;
        //}

    

       
      
    // table creation in db
    public async void createTable()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");
            await connection.CreateTableAsync<Report>();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            createTable();
        }

       
      


        private void btnMyReports_Click(object sender, RoutedEventArgs e)
        {

            if (Frame != null)
            {
                this.Frame.Navigate(typeof(myReports));
            }

        }

        private void btnPrivacy_Click(object sender, RoutedEventArgs e)
        {

            if (Frame != null)
            {
                this.Frame.Navigate(typeof(Privacy));
            }

        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null)
            {
                this.Frame.Navigate(typeof(About));
            }

        }
    }
}


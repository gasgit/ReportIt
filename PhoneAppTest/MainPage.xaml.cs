using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;





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
          

        }
        public async void tableCheck()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("ReportIt.db");



            var result = await connection.QueryAsync<Report>("SELECT name FROM sqlite_master WHERE type='table' AND name='table_name'");
            if (result.Count == 0)
            {

                createTable();
            }

        }



        private void btnReportCouncil_Click(object sender, RoutedEventArgs e)
        {


            var obj = App.Current as App;
            obj.subject = "Report to Council ";

            var objData = App.Current as App;
            objData.datafile = "Data\\councilJSON.txt";

            var org = App.Current as App;
            org.orgName = "Select Council";
            

            if (Frame != null)
            {
                this.Frame.Navigate(typeof(Reporting));
            }
        }

        private void btnReportIspca_Click(object sender, RoutedEventArgs e)
        {
            var obj = App.Current as App;
            obj.subject = "Report to ISPCA \n";

            var objData = App.Current as App;
            objData.datafile = "Data\\ispcaJSON.txt";


            var org = App.Current as App;
            org.orgName = "Select ISPCA/SPCA";

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

        public async Task<bool> DoesDbExist(string ReportIt)
        {
            bool dbexist = true;
            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(ReportIt);

            }
            catch
            {
                dbexist = false;
            }

            return dbexist;
        }

    

       
      

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
    }
}


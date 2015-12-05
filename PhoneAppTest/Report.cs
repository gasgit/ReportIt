using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneAppTest
{

    /// <summary>
    ///  my report class to use for object creation with all the usual get sets
    /// </summary>
    /// 
    [Table ("Reports")]
    class Report
    {
        [PrimaryKey, AutoIncrement]
        public int uid { get; set; }
        public string name { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string message { get; set; }
        public string photo_id { get; set; }
        public string organisation_email { get; set; }
        public string organisation_phone { get; set; }
        public string date { get; set; }

    }

   

    
}

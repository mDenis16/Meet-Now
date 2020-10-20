using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace app.cache {

    public static class voicechannel {
        public class model {
            public int id { get; set; }
            public string uuid { get; set; }
       
        

        }
        public static List<model> list = new List<model> ();

  
    }
}
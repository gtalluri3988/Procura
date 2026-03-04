using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class ZBAPI_CREATEVENDORResponse
    {
        public BAPIRET2[] RETURN { get; set; }
    }

    public class BAPIRET2
    {
        public string TYPE { get; set; }
        public string ID { get; set; }
        public string VENDORCODE { get; set; }
        public string MESSAGE { get; set; }
    }


    public class ZBAPI_CHANGEVENDORResponse
    {
        public BAPIRET2[] RETURN { get; set; }
    }
}

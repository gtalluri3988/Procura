using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ZBAPI_CREATEVENDORClient : IZBAPI_CREATEVENDORClient
    {
        public ZBAPI_CREATEVENDORResponse Execute(SAP_ZBAPI_CREATEVENDOR request)
        {
            var random = new Random();
            int code = random.Next(100000, 999999);
            return new ZBAPI_CREATEVENDORResponse
            {
                RETURN = new[]
                {
                new BAPIRET2
                {
                    TYPE = "S",
                    ID = "ZVENDOR",
                    VENDORCODE = code.ToString(),
                    MESSAGE =  $"Vendor {code.ToString()} Created (SIMULATED)"
                }
            }
            };
        }


        public ZBAPI_CHANGEVENDORResponse Execute(SAP_ZBAPI_CHANGEVENDOR request)
        {
            var random = new Random();
            int code = random.Next(100000, 999999);
            return new ZBAPI_CHANGEVENDORResponse
            {
                RETURN = new[]
                {
                new BAPIRET2
                {
                    TYPE = "S",
                    ID = "ZVENDOR",
                    VENDORCODE = code.ToString(),
                    MESSAGE =  $"Vendor Details updated successfully"
                }
            }
            };
        }
    }
}

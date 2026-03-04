using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class RealZBAPI_CREATEVENDORClient : IZBAPI_CREATEVENDORClient
    {
        public ZBAPI_CREATEVENDORResponse Execute(SAP_ZBAPI_CREATEVENDOR request)
        {
            var client = new ZBAPI_CREATEVENDORClient();
            return client.Execute(request);
        }

        public ZBAPI_CHANGEVENDORResponse Execute(SAP_ZBAPI_CHANGEVENDOR request)
        {
            var client = new ZBAPI_CREATEVENDORClient();
            return client.Execute(request);
        }
    }
}

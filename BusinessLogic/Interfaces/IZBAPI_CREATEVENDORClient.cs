using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IZBAPI_CREATEVENDORClient
    {
        ZBAPI_CREATEVENDORResponse Execute(SAP_ZBAPI_CREATEVENDOR request);
        ZBAPI_CHANGEVENDORResponse Execute(SAP_ZBAPI_CHANGEVENDOR request);
    }
}

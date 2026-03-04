using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public enum Roles
    {
        [Display(Name = "System Admin")]
        SystemAdmin = 1,
        [Display(Name = "Business User")]
        BusinessUser = 2,
        [Display(Name = "Approver")]
        Approver = 3,
        [Display(Name = "Vendor")]
        Vendor = 4,
        [Display(Name = "Bidder")]
        Bidder = 5,
        [Display(Name = "Resident User Admin")]
        ResidentUserAdmin =6,

    }

    public enum PaymentTypeEnum
    {
        Facility = 26,
        Parking = 3,
        Maintenance = 2
    }

   

}

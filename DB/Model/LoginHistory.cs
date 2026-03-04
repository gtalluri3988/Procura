using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Ip { get; set; }
        public string UserName { get; set; }
        public string? RecaptchaScore { get; set; }
        public string? Response { get; set; }
        public DateTime? JwtTokenExpiryDate { get; set; }
        public bool Online { get; set; }
        public string? DataResponse { get; set; }
        
    }
}

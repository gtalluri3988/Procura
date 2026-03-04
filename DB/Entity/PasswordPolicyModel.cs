using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class PasswordPolicyModel
    {
      
            public int MinimumPasswordLength { get; set; }

            public int MinimumNumericCharacters { get; set; }

            public int MinimumAlphaCharacters { get; set; }

            public int MinimumUppercaseCharacters { get; set; }

            public int MinimumLowercaseCharacters { get; set; }
            public int PassWordHistory { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorDeclaration:BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public bool EsqQuestionnaireAccepted { get; set; }
        public bool ConfidentialityAgreementAccepted { get; set; }
        public bool PoTermsAccepted { get; set; }
        public bool CodeOfConductAccepted { get; set; }
        public bool PdpAAccepted { get; set; }
        public bool EnvironmentalPolicyAccepted { get; set; }
        public bool NoGiftPolicyAccepted { get; set; }
        public bool IntegrityDeclarationAccepted { get; set; }
        public bool FinalDeclarationAccepted { get; set; }

        //public Vendor? Vendor { get; set; }
    }
}

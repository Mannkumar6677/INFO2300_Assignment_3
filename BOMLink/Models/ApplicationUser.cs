using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BOMLink.Models {
    public class ApplicationUsers : IdentityUser {
        #region Properties
        // Full Name
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //// Role Relationship
        //public string RoleId { get; set; }
        //public Role Role { get; set; }

        // Navigation properties
        public ICollection<BOM> BOMs { get; set; } = new List<BOM>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<RFQ> RFQs { get; set; } = new List<RFQ>();
        public ICollection<PO> POs { get; set; } = new List<PO>();

        #endregion
    }
}

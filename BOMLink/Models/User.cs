//using Microsoft.AspNetCore.Identity;

//namespace BOMLink.Models {
//    public class User {
//        #region Properties
//        public int UserId { get; set; }
//        public string Username { get; set; }
//        public string HashedPassword { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public int RoleId { get; set; }
//        public Role Role { get; set; }

//        // Navigation property for BOMs
//        public ICollection<BOM> BOMs { get; set; } = new List<BOM>();

//        // Navigation property for Jobs
//        public ICollection<Job> Jobs { get; set; } = new List<Job>();

//        // Navigation property for RFQ
//        public ICollection<RFQ> RFQs { get; set; } = new List<RFQ>();

//        // Navigation property for POs
//        public ICollection<PO> POs { get; set; } = new List<PO>();
//        #endregion
//    }
//}

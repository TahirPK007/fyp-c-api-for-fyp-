//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace fyp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class juniorDoctor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public juniorDoctor()
        {
            this.acceptCases = new HashSet<acceptCase>();
            this.histories = new HashSet<history>();
        }
    
        public int jrdoc_id { get; set; }
        public string full_name { get; set; }
        public string father_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string dob { get; set; }
        public string contact { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<acceptCase> acceptCases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<history> histories { get; set; }
    }
}

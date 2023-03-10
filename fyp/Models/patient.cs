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
    
    public partial class patient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public patient()
        {
            this.acceptCases = new HashSet<acceptCase>();
            this.histories = new HashSet<history>();
        }
    
        public int patient_id { get; set; }
        public Nullable<int> cnic { get; set; }
        public string full_name { get; set; }
        public string relation { get; set; }
        public string relative_name { get; set; }
        public string dob { get; set; }
        public string gender { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<acceptCase> acceptCases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<history> histories { get; set; }
    }
}

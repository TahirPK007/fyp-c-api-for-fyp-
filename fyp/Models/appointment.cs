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
    
    public partial class appointment
    {
        public int appointment_id { get; set; }
        public Nullable<int> patient_id { get; set; }
        public Nullable<int> jrdoc_id { get; set; }
        public Nullable<double> rating { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> srdoc_id { get; set; }
        public Nullable<int> visit_id { get; set; }
        public Nullable<int> shown { get; set; }
        public Nullable<int> nurseID { get; set; }
    }
}

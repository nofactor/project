//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhungNoiProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FeedBack
    {
        public int fbID { get; set; }
        public Nullable<System.DateTime> fbTime { get; set; }
        public string fbDetails { get; set; }
        public Nullable<int> memberID { get; set; }
    
        public virtual Member Member { get; set; }
    }
}

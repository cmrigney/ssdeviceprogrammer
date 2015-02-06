using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerPortal.Models
{
    // Models returned by MeController actions.
    public class GetViewModel
    {
        public string CustomerCode { get; set; }
    }
}
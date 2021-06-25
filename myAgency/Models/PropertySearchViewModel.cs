using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace myAgency.Models
{
    public class PropertySearchViewModel
    {
        [Display(Name="Surface minimale :")]
        public int minSurface { get; set; }

        [Display(Name = "Prix maximal :")]
        public int maxPrice { get; set; }

        public IList<int> options { get; set; }

    }
}
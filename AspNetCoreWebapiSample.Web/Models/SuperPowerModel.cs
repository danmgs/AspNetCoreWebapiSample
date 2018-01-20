using System;
using System.ComponentModel.DataAnnotations;


namespace AspNetCoreWebapiSample.Web.Models
{
    public class SuperPowerModel
    {        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}

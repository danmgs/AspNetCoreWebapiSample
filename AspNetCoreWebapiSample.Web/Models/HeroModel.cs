using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebapiSample.Web.Models
{
    public class HeroModel
    {
        public int Id { get; set; }
        [Required]
        public int Code { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        public int SuperPowerId { get; set; }
        
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }        
    }
}

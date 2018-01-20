using AspNetCoreWebapiSample.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreWebapiSample.Domain.Entities
{
    public class Hero : EntityBase
    {        
        [Required]        
        public int Code { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public int SuperPowerId { get; set; }


        [ForeignKey("SuperPowerId")]
        public SuperPower SuperPower { get; set; }
        

    }
}

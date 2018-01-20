
using AspNetCoreWebapiSample.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebapiSample.Domain.Entities
{
    public class SuperPower : EntityBase
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

    }
}

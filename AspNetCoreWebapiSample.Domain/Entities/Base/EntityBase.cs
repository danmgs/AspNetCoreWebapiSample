using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebapiSample.Domain.Entities.Base
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public DateTime UpdateDate { get; set; }        

    }
}

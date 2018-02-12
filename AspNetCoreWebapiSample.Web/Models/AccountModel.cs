using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebapiSample.Web.Models
{
    public class RegisterAccountModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role{ get; set; }

        [Required]
        [MinLength(length: 6)]                
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }


    public class AuthenticateModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]        
        public string Password { get; set; }

    }

}

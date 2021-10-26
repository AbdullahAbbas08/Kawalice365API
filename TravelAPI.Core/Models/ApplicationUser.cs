using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAPI.Models
{
    public class ApplicationUser :IdentityUser
    {
        [Required,StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(255)]
        public string LastName { get; set; }   
        [Required]
        public string  Address { get; set; }
        public string  Description { get; set; }
        [Required]
        public string  LogoPath { get; set; }
    }
}

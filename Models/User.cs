using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class User
    {
        [Key]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
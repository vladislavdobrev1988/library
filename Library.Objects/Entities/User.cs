using Library.Objects.Attributes;
using Library.Objects.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Library.Objects.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [DefaultStringLength]
        public string Email { get; set; }

        [Required]
        [DefaultStringLength]
        public string FirstName { get; set; }

        [Required]
        [DefaultStringLength]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string PasswordHash { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using Library.Objects.Entities.Base;

namespace Library.Objects.Entities
{
    public class AccessToken : BaseEntity
    {
        [Required]
        [StringLength(1000)]
        public string Token { get; set; }

        [Required]
        public DateTime Expires { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

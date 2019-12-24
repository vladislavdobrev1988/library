using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Library.Objects.Attributes;
using Library.Objects.Entities.Base;

namespace Library.Objects.Entities
{
    public class Author : BaseEntity
    {
        [Required]
        [DefaultStringLength]
        public string FirstName { get; set; }

        [Required]
        [DefaultStringLength]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public IList<Book> Books { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using Library.Objects.Attributes;
using Library.Objects.Entities.Base;

namespace Library.Objects.Entities
{
    public class Book : BaseEntity
    {
        [Required]
        [DefaultStringLength]
        public string Title { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}

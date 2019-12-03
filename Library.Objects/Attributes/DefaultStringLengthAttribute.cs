using System.ComponentModel.DataAnnotations;

namespace Library.Objects.Attributes
{
    public class DefaultStringLengthAttribute : StringLengthAttribute
    {
        public DefaultStringLengthAttribute() : base(100) { }
    }
}

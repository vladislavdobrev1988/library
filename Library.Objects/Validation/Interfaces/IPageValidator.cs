using Library.Objects.Helpers.Request;

namespace Library.Objects.Validation.Interfaces
{
    public interface IPageValidator
    {
        string Validate(PageRequest request);
    }
}

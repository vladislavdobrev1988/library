using System.Threading.Tasks;

namespace Library.Objects.Services.Interfaces
{
    public interface ITextAppender
    {
        Task AppendTextAsync(string text);
    }
}

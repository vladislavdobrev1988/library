using System;
using System.Threading.Tasks;

namespace Library.Objects.Services.Interfaces
{
    public interface IExceptionLogger
    {
        Task LogAsync(Exception exception);
    }
}

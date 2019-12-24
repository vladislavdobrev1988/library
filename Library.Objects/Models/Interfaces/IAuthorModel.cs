﻿using System.Threading.Tasks;
using Library.Objects.Helpers.Response;
using Library.Objects.Proxies;

namespace Library.Objects.Models.Interfaces
{
    public interface IAuthorModel
    {
        Task<IdResponse> CreateAuthorAsync(AuthorProxy author);
        Task<AuthorProxy> GetAuthorByIdAsync(int id);
    }
}

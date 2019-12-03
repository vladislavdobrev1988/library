﻿using Library.Objects.Proxies;
using System.Threading.Tasks;

namespace Library.Objects.Models.Interfaces
{
    public interface IUserModel
    {
        Task SaveAsync(UserProxy user);
    }
}

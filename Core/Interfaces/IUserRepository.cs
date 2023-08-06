using System;
using Core.Models;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        public User GetUser(string username);
    }
}


using System;
using Core.DTOs;
using Core.Models;

namespace Core.Interfaces
{
    public interface ILoginRepository
    {
        public bool IsValidLogin(LoginRequest login, User user);

        public LoginSuccess GetJwt(User user);
    }
}


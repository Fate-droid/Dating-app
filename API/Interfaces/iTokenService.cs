using System;
using API.Entities;

namespace API.Interfaces;

public interface iTokenService
{
    Task<string> CreateToken(AppUser user);

    string GenerateRefreshToken(); 
}

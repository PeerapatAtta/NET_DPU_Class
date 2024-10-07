﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MonolithAPI.Models;

namespace MonolithAPI.Helpers;

public class TokenHelper
{
    private readonly IConfigurationSection jwtSettings;
    private readonly IConfigurationSection refreshTokenSettings;
    private readonly UserManager<UserModel> userManager;

    public TokenHelper(IConfiguration configuration, UserManager<UserModel> userManager)
    {
        this.jwtSettings = configuration.GetSection("JwtSettings");
        this.refreshTokenSettings = configuration.GetSection("RefreshTokenSettings");
        this.userManager = userManager;
    }

    public async Task<string> CreateJwtToken(UserModel user)
    {
        var signingCredentials = CreateSigningCredentials();
        var claims = await CreateClaims(user);
        var jwtSecurityToken = CreatetJwtSecurityToken(signingCredentials, claims);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    public string CreateRefreshToken()
    {
        var rendomNumber = new byte[Convert.ToInt32(refreshTokenSettings["TokenLength"])];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(rendomNumber);
        }
        return Convert.ToBase64String(rendomNumber);
    }

    public async Task<(string AccessToken, string RefreshToken)> CreateToken(UserModel user, bool populateExp = true)
    {
        var accessToken = await CreateJwtToken(user);
        user.RefreshToken = CreateRefreshToken();

        if (populateExp)
        {
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenSettings["ExpiryInMinutes"]));
        }

        await userManager.UpdateAsync(user);

        return (accessToken, user.RefreshToken);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> CreateClaims(UserModel user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.GivenName, user.FirstName + " " + user.LastName),
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken CreatetJwtSecurityToken(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: jwtSettings["ValidIssuer"],
            audience: jwtSettings["ValidAudience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            signingCredentials: signingCredentials
        );
        return token;
    }

}

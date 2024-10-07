using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonolithAPI.DTOs.Reponse;
using MonolithAPI.DTOs.Request;
using MonolithAPI.Helpers;
using MonolithAPI.Models;

namespace MonolithAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<UserModel> userManager;
    private readonly TokenHelper tokenHelper;

    public AccountsController(UserManager<UserModel> userManager, TokenHelper tokenHelper)
    {
        this.userManager = userManager;
        this.tokenHelper = tokenHelper;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDTO request)
    {
        // convert request into user model
        var newUser = new UserModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        // create new user
        var result = await userManager.CreateAsync(newUser, request.Password!);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);
            return BadRequest(new { Errors = errors });
        }

        // assign user role
        try
        {
            await userManager.AddToRoleAsync(newUser, request.Role!);
        }
        catch (Exception ex)
        {
            await userManager.DeleteAsync(newUser);
            var errors = new[] { ex.Message };
            return BadRequest(new { Errors = errors });
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginUser(LoginUserDTO request)
    {
        // find user by email
        var user = await userManager.FindByEmailAsync(request.Email!);
        // check user and password
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password!))
        {
            var errors = new[] { "Invalid email or password." };
            return Unauthorized(new { Errors = errors });
        }
        // create new token
        var token = await tokenHelper.CreateToken(user);

        return Ok(new TokenResultDTO { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });
    }

}

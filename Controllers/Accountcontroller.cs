using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;


[ApiController]
public class Accountcontroller : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post([FromBody] RegisterViewModel model,
                                          [FromServices] EmailService emailService,
                                          [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email
                    .Replace("@", "-")
                    .Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email, subject: "Bem vindo ao blog!", body: $"Sua senha:<strong>{password}</strong>");

            return Ok(new ResultViewModel<dynamic>(new { user = user.Email }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<string>("04Z01 - Ocorreu um erro ao criar este usuário!"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("04Z99 - Um erro inesperado ocorreu!"));
        }
    }


    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model,
                                           [FromServices] BlogDataContext context,
                                           [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user is null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválido!"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválido!"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("04Z98 - Um erro inesperado ocorreu!"));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model,
                                                 [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}_avatar.jpg";
        var data = new Regex(@"^data:image V [a-z]+;base64,").Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("04Z97 - Um erro inesperado ocorreu!"));
        }

        var user = await context.Users
                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<User>("Usuário não encontrado!"));

        user.Image = $"https://localhost:0000/images/{fileName}";

        try
        {
            context.Update(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("04Z96 - Um erro inesperado ocorreu!"));
        }
    }
}
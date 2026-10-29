using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
        {
            return BadRequest("Le login est obligatoire.");
        }

        var client = _httpClientFactory.CreateClient("GestionOrdinateursApi");
        var response = await client.GetAsync(
            $"api/Users/{Uri.EscapeDataString(request.Login)}",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Unauthorized("Cet utilisateur n'existe pas.");
        }

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>(
            cancellationToken);

        if (user is null)
        {
            return Unauthorized("Cet utilisateur n'existe pas.");
        }

        var keyValue = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyValue))
        {
            throw new InvalidOperationException(
                "La configuration Jwt:Key est obligatoire.");
        }

        var expires = DateTime.UtcNow.AddMinutes(15);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("userId", user.Id.ToString())
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return Ok(new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            new DateTimeOffset(expires, TimeSpan.Zero)));
    }
}

public sealed record LoginRequest(string Login);

public sealed record TokenResponse(string Token, DateTimeOffset Expires);

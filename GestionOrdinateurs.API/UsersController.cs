using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IDbContextFactory<ParcInformatiqueContext> _contextFactory;

    public UsersController(
        IDbContextFactory<ParcInformatiqueContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<User>>> GetAll(
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(
            cancellationToken);

        return Ok(await context.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .ToListAsync(cancellationToken));
    }

    [HttpGet("{login}")]
    public async Task<ActionResult<User>> GetByLogin(
        string login,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(
            cancellationToken);

        var user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                candidate => candidate.Login == login,
                cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/bus-routes")]
[Authorize]
public sealed class BusRoutesController : ControllerBase
{
    private readonly IBusRouteRepository _routes;

    public BusRoutesController(IBusRouteRepository routes) => _routes = routes;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BusRouteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await _routes.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster},{Roles.Accountant}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBusRouteRequest request, CancellationToken ct)
    {
        var id = await _routes.InsertAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }
}

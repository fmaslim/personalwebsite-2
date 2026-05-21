using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers;

[ApiController]
[Route("api/v2")]
public class PersonsV2Controller : ControllerBase
{
    private readonly IPersonV2Service _personService;

    public PersonsV2Controller(IPersonV2Service personService)
    {
        _personService = personService;
    }

    [HttpGet("SearchPersons")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponseDto<PersonSearchDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchPersons(
        [FromQuery] string? name,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        var result = await _personService.SearchPersonsAsync(name ?? string.Empty, sortBy ?? string.Empty, sortDir ?? string.Empty, pageNumber, pageSize);
        return result.ToActionResult();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Controllers
{
    [Route("api/personnel")]
    [ApiController]
    public class PersonnelController : ControllerBase
    {
        private readonly IPersonnelService _personnelService;

        public PersonnelController(IPersonnelService personnelService)
        {
            _personnelService = personnelService;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<PagedResult<PersonnelDto>>> GetPersonnelAsync([FromQuery] GetPersonnelQuery query, CancellationToken ct)
        {
            if (query.PageNumber <= 0 || query.PageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            if (query.PageSize > 100)
            {
                return BadRequest("Page size must be less than or equal to 100.");
            }
            if (query.FacilityId <= 0)
            {
                return BadRequest("FacilityId must be greater than zero.");
            }
            var result = await _personnelService.GetPersonnelAsync(query, ct);
            return Ok(result);
        }
    }
}

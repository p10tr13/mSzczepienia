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
        private readonly ILogger<PersonnelController> _logger;

        public PersonnelController(IPersonnelService personnelService, ILogger<PersonnelController> logger)
        {
            _personnelService = personnelService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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

        [HttpGet("search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PagedResult<PersonnelDto>>> SearchPersonnelAsync([FromQuery] SearchPersonnelQuery query, CancellationToken ct)
        {
            if (query.Page <= 0 || query.PageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            if (query.PageSize > 100)
            {
                return BadRequest("Page size must be less than or equal to 100.");
            }

            try
            {
                var result = await _personnelService.SearchPersonnelAsync(query, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for personnel.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<DetailedPersonnelDto>> GetDetailedPersonnelByIdAsync([FromRoute] int id, CancellationToken ct)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be greater than zero.");
            }
            try
            {
                var result = await _personnelService.GetDetailedPersonnelByIdAsync(id, ct);
                return Ok(result);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Personnel with ID {Id} not found.", id);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving personnel details for ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}

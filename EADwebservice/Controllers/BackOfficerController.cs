using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class BackOfficerController : ControllerBase
{
    private readonly BackOfficerService _backOfficerService;

    public BackOfficerController(BackOfficerService backOfficerService)
    {
        _backOfficerService = backOfficerService;
    }

    // GET: api/backofficer
    [HttpGet]
    public async Task<IActionResult> GetAllBackOfficers()
    {
        var backOfficers = await _backOfficerService.GetAllBackOfficersAsync();
        return Ok(backOfficers);
    }

    // GET: api/backofficer/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBackOfficerById(string id)
    {
        var backOfficer = await _backOfficerService.GetBackOfficerByIdAsync(id);
        if (backOfficer == null)
        {
            return NotFound("BackOfficer not found");
        }
        return Ok(backOfficer);
    }

    // POST: api/backofficer
    [HttpPost]
    public async Task<IActionResult> CreateBackOfficer([FromBody] BackOfficer backOfficer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _backOfficerService.CreateBackOfficerAsync(backOfficer);
        return CreatedAtAction(nameof(GetBackOfficerById), new { id = backOfficer.Id }, backOfficer);
    }

    // PUT: api/backofficer/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBackOfficer(string id, [FromBody] BackOfficer updatedBackOfficer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingBackOfficer = await _backOfficerService.GetBackOfficerByIdAsync(id);
        if (existingBackOfficer == null)
        {
            return NotFound("BackOfficer not found");
        }

        await _backOfficerService.UpdateBackOfficerAsync(id, updatedBackOfficer);
        return Ok(updatedBackOfficer);
    }

    // DELETE: api/backofficer/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBackOfficer(string id)
    {
        var existingBackOfficer = await _backOfficerService.GetBackOfficerByIdAsync(id);
        if (existingBackOfficer == null)
        {
            return NotFound("BackOfficer not found");
        }

        await _backOfficerService.DeleteBackOfficerAsync(id);
        return Ok("BackOfficer deleted successfully");
    }

    // POST: api/backofficer/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] BackOfficerLoginDto loginDto)
    {
        // Authenticate the back officer using the username and password
        var backOfficer = await _backOfficerService.AuthenticateBackOfficerAsync(loginDto.Username, loginDto.Password);

        if (backOfficer == null)
        {
            return Unauthorized("Invalid username or password");
        }

        // Return back officer details including role, username, and id (without IsActive)
        var response = new
        {
            Id = backOfficer.Id,
            Username = backOfficer.Username,
            Role = backOfficer.Role
        };

        return Ok(response);
    }

}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class VendorController : ControllerBase
{
    private readonly VendorService _vendorService;

    public VendorController(VendorService vendorService)
    {
        _vendorService = vendorService;
    }

    // GET: api/vendor - Get all vendors
    [HttpGet]
    public async Task<IActionResult> GetAllVendors()
    {
        var vendors = await _vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }

    // GET: api/vendor/{id} - Get a specific vendor by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVendorById(string id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        if (vendor == null) return NotFound("Vendor not found");
        return Ok(vendor);
    }

    // POST: api/vendor - Create a new vendor (only Admin should be allowed)
    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] Vendor vendor)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _vendorService.CreateVendorAsync(vendor);
        return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
    }

    // PUT: api/vendor/{id}/comment - Add or update a comment and ranking
    [HttpPut("{id}/comment")]
    public async Task<IActionResult> AddOrUpdateComment(string id, [FromBody] Comment comment)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        if (vendor == null) return NotFound("Vendor not found");

        await _vendorService.AddOrUpdateCommentAsync(id, comment);
        return Ok("Comment added or updated successfully");
    }

    // DELETE: api/vendor/{id} - Delete a specific vendor by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVendor(string id)
    {
        try
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound("Vendor not found");

            await _vendorService.DeleteVendorAsync(id);
            return Ok(new { message = "Vendor deleted successfully" });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}

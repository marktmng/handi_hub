using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistController(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        // ✅ GET artists (all or by ID) — returns ArtistDto (includes user details)
        [Authorize]
        [HttpGet("Getartists")]
        public async Task<IActionResult> GetArtists([FromQuery] int? artistId = null)
        {
            var artists = await _artistRepository.GetArtistsAsync(artistId);
            return Ok(artists);
        }

        // ✅ POST create or update artist
        [Authorize]
        [HttpPost("UpsertArtist")]
        public async Task<IActionResult> UpsertArtistAsync([FromBody] Artist artist)
        {
            if (artist == null)
                return BadRequest("Artist is null.");

            var result = await _artistRepository.UpsertArtistAsync(artist);

            if (result != null && result.ArtistId.HasValue)
                return Ok(result); // returns basic Artist model (no user details)

            return BadRequest("Failed to process artist data.");
        }

        // ✅ PUT update existing artist by ID
        [HttpPut("Update/{artistId}")]
        [Authorize]
        public async Task<IActionResult> UpdateArtist(int artistId, [FromBody] Artist artist)
        {
            if (artist == null || artist.ArtistId != artistId)
                return BadRequest("Artist ID mismatch or artist is null.");

            var updated = await _artistRepository.UpsertArtistAsync(artist);

            if (updated != null && updated.ArtistId.HasValue)
                return Ok(updated);

            return BadRequest("Failed to update artist.");
        }

        // ✅ DELETE artist by ID
        [HttpDelete("Delete/{artistId}")]
        [Authorize]
        public async Task<IActionResult> DeleteArtist(int artistId)
        {
            var deleted = await _artistRepository.DeleteArtistAsync(artistId);

            if (deleted)
                return Ok($"Artist with ID {artistId} deleted successfully.");

            return NotFound($"Artist with ID {artistId} was not found.");
        }
    }
}

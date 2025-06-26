using DotnetAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetArtistsAsync(int? artistId = null); // Removed genre
        Task<Artist> UpsertArtistAsync(Artist artist);
        Task<bool> DeleteArtistAsync(int artistId);
    }

}

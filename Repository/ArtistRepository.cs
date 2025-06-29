using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Repository
{
    public class ArtistRepository : IArtistRepository // Interface for artist repository
    {
        private readonly DataContext _context; // DataContext to access the database

        public ArtistRepository(DataContext context)
        {
            _context = context;
        }

        // ✅ Get artists with optional filter
        public async Task<IEnumerable<ArtistDto>> GetArtistsAsync(int? artistId = null)
        {
            var artistIdParam = new SqlParameter("@ArtistId", artistId.HasValue ? artistId.Value : (object)DBNull.Value);

            return await _context.ArtistDtos
                .FromSqlRaw("EXEC HandiHub.spArtists_Get @ArtistId", artistIdParam)
                .ToListAsync();
        }

        // ✅ Insert or update artist
        public async Task<Artist> UpsertArtistAsync(Artist artist)
        {
            var artistIdParam = new SqlParameter("@ArtistId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = artist.ArtistId ?? (object)DBNull.Value
            };

            var userIdParam = new SqlParameter("@UserId", artist.UserId);
            var bioParam = new SqlParameter("@Bio", artist.Bio ?? (object)DBNull.Value);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spArtists_Upsert @ArtistId OUTPUT, @UserId, @Bio",
                artistIdParam, userIdParam, bioParam);

            artist.ArtistId = (int?)artistIdParam.Value;
            return artist;
        }

        // ✅ Delete artist by ID
        public async Task<bool> DeleteArtistAsync(int artistId)
        {
            var artistIdParam = new SqlParameter("@ArtistId", artistId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spArtists_Delete @ArtistId, @RowsAffected OUTPUT",
                artistIdParam, rowsAffectedParam);

            return (int)rowsAffectedParam.Value > 0;
        }
    }
}

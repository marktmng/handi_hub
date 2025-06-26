using DotnetAPI.Data;
using DotnetAPI.Models;
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

        public async Task<IEnumerable<Artist>> GetArtistsAsync(int? artistId = null)
        {
            var parameters = new List<SqlParameter>();

            var artistIdParam = new SqlParameter("@ArtistId", artistId.HasValue ? artistId.Value : (object)DBNull.Value);

            parameters.Add(artistIdParam);

            string sql = "EXEC HandiHub.spArtists_Get @ArtistId";

            return await _context.Artists
                .FromSqlRaw(sql, parameters.ToArray())
                .ToListAsync();
        }

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

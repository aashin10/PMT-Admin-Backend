using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class BoardBoardColumnMapRepository : GenericRepository<BoardBoardColumnMap>, IBoardBoardColumnMapRepository
    {
        private readonly AppDbContext _context;

        public BoardBoardColumnMapRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BoardBoardColumnMap>> CreateMultipleMappingsAsync(List<BoardBoardColumnMap> mappings)
        {
            await _context.BoardBoardColumnMaps.AddRangeAsync(mappings);
            await _context.SaveChangesAsync();
            return mappings;
        }

        public async Task<IReadOnlyList<BoardBoardColumnMap>> GetMappingsByBoardIdAsync(int boardId)
        {
            return await _context.BoardBoardColumnMaps
                .Where(m => m.BoardId == boardId)
                .Include(m => m.Board)
                .Include(m => m.BoardColumn)
                .ToListAsync();
        }
    }
}
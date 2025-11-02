using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class BoardColumnRepository : GenericRepository<BoardColumn>, IBoardColumnRepository
    {
        private readonly AppDbContext _context;

        public BoardColumnRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BoardColumn>> GetColumnsByBoardIdAsync(int boardId)
        {
            return await _context.BoardBoardColumnMaps
                .Where(m => m.BoardId == boardId)
                .Include(m => m.BoardColumn)
                .Select(m => m.BoardColumn!)
                .ToListAsync();
        }

        public async Task<List<BoardColumn>> CreateMultipleAsync(List<BoardColumn> columns)
        {
            await _context.BoardColumns.AddRangeAsync(columns);
            await _context.SaveChangesAsync();
            return columns;
        }
    }
}
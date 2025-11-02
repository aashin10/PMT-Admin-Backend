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
    public class BoardRepository : GenericRepository<Board>, IBoardRepository
    {
        private readonly AppDbContext _context;

        public BoardRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Board?> GetBoardByProjectIdAsync(Guid projectId)
        {
            return await _context.Boards
                .Include(b => b.Project)
                .Include(b => b.Team)
                .Where(b => b.ProjectId == projectId)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Board>> GetBoardsByProjectIdAsync(Guid projectId)
        {
            return await _context.Boards
                .Include(b => b.Project)
                .Include(b => b.Team)
                .Where(b => b.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<Board> CreateBoardWithColumnsAsync(Board board, List<BoardColumn> columns)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create the board first
                var createdBoard = await CreateAsync(board);
                
                // Create the columns
                foreach (var column in columns)
                {
                    await _context.BoardColumns.AddAsync(column);
                }
                await _context.SaveChangesAsync();

                // Create the mappings
                var mappings = columns.Select(column => new BoardBoardColumnMap
                {
                    BoardId = createdBoard.Id,
                    BoardColumnId = column.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                foreach (var mapping in mappings)
                {
                    await _context.BoardBoardColumnMaps.AddAsync(mapping);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return createdBoard;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
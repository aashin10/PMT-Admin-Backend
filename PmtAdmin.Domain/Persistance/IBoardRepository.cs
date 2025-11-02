using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IBoardRepository : IGenericRepository<Board>
    {
        Task<Board?> GetBoardByProjectIdAsync(Guid projectId);
        Task<IReadOnlyList<Board>> GetBoardsByProjectIdAsync(Guid projectId);
        Task<Board> CreateBoardWithColumnsAsync(Board board, List<BoardColumn> columns);
    }
}
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IBoardColumnRepository : IGenericRepository<BoardColumn>
    {
        Task<IReadOnlyList<BoardColumn>> GetColumnsByBoardIdAsync(int boardId);
        Task<List<BoardColumn>> CreateMultipleAsync(List<BoardColumn> columns);
    }
}
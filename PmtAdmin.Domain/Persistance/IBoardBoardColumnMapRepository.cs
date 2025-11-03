using PmtAdmin.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IBoardBoardColumnMapRepository : IGenericRepository<BoardBoardColumnMap>
    {
        Task<List<BoardBoardColumnMap>> CreateMultipleMappingsAsync(List<BoardBoardColumnMap> mappings);
        Task<IReadOnlyList<BoardBoardColumnMap>> GetMappingsByBoardIdAsync(int boardId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance.Dashboard
{
    public interface IProjectReadRepository
    {
        /// <summary>
        /// Return minimal project projection (Id and CreatedAt and DeliveryUnitId/StatusId) for charts.
        /// </summary>
        Task<List<ProjectProjection>> GetAllProjectProjectionsAsync(CancellationToken cancellationToken = default);
    }

    public class ProjectProjection
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? DeliveryUnitId { get; set; }
        public int? StatusId { get; set; }
    }
}

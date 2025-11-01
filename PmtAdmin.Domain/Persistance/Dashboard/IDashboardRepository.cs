using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance.Dashboard
{
    public interface IDashboardRepository
    {
        Task<(IReadOnlyList<Project> Projects, IReadOnlyList<DeliveryUnit> DeliveryUnits)> GetDashboardDataAsync();
    }
}

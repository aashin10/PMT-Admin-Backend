using PmtAdmin.Domain.Entities;
using System.Collections.Generic;

namespace Pmt_Admin.Test.Handlers.Du.Mock
{
    public static class DuMock
    {
        public static List<DeliveryUnit> GetDus()
        {
            return new List<DeliveryUnit>
            {
                new DeliveryUnit { Id = 1, Name = "DU1", Code = "DU001" },
                new DeliveryUnit { Id = 2, Name = "DU2", Code = "DU002" }
            };
        }
    }
}

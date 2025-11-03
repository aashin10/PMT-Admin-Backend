using System;

namespace PmtAdmin.Application.Dto
{
    public class BoardColumnDTO
    {
        public Guid Id { get; set; }
        public int? StatusId { get; set; }
        public string? BoardColumnName { get; set; }
        public string? BoardColor { get; set; }
        public int? Position { get; set; }
        public string? StatusName { get; set; }
    }
}
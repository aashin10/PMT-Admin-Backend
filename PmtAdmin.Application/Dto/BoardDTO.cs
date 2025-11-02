using System;
using System.Collections.Generic;

namespace PmtAdmin.Application.Dto
{
    public class BoardDTO
    {
        public int Id { get; set; }
        public Guid ProjectId { get; set; }
        public int? TeamId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = "Default";
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<BoardColumnDTO> Columns { get; set; } = new List<BoardColumnDTO>();
    }
}
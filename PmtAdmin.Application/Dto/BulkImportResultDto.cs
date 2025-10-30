using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class BulkImportResultDto
    {
        public int SuccessCount { get; set; }
        public int DuplicateCount { get; set; }
        public int ErrorCount { get; set; }
        public int TotalProcessed { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Duplicates { get; set; } = new List<string>();
        public List<UserDto> CreatedUsers { get; set; } = new List<UserDto>();
    }
}

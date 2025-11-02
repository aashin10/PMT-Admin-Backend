using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Application.Dto;
namespace PmtAdmin.Application.Command.Du
{
    public class CreateDuCommand : IRequest<ApiResponse<DuDto>>
    {
        public string Name { get; set; }            // DU Name (e.g., Engineering, Marketing)
        public string Code { get; set; }            // DU Code (e.g., ENG-001, PM-002)
        public string Description { get; set; }     // Purpose or responsibilities
        public string HeadName { get; set; }        // DU Head Name
        public string HeadEmail { get; set; }       // DU Head Email
        //public bool IsActive { get; set; } = true;
    }
}

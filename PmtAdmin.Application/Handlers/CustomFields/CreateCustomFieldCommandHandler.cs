using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PmtAdmin.Application.Command.CustomFields;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.CustomFields
{
    public class CreateCustomFieldCommandHandler : IRequestHandler<CreateCustomFieldCommand, ApiResponse<CustomFieldDTO>>
    {
        private readonly ICustomFieldRepository _customFieldRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public CreateCustomFieldCommandHandler(ICustomFieldRepository customFieldRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _customFieldRepository = customFieldRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CustomFieldDTO>> Handle(CreateCustomFieldCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name))
                    return ApiResponse<CustomFieldDTO>.Fail("Custom field name is required.");
                if (string.IsNullOrWhiteSpace(request.Value))
                    return ApiResponse<CustomFieldDTO>.Fail("Custom field value is required.");

                // Verify project exists
                var project = await _projectRepository.GetQueryable()
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.DeletedAt == null);

                if (project == null)
                    return ApiResponse<CustomFieldDTO>.NotFound("Project not found");

                // Create custom field
                var customField = new CustomField
                {
                    Id = Guid.NewGuid(),
                    ProjectId = request.ProjectId,
                    Name = request.Name.Trim(),
                    Value = request.Value.Trim()
                };

                await _customFieldRepository.CreateAsync(customField);

                var dto = _mapper.Map<CustomFieldDTO>(customField);
                return ApiResponse<CustomFieldDTO>.Success(dto, "Custom field created successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<CustomFieldDTO>.Fail($"Error creating custom field: {message}");
            }
        }
    }
}

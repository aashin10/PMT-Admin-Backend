using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.CustomFields;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.CustomFields
{
    public class UpdateCustomFieldCommandHandler : IRequestHandler<UpdateCustomFieldCommand, ApiResponse<CustomFieldDTO>>
    {
        private readonly ICustomFieldRepository _customFieldRepository;
        private readonly IMapper _mapper;

        public UpdateCustomFieldCommandHandler(ICustomFieldRepository customFieldRepository, IMapper mapper)
        {
            _customFieldRepository = customFieldRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CustomFieldDTO>> Handle(UpdateCustomFieldCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name))
                    return ApiResponse<CustomFieldDTO>.Fail("Custom field name is required.");
                if (string.IsNullOrWhiteSpace(request.Value))
                    return ApiResponse<CustomFieldDTO>.Fail("Custom field value is required.");

                // Get existing custom field
                var customField = await _customFieldRepository.GetByIdAsync(request.Id);

                if (customField == null)
                    return ApiResponse<CustomFieldDTO>.NotFound("Custom field not found");

                // Update fields
                customField.Name = request.Name.Trim();
                customField.Value = request.Value.Trim();

                await _customFieldRepository.SaveChangesAsync();

                var dto = _mapper.Map<CustomFieldDTO>(customField);
                return ApiResponse<CustomFieldDTO>.Success(dto, "Custom field updated successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<CustomFieldDTO>.Fail($"Error updating custom field: {message}");
            }
        }
    }
}

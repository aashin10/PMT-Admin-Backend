using MediatR;
using PmtAdmin.Application.Command.CustomFields;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.CustomFields
{
    public class DeleteCustomFieldCommandHandler : IRequestHandler<DeleteCustomFieldCommand, ApiResponse<string>>
    {
        private readonly ICustomFieldRepository _customFieldRepository;

        public DeleteCustomFieldCommandHandler(ICustomFieldRepository customFieldRepository)
        {
            _customFieldRepository = customFieldRepository;
        }

        public async Task<ApiResponse<string>> Handle(DeleteCustomFieldCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get existing custom field
                var customField = await _customFieldRepository.GetByIdAsync(request.Id);

                if (customField == null)
                    return ApiResponse<string>.NotFound("Custom field not found");

                // Delete custom field
                await _customFieldRepository.DeleteAsync(customField);

                return ApiResponse<string>.Success("", "Custom field deleted successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<string>.Fail($"Error deleting custom field: {message}");
            }
        }
    }
}

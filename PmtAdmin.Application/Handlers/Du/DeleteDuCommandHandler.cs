using MediatR;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.Du
{
    public class DeleteDuCommandHandler : IRequestHandler<DeleteDuCommand, ApiResponse<string>>
    {
        private readonly IDuRepository _repository;

        public DeleteDuCommandHandler(IDuRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<string>> Handle(DeleteDuCommand request, CancellationToken cancellationToken)
        {
            var du = await _repository.GetDuById(request.Id, cancellationToken);
            if (du == null)
                return ApiResponse<string>.Fail("Delivery Unit not found");

            await _repository.DeleteDuAsync(du, cancellationToken);

            return ApiResponse<string>.Success("Delivery Unit deleted successfully");
        }
    }
}

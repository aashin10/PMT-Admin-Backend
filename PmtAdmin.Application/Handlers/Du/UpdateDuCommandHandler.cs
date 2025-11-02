using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.Du
{
    public class UpdateDuCommandHandler : IRequestHandler<UpdateDuCommand, ApiResponse<DuDto>>
    {
        private readonly IDuRepository _repository;
        private readonly IMapper _mapper;

        public UpdateDuCommandHandler(IDuRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<DuDto>> Handle(UpdateDuCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Fetch the Delivery Unit by Id
            var existingDu = await _repository.GetDuById(request.Id, cancellationToken);
            if (existingDu == null)
                return ApiResponse<DuDto>.Fail("Delivery Unit not found");

            // 2️⃣ Update fields (make sure names match your entity)
            existingDu.Name = request.Name;
            existingDu.Description = request.Description;
            existingDu.DuHeadName = request.HeadName;
            existingDu.DuHeadEmail = request.HeadEmail;
            existingDu.UpdatedAt = DateTime.UtcNow;

            // 3️⃣ Persist the changes
            await _repository.UpdateDuAsync(existingDu, cancellationToken);

            // 4️⃣ Map to DTO for response
            var duDto = _mapper.Map<DuDto>(existingDu);

            return ApiResponse<DuDto>.Success(duDto, "Delivery Unit updated successfully");
        }
    }
}

using MediatR;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Delivery_Unit;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Du
{
    public class CreateDuCommandHandler : IRequestHandler<CreateDuCommand, ApiResponse<DuDto>>
    {
        private readonly IDuRepository _repository;
        private readonly AutoMapper.IMapper _mapper;

        public CreateDuCommandHandler(IDuRepository repository, AutoMapper.IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<DuDto>> Handle(CreateDuCommand request, CancellationToken cancellationToken)
        {
            var duEntity = _mapper.Map<DeliveryUnit>(request);
            // await _repository.CreateNewDu(duEntity);
            await _repository.CreateNewDu(duEntity, cancellationToken);
            var duDto = _mapper.Map<DuDto>(duEntity);
            return ApiResponse<DuDto>.Success(duDto, "Delivery Unit created successfully");
        }

    }
}

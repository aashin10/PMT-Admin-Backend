using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Delivery_Unit;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

public class GetAllDuQueryHandler : IRequestHandler<GetAllDuQuery, ApiResponse<List<DuDto>>>
{
    private readonly IDuRepository _repository;
    private readonly AutoMapper.IMapper _mapper;

    public GetAllDuQueryHandler(IDuRepository repository, AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<DuDto>>> Handle(GetAllDuQuery request, CancellationToken cancellationToken)
    {
        var duList = await _repository.GetAllWithProjectCountAsync(cancellationToken);

        if (duList == null || !duList.Any())
        {
            return ApiResponse<List<DuDto>>.Success(new List<DuDto>(), "No projects found");
        }
       // var inProgressCount = duLuprojects.Count(p => p.Status != null && p.Status.Name == "Active");

        var DuDtos = _mapper.Map<List<DuDto>>(duList);


        return ApiResponse<List<DuDto>>.Success(DuDtos);
    }
}

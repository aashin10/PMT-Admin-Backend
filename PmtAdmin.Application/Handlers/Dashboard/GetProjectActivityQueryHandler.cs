using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto.DashboardDTO;
using PmtAdmin.Application.Query.Dashboard;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Dashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Dashboard
{
    public class GetProjectActivityQueryHandler : IRequestHandler<GetProjectActivityQuery, ApiResponse<ChartDataDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectReadRepository _projectReadRepository;
        private readonly DateTime? _currentDate; // For testing

        public GetProjectActivityQueryHandler(
            IMapper mapper, 
            IProjectReadRepository projectReadRepository,
            DateTime? currentDate = null)
        {
            _mapper = mapper;
            _projectReadRepository = projectReadRepository;
            _currentDate = currentDate;
        }

        public async Task<ApiResponse<ChartDataDto>> Handle(GetProjectActivityQuery request, CancellationToken cancellationToken)
        {
            var projections = await _projectReadRepository.GetAllProjectProjectionsAsync(cancellationToken);

            if (projections == null || !projections.Any())
            {
                return ApiResponse<ChartDataDto>.NotFound("No project data available.");
            }

            var createdDates = projections
                .Where(p => p.CreatedAt != default)
                .Select(p => p.CreatedAt)
                .ToList();

            if (!createdDates.Any())
            {
                return ApiResponse<ChartDataDto>.NotFound("No project data available.");
            }

            // Use injected date for testing, otherwise use current UTC date
            var now = _currentDate?.ToUniversalTime() ?? DateTime.UtcNow;

            var chartDto = new ChartDataDto
            {
                Monthly = BuildMonthlyByWeeks(createdDates, now),
                Quarterly = BuildQuarterly(createdDates, now),
                Yearly = BuildYearly(createdDates, now.Year),
                Last5Years = BuildLastNYears(createdDates, 5, now.Year),
                AllTime = BuildAllTime(createdDates)
            };

            return ApiResponse<ChartDataDto>.Success(chartDto, "Project activity chart data fetched successfully.");
        }

        private List<ChartPointDto> BuildMonthlyByWeeks(List<DateTime> dates, DateTime nowUtc)
        {
            var year = nowUtc.Year;
            var month = nowUtc.Month;

            var weeks = new List<(int Start, int End)>
            {
                (1,7),
                (8,14),
                (15,21),
                (22, DateTime.DaysInMonth(year, month))
            };

            var result = new List<ChartPointDto>();
            for (int i = 0; i < weeks.Count; i++)
            {
                var start = new DateTime(year, month, weeks[i].Start);
                var end = new DateTime(year, month, weeks[i].End);
                var count = dates.Count(d => d.Date >= start.Date && d.Date <= end.Date);
                result.Add(new ChartPointDto { Period = $"Week {i + 1}", Projects = count });
            }

            return result;
        }

        private List<ChartPointDto> BuildQuarterly(List<DateTime> dates, DateTime nowUtc)
        {
            var year = nowUtc.Year;
            var quarters = new[]
            {
                (1,3,"Q1 " + year),
                (4,6,"Q2 " + year),
                (7,9,"Q3 " + year),
                (10,12,"Q4 " + year)
            };

            var result = new List<ChartPointDto>();
            foreach (var q in quarters)
            {
                var start = new DateTime(year, q.Item1, 1);
                var end = new DateTime(year, q.Item2, DateTime.DaysInMonth(year, q.Item2));
                var count = dates.Count(d => d.Date >= start.Date && d.Date <= end.Date);
                result.Add(new ChartPointDto { Period = q.Item3, Projects = count });
            }

            return result;
        }

        private List<ChartPointDto> BuildYearly(List<DateTime> dates, int year)
        {
            var result = new List<ChartPointDto>();
            var ci = CultureInfo.InvariantCulture;

            for (int m = 1; m <= 12; m++)
            {
                var start = new DateTime(year, m, 1);
                var end = new DateTime(year, m, DateTime.DaysInMonth(year, m));
                var count = dates.Count(d => d.Year == year && d.Month == m);
                var monthName = ci.DateTimeFormat.GetAbbreviatedMonthName(m);
                result.Add(new ChartPointDto { Period = monthName, Projects = count });
            }

            return result;
        }

        private List<ChartPointDto> BuildLastNYears(List<DateTime> dates, int n, int currentYear)
        {
            var result = new List<ChartPointDto>();

            for (int i = n - 1; i >= 0; i--)
            {
                var y = currentYear - i;
                var count = dates.Count(d => d.Year == y);
                result.Add(new ChartPointDto { Period = y.ToString(), Projects = count });
            }

            return result;
        }

        private List<ChartPointDto> BuildAllTime(List<DateTime> dates)
        {
            var result = new List<ChartPointDto>();
            var years = dates.Select(d => d.Year).Distinct().OrderBy(y => y);

            foreach (var y in years)
            {
                var count = dates.Count(d => d.Year == y);
                result.Add(new ChartPointDto { Period = y.ToString(), Projects = count });
            }

            return result;
        }
    }
}

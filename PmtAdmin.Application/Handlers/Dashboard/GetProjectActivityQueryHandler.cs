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

        public GetProjectActivityQueryHandler(IMapper mapper, IProjectReadRepository projectReadRepository)
        {
            _mapper = mapper;
            _projectReadRepository = projectReadRepository;
        }

        public async Task<ApiResponse<ChartDataDto>> Handle(GetProjectActivityQuery request, CancellationToken cancellationToken)
        {
            var projections = await _projectReadRepository.GetAllProjectProjectionsAsync(cancellationToken);

            if (projections == null || !projections.Any())
            {
                return ApiResponse<ChartDataDto>.Fail("No project data available.");
            }

            var createdDates = projections
                .Where(p => p.CreatedAt != default)
                .Select(p => p.CreatedAt.ToUniversalTime())
                .ToList();

            var now = DateTime.UtcNow;

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

        // === Helper: weekly buckets for current month ===
        private List<ChartPointDto> BuildMonthlyByWeeks(List<DateTime> datesUtc, DateTime nowUtc)
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
                var start = new DateTime(year, month, weeks[i].Start, 0, 0, 0, DateTimeKind.Utc);
                var end = new DateTime(year, month, weeks[i].End, 23, 59, 59, DateTimeKind.Utc);
                var count = datesUtc.Count(d => d >= start && d <= end);
                result.Add(new ChartPointDto { Period = $"Week {i + 1}", Projects = count });
            }

            return result;
        }

        // === Helper: quarterly counts for current year ===
        private List<ChartPointDto> BuildQuarterly(List<DateTime> datesUtc, DateTime nowUtc)
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
                var start = new DateTime(year, q.Item1, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = new DateTime(year, q.Item2, DateTime.DaysInMonth(year, q.Item2), 23, 59, 59, DateTimeKind.Utc);
                var count = datesUtc.Count(d => d >= start && d <= end);
                result.Add(new ChartPointDto { Period = q.Item3, Projects = count });
            }

            return result;
        }

        // === Helper: monthly counts for current year ===
        private List<ChartPointDto> BuildYearly(List<DateTime> datesUtc, int year)
        {
            var result = new List<ChartPointDto>();
            var ci = CultureInfo.InvariantCulture;

            for (int m = 1; m <= 12; m++)
            {
                var start = new DateTime(year, m, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = new DateTime(year, m, DateTime.DaysInMonth(year, m), 23, 59, 59, DateTimeKind.Utc);
                var count = datesUtc.Count(d => d >= start && d <= end);
                var monthName = ci.DateTimeFormat.GetAbbreviatedMonthName(m);
                result.Add(new ChartPointDto { Period = monthName, Projects = count });
            }

            return result;
        }

        // === Helper: last N years (including current) ===
        private List<ChartPointDto> BuildLastNYears(List<DateTime> datesUtc, int n, int currentYear)
        {
            var result = new List<ChartPointDto>();

            for (int i = n - 1; i >= 0; i--)
            {
                var y = currentYear - i;
                var start = new DateTime(y, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = new DateTime(y, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                var count = datesUtc.Count(d => d >= start && d <= end);
                result.Add(new ChartPointDto { Period = y.ToString(), Projects = count });
            }

            return result;
        }

        // === Helper: all-time yearly breakdown ===
        private List<ChartPointDto> BuildAllTime(List<DateTime> datesUtc)
        {
            var result = new List<ChartPointDto>();
            var years = datesUtc.Select(d => d.Year).Distinct().OrderBy(y => y);

            foreach (var y in years)
            {
                var start = new DateTime(y, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var end = new DateTime(y, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                var count = datesUtc.Count(d => d >= start && d <= end);
                result.Add(new ChartPointDto { Period = y.ToString(), Projects = count });
            }

            return result;
        }
    }
}

using FluentValidation;
using PmtAdmin.Application.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Validators
{
    public class GetUsersWithPaginationQueryValidator : AbstractValidator<GetUsersWithPaginationQuery>
    {
        public GetUsersWithPaginationQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrWhiteSpace(sortBy) ||
                    new[] { "name", "email", "type", "status", "createdat" }.Contains(sortBy.ToLower()))
                .WithMessage("SortBy must be one of: name, email, type, status, createdat");

            RuleFor(x => x.SortOrder)
                .Must(sortOrder => string.IsNullOrWhiteSpace(sortOrder) ||
                    new[] { "asc", "desc" }.Contains(sortOrder.ToLower()))
                .WithMessage("SortOrder must be 'asc' or 'desc'");

            RuleFor(x => x.Type)
                .Must(type => string.IsNullOrWhiteSpace(type) ||
                    new[] { "Internal", "External" }.Contains(type, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Type must be 'Internal' or 'External'");

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrWhiteSpace(status) ||
                    new[] { "Active", "Inactive" }.Contains(status, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Status must be 'Active' or 'Inactive'");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("SearchTerm cannot exceed 100 characters");
        }
    }
}

using FluentValidation;
using PmtAdmin.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command.Validators
{
    public class BulkImportUsersCommandValidator : AbstractValidator<BulkImportUsersCommand>
    {
        public BulkImportUsersCommandValidator()
        {
            RuleFor(x => x.Users)
                .NotEmpty().WithMessage("Users list cannot be empty")
                .Must(users => users.Count <= 1000).WithMessage("Cannot import more than 1000 users at once");

            RuleForEach(x => x.Users).SetValidator(new BulkImportUserDtoValidator());
        }
    }

    public class BulkImportUserDtoValidator : AbstractValidator<BulkImportUserDto>
    {
        public BulkImportUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

            RuleFor(x => x.JiraId)
                .MaximumLength(1024).WithMessage("Jira ID cannot exceed 1024 characters");

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrWhiteSpace(status) ||
                    new[] { "Active", "Inactive", "Suspended" }.Contains(status, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Status must be 'Active', 'Inactive', or 'Suspended'");
        }
    }
}

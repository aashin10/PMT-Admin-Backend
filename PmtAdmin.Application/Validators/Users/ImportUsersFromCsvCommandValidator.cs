using FluentValidation;
using PmtAdmin.Application.Command;

namespace PmtAdmin.Application.Validators.Users
{
    public class ImportUsersFromCsvCommandValidator : AbstractValidator<ImportUsersFromCsvCommand>
    {
        public ImportUsersFromCsvCommandValidator()
        {
            RuleFor(command => command.Users)
                .NotEmpty().WithMessage("Users list cannot be empty");

            RuleForEach(command => command.Users)
                .SetValidator(new CsvUserDtoValidator());
        }
    }

    public class CsvUserDtoValidator : AbstractValidator<CsvUserDto>
    {
        public CsvUserDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

            RuleFor(dto => dto.JiraId)
                .MaximumLength(1024).WithMessage("Jira ID must not exceed 1024 characters")
                .When(dto => !string.IsNullOrWhiteSpace(dto.JiraId));

            RuleFor(dto => dto.Status)
                .Must(status => string.IsNullOrWhiteSpace(status) ||
                    new[] { "Active", "Inactive", "Suspended" }.Contains(status, System.StringComparer.OrdinalIgnoreCase))
                .WithMessage("Status must be 'Active', 'Inactive', or 'Suspended'")
                .When(dto => !string.IsNullOrWhiteSpace(dto.Status));
        }
    }
}

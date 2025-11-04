using FluentValidation;
using PmtAdmin.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("User ID must be greater than 0");

            RuleFor(x => x.Email)
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters")
                .EmailAddress()
                .WithMessage("Email must be a valid email address")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.JiraId)
                .MaximumLength(1024)
                .WithMessage("Jira ID cannot exceed 1024 characters")
                .When(x => !string.IsNullOrEmpty(x.JiraId));

            RuleFor(x => x.Type)
                .Must(type => type == "Internal" || type == "External")
                .WithMessage("Type must be either 'Internal' or 'External'")
                .When(x => !string.IsNullOrEmpty(x.Type));
        }
    }
}

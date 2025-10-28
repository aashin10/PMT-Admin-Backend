using FluentValidation;
using PmtAdmin.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

            RuleFor(x => x.Type)
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters");

            RuleFor(x => x.JiraId)
                .MaximumLength(1024).WithMessage("Jira ID cannot exceed 1024 characters");
        }
    }
}

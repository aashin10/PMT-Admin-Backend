using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.CustomException
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(List<ValidationFailure> failures) : base("One or more validation errors occurred.")
        {
            Errors = failures.Select(f => f.ErrorMessage).ToList();
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }
    }
}

using FluentValidation;
using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Validators;

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Student name is required.")
            .MaximumLength(100).WithMessage("Student name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0.")
            .LessThanOrEqualTo(120).WithMessage("Age must be realistic (less than or equal to 120).");

        RuleFor(x => x.Course)
            .NotEmpty().WithMessage("Course is required.")
            .MaximumLength(100).WithMessage("Course name must not exceed 100 characters.");
    }
}

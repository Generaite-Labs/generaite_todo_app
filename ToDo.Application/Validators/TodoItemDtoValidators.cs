using FluentValidation;
using ToDo.Application.DTOs;
using ToDo.Domain.Entities;

namespace ToDo.Application.Validators
{
    public class CreateTodoItemDtoValidator : AbstractValidator<CreateTodoItemDto>
    {
        public CreateTodoItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be today or in the future.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }

    public class UpdateTodoItemDtoValidator : AbstractValidator<UpdateTodoItemDto>
    {
        public UpdateTodoItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be today or in the future.");

            RuleFor(x => x.CompletedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.CompletedAt.HasValue)
                .WithMessage("Completed date cannot be in the future.");

            RuleFor(x => x)
                .Must(x => x.Status != TodoItemStatusDto.COMPLETED || x.CompletedAt.HasValue)
                .WithMessage("Completed date is required when status is set to Completed.");

            RuleFor(x => x)
                .Must(x => x.Status == TodoItemStatusDto.COMPLETED || !x.CompletedAt.HasValue)
                .WithMessage("Completed date should be null when status is not Completed.");
        }
    }
}
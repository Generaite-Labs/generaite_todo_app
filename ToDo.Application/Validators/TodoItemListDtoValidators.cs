using FluentValidation;
using ToDo.Application.DTOs;

namespace ToDo.Application.Validators
{
    public class CreateTodoItemListDtoValidator : AbstractValidator<CreateTodoItemListDto>
    {
        public CreateTodoItemListDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }

    public class UpdateTodoItemListDtoValidator : AbstractValidator<UpdateTodoItemListDto>
    {
        public UpdateTodoItemListDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
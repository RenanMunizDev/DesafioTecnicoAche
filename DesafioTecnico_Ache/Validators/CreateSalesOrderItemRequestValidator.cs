using DesafioTecnico_Ache.DTOs.Requests;
using FluentValidation;

namespace DesafioTecnico_Ache.Validators;

/// <summary>
/// Validator para CreateSalesOrderItemRequest usando FluentValidation
/// </summary>
public class CreateSalesOrderItemRequestValidator : AbstractValidator<CreateSalesOrderItemRequest>
{
    public CreateSalesOrderItemRequestValidator()
    {
        RuleFor(x => x.MaterialCode)
            .NotEmpty().WithMessage("Código do material é obrigatório")
            .MaximumLength(18).WithMessage("Código do material deve ter no máximo 18 caracteres");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(999999).WithMessage("Quantidade não pode exceder 999999");

        RuleFor(x => x.UnitOfMeasure)
            .NotEmpty().WithMessage("Unidade de medida é obrigatória")
            .Length(2, 3).WithMessage("Unidade de medida deve ter entre 2 e 3 caracteres")
            .Must(BeValidUnitOfMeasure).WithMessage("Unidade de medida inválida");

        RuleFor(x => x.Plant)
            .NotEmpty().WithMessage("Centro fornecedor é obrigatório")
            .Length(4).WithMessage("Centro fornecedor deve ter 4 caracteres");

        RuleFor(x => x.BatchNumber)
            .MaximumLength(10).WithMessage("Número do lote deve ter no máximo 10 caracteres")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));
    }

    private bool BeValidUnitOfMeasure(string unitOfMeasure)
    {
        var validUnits = new[] { "UN", "KG", "L", "M", "CX", "PC" };
        return validUnits.Contains(unitOfMeasure.ToUpper());
    }
}

using DesafioTecnico_Ache.DTOs.Requests;
using FluentValidation;

namespace DesafioTecnico_Ache.Validators;

/// <summary>
/// Validator para CreateSalesOrderRequest usando FluentValidation
/// Aplica princípios DRY (Don't Repeat Yourself) e Single Responsibility
/// </summary>
public class CreateSalesOrderRequestValidator : AbstractValidator<CreateSalesOrderRequest>
{
    public CreateSalesOrderRequestValidator()
    {
        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("Tipo de documento é obrigatório")
            .Length(2, 4).WithMessage("Tipo de documento deve ter entre 2 e 4 caracteres")
            .Must(BeValidDocumentType).WithMessage("Tipo de documento inválido. Use: OR, RE, CR, DR");

        RuleFor(x => x.SalesOrganization)
            .NotEmpty().WithMessage("Organização de vendas é obrigatória")
            .Length(4).WithMessage("Organização de vendas deve ter 4 caracteres");

        RuleFor(x => x.DistributionChannel)
            .NotEmpty().WithMessage("Canal de distribuição é obrigatório")
            .Length(2).WithMessage("Canal de distribuição deve ter 2 caracteres");

        RuleFor(x => x.Division)
            .NotEmpty().WithMessage("Setor de atividade é obrigatório")
            .Length(2).WithMessage("Setor de atividade deve ter 2 caracteres");

        RuleFor(x => x.CustomerCode)
            .NotEmpty().WithMessage("Código do cliente é obrigatório")
            .MaximumLength(10).WithMessage("Código do cliente deve ter no máximo 10 caracteres");

        RuleFor(x => x.RequestedDeliveryDate)
            .NotEmpty().WithMessage("Data de entrega solicitada é obrigatória")
            .Must(BeValidDate).WithMessage("Data de entrega deve ser futura");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Moeda é obrigatória")
            .Length(3).WithMessage("Moeda deve ter 3 caracteres (ex: BRL, USD)")
            .Must(BeValidCurrency).WithMessage("Moeda inválida");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item")
            .Must(x => x.Count > 0).WithMessage("O pedido deve conter pelo menos um item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSalesOrderItemRequestValidator());
    }

    private bool BeValidDocumentType(string documentType)
    {
        var validTypes = new[] { "OR", "RE", "CR", "DR" }; // OR=Order, RE=Returns, CR=Credit, DR=Debit
        return validTypes.Contains(documentType.ToUpper());
    }

    private bool BeValidDate(DateTime date)
    {
        return date.Date >= DateTime.UtcNow.Date;
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "BRL", "USD", "EUR" };
        return validCurrencies.Contains(currency.ToUpper());
    }
}

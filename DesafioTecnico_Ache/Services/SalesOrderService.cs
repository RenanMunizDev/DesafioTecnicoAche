using DesafioTecnico_Ache.DTOs.Requests;
using DesafioTecnico_Ache.DTOs.Responses;
using DesafioTecnico_Ache.Interfaces;
using DesafioTecnico_Ache.Models;
using FluentValidation;

namespace DesafioTecnico_Ache.Services;

/// <summary>
/// Implementação do serviço de Sales Orders
/// Camada de serviço que contém a lógica de negócio
/// Segue princípios SOLID:
/// - Single Responsibility: apenas lógica de negócio de sales orders
/// - Open/Closed: extensível via interface
/// - Dependency Inversion: depende de abstrações (interfaces)
/// </summary>
public class SalesOrderService : ISalesOrderService
{
    private readonly ISalesOrderRepository _repository;
    private readonly IValidator<CreateSalesOrderRequest> _validator;
    private readonly ILogger<SalesOrderService> _logger;

    public SalesOrderService(
        ISalesOrderRepository repository,
        IValidator<CreateSalesOrderRequest> validator,
        ILogger<SalesOrderService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<SalesOrderResponse?> GetSalesOrderAsync(string salesOrderNumber)
    {
        _logger.LogInformation("Buscando pedido de vendas: {SalesOrderNumber}", salesOrderNumber);

        // Input sanitization (OWASP)
        if (string.IsNullOrWhiteSpace(salesOrderNumber))
        {
            _logger.LogWarning("Tentativa de busca com número de pedido vazio");
            return null;
        }

        var salesOrder = await _repository.GetByNumberAsync(salesOrderNumber);
        
        if (salesOrder == null)
        {
            _logger.LogInformation("Pedido não encontrado: {SalesOrderNumber}", salesOrderNumber);
            return null;
        }

        return MapToResponse(salesOrder);
    }

    public async Task<IEnumerable<SalesOrderResponse>> GetSalesOrdersByCustomerAsync(string customerCode)
    {
        _logger.LogInformation("Buscando pedidos do cliente: {CustomerCode}", customerCode);

        // Input sanitization (OWASP)
        if (string.IsNullOrWhiteSpace(customerCode))
        {
            _logger.LogWarning("Tentativa de busca com código de cliente vazio");
            return Enumerable.Empty<SalesOrderResponse>();
        }

        var salesOrders = await _repository.GetByCustomerAsync(customerCode);
        return salesOrders.Select(MapToResponse);
    }

    public async Task<SalesOrderResponse> CreateSalesOrderAsync(CreateSalesOrderRequest request)
    {
        _logger.LogInformation("Iniciando criação de pedido de vendas para cliente: {CustomerCode}", request.CustomerCode);

        // Validação com FluentValidation
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validação falhou ao criar pedido: {Errors}", errors);
            throw new ValidationException(validationResult.Errors);
        }

        // Validações de negócio adicionais
        await ValidateBusinessRules(request);

        // Mapear para modelo de domínio
        var salesOrder = MapToDomain(request);

        // Criar no repositório (simulação de chamada SAP OData)
        var createdOrder = await _repository.CreateAsync(salesOrder);

        _logger.LogInformation("Pedido criado com sucesso: {SalesOrderNumber}", createdOrder.SalesOrderNumber);

        return MapToResponse(createdOrder);
    }

    private async Task ValidateBusinessRules(CreateSalesOrderRequest request)
    {
        // Validar se cliente existe no SAP
        var customerExists = await _repository.CustomerExistsAsync(request.CustomerCode);
        if (!customerExists)
        {
            _logger.LogWarning("Cliente não encontrado no SAP: {CustomerCode}", request.CustomerCode);
            throw new BusinessException($"Cliente '{request.CustomerCode}' não encontrado no SAP");
        }

        // Validar se materiais existem no SAP
        foreach (var item in request.Items)
        {
            var materialExists = await _repository.MaterialExistsAsync(item.MaterialCode);
            if (!materialExists)
            {
                _logger.LogWarning("Material não encontrado no SAP: {MaterialCode}", item.MaterialCode);
                throw new BusinessException($"Material '{item.MaterialCode}' não encontrado no SAP");
            }
        }
    }

    private SalesOrder MapToDomain(CreateSalesOrderRequest request)
    {
        return new SalesOrder
        {
            DocumentType = request.DocumentType,
            SalesOrganization = request.SalesOrganization,
            DistributionChannel = request.DistributionChannel,
            Division = request.Division,
            CustomerCode = request.CustomerCode,
            RequestedDeliveryDate = request.RequestedDeliveryDate,
            PurchaseOrderNumber = request.PurchaseOrderNumber,
            Currency = request.Currency,
            Items = request.Items.Select(i => new SalesOrderItem
            {
                MaterialCode = i.MaterialCode,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                Plant = i.Plant,
                StorageLocation = i.StorageLocation,
                BatchNumber = i.BatchNumber
            }).ToList()
        };
    }

    private SalesOrderResponse MapToResponse(SalesOrder salesOrder)
    {
        return new SalesOrderResponse
        {
            SalesOrderNumber = salesOrder.SalesOrderNumber,
            DocumentType = salesOrder.DocumentType,
            SalesOrganization = salesOrder.SalesOrganization,
            DistributionChannel = salesOrder.DistributionChannel,
            Division = salesOrder.Division,
            CustomerCode = salesOrder.CustomerCode,
            CustomerName = salesOrder.CustomerName,
            OrderDate = salesOrder.OrderDate,
            RequestedDeliveryDate = salesOrder.RequestedDeliveryDate,
            PurchaseOrderNumber = salesOrder.PurchaseOrderNumber,
            Currency = salesOrder.Currency,
            TotalAmount = salesOrder.TotalAmount,
            Status = salesOrder.Status,
            CreatedAt = salesOrder.CreatedAt,
            UpdatedAt = salesOrder.UpdatedAt,
            Items = salesOrder.Items.Select(i => new SalesOrderItemResponse
            {
                ItemNumber = i.ItemNumber,
                MaterialCode = i.MaterialCode,
                MaterialDescription = i.MaterialDescription,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                Plant = i.Plant,
                StorageLocation = i.StorageLocation,
                BatchNumber = i.BatchNumber,
                ExpirationDate = i.ExpirationDate
            }).ToList()
        };
    }
}

/// <summary>
/// Exceção customizada para erros de negócio
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

using DesafioTecnico_Ache.DTOs.Requests;
using DesafioTecnico_Ache.DTOs.Responses;
using DesafioTecnico_Ache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTecnico_Ache.Controllers;

/// <summary>
/// Controller para operações de Sales Orders (Pedidos de Vendas) do SAP S/4HANA
/// Integração simulada via OData v4 / REST API com módulo SD (Sales & Distribution)
/// 
/// Endpoint SAP real seria: https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
/// Documentação: SAP API Business Hub - Sales Order API
/// </summary>
[ApiController]
[Route("api/sap/[controller]")]
[Produces("application/json")]
public class SalesOrdersController : ControllerBase
{
    private readonly ISalesOrderService _salesOrderService;
    private readonly ILogger<SalesOrdersController> _logger;

    public SalesOrdersController(
        ISalesOrderService salesOrderService,
        ILogger<SalesOrdersController> logger)
    {
        _salesOrderService = salesOrderService;
        _logger = logger;
    }

    /// <summary>
    /// Busca um pedido por número
    /// </summary>
    /// <param name="salesOrderNumber">Número do pedido (ex: SO0000001000)</param>
    /// <returns>Detalhes do pedido de vendas</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// <code>
    /// GET /api/sap/salesorders/SO0000001000
    /// Headers:
    ///   X-API-Key: your-api-key-here
    /// </code>
    /// </remarks>
    /// <response code="200">Pedido encontrado com sucesso</response>
    /// <response code="401">API Key não fornecida ou inválida</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="429">Limite de requisições excedido</response>
    [HttpGet("{salesOrderNumber}")]
    [ProducesResponseType(typeof(SalesOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SalesOrderResponse>> GetSalesOrder(string salesOrderNumber)
    {
        _logger.LogInformation("GET SalesOrder - Número: {SalesOrderNumber}", salesOrderNumber);

        var salesOrder = await _salesOrderService.GetSalesOrderAsync(salesOrderNumber);

        if (salesOrder == null)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorCode = "NOT_001",
                Message = $"Pedido de vendas '{salesOrderNumber}' não encontrado",
                Path = Request.Path
            });
        }

        return Ok(salesOrder);
    }

    /// <summary>
    /// Busca pedidos por código do cliente
    /// </summary>
    /// <param name="customerCode">Código do cliente (ex: C001)</param>
    /// <returns>Lista de pedidos do cliente</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// <code>
    /// GET /api/sap/salesorders/customer/C001
    /// Headers:
    ///   X-API-Key: your-api-key-here
    /// </code>
    /// </remarks>
    /// <response code="200">Lista de pedidos retornada com sucesso (pode ser vazia)</response>
    /// <response code="401">API Key não fornecida ou inválida</response>
    /// <response code="429">Limite de requisições excedido</response>
    [HttpGet("customer/{customerCode}")]
    [ProducesResponseType(typeof(IEnumerable<SalesOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<SalesOrderResponse>>> GetSalesOrdersByCustomer(string customerCode)
    {
        _logger.LogInformation("GET SalesOrders by Customer - Código: {CustomerCode}", customerCode);

        var salesOrders = await _salesOrderService.GetSalesOrdersByCustomerAsync(customerCode);

        return Ok(salesOrders);
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    /// <param name="request">Dados do pedido a ser criado</param>
    /// <returns>Pedido criado com número gerado</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// <code>
    /// POST /api/sap/salesorders
    /// Headers:
    ///   X-API-Key: your-api-key-here
    ///   Content-Type: application/json
    /// Body:
    /// {
    ///   "documentType": "OR",
    ///   "salesOrganization": "1000",
    ///   "distributionChannel": "10",
    ///   "division": "00",
    ///   "customerCode": "C001",
    ///   "requestedDeliveryDate": "2024-06-01T00:00:00Z",
    ///   "purchaseOrderNumber": "PO-2024-001",
    ///   "currency": "BRL",
    ///   "items": [
    ///     {
    ///       "materialCode": "M001",
    ///       "quantity": 10,
    ///       "unitOfMeasure": "UN",
    ///       "plant": "1000",
    ///       "storageLocation": "0001",
    ///       "batchNumber": "LOTE2024001"
    ///     }
    ///   ]
    /// }
    /// </code>
    /// <para>
    /// Clientes válidos mockados: C001, C002, C003, C004, C005
    /// </para>
    /// <para>
    /// Materiais válidos mockados: M001 (Paracetamol), M002 (Ibuprofeno), M003 (Dipirona), M004 (Amoxicilina), M005 (Omeprazol)
    /// </para>
    /// </remarks>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados de entrada inválidos (erro de validação)</response>
    /// <response code="401">API Key não fornecida ou inválida</response>
    /// <response code="422">Erro de regra de negócio (ex: cliente não existe)</response>
    /// <response code="429">Limite de requisições excedido</response>
    [HttpPost]
    [ProducesResponseType(typeof(SalesOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SalesOrderResponse>> CreateSalesOrder([FromBody] CreateSalesOrderRequest request)
    {
        _logger.LogInformation("POST SalesOrder - Cliente: {CustomerCode}, Itens: {ItemCount}",
            request.CustomerCode,
            request.Items.Count);

        var createdOrder = await _salesOrderService.CreateSalesOrderAsync(request);

        return CreatedAtAction(
            nameof(GetSalesOrder),
            new { salesOrderNumber = createdOrder.SalesOrderNumber },
            createdOrder);
    }
}

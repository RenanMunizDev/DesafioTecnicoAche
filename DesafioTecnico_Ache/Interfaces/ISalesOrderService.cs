using DesafioTecnico_Ache.DTOs.Requests;
using DesafioTecnico_Ache.DTOs.Responses;

namespace DesafioTecnico_Ache.Interfaces;

/// <summary>
/// Interface de serviço para lógica de negócio de Sales Orders
/// Camada de serviço seguindo Service Layer Pattern
/// </summary>
public interface ISalesOrderService
{
    /// <summary>
    /// Busca um pedido de vendas por número
    /// </summary>
    /// <param name="salesOrderNumber">Número do documento SAP</param>
    /// <returns>Response do pedido ou null se não encontrado</returns>
    Task<SalesOrderResponse?> GetSalesOrderAsync(string salesOrderNumber);

    /// <summary>
    /// Busca pedidos de vendas por código do cliente
    /// </summary>
    /// <param name="customerCode">Código do cliente no SAP</param>
    /// <returns>Lista de pedidos do cliente</returns>
    Task<IEnumerable<SalesOrderResponse>> GetSalesOrdersByCustomerAsync(string customerCode);

    /// <summary>
    /// Cria um novo pedido de vendas no SAP
    /// </summary>
    /// <param name="request">Dados do pedido a ser criado</param>
    /// <returns>Response do pedido criado</returns>
    Task<SalesOrderResponse> CreateSalesOrderAsync(CreateSalesOrderRequest request);
}

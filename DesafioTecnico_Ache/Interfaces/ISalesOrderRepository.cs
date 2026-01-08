using DesafioTecnico_Ache.Models;

namespace DesafioTecnico_Ache.Interfaces;

/// <summary>
/// Interface de repositório para acesso aos dados de Sales Orders do SAP
/// Abstração da camada de dados seguindo Repository Pattern
/// </summary>
public interface ISalesOrderRepository
{
    /// <summary>
    /// Busca um pedido de vendas por número
    /// </summary>
    /// <param name="salesOrderNumber">Número do documento SAP</param>
    /// <returns>Pedido de vendas ou null se não encontrado</returns>
    Task<SalesOrder?> GetByNumberAsync(string salesOrderNumber);

    /// <summary>
    /// Busca pedidos de vendas por código do cliente
    /// </summary>
    /// <param name="customerCode">Código do cliente no SAP</param>
    /// <returns>Lista de pedidos do cliente</returns>
    Task<IEnumerable<SalesOrder>> GetByCustomerAsync(string customerCode);

    /// <summary>
    /// Busca todos os pedidos com paginação
    /// </summary>
    /// <param name="pageNumber">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de pedidos</returns>
    Task<IEnumerable<SalesOrder>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Cria um novo pedido de vendas
    /// </summary>
    /// <param name="salesOrder">Objeto do pedido a ser criado</param>
    /// <returns>Pedido criado com número gerado</returns>
    Task<SalesOrder> CreateAsync(SalesOrder salesOrder);

    /// <summary>
    /// Verifica se um cliente existe no SAP
    /// </summary>
    /// <param name="customerCode">Código do cliente</param>
    /// <returns>True se o cliente existe</returns>
    Task<bool> CustomerExistsAsync(string customerCode);

    /// <summary>
    /// Verifica se um material existe no SAP
    /// </summary>
    /// <param name="materialCode">Código do material</param>
    /// <returns>True se o material existe</returns>
    Task<bool> MaterialExistsAsync(string materialCode);
}

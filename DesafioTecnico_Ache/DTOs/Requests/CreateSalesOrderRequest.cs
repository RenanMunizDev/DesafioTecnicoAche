namespace DesafioTecnico_Ache.DTOs.Requests;

/// <summary>
/// DTO para criação de pedido de vendas via integração OData/REST com SAP S/4HANA
/// Segue o padrão OData v4 do SAP API Business Hub
/// </summary>
public class CreateSalesOrderRequest
{
    /// <summary>
    /// Tipo de documento de vendas (obrigatório)
    /// Ex: OR (Standard Order), RE (Returns)
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Organização de vendas (obrigatório)
    /// </summary>
    public string SalesOrganization { get; set; } = string.Empty;

    /// <summary>
    /// Canal de distribuição (obrigatório)
    /// </summary>
    public string DistributionChannel { get; set; } = string.Empty;

    /// <summary>
    /// Setor de atividade (obrigatório)
    /// </summary>
    public string Division { get; set; } = string.Empty;

    /// <summary>
    /// Código do cliente (obrigatório)
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Data de entrega solicitada (obrigatório)
    /// </summary>
    public DateTime RequestedDeliveryDate { get; set; }

    /// <summary>
    /// Número do pedido do cliente (opcional)
    /// </summary>
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>
    /// Moeda do documento (opcional, padrão BRL)
    /// </summary>
    public string Currency { get; set; } = "BRL";

    /// <summary>
    /// Itens do pedido (obrigatório, mínimo 1)
    /// </summary>
    public List<CreateSalesOrderItemRequest> Items { get; set; } = new();
}

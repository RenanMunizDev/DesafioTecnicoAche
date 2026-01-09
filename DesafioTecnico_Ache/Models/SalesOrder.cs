namespace DesafioTecnico_Ache.Models;

/// <summary>
/// Representa um pedido de vendas no SAP S/4HANA módulo SD (Sales &amp; Distribution)
/// Equivalente à tabela VBAK (Sales Document: Header Data) no SAP
/// </summary>
public class SalesOrder
{
    /// <summary>
    /// Número do documento de vendas (VBELN)
    /// </summary>
    public string SalesOrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento de vendas (AUART)
    /// Ex: OR (Standard Order), RE (Returns), etc.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Organização de vendas (VKORG)
    /// </summary>
    public string SalesOrganization { get; set; } = string.Empty;

    /// <summary>
    /// Canal de distribuição (VTWEG)
    /// </summary>
    public string DistributionChannel { get; set; } = string.Empty;

    /// <summary>
    /// Setor de atividade (SPART)
    /// </summary>
    public string Division { get; set; } = string.Empty;

    /// <summary>
    /// Código do cliente (KUNNR)
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Nome do cliente
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Data do pedido (ERDAT)
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Data de entrega solicitada (VDATU)
    /// </summary>
    public DateTime RequestedDeliveryDate { get; set; }

    /// <summary>
    /// Número do pedido do cliente (BSTKD)
    /// </summary>
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>
    /// Moeda do documento (WAERK)
    /// </summary>
    public string Currency { get; set; } = "BRL";

    /// <summary>
    /// Valor total líquido (NETWR)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Status do documento (STATV)
    /// Ex: A (Aberto), B (Bloqueado), C (Completo)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// Itens do pedido (equivalente à VBAP - Sales Document: Item Data)
    /// </summary>
    public List<SalesOrderItem> Items { get; set; } = new();

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de última modificação
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

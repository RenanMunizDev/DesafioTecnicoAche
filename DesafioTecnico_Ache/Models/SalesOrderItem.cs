namespace DesafioTecnico_Ache.Models;

/// <summary>
/// Representa um item do pedido de vendas no SAP S/4HANA
/// Equivalente à tabela VBAP (Sales Document: Item Data) no SAP
/// </summary>
public class SalesOrderItem
{
    /// <summary>
    /// Número do item (POSNR)
    /// </summary>
    public string ItemNumber { get; set; } = string.Empty;

    /// <summary>
    /// Código do material (MATNR)
    /// </summary>
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do material (ARKTX)
    /// </summary>
    public string MaterialDescription { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade pedida (KWMENG)
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unidade de medida (VRKME)
    /// Ex: UN, KG, L, etc.
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Preço unitário (NETPR)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Valor total do item (NETWR)
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Centro fornecedor (WERKS)
    /// </summary>
    public string Plant { get; set; } = string.Empty;

    /// <summary>
    /// Depósito (LGORT)
    /// </summary>
    public string StorageLocation { get; set; } = string.Empty;

    /// <summary>
    /// Lote (CHARG) - importante para indústria farmacêutica
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// Data de validade - crítico para produtos farmacêuticos
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}

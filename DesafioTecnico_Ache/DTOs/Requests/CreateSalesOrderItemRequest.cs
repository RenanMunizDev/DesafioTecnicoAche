namespace DesafioTecnico_Ache.DTOs.Requests;

/// <summary>
/// DTO para item do pedido de vendas
/// </summary>
public class CreateSalesOrderItemRequest
{
    /// <summary>
    /// Código do material (obrigatório)
    /// </summary>
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade pedida (obrigatório, deve ser maior que zero)
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unidade de medida (obrigatório)
    /// Ex: UN, KG, L
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Centro fornecedor (obrigatório)
    /// </summary>
    public string Plant { get; set; } = string.Empty;

    /// <summary>
    /// Depósito (opcional)
    /// </summary>
    public string? StorageLocation { get; set; }

    /// <summary>
    /// Lote - importante para rastreabilidade farmacêutica (opcional)
    /// </summary>
    public string? BatchNumber { get; set; }
}

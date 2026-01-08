namespace DesafioTecnico_Ache.DTOs.Responses;

/// <summary>
/// DTO de resposta para item do pedido de vendas
/// </summary>
public class SalesOrderItemResponse
{
    public string ItemNumber { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialDescription { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Plant { get; set; } = string.Empty;
    public string? StorageLocation { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

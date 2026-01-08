namespace DesafioTecnico_Ache.DTOs.Responses;

/// <summary>
/// DTO de resposta para pedido de vendas
/// Segue padrão OData v4 do SAP S/4HANA
/// </summary>
public class SalesOrderResponse
{
    public string SalesOrderNumber { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string SalesOrganization { get; set; } = string.Empty;
    public string DistributionChannel { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime RequestedDeliveryDate { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<SalesOrderItemResponse> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using DesafioTecnico_Ache.Interfaces;
using DesafioTecnico_Ache.Models;

namespace DesafioTecnico_Ache.Repositories;

/// <summary>
/// Implementação mockada do repositório de Sales Orders
/// Simula integração com SAP S/4HANA via OData v4 / REST API
/// 
/// Em produção, esta classe seria substituída por uma implementação real que:
/// - Utilizaria SAP Cloud SDK for .NET
/// - Consumiria endpoints OData do SAP API Business Hub
/// - Endpoint exemplo: https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
/// - Implementaria autenticação OAuth 2.0 ou Basic Auth
/// - Trataria timeout e retry policies
/// </summary>
public class SapSalesOrderRepository : ISalesOrderRepository
{
    // Simulação de dados em memória (mockado)
    private readonly List<SalesOrder> _salesOrders = new();
    private readonly List<string> _validCustomers = new() { "C001", "C002", "C003", "C004", "C005" };
    private readonly Dictionary<string, (string Description, decimal Price)> _validMaterials = new()
    {
        { "M001", ("Paracetamol 500mg - Caixa c/ 20 comprimidos", 15.50m) },
        { "M002", ("Ibuprofeno 600mg - Caixa c/ 10 comprimidos", 22.80m) },
        { "M003", ("Dipirona Sódica 500mg - Caixa c/ 30 comprimidos", 18.90m) },
        { "M004", ("Amoxicilina 500mg - Caixa c/ 21 cápsulas", 35.60m) },
        { "M005", ("Omeprazol 20mg - Caixa c/ 28 cápsulas", 28.70m) }
    };

    private int _orderCounter = 1000;

    public SapSalesOrderRepository()
    {
        // Inicializar com alguns dados mockados
        InitializeMockData();
    }

    public Task<SalesOrder?> GetByNumberAsync(string salesOrderNumber)
    {
        var order = _salesOrders.FirstOrDefault(o => o.SalesOrderNumber == salesOrderNumber);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<SalesOrder>> GetByCustomerAsync(string customerCode)
    {
        var orders = _salesOrders
            .Where(o => o.CustomerCode == customerCode)
            .OrderByDescending(o => o.OrderDate)
            .AsEnumerable();
        
        return Task.FromResult(orders);
    }

    public Task<IEnumerable<SalesOrder>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var orders = _salesOrders
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsEnumerable();
        
        return Task.FromResult(orders);
    }

    public Task<SalesOrder> CreateAsync(SalesOrder salesOrder)
    {
        // Gerar número do pedido (simulando SAP number range)
        salesOrder.SalesOrderNumber = $"SO{++_orderCounter:D10}";
        salesOrder.OrderDate = DateTime.UtcNow;
        salesOrder.CreatedAt = DateTime.UtcNow;
        salesOrder.Status = "A"; // Aberto

        // Processar itens
        for (int i = 0; i < salesOrder.Items.Count; i++)
        {
            var item = salesOrder.Items[i];
            item.ItemNumber = $"{(i + 1) * 10:D6}"; // Item number: 000010, 000020, etc.

            // Buscar informações do material (simulado)
            if (_validMaterials.TryGetValue(item.MaterialCode, out var materialInfo))
            {
                item.MaterialDescription = materialInfo.Description;
                item.UnitPrice = materialInfo.Price;
                item.TotalPrice = item.Quantity * item.UnitPrice;

                // Simular data de validade para produtos farmacêuticos (2 anos)
                item.ExpirationDate = DateTime.UtcNow.AddYears(2);
            }
        }

        // Calcular total do pedido
        salesOrder.TotalAmount = salesOrder.Items.Sum(i => i.TotalPrice);

        // Buscar nome do cliente (simulado)
        salesOrder.CustomerName = GetCustomerName(salesOrder.CustomerCode);

        _salesOrders.Add(salesOrder);

        return Task.FromResult(salesOrder);
    }

    public Task<bool> CustomerExistsAsync(string customerCode)
    {
        return Task.FromResult(_validCustomers.Contains(customerCode));
    }

    public Task<bool> MaterialExistsAsync(string materialCode)
    {
        return Task.FromResult(_validMaterials.ContainsKey(materialCode));
    }

    private void InitializeMockData()
    {
        // Criar alguns pedidos mockados para demonstração
        _salesOrders.Add(new SalesOrder
        {
            SalesOrderNumber = "SO0000001000",
            DocumentType = "OR",
            SalesOrganization = "1000",
            DistributionChannel = "10",
            Division = "00",
            CustomerCode = "C001",
            CustomerName = "Drogaria São Paulo LTDA",
            OrderDate = DateTime.UtcNow.AddDays(-5),
            RequestedDeliveryDate = DateTime.UtcNow.AddDays(2),
            PurchaseOrderNumber = "PO-2024-001",
            Currency = "BRL",
            TotalAmount = 450.50m,
            Status = "A",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Items = new List<SalesOrderItem>
            {
                new()
                {
                    ItemNumber = "000010",
                    MaterialCode = "M001",
                    MaterialDescription = "Paracetamol 500mg - Caixa c/ 20 comprimidos",
                    Quantity = 10,
                    UnitOfMeasure = "UN",
                    UnitPrice = 15.50m,
                    TotalPrice = 155.00m,
                    Plant = "1000",
                    StorageLocation = "0001",
                    BatchNumber = "LOTE2024001",
                    ExpirationDate = DateTime.UtcNow.AddYears(2)
                },
                new()
                {
                    ItemNumber = "000020",
                    MaterialCode = "M003",
                    MaterialDescription = "Dipirona Sódica 500mg - Caixa c/ 30 comprimidos",
                    Quantity = 15,
                    UnitOfMeasure = "UN",
                    UnitPrice = 18.90m,
                    TotalPrice = 283.50m,
                    Plant = "1000",
                    StorageLocation = "0001",
                    BatchNumber = "LOTE2024002",
                    ExpirationDate = DateTime.UtcNow.AddYears(2)
                }
            }
        });
    }

    private string GetCustomerName(string customerCode)
    {
        return customerCode switch
        {
            "C001" => "Drogaria São Paulo LTDA",
            "C002" => "Farmácia Pague Menos S.A.",
            "C003" => "Drogasil S.A.",
            "C004" => "Raia Drogasil S.A.",
            "C005" => "Panvel Farmácias S.A.",
            _ => "Cliente Desconhecido"
        };
    }
}

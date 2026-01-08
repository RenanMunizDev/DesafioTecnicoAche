# ?? SAP S/4HANA Sales Order Integration API

<div align="center">

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)
![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?style=for-the-badge&logo=sap)
![License](https://img.shields.io/badge/license-MIT-green?style=for-the-badge)

**API REST profissional para integração com SAP S/4HANA - Módulo SD (Sales & Distribution)**

[Sobre](#-sobre-o-projeto) • [Arquitetura](#-arquitetura) • [Segurança](#-segurança-owasp) • [Como Usar](#-como-executar) • [Endpoints](#-endpoints-da-api)

</div>

---

## ?? Sobre o Projeto

API REST desenvolvida em **.NET 8 (LTS)** para integração com **SAP S/4HANA** - Módulo **SD (Sales & Distribution)**, simulando operações de pedidos de vendas através de integração **OData v4 / REST API**.

Este projeto foi desenvolvido como parte do desafio técnico **Aché Laboratórios Farmacêuticos**, demonstrando:

? Arquitetura profissional e escalável  
? Boas práticas de desenvolvimento (SOLID, DRY, KISS)  
? Segurança conforme OWASP Top 10  
? Código limpo e bem documentado  
? Padrões de projeto reconhecidos  

---

## ??? Arquitetura e Padrões de Projeto

### **Clean Architecture**
```
???????????????????????????????????????????
?          API Controllers                ?  ? Camada de Apresentação
???????????????????????????????????????????
?       Service Layer (Business)          ?  ? Lógica de Negócio
???????????????????????????????????????????
?    Repository Pattern (Data Access)     ?  ? Acesso a Dados
???????????????????????????????????????????
?       Domain Models & Entities          ?  ? Modelos de Domínio
???????????????????????????????????????????
```

### **Padrões de Projeto Aplicados**

<table>
<tr>
<td width="50%">

#### 1. ??? **Repository Pattern**
- Abstração da camada de acesso a dados
- Interface `ISalesOrderRepository`
- Implementação mockada `SapSalesOrderRepository`
- Facilita troca de implementação (mock ? SAP real)

#### 2. ?? **Service Layer Pattern**
- Lógica de negócio isolada em serviços
- Interface `ISalesOrderService`
- Implementação `SalesOrderService`
- Validações de regras de negócio

#### 3. ?? **Dependency Injection**
- Inversão de controle (IoC)
- Baixo acoplamento entre componentes
- Facilita testes unitários

</td>
<td width="50%">

#### 4. ?? **DTO Pattern**
- Separação entre modelos de domínio e transferência de dados
- **Requests**: `CreateSalesOrderRequest`, `CreateSalesOrderItemRequest`
- **Responses**: `SalesOrderResponse`, `SalesOrderItemResponse`, `ApiErrorResponse`

#### 5. ?? **Middleware Pattern**
- `GlobalExceptionHandlingMiddleware`: tratamento global de exceções
- `ApiKeyAuthenticationMiddleware`: autenticação por API Key
- `RateLimitingMiddleware`: proteção contra abuso de API

</td>
</tr>
</table>

---

## ?? Segurança (OWASP)

### **Implementações de Segurança Conforme OWASP Top 10 2021**

| OWASP Category | Implementação | Status |
|----------------|---------------|--------|
| **A01:2021 – Broken Access Control** | ? Autenticação via API Key obrigatória<br>? Validação de chaves em toda requisição<br>? Logging de tentativas não autorizadas | ? Implementado |
| **A03:2021 – Injection** | ? Input validation com FluentValidation<br>? Sanitização de entradas<br>? Uso de tipos fortemente tipados | ? Implementado |
| **A04:2021 – Insecure Design** | ? Rate Limiting (100 req/min)<br>? Proteção contra DoS<br>? Timeout configurável | ? Implementado |
| **A05:2021 – Security Misconfiguration** | ? Diferentes configurações por ambiente<br>? Logs detalhados apenas em Development<br>? Não exposição de stack traces em produção | ? Implementado |
| **A09:2021 – Security Logging** | ? Logging estruturado com Serilog<br>? Auditoria de todas as operações<br>? Retenção de logs (30 dias dev / 90 dias prod) | ? Implementado |
| **A10:2021 – SSRF** | ? URLs do SAP configuradas e validadas<br>? Timeout de requisições<br>? Retry policy controlado | ? Implementado |

---

## ?? Princípios SOLID

<table>
<tr>
<td align="center" width="20%">
<h3>S</h3>
<b>Single<br>Responsibility</b>
<hr>
Cada classe tem uma única responsabilidade
</td>
<td align="center" width="20%">
<h3>O</h3>
<b>Open/<br>Closed</b>
<hr>
Extensível via interfaces, fechado para modificação
</td>
<td align="center" width="20%">
<h3>L</h3>
<b>Liskov<br>Substitution</b>
<hr>
Implementações intercambiáveis sem quebrar o sistema
</td>
<td align="center" width="20%">
<h3>I</h3>
<b>Interface<br>Segregation</b>
<hr>
Interfaces específicas e coesas
</td>
<td align="center" width="20%">
<h3>D</h3>
<b>Dependency<br>Inversion</b>
<hr>
Depende de abstrações, não implementações
</td>
</tr>
</table>

### **Outros Princípios Aplicados**

- ?? **DRY (Don't Repeat Yourself)**: Validações centralizadas, mappers reutilizáveis
- ?? **KISS (Keep It Simple, Stupid)**: Código limpo, legível e direto ao ponto
- ?? **Clean Code**: Nomes descritivos, métodos pequenos e focados

---

## ?? Integração SAP S/4HANA

### **Tipo de Integração: OData v4 / REST API**

#### **Endpoints SAP Reais (Produção)**
```http
Base URL: https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
Documentação: SAP API Business Hub
Autenticação: OAuth 2.0 / Basic Auth
Formato: JSON / XML
```

#### **Operações Simuladas**

<table>
<tr>
<th>Operação</th>
<th>Endpoint SAP Real</th>
<th>Descrição</th>
</tr>
<tr>
<td><code>GET</code></td>
<td><code>/A_SalesOrder('SO0000001000')?$expand=to_Item</code></td>
<td>Buscar pedido por número com itens</td>
</tr>
<tr>
<td><code>GET</code></td>
<td><code>/A_SalesOrder?$filter=SoldToParty eq 'C001'</code></td>
<td>Buscar pedidos por cliente</td>
</tr>
<tr>
<td><code>POST</code></td>
<td><code>/A_SalesOrder</code></td>
<td>Criar novo pedido de vendas</td>
</tr>
</table>

### **Mapeamento SAP**

#### **Tabelas SAP Representadas**
- **VBAK**: Sales Document - Header Data
- **VBAP**: Sales Document - Item Data
- **KNA1**: Customer Master

#### **Campos SAP Mapeados**

| Campo SAP | Propriedade C# | Descrição |
|-----------|----------------|-----------|
| `VBELN` | `SalesOrderNumber` | Número do documento de vendas |
| `AUART` | `DocumentType` | Tipo de documento (OR, RE, CR) |
| `VKORG` | `SalesOrganization` | Organização de vendas |
| `VTWEG` | `DistributionChannel` | Canal de distribuição |
| `SPART` | `Division` | Setor de atividade |
| `KUNNR` | `CustomerCode` | Código do cliente |
| `MATNR` | `MaterialCode` | Código do material |
| `KWMENG` | `Quantity` | Quantidade pedida |
| `CHARG` | `BatchNumber` | Lote (rastreabilidade farmacêutica) |

---

## ?? Endpoints da API

### **Base URL**
```
https://localhost:{port}/api/sap/salesorders
```

### **1. GET - Buscar Pedido por Número**

```http
GET /api/sap/salesorders/{salesOrderNumber}
Headers:
  X-API-Key: demo-api-key-12345
```

**Resposta 200 OK:**
```json
{
  "salesOrderNumber": "SO0000001000",
  "documentType": "OR",
  "customerCode": "C001",
  "customerName": "Drogaria São Paulo LTDA",
  "orderDate": "2024-01-15T10:30:00Z",
  "totalAmount": 450.50,
  "currency": "BRL",
  "status": "A",
  "items": [
    {
      "itemNumber": "000010",
      "materialCode": "M001",
      "materialDescription": "Paracetamol 500mg - Caixa c/ 20 comprimidos",
      "quantity": 10,
      "unitPrice": 15.50,
      "totalPrice": 155.00,
      "batchNumber": "LOTE2024001",
      "expirationDate": "2026-01-15T00:00:00Z"
    }
  ]
}
```

### **2. GET - Buscar Pedidos por Cliente**

```http
GET /api/sap/salesorders/customer/{customerCode}
Headers:
  X-API-Key: demo-api-key-12345
```

### **3. POST - Criar Novo Pedido**

```http
POST /api/sap/salesorders
Headers:
  X-API-Key: demo-api-key-12345
  Content-Type: application/json

Body:
{
  "documentType": "OR",
  "salesOrganization": "1000",
  "distributionChannel": "10",
  "division": "00",
  "customerCode": "C001",
  "requestedDeliveryDate": "2024-06-01T00:00:00Z",
  "currency": "BRL",
  "items": [
    {
      "materialCode": "M001",
      "quantity": 10,
      "unitOfMeasure": "UN",
      "plant": "1000",
      "storageLocation": "0001",
      "batchNumber": "LOTE2024001"
    }
  ]
}
```

**Resposta 201 Created:**
```json
{
  "salesOrderNumber": "SO0000001001",
  "message": "Pedido criado com sucesso"
}
```

---

## ?? Dados Mockados

### **Clientes Disponíveis**

| Código | Nome | Tipo |
|--------|------|------|
| `C001` | Drogaria São Paulo LTDA | Rede de Farmácias |
| `C002` | Farmácia Pague Menos S.A. | Rede de Farmácias |
| `C003` | Drogasil S.A. | Rede de Farmácias |
| `C004` | Raia Drogasil S.A. | Rede de Farmácias |
| `C005` | Panvel Farmácias S.A. | Rede de Farmácias |

### **Materiais Disponíveis**

| Código | Descrição | Preço (R$) |
|--------|-----------|------------|
| `M001` | Paracetamol 500mg - Caixa c/ 20 comprimidos | 15,50 |
| `M002` | Ibuprofeno 600mg - Caixa c/ 10 comprimidos | 22,80 |
| `M003` | Dipirona Sódica 500mg - Caixa c/ 30 comprimidos | 18,90 |
| `M004` | Amoxicilina 500mg - Caixa c/ 21 cápsulas | 35,60 |
| `M005` | Omeprazol 20mg - Caixa c/ 28 cápsulas | 28,70 |

### **API Keys Válidas (Development)**
```
demo-api-key-12345
test-api-key-67890
ache-pharma-api-key
```

---

## ??? Tecnologias Utilizadas

<table>
<tr>
<td align="center" width="33%">
<img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/dotnetcore/dotnetcore-original.svg" width="64" height="64" alt=".NET"/>
<br><b>.NET 8 (LTS)</b>
<br>Framework principal
</td>
<td align="center" width="33%">
<img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg" width="64" height="64" alt="C#"/>
<br><b>C# 12</b>
<br>Linguagem de programação
</td>
<td align="center" width="33%">
<img src="https://www.vectorlogo.zone/logos/getpostman/getpostman-icon.svg" width="64" height="64" alt="Swagger"/>
<br><b>Swagger/OpenAPI</b>
<br>Documentação da API
</td>
</tr>
<tr>
<td align="center">
<b>FluentValidation</b>
<br>Validação de dados
</td>
<td align="center">
<b>Serilog</b>
<br>Logging estruturado
</td>
<td align="center">
<b>SAP S/4HANA</b>
<br>ERP Integration
</td>
</tr>
</table>

---

## ?? Estrutura do Projeto

```
DesafioTecnico_Ache/
?
??? ?? Controllers/
?   ??? SalesOrdersController.cs         # Endpoints REST
?
??? ?? Services/
?   ??? SalesOrderService.cs             # Lógica de negócio
?
??? ?? Repositories/
?   ??? SapSalesOrderRepository.cs       # Acesso a dados (mockado)
?
??? ?? Interfaces/
?   ??? ISalesOrderService.cs            # Contrato do serviço
?   ??? ISalesOrderRepository.cs         # Contrato do repositório
?
??? ?? Models/
?   ??? SalesOrder.cs                    # Modelo de domínio
?   ??? SalesOrderItem.cs                # Item do pedido
?
??? ?? DTOs/
?   ??? ?? Requests/
?   ?   ??? CreateSalesOrderRequest.cs
?   ?   ??? CreateSalesOrderItemRequest.cs
?   ??? ?? Responses/
?       ??? SalesOrderResponse.cs
?       ??? SalesOrderItemResponse.cs
?       ??? ApiErrorResponse.cs
?
??? ?? Validators/
?   ??? CreateSalesOrderRequestValidator.cs
?   ??? CreateSalesOrderItemRequestValidator.cs
?
??? ?? Middlewares/
?   ??? GlobalExceptionHandlingMiddleware.cs
?   ??? ApiKeyAuthenticationMiddleware.cs
?   ??? RateLimitingMiddleware.cs
?
??? ?? Program.cs                        # Configuração da aplicação
??? ?? appsettings.json                  # Configurações Development
??? ?? appsettings.Production.json       # Configurações Production
??? ?? README.md                         # Documentação do projeto
```

---

## ?? Como Executar

### **Pré-requisitos**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- Git (opcional)

### **Passos**

#### 1?? **Clone o repositório**
```bash
git clone https://github.com/RenanMunizDev/DesafioTecnicoAche.git
cd DesafioTecnicoAche
```

#### 2?? **Restaurar dependências**
```bash
dotnet restore
```

#### 3?? **Executar a aplicação**
```bash
dotnet run --project DesafioTecnico_Ache
```

#### 4?? **Acessar o Swagger**
```
https://localhost:5001/swagger
ou
http://localhost:5000/swagger
```

#### 5?? **Testar os endpoints**
- Clique em **"Authorize"** no Swagger
- Insira a API Key: `demo-api-key-12345`
- Experimente os endpoints GET e POST

---

## ?? Testes com Swagger

### **Teste 1: Buscar Pedido Existente**
1. Acesse `/swagger`
2. Clique em **"Authorize"** e insira: `demo-api-key-12345`
3. Execute `GET /api/sap/salesorders/SO0000001000`
4. ? Deve retornar o pedido mockado com itens

### **Teste 2: Criar Novo Pedido**
1. Execute `POST /api/sap/salesorders`
2. Use o JSON de exemplo fornecido no Swagger
3. ? Verifique o número do pedido criado (ex: `SO0000001001`)
4. Use o GET para buscar o pedido recém-criado

### **Teste 3: Validações e Segurança**
- ? Tente criar pedido **sem API Key** ? 401 Unauthorized
- ? Tente criar pedido com **cliente inválido** (ex: `C999`) ? 422 Unprocessable Entity
- ? Tente criar pedido com **dados inválidos** (quantidade negativa) ? 400 Bad Request
- ? Faça **mais de 100 requisições em 1 minuto** ? 429 Too Many Requests

---

## ?? Melhorias Futuras (Produção Real)

### **Integração Real com SAP**
- [ ] Implementar **SAP Cloud SDK for .NET**
- [ ] Configurar **OAuth 2.0 / SAML**
- [ ] Implementar **retry policies** com Polly
- [ ] Adicionar **circuit breaker pattern**
- [ ] **Cache distribuído** com Redis

### **Testes**
- [ ] **Testes unitários** (xUnit)
- [ ] **Testes de integração**
- [ ] **Testes de carga** (K6, JMeter)
- [ ] **Cobertura de código** > 80%

### **Observabilidade**
- [ ] **Application Insights** / Elastic APM
- [ ] **Métricas** com Prometheus
- [ ] **Dashboards** com Grafana
- [ ] **Distributed tracing**

### **DevOps**
- [ ] Pipeline **CI/CD** (Azure DevOps, GitHub Actions)
- [ ] **Containerização** (Docker)
- [ ] **Kubernetes** deployment
- [ ] **Infrastructure as Code** (Terraform)

### **Segurança Adicional**
- [ ] **mTLS** (mutual TLS)
- [ ] **API Gateway** (Azure API Management)
- [ ] **WAF** (Web Application Firewall)
- [ ] **Secrets management** (Azure Key Vault)

---

## ?? Licença

Este projeto foi desenvolvido para fins de avaliação técnica.

---

## ?? Autor

<div align="center">

**Desenvolvido com ?? para o desafio técnico Aché Laboratórios Farmacêuticos**

### **Competências Técnicas Demonstradas**

? Arquitetura limpa e escalável  
? Código profissional e bem documentado  
? Segurança (OWASP Top 10)  
? Boas práticas (SOLID, DRY, KISS)  
? Padrões de projeto reconhecidos  
? Conhecimento de integração SAP S/4HANA  

---

**Se tiver dúvidas ou sugestões, sinta-se à vontade para abrir uma issue!**

[? Voltar ao topo](#-sap-s4hana-sales-order-integration-api)

</div>

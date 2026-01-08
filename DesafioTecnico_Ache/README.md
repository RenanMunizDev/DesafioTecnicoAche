# SAP S/4HANA Sales Order Integration API

## ?? Descrição do Projeto

API REST desenvolvida em **.NET 8 (LTS)** para integração com **SAP S/4HANA** - Módulo **SD (Sales & Distribution)**, simulando operações de pedidos de vendas através de integração **OData v4 / REST API**.

Este projeto foi desenvolvido como parte do desafio técnico Aché Laboratórios Farmacêuticos, demonstrando arquitetura profissional, boas práticas de desenvolvimento e segurança.

---

## ??? Arquitetura e Padrões de Projeto

### **Clean Architecture**
- Separação clara de responsabilidades em camadas
- Independência de frameworks externos
- Testabilidade e manutenibilidade

### **Padrões de Projeto Aplicados**

#### 1. **Repository Pattern**
- Abstração da camada de acesso a dados
- Interface `ISalesOrderRepository`
- Implementação mockada `SapSalesOrderRepository`
- Facilita troca de implementação (mock ? SAP real)

#### 2. **Service Layer Pattern**
- Lógica de negócio isolada em serviços
- Interface `ISalesOrderService`
- Implementação `SalesOrderService`
- Validações de regras de negócio

#### 3. **Dependency Injection**
- Inversão de controle (IoC)
- Baixo acoplamento entre componentes
- Facilita testes unitários

#### 4. **DTO Pattern**
- Separação entre modelos de domínio e transferência de dados
- Requests: `CreateSalesOrderRequest`, `CreateSalesOrderItemRequest`
- Responses: `SalesOrderResponse`, `SalesOrderItemResponse`, `ApiErrorResponse`

#### 5. **Middleware Pattern**
- `GlobalExceptionHandlingMiddleware`: tratamento global de exceções
- `ApiKeyAuthenticationMiddleware`: autenticação por API Key
- `RateLimitingMiddleware`: proteção contra abuso de API

---

## ?? Segurança (OWASP)

### **Implementações de Segurança**

#### ? **A01:2021 – Broken Access Control**
- Autenticação via API Key obrigatória
- Validação de chaves em toda requisição
- Logging de tentativas não autorizadas

#### ? **A03:2021 – Injection**
- Input validation com FluentValidation
- Sanitização de entradas
- Uso de tipos fortemente tipados

#### ? **A04:2021 – Insecure Design**
- Rate Limiting (100 req/min)
- Proteção contra DoS
- Timeout configurável

#### ? **A05:2021 – Security Misconfiguration**
- Diferentes configurações por ambiente
- Logs detalhados apenas em Development
- Não exposição de stack traces em produção

#### ? **A09:2021 – Security Logging and Monitoring**
- Logging estruturado com Serilog
- Auditoria de todas as operações
- Retenção de logs (30 dias dev / 90 dias prod)

#### ? **A10:2021 – Server-Side Request Forgery (SSRF)**
- URLs do SAP configuradas e validadas
- Timeout de requisições
- Retry policy controlado

---

## ?? Princípios de Desenvolvimento

### **SOLID**

#### **S - Single Responsibility Principle**
- Cada classe tem uma única responsabilidade
- `SalesOrderService`: apenas lógica de negócio
- `SapSalesOrderRepository`: apenas acesso a dados
- Controllers: apenas roteamento HTTP

#### **O - Open/Closed Principle**
- Extensível via interfaces
- Fechado para modificação direta
- `ISalesOrderRepository` permite múltiplas implementações

#### **L - Liskov Substitution Principle**
- Implementações podem ser substituídas sem quebrar o sistema
- Mock pode ser substituído por implementação real SAP

#### **I - Interface Segregation Principle**
- Interfaces específicas e coesas
- `ISalesOrderRepository` vs `ISalesOrderService`
- Clientes não dependem de métodos que não usam

#### **D - Dependency Inversion Principle**
- Dependência de abstrações (interfaces), não implementações
- Injeção de dependências via construtor
- Facilita testes e manutenção

### **DRY (Don't Repeat Yourself)**
- Validações centralizadas com FluentValidation
- Mappers de domínio ? DTO em métodos reutilizáveis
- Middlewares compartilhados

### **KISS (Keep It Simple, Stupid)**
- Código limpo e legível
- Nomes descritivos
- Métodos pequenos e focados
- Comentários apenas quando necessário

---

## ?? Integração SAP S/4HANA

### **Tipo de Integração: OData v4 / REST API**

#### **Endpoints SAP Reais (Produção)**
```
Base URL: https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
Documentação: SAP API Business Hub
```

#### **Operações Simuladas**

**GET - Buscar Pedido por Número**
```
GET /A_SalesOrder('SO0000001000')?$expand=to_Item
```

**GET - Buscar Pedidos por Cliente**
```
GET /A_SalesOrder?$filter=SoldToParty eq 'C001'&$expand=to_Item
```

**POST - Criar Pedido**
```
POST /A_SalesOrder
Content-Type: application/json
Authorization: Basic/OAuth2
```

### **Mapeamento SAP**

#### **Tabelas SAP Representadas**
- **VBAK**: Sales Document - Header Data
- **VBAP**: Sales Document - Item Data
- **KNA1**: Customer Master

#### **Campos SAP Mapeados**
- `VBELN` ? SalesOrderNumber
- `AUART` ? DocumentType
- `VKORG` ? SalesOrganization
- `VTWEG` ? DistributionChannel
- `SPART` ? Division
- `KUNNR` ? CustomerCode
- `MATNR` ? MaterialCode
- `KWMENG` ? Quantity
- `CHARG` ? BatchNumber (importante para farmacêuticos)

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
  "totalAmount": 450.50,
  "items": [...]
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
      "plant": "1000"
    }
  ]
}
```

---

## ?? Dados Mockados

### **Clientes Disponíveis**
- `C001`: Drogaria São Paulo LTDA
- `C002`: Farmácia Pague Menos S.A.
- `C003`: Drogasil S.A.
- `C004`: Raia Drogasil S.A.
- `C005`: Panvel Farmácias S.A.

### **Materiais Disponíveis**
- `M001`: Paracetamol 500mg - R$ 15,50
- `M002`: Ibuprofeno 600mg - R$ 22,80
- `M003`: Dipirona Sódica 500mg - R$ 18,90
- `M004`: Amoxicilina 500mg - R$ 35,60
- `M005`: Omeprazol 20mg - R$ 28,70

### **API Keys Válidas (Development)**
- `demo-api-key-12345`
- `test-api-key-67890`
- `ache-pharma-api-key`

---

## ??? Tecnologias Utilizadas

- **.NET 8 (LTS)** - Framework principal
- **ASP.NET Core Web API** - API REST
- **FluentValidation** - Validação de dados
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API

---

## ?? Estrutura do Projeto

```
DesafioTecnico_Ache/
??? Controllers/
?   ??? SalesOrdersController.cs       # Endpoints REST
??? Services/
?   ??? SalesOrderService.cs           # Lógica de negócio
??? Repositories/
?   ??? SapSalesOrderRepository.cs     # Acesso a dados (mockado)
??? Interfaces/
?   ??? ISalesOrderService.cs          # Contrato do serviço
?   ??? ISalesOrderRepository.cs       # Contrato do repositório
??? Models/
?   ??? SalesOrder.cs                  # Modelo de domínio
?   ??? SalesOrderItem.cs              # Item do pedido
??? DTOs/
?   ??? Requests/
?   ?   ??? CreateSalesOrderRequest.cs
?   ?   ??? CreateSalesOrderItemRequest.cs
?   ??? Responses/
?       ??? SalesOrderResponse.cs
?       ??? SalesOrderItemResponse.cs
?       ??? ApiErrorResponse.cs
??? Validators/
?   ??? CreateSalesOrderRequestValidator.cs
?   ??? CreateSalesOrderItemRequestValidator.cs
??? Middlewares/
?   ??? GlobalExceptionHandlingMiddleware.cs
?   ??? ApiKeyAuthenticationMiddleware.cs
?   ??? RateLimitingMiddleware.cs
??? Program.cs                         # Configuração da aplicação
??? appsettings.json                   # Configurações Development
??? appsettings.Production.json        # Configurações Production
```

---

## ?? Como Executar

### **Pré-requisitos**
- .NET 8 SDK
- Visual Studio 2022 ou VS Code

### **Passos**

1. **Clone o repositório**
```bash
git clone <repository-url>
cd DesafioTecnico_Ache
```

2. **Restaurar dependências**
```bash
dotnet restore
```

3. **Executar a aplicação**
```bash
dotnet run --project DesafioTecnico_Ache
```

4. **Acessar o Swagger**
```
https://localhost:{port}/swagger
```

5. **Testar os endpoints**
- Use a API Key: `demo-api-key-12345` no header `X-API-Key`
- Experimente os endpoints GET e POST

---

## ?? Testes com Swagger

### **Teste 1: Buscar Pedido Existente**
1. Acesse `/swagger`
2. Clique em "Authorize" e insira: `demo-api-key-12345`
3. Execute GET `/api/sap/salesorders/SO0000001000`

### **Teste 2: Criar Novo Pedido**
1. Execute POST `/api/sap/salesorders`
2. Use o JSON de exemplo fornecido
3. Verifique o número do pedido criado
4. Use o GET para buscar o pedido recém-criado

### **Teste 3: Validações**
- Tente criar pedido sem API Key ? 401
- Tente criar pedido com cliente inválido ? 422
- Tente criar pedido com dados inválidos ? 400

---

## ?? Melhorias Futuras (Produção Real)

### **Integração Real com SAP**
- [ ] Implementar SAP Cloud SDK for .NET
- [ ] Configurar OAuth 2.0 / SAML
- [ ] Implementar retry policies com Polly
- [ ] Adicionar circuit breaker pattern
- [ ] Cache distribuído com Redis

### **Testes**
- [ ] Testes unitários (xUnit)
- [ ] Testes de integração
- [ ] Testes de carga (K6, JMeter)
- [ ] Cobertura de código > 80%

### **Observabilidade**
- [ ] Application Insights / Elastic APM
- [ ] Métricas com Prometheus
- [ ] Dashboards com Grafana
- [ ] Distributed tracing

### **DevOps**
- [ ] Pipeline CI/CD (Azure DevOps, GitHub Actions)
- [ ] Containerização (Docker)
- [ ] Kubernetes deployment
- [ ] Infrastructure as Code (Terraform)

### **Segurança Adicional**
- [ ] mTLS (mutual TLS)
- [ ] API Gateway (Azure API Management)
- [ ] WAF (Web Application Firewall)
- [ ] Secrets management (Azure Key Vault)

---

## ?? Licença

Este projeto foi desenvolvido para fins de avaliação técnica.

---

## ?? Autor

Desenvolvido com ?? para o desafio técnico Aché Laboratórios Farmacêuticos.

**Competência Técnica Demonstrada:**
? Arquitetura limpa e escalável  
? Código profissional e bem documentado  
? Segurança (OWASP)  
? Boas práticas (SOLID, DRY, KISS)  
? Padrões de projeto reconhecidos  
? Conhecimento de integração SAP  

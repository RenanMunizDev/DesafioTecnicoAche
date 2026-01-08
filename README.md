# ?? SAP S/4HANA Sales Order Integration API

<div align="center">

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)
![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?style=for-the-badge&logo=sap)

**API REST profissional para integração com SAP S/4HANA - Módulo SD (Sales & Distribution)**

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

Separação clara de responsabilidades em camadas:
- **Controllers**: Camada de apresentação (API REST)
- **Services**: Lógica de negócio
- **Repositories**: Acesso a dados (integração SAP mockada)
- **Models**: Modelos de domínio

### **Padrões de Projeto Aplicados**

#### 1. ?? **Repository Pattern**
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

#### 4. ?? **DTO Pattern**
- Separação entre modelos de domínio e transferência de dados
- Requests e Responses específicos para API

#### 5. ?? **Middleware Pattern**
- `GlobalExceptionHandlingMiddleware`: tratamento global de exceções
- `ApiKeyAuthenticationMiddleware`: autenticação por API Key
- `RateLimitingMiddleware`: proteção contra abuso de API

---

## ?? Segurança (OWASP)

### **Implementações de Segurança Conforme OWASP Top 10 2021**

??? **A01:2021 – Broken Access Control**
- Autenticação via API Key obrigatória
- Validação de chaves em toda requisição
- Logging de tentativas não autorizadas

??? **A03:2021 – Injection**
- Input validation com FluentValidation
- Sanitização de entradas
- Uso de tipos fortemente tipados

??? **A04:2021 – Insecure Design**
- Rate Limiting (100 req/min)
- Proteção contra DoS
- Timeout configurável

??? **A05:2021 – Security Misconfiguration**
- Diferentes configurações por ambiente
- Logs detalhados apenas em Development
- Não exposição de stack traces em produção

??? **A09:2021 – Security Logging**
- Logging estruturado com Serilog
- Auditoria de todas as operações
- Retenção de logs (30 dias dev / 90 dias prod)

---

## ?? Princípios SOLID

**S** - Single Responsibility: Cada classe tem uma única responsabilidade  
**O** - Open/Closed: Extensível via interfaces, fechado para modificação  
**L** - Liskov Substitution: Implementações intercambiáveis  
**I** - Interface Segregation: Interfaces específicas e coesas  
**D** - Dependency Inversion: Depende de abstrações, não implementações  

### **Outros Princípios**

- ?? **DRY**: Validações centralizadas, mappers reutilizáveis
- ?? **KISS**: Código limpo, legível e direto ao ponto
- ?? **Clean Code**: Nomes descritivos, métodos pequenos

---

## ?? Integração SAP S/4HANA

### **Tipo de Integração: OData v4 / REST API**

**Endpoint Real SAP:**
```
https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
```

**Documentação:** SAP API Business Hub

### **Mapeamento SAP**

**Tabelas Representadas:**
- **VBAK**: Sales Document - Header Data
- **VBAP**: Sales Document - Item Data
- **KNA1**: Customer Master

**Campos Mapeados:**
- `VBELN` ? SalesOrderNumber
- `KUNNR` ? CustomerCode
- `MATNR` ? MaterialCode
- `CHARG` ? BatchNumber (rastreabilidade farmacêutica)

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

## ??? Dados Mockados

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

### **API Keys (Development)**

```
demo-api-key-12345
test-api-key-67890
ache-pharma-api-key
```

---

## ??? Tecnologias

- **.NET 8 (LTS)** - Framework principal
- **ASP.NET Core Web API** - API REST
- **FluentValidation** - Validação de dados
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API

---

## ?? Estrutura do Projeto

```
DesafioTecnico_Ache/
??? Controllers/          # Endpoints REST
??? Services/             # Lógica de negócio
??? Repositories/         # Acesso a dados (mockado)
??? Interfaces/           # Contratos
??? Models/               # Modelos de domínio
??? DTOs/                 # Data Transfer Objects
??? Validators/           # FluentValidation
??? Middlewares/          # Segurança e tratamento
??? Program.cs            # Configuração
```

---

## ?? Como Executar

### **Pré-requisitos**
- .NET 8 SDK
- Visual Studio 2022 ou VS Code

### **Passos**

1. **Clone o repositório**
```bash
git clone https://github.com/RenanMunizDev/DesafioTecnicoAche.git
cd DesafioTecnicoAche
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
https://localhost:5001/swagger
```

5. **Autorizar com API Key**
- Clique em "Authorize"
- Insira: `demo-api-key-12345`

---

## ?? Testes com Swagger

### **Teste 1: Buscar Pedido**
- Execute `GET /api/sap/salesorders/SO0000001000`
- ? Deve retornar pedido mockado

### **Teste 2: Criar Pedido**
- Execute `POST /api/sap/salesorders`
- Use o JSON de exemplo
- ? Pedido criado com sucesso

### **Teste 3: Validações**
- ? Sem API Key ? 401
- ? Cliente inválido ? 422
- ? Dados inválidos ? 400
- ? Exceder rate limit ? 429

---

## ?? Melhorias Futuras

### **Produção**
- SAP Cloud SDK for .NET
- OAuth 2.0 / SAML
- Polly (retry policies)
- Redis (cache)
- Circuit breaker

### **Testes**
- Testes unitários (xUnit)
- Testes de integração
- Cobertura > 80%

### **DevOps**
- CI/CD (GitHub Actions)
- Docker
- Kubernetes

---

## ????? Autor

**Desenvolvido com ?? para o desafio técnico Aché Laboratórios Farmacêuticos**

### **Competências Demonstradas**

? Arquitetura limpa e escalável  
? Código profissional  
? Segurança (OWASP)  
? Boas práticas (SOLID, DRY, KISS)  
? Padrões de projeto  
? Integração SAP S/4HANA  

---

[?? Voltar ao topo](#-sap-s4hana-sales-order-integration-api)

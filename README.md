# ?? SAP S/4HANA Sales Order Integration API

<div align="center">

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)
![SAP](https://img.shields.io/badge/SAP-S%2F4HANA-0FAAFF?style=for-the-badge&logo=sap)

**API REST profissional para integra��o com SAP S/4HANA - M�dulo SD (Sales & Distribution)**

</div>

---

## ?? Sobre o Projeto

API REST desenvolvida em **.NET 8 (LTS)** para integra��o com **SAP S/4HANA** - M�dulo **SD (Sales & Distribution)**, simulando opera��es de pedidos de vendas atrav�s de integra��o **OData v4 / REST API**.

Este projeto foi desenvolvido como parte do desafio t�cnico **Ach� Laborat�rios Farmac�uticos**, demonstrando:

? Arquitetura profissional e escal�vel  
? Boas pr�ticas de desenvolvimento (SOLID, DRY, KISS)  
? Seguran�a conforme OWASP Top 10  
? C�digo limpo e bem documentado  
? Padr�es de projeto reconhecidos  

---

## ??? Arquitetura e Padr�es de Projeto

### **Clean Architecture**

Separa��o clara de responsabilidades em camadas:
- **Controllers**: Camada de apresenta��o (API REST)
- **Services**: L�gica de neg�cio
- **Repositories**: Acesso a dados (integra��o SAP mockada)
- **Models**: Modelos de dom�nio

### **Padr�es de Projeto Aplicados**

#### 1. ?? **Repository Pattern**
- Abstra��o da camada de acesso a dados
- Interface `ISalesOrderRepository`
- Implementa��o mockada `SapSalesOrderRepository`
- Facilita troca de implementa��o (mock ? SAP real)

#### 2. ?? **Service Layer Pattern**
- L�gica de neg�cio isolada em servi�os
- Interface `ISalesOrderService`
- Implementa��o `SalesOrderService`
- Valida��es de regras de neg�cio

#### 3. ?? **Dependency Injection**
- Invers�o de controle (IoC)
- Baixo acoplamento entre componentes
- Facilita testes unit�rios

#### 4. ?? **DTO Pattern**
- Separa��o entre modelos de dom�nio e transfer�ncia de dados
- Requests e Responses espec�ficos para API

#### 5. ?? **Middleware Pattern**
- `GlobalExceptionHandlingMiddleware`: tratamento global de exce��es
- `ApiKeyAuthenticationMiddleware`: autentica��o por API Key
- `RateLimitingMiddleware`: prote��o contra abuso de API

---

## ?? Seguran�a (OWASP)

### **Implementa��es de Seguran�a Conforme OWASP Top 10 2021**

??? **A01:2021 � Broken Access Control**
- Autentica��o via API Key obrigat�ria
- Valida��o de chaves em toda requisi��o
- Logging de tentativas n�o autorizadas

??? **A03:2021 � Injection**
- Input validation com FluentValidation
- Sanitiza��o de entradas
- Uso de tipos fortemente tipados

??? **A04:2021 � Insecure Design**
- Rate Limiting (100 req/min)
- Prote��o contra DoS
- Timeout configur�vel

??? **A05:2021 � Security Misconfiguration**
- Diferentes configura��es por ambiente
- Logs detalhados apenas em Development
- N�o exposi��o de stack traces em produ��o

??? **A09:2021 � Security Logging**
- Logging estruturado com Serilog
- Auditoria de todas as opera��es
- Reten��o de logs (30 dias dev / 90 dias prod)

---

## ?? Princ�pios SOLID

**S** - Single Responsibility: Cada classe tem uma �nica responsabilidade  
**O** - Open/Closed: Extens�vel via interfaces, fechado para modifica��o  
**L** - Liskov Substitution: Implementa��es intercambi�veis  
**I** - Interface Segregation: Interfaces espec�ficas e coesas  
**D** - Dependency Inversion: Depende de abstra��es, n�o implementa��es  

### **Outros Princ�pios**

- ?? **DRY**: Valida��es centralizadas, mappers reutiliz�veis
- ?? **KISS**: C�digo limpo, leg�vel e direto ao ponto
- ?? **Clean Code**: Nomes descritivos, m�todos pequenos

---

## ?? Integra��o SAP S/4HANA

### **Tipo de Integra��o: OData v4 / REST API**

**Endpoint Real SAP:**
```
https://{host}/sap/opu/odata/sap/API_SALES_ORDER_SRV
```

**Documenta��o:** SAP API Business Hub

### **Mapeamento SAP**

**Tabelas Representadas:**
- **VBAK**: Sales Document - Header Data
- **VBAP**: Sales Document - Item Data
- **KNA1**: Customer Master

**Campos Mapeados:**
- `VBELN` ? SalesOrderNumber
- `KUNNR` ? CustomerCode
- `MATNR` ? MaterialCode
- `CHARG` ? BatchNumber (rastreabilidade farmac�utica)

---

## ?? Endpoints da API

### **Base URL**
```
https://localhost:{port}/api/sap/salesorders
```

### **1. GET - Buscar Pedido por N�mero**

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

### **Clientes Dispon�veis**

- `C001`: Drogaria S�o Paulo LTDA
- `C002`: Farm�cia Pague Menos S.A.
- `C003`: Drogasil S.A.
- `C004`: Raia Drogasil S.A.
- `C005`: Panvel Farm�cias S.A.

### **Materiais Dispon�veis**

- `M001`: Paracetamol 500mg - R$ 15,50
- `M002`: Ibuprofeno 600mg - R$ 22,80
- `M003`: Dipirona S�dica 500mg - R$ 18,90
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
- **FluentValidation** - Valida��o de dados
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documenta��o da API

---

## ?? Estrutura do Projeto

```
DesafioTecnico_Ache/
??? Controllers/          # Endpoints REST
??? Services/             # L�gica de neg�cio
??? Repositories/         # Acesso a dados (mockado)
??? Interfaces/           # Contratos
??? Models/               # Modelos de dom�nio
??? DTOs/                 # Data Transfer Objects
??? Validators/           # FluentValidation
??? Middlewares/          # Seguran�a e tratamento
??? Program.cs            # Configura��o
```

---

## ?? Como Executar

### **Pr�-requisitos**
- .NET 8 SDK
- Visual Studio 2022 ou VS Code

### **Passos**

1. **Clone o reposit�rio**
```bash
git clone https://github.com/RenanMunizDev/DesafioTecnicoAche.git
cd DesafioTecnicoAche
```

2. **Restaurar depend�ncias**
```bash
dotnet restore
```

3. **Executar a aplica��o**
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

### **Teste 3: Valida��es**
- ? Sem API Key ? 401
- ? Cliente inv�lido ? 422
- ? Dados inv�lidos ? 400
- ? Exceder rate limit ? 429

---

## ?? Melhorias Futuras

### **Produ��o**
- SAP Cloud SDK for .NET
- OAuth 2.0 / SAML
- Polly (retry policies)
- Redis (cache)
- Circuit breaker

### **Testes**
- Testes unit�rios (xUnit)
- Testes de integra��o
- Cobertura > 80%

### **DevOps**
- CI/CD (GitHub Actions)
- Docker
- Kubernetes

---

## ????? Autor

**Desenvolvido com ?? para o desafio t�cnico Ach� Laborat�rios Farmac�uticos**

### **Compet�ncias Demonstradas**

? Arquitetura limpa e escal�vel  
? C�digo profissional  
? Seguran�a (OWASP)  
? Boas pr�ticas (SOLID, DRY, KISS)  
? Padr�es de projeto  
? Integra��o SAP S/4HANA  

---

[?? Voltar ao topo](#-sap-s4hana-sales-order-integration-api)

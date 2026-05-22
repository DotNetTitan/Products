# Products API

A production-grade RESTful API for managing products built with .NET 8.0 using Clean Architecture, CQRS pattern, Entity Framework Core, and Serilog.

## Architecture

This project follows **Clean Architecture** principles with four distinct layers:

```
Products.Api            # Web API layer (Controllers, Middlewares, Filters, Authentication)
Products.Application    # Application layer (CQRS Handlers, Commands, Queries, Validators, Abstractions)
Products.Domain         # Domain layer (Entities, Interfaces)
Products.Infrastructure # Infrastructure layer (EF Core, Database, Configurations, Caching)
```

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 8.0 |
| Web Framework | ASP.NET Core |
| Database | SQL Server |
| ORM | Entity Framework Core 8.0 |
| Validation | FluentValidation |
| API Documentation | Swagger (Swashbuckle) |
| Caching | In-Memory Cache via `ICacheService` abstraction |
| Authentication | API Key (`X-API-Key` header) |
| Logging | Serilog (structured logging to console) |
| Testing | xUnit, NSubstitute, FluentAssertions |

## Project Structure

```
Products/
├── src/
│   ├── Products.Api/
│   │   ├── Auth/
│   │   │   └── ApiKeyAuthenticationHandler.cs # API key validation handler
│   │   ├── Controllers/
│   │   │   └── ProductsController.cs          # REST endpoints for products
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs            # Generic FluentValidation filter
│   │   ├── Middlewares/
│   │   │   ├── ExceptionHandlingMiddleware.cs # Global error handling
│   │   │   ├── RequestLoggingMiddleware.cs    # Structured request logging
│   │   │   └── MiddlewareExtensions.cs
│   │   ├── Program.cs                         # Application entry point
│   │   ├── appsettings.json
│   │   └── Products.Api.csproj
│   ├── Products.Application/
│   │   ├── Abstractions/
│   │   │   ├── IApplicationDbContext.cs
│   │   │   ├── ICacheService.cs
│   │   │   ├── ICommandHandler.cs
│   │   │   └── IQueryHandler.cs
│   │   ├── Common/
│   │   │   ├── Caching/
│   │   │   │   ├── CacheKeys.cs
│   │   │   │   └── CacheDurations.cs
│   │   │   └── Pagination/
│   │   │       └── PagedResponse.cs
│   │   └── Features/Products/
│   │       ├── CreateProduct/
│   │       ├── DeleteProduct/
│   │       ├── GetProducts/
│   │       ├── GetProductById/
│   │       ├── UpdateProduct/
│   │       └── Responses/                     # Shared response DTOs
│   ├── Products.Domain/
│   │   ├── Common/
│   │   │   └── IAuditable.cs
│   │   └── Entities/
│   │       └── Product.cs
│   └── Products.Infrastructure/
│       ├── Configurations/
│       │   └── ProductConfiguration.cs
│       ├── Data/
│       │   └── ApplicationDbContext.cs
│       ├── Migrations/
│       ├── Services/
│       │   └── CacheService.cs                # ICacheService implementation
│       └── DependencyInjection.cs
├── tests/
│   ├── Products.UnitTests/                    # 48 unit tests
│   ├── Products.IntegrationTests/             # 11 integration tests
│   └── Products.E2ETests/                     # 4 end-to-end tests
├── .github/workflows/
│   └── ci.yml                                # CI pipeline
├── Products.slnx
└── README.md
```

## Features

### Product Management
- **Create Product** - Create a new product with name, description, price, and stock quantity
- **Get All Products** - Retrieve paginated list of products with search, sort, and pagination
- **Get Product by ID** - Retrieve a specific product by its unique identifier (cached)
- **Update Product** - Update an existing product's details (cache-evicting)
- **Delete Product** - Soft delete a product (marks as deleted without physical removal)

### Technical Features
- **CQRS Pattern** - Separate command/query interfaces and handlers
- **Caching Abstraction** - `ICacheService` decouples application from `IMemoryCache`, allowing swap to Redis/Distributed cache
- **FluentValidation** - Declarative validation with generic `ValidationFilter<T>`
- **Global Exception Handling** - Centralized error handling with RFC 7807 ProblemDetails
- **API Key Authentication** - Validated via `X-API-Key` header
- **Serilog Logging** - Structured request logging with duration tracking
- **Soft Delete** - Products are marked as deleted rather than physically removed (global query filter)
- **Auditable Entities** - Automatic creation and update timestamps via `SaveChanges` override
- **Health Checks** - EF Core database health check at `/health`
- **CI/CD** - GitHub Actions workflow for build, test, and publish

## API Endpoints

All endpoints require an API key passed via the `X-API-Key` header.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/products` | Create a new product |
| GET | `/api/products` | Get all products (paginated, with search & sort) |
| GET | `/api/products/{id}` | Get a product by ID |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Request/Response Examples

#### Create Product
```json
POST /api/products
{
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "stockQuantity": 100
}
```

#### Response (201 Created)
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### Get Product by ID
```json
GET /api/products/550e8400-e29b-41d4-a716-446655440000

{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "stockQuantity": 100,
  "createdAtUtc": "2026-05-21T12:00:00Z"
}
```

#### Get Products (with pagination)
```
GET /api/products?Page=1&PageSize=10&Search=laptop&SortBy=price&Descending=true
```

| Query Param | Type | Default | Description |
|-------------|------|---------|-------------|
| Page | int | 1 | Page number (min 1) |
| PageSize | int | 10 | Items per page (1-100) |
| Search | string | null | Searches name and description |
| SortBy | string | null | Sort field: `name` or `price` |
| Descending | bool | false | Sort descending |

#### Response
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "Product Name",
      "description": "Product Description",
      "price": 99.99,
      "stockQuantity": 100,
      "createdAtUtc": "2026-05-21T12:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42
}
```

#### Update Product
```json
PUT /api/products/550e8400-e29b-41d4-a716-446655440000
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Updated Name",
  "description": "Updated Description",
  "price": 149.99,
  "stockQuantity": 50
}
```

Response: `204 No Content`

#### Delete Product
```
DELETE /api/products/550e8400-e29b-41d4-a716-446655440000
```

Response: `204 No Content`

## Validation Rules

### CreateProductCommand / UpdateProductCommand
| Field | Rules |
|-------|-------|
| Id | Required (Update only), must be a valid GUID |
| Name | Required, max 200 characters |
| Description | Required, max 2000 characters |
| Price | Required, must be >= 0 |
| StockQuantity | Required, must be >= 0 |

### GetProductsQuery
| Field | Rules |
|-------|-------|
| Page | Default: 1, Min: 1 |
| PageSize | Default: 10, Min: 1, Max: 100 |
| SortBy | Must be `name` or `price` if provided |

## Authentication

The API uses **API key authentication**. All endpoints require a valid API key sent via the `X-API-Key` header.

### Configuration

In `appsettings.json`:
```json
"ApiKey": {
  "Key": "your-api-key-here"
}
```

For local development, set the key via .NET User Secrets:
```bash
dotnet user-secrets set "ApiKey:Key" "your-generated-api-key"
```

### Using the API Key

Include the key in all requests:
```
X-API-Key: your-api-key
```

## Database

The project uses **SQL Server** with Entity Framework Core for data access.

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ProductsDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

For local development, use .NET User Secrets to override:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (Local or Express edition)
- Visual Studio 2022+, VS Code, or Rider

### Setup

1. **Clone and restore**
   ```bash
   git clone <repo-url>
   cd Products
   dotnet restore
   ```

2. **Update secrets**
   ```bash
   dotnet user-secrets init --project src/Products.Api
   dotnet user-secrets set "ApiKey:Key" "your-api-key"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update --project src/Products.Infrastructure --startup-project src/Products.Api
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/Products.Api
   ```

5. **Access Swagger UI**
   Open `http://localhost:5175/swagger`

## Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Products.UnitTests
dotnet test tests/Products.IntegrationTests
dotnet test tests/Products.E2ETests
```

### Test Breakdown
| Project | Type | Count | Description |
|---------|------|-------|-------------|
| UnitTests | Unit | 48 | Domain entities, handlers, validators |
| IntegrationTests | Integration | 11 | API endpoints via WebApplicationFactory |
| E2ETests | E2E | 4 | Full user scenarios |

## CI/CD

The project includes a GitHub Actions workflow (`.github/workflows/ci.yml`) that:
- Runs on push or pull request to `main`
- Restores, builds, and tests the solution
- Uploads test results as artifacts

### Setting up the Pipeline

1. Push the repository to GitHub
2. The workflow runs automatically on any push/PR to `main`
3. View pipeline runs: GitHub repository → **Actions** tab → **CI** workflow
4. Test results are uploaded as build artifacts — download them from the workflow run page

The pipeline uses `ubuntu-latest` with .NET 8.0 SDK. No additional secrets are needed for the build and test phase.

### Required Secrets for Production Deployment

If you extend the pipeline to deploy, add these secrets in GitHub → **Settings** → **Secrets and variables** → **Actions**:

| Secret | Description |
|--------|-------------|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Azure App Service publish profile |
| `API_KEY__KEY` | API key for authentication |
| `CONNECTION_STRINGS__DEFAULT_CONNECTION` | SQL Server connection string |

## Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ProductsDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "ApiKey": {
    "Key": ""
  }
}
```

Sensitive values (connection strings, API keys) should be stored in User Secrets, environment variables, or Azure Key Vault.

### Building and Running

```bash
dotnet build
dotnet run --project src/Products.Api
dotnet publish -c Release -o ./publish
```

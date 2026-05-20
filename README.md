# Products API

A RESTful API for managing products built with .NET 8.0 using Clean Architecture, CQRS pattern, and Entity Framework Core.

## Architecture

This project follows **Clean Architecture** principles with four distinct layers:

```
Products.Api            # Web API layer (Controllers, Middlewares, Filters)
Products.Application    # Application layer (CQRS Handlers, Commands, Queries, Validators)
Products.Domain         # Domain layer (Entities, Interfaces)
Products.Infrastructure # Infrastructure layer (EF Core, Database, Configurations)
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
| Caching | In-Memory Cache |

## Project Structure

```
Products/
├── Products.Api/
│   ├── Controllers/
│   │   └── ProductsController.cs      # REST endpoints for products
│   ├── Filters/
│   │   └── ValidationFilter.cs        # Request validation filter
│   ├── Middlewares/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── MiddlewareExtensions.cs
│   ├── Program.cs                     # Application entry point
│   ├── appsettings.json               # Configuration
│   └── Products.Api.csproj
├── Products.Application/
│   ├── Abstractions/
│   │   └── IApplicationDbContext.cs
│   ├── Common/
│   │   ├── Caching/
│   │   │   ├── CacheKeys.cs
│   │   │   └── CacheDurations.cs
│   │   └── Pagination/
│   │       └── PagedResponse.cs
│   └── Features/Products/
│       ├── CreateProduct/             # Create product feature (CQRS)
│       ├── DeleteProduct/             # Delete product feature (CQRS)
│       ├── GetProducts/               # Get all products feature (CQRS)
│       ├── GetProductById/            # Get single product feature (CQRS)
│       └── UpdateProduct/             # Update product feature (CQRS)
├── Products.Domain/
│   ├── Common/
│   │   └── IAuditable.cs              # Auditable interface
│   └── Entities/
│       └── Product.cs                 # Product entity
├── Products.Infrastructure/
│   ├── Configurations/
│   │   └── ProductConfiguration.cs    # EF Core entity configuration
│   ├── Data/
│   │   └── ApplicationDbContext.cs    # EF Core DbContext
│   ├── Migrations/                    # EF Core migrations
│   ├── DependencyInjection.cs          # DI configuration
│   └── Products.Infrastructure.csproj
└── Products.slnx
```

## Features

### Product Management
- **Create Product** - Create a new product with name, description, price, and stock quantity
- **Get All Products** - Retrieve paginated list of products with optional filtering
- **Get Product by ID** - Retrieve a specific product by its unique identifier
- **Update Product** - Update an existing product's details
- **Delete Product** - Soft delete a product (marks as deleted without physical removal)

### Technical Features
- **CQRS Pattern** - Command Query Responsibility Segregation for clear separation of concerns
- **FluentValidation** - Declarative validation for commands and queries
- **Global Exception Handling** - Centralized error handling middleware
- **In-Memory Caching** - Performance optimization for frequently accessed data
- **Soft Delete** - Products are marked as deleted rather than physically removed
- **Auditable Entities** - Track creation and update timestamps
- **API Validation Filter** - Automatic model validation for incoming requests

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/products` | Create a new product |
| GET | `/api/products` | Get all products (paginated) |
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

#### Response
```json
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
GET /api/products?pageNumber=1&pageSize=10
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

### Migrations

The project includes three migrations:
1. **InitialCreate** - Creates the Products table with all required fields
2. **AddSoftDelete** - Adds soft delete functionality (IsDeleted, DeletedAtUtc)
3. **AddUpdatedAtUtc** - Ensures UpdatedAtUtc field is properly tracked

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (Local or Express edition)
- Visual Studio 2022 or VS Code

### Setup

1. **Restore packages**
   ```bash
   dotnet restore
   ```

2. **Update connection string**
   Modify the connection string in `Products.Api/appsettings.json` to match your SQL Server instance.

3. **Apply migrations**
   ```bash
   cd Products.Infrastructure
   dotnet ef database update
   ```
   Or from the API project:
   ```bash
   cd Products.Api
   dotnet ef database update --startup-project ../Products.Api
   ```

4. **Run the application**
   ```bash
   dotnet run --project Products.Api
   ```

5. **Access Swagger UI**
   Open `http://localhost:5000/swagger` (or the port shown in your terminal)

## Validation Rules

### CreateProductCommand
| Field | Rules |
|-------|-------|
| Name | Required, max 100 characters |
| Description | Required, max 500 characters |
| Price | Required, must be >= 0 |
| StockQuantity | Required, must be >= 0 |

### UpdateProductCommand
| Field | Rules |
|-------|-------|
| Id | Required, must be a valid GUID |
| Name | Required, max 100 characters |
| Description | Required, max 500 characters |
| Price | Required, must be >= 0 |
| StockQuantity | Required, must be >= 0 |

### GetProductsQuery
| Field | Rules |
|-------|-------|
| PageNumber | Default: 1, Min: 1 |
| PageSize | Default: 10, Min: 1, Max: 100 |
| SearchTerm | Optional, searches in name and description |

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
  }
}
```

### appsettings.Development.json
Contains development-specific configuration (typically includes more verbose logging).

## Building and Running

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project Products.Api
```

### Publish
```bash
dotnet publish -c Release -o ./publish
```
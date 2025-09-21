# SimpleInventory

A simple inventory management system built with ASP.NET Core 8, Entity Framework Core, and Razor Pages. This guide will help you quickly set up, run, and test the project locally, including using Swagger for API testing and the built-in UI for manual testing.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- (Optional) [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or use the included SQLite database

---

## Project Structure

- `src/SimpleInventory.Web/` - Main ASP.NET Core web application (API + UI)
- `src/SimpleInventory.Data/` - Entity Framework Core data access layer
- `src/SimpleInventory.Domain/` - Domain models and business logic
- `tests/SimpleInventory.Tests/` - Unit and integration tests

---

## Quickstart

### 1. Clone the Repository

```powershell
git clone https://github.com/prodromos18/SimpleInventoryNet.git
cd SimpleInventoryNet
```

### 2. Restore Dependencies

```powershell
dotnet restore
dotnet build
```

### 3. Database Setup

By default, the project uses SQLite. The database file (`inventory.db`) is included in `src/SimpleInventory.Web/`. If you want to reset or update the database:

```powershell
cd src/SimpleInventory.Web
# create migrations (only needed if migrations are not present)
dotnet ef migrations add InitialCreate -p ../SimpleInventory.Data -s . 

# apply migrations to create/update the SQLite DB
dotnet ef database update -p ../SimpleInventory.Data -s .
```


### 4. Run the Application

From the root or the `src/SimpleInventory.Web/` folder:

```powershell
dotnet run --project src/SimpleInventory.Web
```

The app will start and listen on `https://localhost:5001` and/or `http://localhost:5000` (see console output).

---

## Testing the API with Swagger

1. Open your browser and navigate to:
   - `https://localhost:5001/swagger` or `http://localhost:5000/swagger`
2. Use the interactive Swagger UI to test API endpoints for Products and Categories.

---
---

## Testing the API with API calls

1. Run the following example calls in your terminal
   - **List products with paging** filtering curl "http://localhost:5000/api/products?q=book&categoryId=1&page=1&pageSize=10"
   - Get product by id
   curl http://localhost:5000/api/products/1
    - **Create a category**
    curl -X POST http://localhost:5000/api/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Category1"}' (Duplicate category names return 409 Conflict)
  - **Create a product (POST)**
  curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "My Book",
    "sku": "BOOK001",
    "categoryId": 1,
    "price": 9.99,
    "quantity": 10
  }'




---

## Using the Web UI

1. Open your browser and go to:
   - `https://localhost:5001/` or `http://localhost:5000/`
2. Use the navigation links to manage Products and Categories via the Razor Pages UI.

---

## Running Tests

From the root directory:

```powershell
dotnet test
```

This will run all unit and integration tests in the `tests/` folder.

---

## Configuration

- **Connection Strings:**
  - Edit `src/SimpleInventory.Web/appsettings.json` to change the database provider or connection string.
- **Environment:**
  - Use `appsettings.Development.json` for local overrides.

---

## Troubleshooting

- If you get errors about missing migrations or database, run:
  ```powershell
  dotnet ef database update --project src/SimpleInventory.Data --startup-project src/SimpleInventory.Web
  ```
- Make sure ports 5000/5001 are not blocked by other applications.
- For detailed errors, check the console output or logs.

---

## Useful Commands

- **Add a migration:**
  ```powershell
  dotnet ef migrations add MigrationName --project src/SimpleInventory.Data --startup-project src/SimpleInventory.Web
  ```
- **Update the database:**
  ```powershell
  dotnet ef database update --project src/SimpleInventory.Data --startup-project src/SimpleInventory.Web
  ```

---

---

## Known Limitations

- Some API endpoints (e.g., GET /api/products/{id}) may throw 500 errors.
- UI persistence for PUT operations is not fully implemented.
- Negative prices/quantities are not yet validated in Swagger.
- Seeded data is minimal.
- With some additional time, I am confident these would definitely be fixed, as I needed some more time to get accustomed with .NET


---


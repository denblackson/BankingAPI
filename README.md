# BankingAPI  

BankingAPI is an ASP.NET Core Web API for managing bank accounts and transactions.  

## API Endpoints  

The application exposes two main controllers: **Accounts** and **Transactions**.  

---

### 1) Accounts Controller  

| Method | Endpoint                               | Description                           |
|--------|----------------------------------------|---------------------------------------|
| GET    | `/api/Accounts/GetAllAccounts`         | Returns a list of all accounts.       |
| GET    | `/api/Accounts/GetAllAccountById/{id}` | Returns details of an account by ID.  |
| POST   | `/api/Accounts/CreateAccount`          | Creates a new account.                |
| PUT    | `/api/Accounts/UpdateAccount`          | Updates an existing account.          |
| DELETE | `/api/Accounts/DeleteAccountById/{id}` | Deletes an account by ID.             |

---

### 2) Transactions Controller  

| Method | Endpoint                                | Description                                 |
|--------|-----------------------------------------|---------------------------------------------|
| GET    | `/api/Transaction/GetAllTransactions`   | Returns a list of all transactions.         |
| GET    | `/api/Transaction/GetTransactionById/{id}` | Returns details of a transaction by ID.  |
| DELETE | `/api/Transaction/Delete/{id}`          | Deletes a transaction by ID.                |
| POST   | `/api/Transaction/Deposit`              | Deposits money into an account.             |
| POST   | `/api/Transaction/Withdraw`             | Withdraws money from an account.            |
| POST   | `/api/Transaction/Transfer`             | Transfers money between accounts.           |


## Prerequisites  

Before running the application locally, make sure you have the following installed:  

- [.NET 9 SDK](https://dotnet.microsoft.com/)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (local or remote instance)  
- A text editor or IDE of your choice, such as [Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/), or [VS Code](https://code.visualstudio.com/)  

## Installation  

Clone the repository and install dependencies:  

```bash
git clone https://github.com/denblackson/BankingAPI
cd BankingAPI
dotnet restore
```
## Configure Database Connection
Open appsettings.json and update the connection string to point to your local SQL Server instance:
```bash
"ConnectionStrings": {
    "BankingConnection": "Server=SERVER_NAME;Database=BankingAPIDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
Replace SERVER_NAME with the name of your local SQL Server.

## Apply database migrations:
```bash
dotnet ef database update
```
## Usage
Run the Application:
``` bash
dotnet run --project BankingAPI.Presentation
```
The API will be available at http://localhost:5005/swagger/index.html


# Project description
## Architecture

The BankingAPI project is implemented following **Clean Architecture** principles, separating concerns into multiple projects:

- **BankingAPI.Domain**  
  Contains the core domain entities (`Account`, `Transaction`) and business logic interfaces. This layer is independent of any infrastructure or presentation details.

- **BankingAPI.Application**  
  Implements application services and business rules. It handles `AccountService` and `TransactionService`, orchestrates operations, and interacts with repositories. This layer also contains **DTOs** (Data Transfer Objects) used for communication between layers.

- **BankingAPI.Infrastructure**  
  Provides concrete implementations for data access and external dependencies. It includes:
  - **Repositories** for `Account` and `Transaction` entities using **Entity Framework Core**.
  - Database context configuration for **SQL Server**.
  - Logging via **Serilog**.

- **BankingAPI.Presentation**  
  The Web API layer, exposing **Controllers**:
  - `AccountsController` for managing accounts.
  - `TransactionController` for deposits, withdrawals, and transfers.
  It handles HTTP requests and responses and maps DTOs to domain entities.

- **BankingAPI.Tests**  
  Contains unit tests for services and repositories to ensure correct behavior. Uses **xUnit** and **Moq** for mocking dependencies.

### Technologies Used

- **.NET 9 SDK** for the application framework.
- **Entity Framework Core** for ORM and database interactions.
- **SQL Server** as the relational database.
- **Serilog** for logging.
- **xUnit & Moq** for automated testing.







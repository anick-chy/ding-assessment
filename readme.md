## Architecture

The solution follows a layered architecture:

### Domain
- Contains core business logic  
- `Account` entity handles balance and transaction rules  
- `Transaction` represents individual operations  
- Enforces invariants such as:
  - No negative or zero transactions  
  - No overdraft allowed  

### Application
- Orchestrates use cases via `AccountService`  
- Handles interaction between domain and infrastructure  
- Prepares data for presentation  

### Infrastructure
- Implements persistence using a JSON file  
- `JsonAccountRepository` stores and retrieves transactions  
- Uses DTO mapping to keep domain isolated from serialization concerns  

### Presentation
- Console-based user interface  
- Handles user input and displays output  
- Delegates business logic to application layer  

---

## Design Decisions

### Persistence Choice
A JSON-based repository was chosen to keep the implementation simple and focused on domain logic. The design allows replacing it with a database (e.g., SQLite) without impacting other layers.

### Domain Modeling
- Transactions are treated as entities with identity  
- Balance is derived from transaction history  
- Clear separation between creation and rehydration (`Create` vs `Restore`)  

### Error Handling
- Domain-specific exceptions are used for business rules  
- Application layer does not suppress domain behavior  

### Extensibility
- Repository pattern allows easy swapping of persistence mechanism  
- Presentation layer is abstracted via an interface  

---

## Running the Application

1. Ensure `.NET 8` is installed  

2. Update `appsettings.json` if needed:

```json
{
  "Storage": {
    "FilePath": "transactions.json"
  }
}
```

## Run the Application
```bash
dotnet run
```

## Testing
The solution includes unit tests for:

- Domain logic (balance calculation, validation, invariants)
- Application behavior (interaction with repository and presentation)

Tests are written using xUnit and Moq.

### Notes
- The system assumes a single account for simplicity
- Concurrency is not handled as it is outside the scope of this application
- Data integrity is assumed at the persistence level
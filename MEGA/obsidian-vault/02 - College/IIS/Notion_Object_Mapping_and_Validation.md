# Notion Object Mapping & Validation Strategy

## 1. Why Validation? (Beyond Requirements)
In an **Interoperability** system, validation acts as **Contract Enforcement**:
- **Data Integrity:** Ensures external XML/JSON data is complete and formatted correctly before hitting business logic or the database.
- **Fail Fast:** Rejects malformed payloads at the API entry point, preventing deep-system errors.
- **Documentation:** Schemas (`.xsd`, `.json`) serve as a formal definition of "Valid Data" for other developers and frontends.

## 2. Validation Points in Application Flow
- **Input Validation (Requirement 1):** User uploads a file -> Validate against JSON Schema/XSD -> Map to `NotionObject` -> Save to DB.
- **Output Validation (Requirement 3):** Fetch from Live Notion API -> Generate internal XML -> **Validate XML** -> Perform XPath Filtering -> Return via SOAP.

## 3. The Flattening Strategy
Notion's raw response is deeply nested. We "flatten" it into a universal `NotionObject` to simplify system interactions.

### Benefits:
- **XPath Simplicity:** Changes a complex query into a simple `//NotionObject[contains(Title, 'term')]`.
- **Database Efficiency:** Maps directly to a single SQL table.
- **Maintainable Schemas:** Easier to write/update schemas for flat structures.

## 5. Hybrid API Strategy (Requirement 5)
To fulfill full CRUD requirements, we leverage Notion's **Pages API** alongside **Search**:

| Action | HTTP Method | Notion Endpoint | Purpose |
| :--- | :--- | :--- | :--- |
| **Search/Read** | `GET` | `/v1/search` | Discovery of pages/databases. |
| **Read One** | `GET` | `/v1/pages/{id}` | Retrieve specific page details. |
| **Create** | `POST` | `/v1/pages` | Create a new Notion page. |
| **Update** | `PATCH` | `/v1/pages/{id}`| Partial update of properties. |
| **Delete** | `DELETE` | `/v1/pages/{id}`| Archive the page (`archived: true`). |

## 6. The "Switch" Implementation
We use the **Strategy Pattern** in the `API` layer:
1. **Config:** `appsettings.json` contains `ApiMode: "public"` (Live) or `"custom"` (Local SQL).
2. **Interface:** `INotionRepository` defines the CRUD contract.
3. **DI Registration:** `Program.cs` registers either `NotionApiRepository` or `LocalDbRepository` based on the config.
4. **Result:** The same API endpoints/GraphQL work seamlessly regardless of the data source.

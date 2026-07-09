🛡️ Remote Backups

A client-server application for data storage and backup management. The project was developed with a focus on scalability, code quality, and robust architecture, demonstrating the practical implementation of design patterns within the .NET ecosystem.

💡 Architectural Concept

The project was implemented using Vertical Slice Architecture. The REST API code is organized around business functionalities (features).

As a result, each "vertical slice" (e.g., Upload) functions as a fully autonomous entity—spanning everything from HTTP request handling to data operations—which aligns perfectly with the CQRS approach. This minimizes coupling, simplifies testing, and ensures the application is ready for growth in an enterprise environment.

🚀 Tech stack

Backend (REST API)

.NET 10

Vertical Slices & Minimal APIs – maximum code cohesion around business functions.

Entity Framework Core

JWT (JSON Web Tokens) – Stateless, secure authorization and authentication.

Frontend (SPA)

Blazor WebAssembly – an interactive, client-side rendered interface written entirely in C#.

✨ Key Features

Full file lifecycle management – ​​seamless upload, secure download, and backup deletion.

Identity module – complete registration and login process with secure resource access.

User dashboard – clear view of metadata (file sizes, creation dates).

🧪 Testing and Quality Assurance

Unit tests: These verify key business logic within isolated handlers, without involving infrastructure. They are used to rapidly check validation rules and data transformations.

Integration tests with Testcontainers: Instead of using built-in mocks or in-memory databases, the application utilizes the Testcontainers library. It automatically spins up actual Docker containers (e.g., a relational database) for the duration of the tests. This provides 100% certainty that the entire flow—from the HTTP request, through the database, to the returned response—works correctly.

To run all tests (Docker Desktop must be running), use the command:
dotnet test

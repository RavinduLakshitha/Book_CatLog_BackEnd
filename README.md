# Book_CatLog
 
A high-performance backend service for managing a bookstore's inventory, built with .NET 9.0 and following Clean Architecture principles.

This service prioritizes scalability and maintainability through the use of CQRS (Command Query Responsibility Segregation) implemented with MediatR.

# 🛠️Technologies Used

Framework	    .NET SDK 9.0 (C#)
Architecture	Clean Architecture (4 Layers)
Patterns	    CQRS + MediatR
Database	    SQLite
ORM	          Dapper
Testing	      XUnit & Moq


# 🏗️ Architecture
The application is split into four distinct, highly decoupled layers:

1. BookCatalog.Domain: Entities (e.g., Book) and Core Business Logic. (Inner Layer)
2. BookCatalog.Application: CQRS Handlers, DTOs, and Interfaces (e.g., IBookRepository).
3. BookCatalog.Infrastructure: Implementation of data access (Dapper/SQLite) and configuration.4
4. BookCatalog.API: Presentation Layer (Controllers, DI, Swagger). (Outer Layer)
   
# 🛠️ Getting Started

Prerequisites
.NET SDK 9.0
Git

Installation

Clone the Repository:Bashgit clone [(https://github.com/RavinduLakshitha/Book_CatLog_BackEnd)]
cd BookCatalog


Restore Dependencies:
Bashdotnet restore

# ▶️ Run the Application

Navigate to the BookCatalog.API directory and use the dotnet watch command to run the application with automatic reloading on file changes.

> cd BookCatalog.API
>dotnet watch run --launch-profile https

The application will start on an address similar to https://localhost:5125/.

# 🌐 API Endpoints (Swagger)

The project includes Swagger for API documentation and testing. Once the application is running, open your browser to:
👉 https://localhost:<Port>/swagger

HTTP Method         Endpoint         Description
GET                 /api/books       Retrieves a list of all books (DTOs).
POST                /api/books       Creates a new book record.
PUT                 /api/books/{id}  Updates an existing book.
DELETE              /api/books/{id}  Deletes a book by ID.

# 🔬 Testing 
Unit tests for the Application Layer (CQRS handlers and DTO mapping) are implemented using XUnit and Moq.

How to Run Tests

Navigate to the root directory (BookCatalog/).
Execute the dotnet test command: dotnet test

# ⚙️ Configuration

The database connection string is managed in BookCatalog.API/appsettings.json.

JSON{
  "ConnectionStrings": {
    "SqliteConnection": "Data Source=books.db"
  }
}


The books.db file is automatically created and initialized with the required table schema when the application first starts (via DbInitializer in Program.cs). Note: This file is ignored by Git via the .gitignore file.


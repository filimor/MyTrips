# SRS

## Overview

This API stores and retrieves data about leisure trips, including flights, hosting and destinations. It also allows to write a review about the visited place.

## Requirements

- With an open theme.
- Build a **C# API** with access to a **SQL Server** database.
- Just a service where I can **store and retrieve data** of some type chosen by you using **HTTP** and **JSON**
- There are no restrictions on what can be developed.
- It is **essential** that you host your API and database on **Microsoft Azure** (within the free tier)
- It is also **essential** that you document your API.
- Use your creativity.

## General Constraints

## Technologies

- Framework: ASP.NET 8
- Database: SQL Server
- ORM: RepoDB
- Migrations: Fluent Migrator
- Logging: Serilog
- Documentation: Swagger/OpenAPI
- Mapping: AutoMapper
- Caching: Redis
- Containerization: Docker and Docker Compose
- CI/CD: GitHub Actions
- Testing: xUnit, Moq, Bogus and Fluent Assertions
- Code coverage: Coverlet
- Code quality: SonarLint
- Business rules: FLuent Validation

## Tools

- Visual Studio 2022 Community Edition
- VSCode
- SQL Server Management Studio
- ReSharper
- Azure Storage Explorer
- Insomnia
- Docker Desktop
- GitKraken
- Astah UML

## Use Case Diagrams

![Clients Management.png](diagrams/diagrams/Use%20Cases/Clients%20Management.png)

![Trips Management.png](diagrams/diagrams/Use%20Cases/Trips%20Management.png)

## ER Model

### Clients Table

|     | Field | Type         | Other Constraints |
| --- | ----- | ------------ | ----------------- |
| PK  | Id    | INT          | NOT NULL          |
|     | Name  | NVARCHAR(50) | NOT NULL          |
|     | Email | NVARCAHR(50) | NOT NULL, UNIQUE  |

### Trips Table

|     | Field            | Type | Other Constraints      |
| --- | ---------------- | ---- | ---------------------- |
| PK  | Id               | INT  | NOT NULL               |
|     | StartDate        | DATE | NOT NULL               |
|     | EndDate          | DATE | NULL                   |
| FK  | ClientId         | INT  | REFERENCES Clients(Id) |
| FK  | InboundFlightId  | INT  | REFERENCES Flight(Id)  |
| FK  | OutboundFlightId | INT  | REFERENCES Flight(Id)  |
| FK  | HotelId          | INT  | REFERENCES Hotels(Id)  |

### Flights Table

|     | Field     | Type     | Other Constraints |
| --- | --------- | -------- | ----------------- |
| PK  | Id        | INT      | NOT NULL          |
|     | Number    | NVARCHAR | NOT NULL          |
|     | Departure | DATETIME | NOT NULL          |
|     | Arrival   | DATETIME | NOT NULL          |

### Hotels Table

|     | Field         | Type         | Other Constraints |
| --- | ------------- | ------------ | ----------------- |
| PK  | Id            | INT          | NOT NULL          |
|     | Name          | NVARCHAR(50) | NOT NULL          |
|     | Rating        | INT          | NULL              |
| FK  | DestinationId | INT          | NOT NULL          |

### Destinations Table

|     | Field | Type         | Other Constraints |
| --- | ----- | ------------ | ----------------- |
| PK  | Id    | INT          | NOT NULL          |
|     | Name  | NVARCAHR(50) | NOT NULL          |

## Component Diagrams

![Architecture Diagram.png](diagrams/diagrams/Component%20Diagrams/Architecture%20Diagram.png)

![Component Diagram.png](diagrams/diagrams/Component%20Diagrams/Component%20Diagram.png)

## Sequence Diagrams

### Clients
![Get Clients.png](diagrams/diagrams/Sequence%20Diagrams/Clients/Get%20Clients.png)

![Get Client by Id.png](diagrams/diagrams/Sequence%20Diagrams/Clients/Get%20Client%20by%20Id.png)

![Create Client.png](diagrams/diagrams/Sequence%20Diagrams/Clients/Create%20Client.png)

![Update Client.png](diagrams/diagrams/Sequence%20Diagrams/Clients/Update%20Client.png)

![Delete Client.png](diagrams/diagrams/Sequence%20Diagrams/Clients/Delete%20Client.png)

### Trips

![Get Trips.png](diagrams/diagrams/Sequence%20Diagrams/Trips/Get%20Trips.png)

![Get my Trips.png](diagrams/diagrams/Sequence%20Diagrams/Trips/Get%20my%20Trips.png)

![Get Trip Details.png](diagrams/diagrams/Sequence%20Diagrams/Trips/Get%20Trip%20Details.png)

![Book a Trip.png](diagrams/diagrams/Sequence%20Diagrams/Trips/Book%20a%20Trip.png)

![Cancel a Trip.png](diagrams/diagrams/Sequence%20Diagrams/Trips/Cancel%20a%20Trip.png)

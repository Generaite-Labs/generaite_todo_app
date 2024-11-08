ToDo.sln
│
├── src/
│   ├── ToDo.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Aggregates/
│   │   ├── Events/
│   │   ├── Common/
│   │   └── Interfaces/
│   │
│   ├── ToDo.Application/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Mappers/
│   │   ├── Exceptions/
│   │   └── Validators/
│   │
│   ├── ToDo.Infrastructure/
│   │   ├── Configurations/
│   │   ├── Migrations/
│   │   ├── Repositories/
│   │   ├── Identity/
│   │   ├── Logging/
│   │   ├── Persistence/
│   │   └── ExternalServices/
│   │
│   ├── ToDo.API/
│   │   ├── Controllers/
│   │   ├── Hubs/
│   │   ├── Filters/
│   │   ├── Services/
│   │   └── Startup.cs
│   │
│   ├── ToDo.WebClient/
│   │   ├── Pages/
│   │   ├── Components/
│   │   ├── Services/
│   │   ├── Auth/
│   │   └── wwwroot/
│   │       ├── css/
│   │       └── js/
│   │
│   └── ToDo.Shared/
│       ├── Models/
│       └── Constants/
│
├── tests/
│   ├── ToDo.Domain.Tests/
│   ├── ToDo.Application.Tests/
│   ├── ToDo.Infrastructure.Tests/
│   ├── ToDo.API.Tests/
│   ├── ToDo.WebClient.Tests/
│   ├── ToDo.IntegrationTests/
│   └── ToDo.E2ETests/
│       └── Features/
│
├── docs/
│   ├── Architecture/
│   ├── API/
│   └── UserGuide/
│
├── scripts/
│   ├── Build/
│   └── Deploy/
│
└── SolutionItems/
    ├── .gitignore
    ├── README.md
    └── Directory.Build.props
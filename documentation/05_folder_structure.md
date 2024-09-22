ToDo.sln
│
├── src/
│   ├── ToDo.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Aggregates/
│   │   ├── Events/
│   │   └── Interfaces/
│   │
│   ├── ToDo.Application/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Validators/
│   │
│   ├── ToDo.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   ├── Identity/
│   │   ├── Logging/
│   │   └── ExternalServices/
│   │
│   ├── ToDo.API/
│   │   ├── Controllers/
│   │   ├── Hubs/
│   │   ├── Filters/
│   │   └── Startup.cs
│   │
│   ├── ToDo.WebClient/
│   │   ├── Pages/
│   │   ├── Components/
│   │   ├── Services/
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
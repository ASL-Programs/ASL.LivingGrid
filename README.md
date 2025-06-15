# ASL.LivingGrid

**Innovative, secure and fully modular enterprise management and analytics platform**

## Overview

ASL.LivingGrid is a comprehensive enterprise platform consisting of 5 independent modules:

1. **WebAdminPanel** - Blazor Server admin interface with Kestrel self-hosting
2. **ManagerPanel** - WPF desktop application for property management
3. **SmartCustomerApp** - .NET MAUI mobile app for iOS/Android
4. **ReportingDesktop** - WPF desktop reporting application
5. **ReportingMobile** - .NET MAUI mobile reporting app

## Key Features

- **4-Language Support**: Azerbaijani (AZ), English (EN), Turkish (TR), Russian (RU)
- **Modular Architecture**: Each module is completely independent with its own solution
- **Enterprise Security**: 2FA, SSO, audit logs, role-based permissions
- **Plugin System**: Extensible architecture with plugin support
- **UI-Driven Configuration**: All settings managed through user interface
- **Cross-Platform**: Desktop (Windows), Web, Mobile (iOS/Android)

## Technology Stack

- **.NET 9.0** - Core framework
- **Blazor Server** - Web admin interface
- **WPF** - Desktop applications
- **.NET MAUI** - Cross-platform mobile development
- **Kestrel** - Self-hosted web server
- **Entity Framework Core** - Data access
- **SignalR** - Real-time communication

## Project Structure

```
ASL.LivingGrid/
├── WebAdminPanel/          # Blazor admin interface
├── ManagerPanel/           # WPF property management
├── SmartCustomerApp/       # MAUI mobile customer app
├── ReportingDesktop/       # WPF reporting application
├── ReportingMobile/        # MAUI mobile reporting
├── Shared/                 # Common libraries and resources
├── Docs/                   # Documentation and guides
├── Scripts/                # Build and deployment scripts
└── ASL.LivingGrid.sln     # Master solution file
```

## Getting Started

1. **Prerequisites**: .NET 9.0 SDK, Visual Studio 2022 17.8+
2. **Clone**: `git clone [repository-url]`
3. **Build**: Open `ASL.LivingGrid.sln` in Visual Studio
4. **Run**: Each module can be run independently from its own solution

## Development Workflow

Please read `Read before you start working.md` before beginning any development work. The project follows strict Core Rules and workflow guidelines with progress tracking through TODO.md files.

## Contact

**Author & Maintainer**: Vusal Azer oglu Mastaliyev  
**Email**: mr.lasuv@gmail.com  
**Phone**: +994513331383  
**Role**: Lead Developer, System Architect, Enterprise Solution Lead

## License

All rights reserved. Enterprise solution by ASL.

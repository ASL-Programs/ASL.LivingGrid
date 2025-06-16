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
- **Theme Marketplace**: WebAdminPanel includes a page to list available themes,
  import new ones, or export existing styles. Themes reside in `wwwroot/themes`
  and custom themes can be added by placing a folder there or using the import
  option.

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
├── For Developer/          # Module-specific developer docs
└── ASL.LivingGrid.sln     # Master solution file
```

## Getting Started

### 1. Prerequisites

- .NET **9.0** SDK (see installation steps below)
- Visual Studio 2022 17.8+

### 2. Clone the Repository

```bash
git clone [repository-url]
cd ASL.LivingGrid
```

### 3. Install the .NET 9.0 SDK

If `dotnet --list-sdks` does not show a 9.0 SDK, use the official install script:

```bash
curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$PATH:$DOTNET_ROOT"
```

### 4. Build & Test

Use the provided PowerShell script to restore packages, build the solution and
run all tests:

```powershell
pwsh ./Scripts/build.ps1
```

You can also execute the commands manually:

```bash
dotnet build ASL.LivingGrid.sln
dotnet test ASL.LivingGrid.sln --no-build
```

### 5. Storybook

Component önizləmələri üçün Storybook konfiqurasiyası `WebAdminPanel` modulunda
yer alır. Aşağıdakı əmrlə Storybook serverini işə sala bilərsiniz:

```bash
cd WebAdminPanel
npm run storybook
```

### 6. Run

Each module can be run independently from its own solution using `dotnet run`.

### Offline Assets

Marketplace previews reference local SVG images stored under
`WebAdminPanel/ASL.LivingGrid.WebAdminPanel/wwwroot/images/`. These lightweight
placeholders load even without internet access.

### 7. Deployment

Publish release binaries with the deploy script:

```bash
bash ./Scripts/deploy.sh [Release|Debug]
```

Artifacts are placed in the `publish/` directory.

### 8. Changelog & Documentation

Change qeydlərini yaratmaq üçün:

```bash
bash ./Scripts/generate-changelog.sh
```

Sənədləşməni generasiya etmək üçün `docfx` istifadə olunur:

```bash
bash ./Scripts/generate-docs.sh
```

### 9. Configuration

To enforce HTTPS redirection when hosting the WebAdminPanel, set the
`Security:RequireHttps` key in `appsettings.json` (or as an environment
variable). The previous `ForceHttps` key is still accepted for backwards
compatibility but will be removed in future versions.

Slack integration can be enabled by setting
`Notifications:EnableSlackNotifications` to `true` and specifying the
`Notifications:SlackWebhookUrl` in `appsettings.json`.

Wireframe previews are protected with a secret token. The default
`appsettings.json` leaves `Security:PreviewSecret` blank, so you must
provide `Security__PreviewSecret` via an environment variable or the
`dotnet user-secrets` tool before launch. **Do not store the value in
`appsettings.json`.** Without this secret the application refuses to
start.

Localization update checks are controlled by
`Localization:UpdateIntervalMinutes`. This sets how often the
`LocalizationUpdateService` looks for new language packs (default: 30
minutes).

To automatically create a default administrator account in the WebAdminPanel,
set the `DEFAULT_ADMIN_EMAIL` and `DEFAULT_ADMIN_PASSWORD` environment variables
before first launch. If these variables are not defined, no admin user is
created.

## Development Workflow

Please read `Read before you start working.md` before beginning any development work. The project follows strict Core Rules and workflow guidelines with progress tracking through TODO.md files. Continuous integration runs via GitHub Actions using `.github/workflows/dotnet.yml`. Deployment with automatic rollback is handled by '.github/workflows/cicd.yml'.

## Contact

**Author & Maintainer**: Vusal Azer oglu Mastaliyev  
**Email**: mr.lasuv@gmail.com  
**Phone**: +994513331383  
**Role**: Lead Developer, System Architect, Enterprise Solution Lead

## License

This project is released under the [MIT License](LICENSE.txt).

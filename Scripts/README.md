# Scripts

Utility scripts to build, test and deploy the solution.

- **build.ps1** – PowerShell script that restores packages, builds all projects and runs the unit tests. Usage:
  ```powershell
  pwsh ./Scripts/build.ps1 [Release|Debug]
  ```
- **deploy.sh** – Bash script that publishes the solution to a `publish` folder. Usage:
  ```bash
  bash ./Scripts/deploy.sh [Release|Debug]
  ```

Both scripts expect .NET 9 SDK to be installed. They automatically detect their
own location, so you can run them from any directory.

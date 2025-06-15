@echo off
echo ASL LivingGrid Web Admin Panel Launcher
echo ======================================
echo.
echo Select launch mode:
echo 1. Standalone EXE with Tray Icon
echo 2. Web Hosting Mode (Development)
echo 3. Windows Service Mode
echo 4. Exit
echo.
set /p choice="Enter your choice (1-4): "

if "%choice%"=="1" goto standalone
if "%choice%"=="2" goto webhosting
if "%choice%"=="3" goto service
if "%choice%"=="4" goto exit
goto invalid

:standalone
echo.
echo Starting in Standalone Mode with Tray Icon...
echo The application will run in the system tray.
echo Right-click the tray icon to access options.
echo.
start "" "ASL.LivingGrid.WebAdminPanel.exe" --standalone
goto end

:webhosting
echo.
echo Starting in Web Hosting Mode...
echo The application will run as a console application.
echo.
start "" "ASL.LivingGrid.WebAdminPanel.exe"
goto end

:service
echo.
echo Starting as Windows Service...
echo Note: Service must be installed first using:
echo sc create "ASL LivingGrid" binPath= "%CD%\ASL.LivingGrid.WebAdminPanel.exe"
echo.
net start "ASL LivingGrid"
goto end

:invalid
echo.
echo Invalid choice. Please try again.
echo.
pause
goto start

:exit
echo.
echo Goodbye!
goto end

:end
echo.
echo Application started. Check system tray or console window.
pause

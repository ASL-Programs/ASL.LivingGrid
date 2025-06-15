using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TrayIconService : IHostedService, IDisposable
{
    private readonly ILogger<TrayIconService> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private NotifyIcon? _notifyIcon;
    private readonly string _appName = "ASL LivingGrid Web Admin Panel";

    public TrayIconService(ILogger<TrayIconService> logger, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Only create tray icon on Windows
            if (OperatingSystem.IsWindows())
            {
                CreateTrayIcon();
                _logger.LogInformation("Tray icon service started");
            }
            else
            {
                _logger.LogInformation("Tray icon not supported on this platform");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start tray icon service");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _notifyIcon?.Dispose();
            _logger.LogInformation("Tray icon service stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping tray icon service");
        }

        return Task.CompletedTask;
    }

    private void CreateTrayIcon()
    {
        if (!OperatingSystem.IsWindows()) return;

        try
        {
            // Create a simple icon (in production, use a proper .ico file)
            using var bitmap = new Bitmap(16, 16);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Blue, 0, 0, 16, 16);
            graphics.DrawString("A", new Font("Arial", 10, FontStyle.Bold), Brushes.White, 0, 0);
            
            var icon = Icon.FromHandle(bitmap.GetHicon());

            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = _appName,
                Visible = true
            };

            // Create context menu
            var contextMenu = new ContextMenuStrip();
            
            contextMenu.Items.Add("Open Admin Panel", null, (s, e) => OpenAdminPanel());
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Show Console", null, (s, e) => ShowConsole());
            contextMenu.Items.Add("Hide Console", null, (s, e) => HideConsole());
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApplication());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => OpenAdminPanel();

            // Show balloon tip on startup
            _notifyIcon.ShowBalloonTip(3000, _appName, "Application is running in the system tray", ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create tray icon");
        }
    }

    private void OpenAdminPanel()
    {
        try
        {
            var url = "http://localhost:5000";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            _logger.LogInformation("Opened admin panel in browser");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open admin panel");
        }
    }

    private void ShowConsole()
    {
        if (OperatingSystem.IsWindows())
        {
            var handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, 5); // SW_SHOW
                _logger.LogInformation("Console window shown");
            }
        }
    }

    private void HideConsole()
    {
        if (OperatingSystem.IsWindows())
        {
            var handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, 0); // SW_HIDE
                _logger.LogInformation("Console window hidden");
            }
        }
    }

    private void ExitApplication()
    {
        _logger.LogInformation("Exit requested from tray icon");
        _appLifetime.StopApplication();
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
    }

    // Windows API imports for console manipulation
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}

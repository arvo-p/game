using System.Runtime.InteropServices;

namespace game;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
	[DllImport("kernel32", SetLastError = true)]
    private static extern bool AttachConsole(int dwProcessId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [STAThread]
    static void Main()
    {
       AttachConsole(-1);
       ApplicationConfiguration.Initialize();
       Application.Run(new Form1());
    }    
}

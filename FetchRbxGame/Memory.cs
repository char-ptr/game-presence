using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace FetchRbxGame;

public class GameMemory
{
    public Process? gameProcess;

    public GameMemory(string GameName)
    {
        gameProcess = Process.GetProcessesByName(GameName).First();
    }

    public void OpenProcess(string GameName)
    {
        gameProcess = Process.GetProcessesByName(GameName).First();
    }
    public byte[] ReadProcessMem(IntPtr Address, int size)
    {
        byte[] buf = new byte[size];
        WindowsAPI.ReadProcessMemory(gameProcess.Handle, Address, buf, size, out var bytesread);
        return buf;
    }
    public T ReadProcessMem<T>(IntPtr Address) where T : struct
    {
        if (gameProcess == null)
        {
            throw new Exception("No process is currently open");
        }
        T[] buffer = new T[Marshal.SizeOf<T>()];
        WindowsAPI.ReadProcessMemory(gameProcess.Handle, Address, buffer, Marshal.SizeOf<T>(), out var bytesread);
        return buffer.First();
    }
}
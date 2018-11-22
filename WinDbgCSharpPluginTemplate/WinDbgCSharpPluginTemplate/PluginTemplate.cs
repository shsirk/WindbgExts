using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WinDbgCSharpPluginTemplate.Debugger;

namespace WinDbgCSharpPluginTemplate
{
    public enum HRESULT : uint
    {
        S_OK = 0,
        E_ABORT = 4,
        E_ACCESSDENIED = 0x80070005,
        E_FAIL = 0x80004005,
        E_HANDLE = 0x80070006,
        E_INVALIDARG = 0x80070057,
        E_NOINTERFACE = 0x80004002,
        E_NOTIMPL = 0x80004001,
        E_OUTOFMEMORY = 0x8007000E,
        E_POINTER = 0x80004003,
        E_UNEXPECTED = 0x8000FFFF

    }
    public class WinDbgCSharpPluginTemplate
    {
        [DllImport("dbgeng.dll")]
        internal static extern uint DebugCreate(ref Guid InterfaceId, [MarshalAs(UnmanagedType.IUnknown)] out object Interface);

        internal static HRESULT LastHR;

        internal static IDebugClient5 client = null;
        internal static IDebugControl6 control = null;

        private static HRESULT Int2HResult(int Result)
        {
            return Int2HResult(BitConverter.ToUInt32(BitConverter.GetBytes(Result), 0));
        }

        private static HRESULT Int2HResult(uint Result)
        {
            HRESULT hr = HRESULT.E_UNEXPECTED;
            try
            {
                hr = (HRESULT)Result;
            }
            catch { }
            return hr;
        }

        private static IDebugClient CreateIDebugClient()
        {
            Guid guid = new Guid("27fe5639-8407-4f47-8364-ee118fb08ac8");
            object obj;
            var hr = DebugCreate(ref guid, out obj);
            if (hr < 0)
            {
                LastHR = Int2HResult(hr);
                return null;
            }
            IDebugClient client = (IDebugClient5)obj;
            return client;
        }

        internal static void InitializeApis()
        {
            LastHR = HRESULT.S_OK;
            if (client == null)
            {
                try
                {
                    client = (IDebugClient5)CreateIDebugClient();
                    control = (IDebugControl6)client;
                }
                catch
                {
                    LastHR = HRESULT.E_UNEXPECTED;
                }
            }
        }

        internal static string GetLogPath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(assemblyPath), "windbg_template_log.txt");
        }

        #region Mandatory

        [DllExport]
        public static HRESULT DebugExtensionInitialize(ref uint Version, ref uint Flags)
        {
            uint Major = 1;
            uint Minor = 0;
            Version = (Major << 16) + Minor;
            Flags = 0;

            InitializeApis();
            if (client == null || control == null)
                return HRESULT.E_FAIL;

            return HRESULT.S_OK;
        }

        [DllExport]
        public static void DebugExtensionNotify(uint Notify, ulong Argument)
        {

        }

        [DllExport]
        public static HRESULT DebugExtensionUninitialize()
        {
            return HRESULT.S_OK;
        }

        #endregion

        #region Commands


        [DllExport]
        public static HRESULT help(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string Args)
        {
            WriteLine("Custom Windbg extension template with C#\n");
            return HRESULT.S_OK;
        }

        #endregion

        #region Helpers

        private static readonly string pFormat = String.Format(":x{0}", Marshal.SizeOf(IntPtr.Zero) * 2);
        public static string PointerFormat(string Message)
        {
            return Message.Replace(":%p", pFormat);
        }

        public static void Write(string Message, params object[] Params)
        {
            if (Params == null)
                Out(Message);
            else
                Out(String.Format(PointerFormat(Message), Params));
        }

        public static void WriteLine(string Message, params object[] Params)
        {
            if (Params == null)
                Out(Message);
            else
                Out(String.Format(PointerFormat(Message), Params));
            Out("\n");
        }

        public static void WriteDml(string Message, params object[] Params)
        {
            if (Params == null)
                OutDml(Message);
            else
                OutDml(String.Format(PointerFormat(Message), Params));
        }

        public static void WriteDmlLine(string Message, params object[] Params)
        {
            if (Params == null)
                OutDml(Message);
            else
                OutDml(String.Format(PointerFormat(Message), Params));
            Out("\n");
        }
        public static void Out(string Message)
        {
            if (control != null)
                control.ControlledOutput(DEBUG_OUTCTL.ALL_CLIENTS, DEBUG_OUTPUT.NORMAL, Message);
        }

        public static void OutDml(string Message)
        {
            if (control != null)
                control.ControlledOutput(DEBUG_OUTCTL.ALL_CLIENTS | DEBUG_OUTCTL.DML, DEBUG_OUTPUT.NORMAL, Message);
        }

        #endregion
    }
}

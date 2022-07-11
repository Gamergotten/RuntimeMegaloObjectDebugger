using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace RuntimeMegaloObjectDebugger
{
    public class Mem
    {
        // CONSTANTS 
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        // DYNAMIC VARIABLES
        private Process? hooked_process;
        private IntPtr p_handle;

        // CORE PROCESS FUNCTIONS
        public bool hook_and_open_process(string p_name, bool read_only)
        {
            //var amogus = Process.GetProcesses();
            hooked_process = Process.GetProcessesByName(p_name).FirstOrDefault();
            if (hooked_process == null) 
                return false; // skip script if null
            //p_handle = OpenProcess(read_only? PROCESS_WM_READ : PROCESS_ALL_ACCESS, false, hooked_process.Id);
            p_handle = hooked_process.Handle; // this should be significantly more efficient
            return p_handle != IntPtr.Zero;
        }
        public void close_or_clear_process()
        {
            hooked_process = null; p_handle = IntPtr.Zero;
        }

        // CORE MEM FUNCTIONS
        public byte[]? read_p_mem(long address, int length) // read length from address, return read mem (byte[]?)
        {
            if (hooked_process == null)
                return null;
            byte[] buffer = new byte[length];
            // return ReadProcessMemory((int)p_Handle, address, buffer, buffer.Length) ? null : buffer; // this is probably more efficient, but needs more research
            int bytesWritten = 0;
            if (!ReadProcessMemory(p_handle, (IntPtr)address, buffer, buffer.Length, ref bytesWritten))
                return null;
            return buffer;
        }
        public bool write_p_mem(long address, byte[] write) // write bytes at address, return completion status (bool)
        {
            if (hooked_process == null)
                return false;
            int bytesWritten = 0;
            return WriteProcessMemory(p_handle, (IntPtr)address, write, write.Length, ref bytesWritten);
        }


        public int bytes_to_int(byte[] bytes)
        {
            if (bytes == null)
                return -1;
            int converted_int = 0;
            for (int i = 0; i < bytes.Length; i++) 
            {
                converted_int += bytes[i] << (8 * i);
            }
            return converted_int;
        }

        // ADVANCED MEM FUNCTIONS
        // READING
        public int read_int8(long address)
        {
            byte[]? read_var = read_p_mem(address, 1);
            return read_var != null? read_var[0] : -1;
        }
        public int read_int16(long address)
        {
            byte[]? read_var = read_p_mem(address, 2);
            return read_var != null ? BitConverter.ToInt16(read_var) : -1;
        }
        public int read_int24(long address)
        {   // this one will surely have issues lol
            byte[]? read_var = read_p_mem(address, 3);
            return read_var != null ? bytes_to_int(read_var) : -1;
        }
        public int read_int32(long address)
        {
            byte[]? read_var = read_p_mem(address, 4);
            return read_var != null ? BitConverter.ToInt32(read_var) : -1;
        }
        public long read_int64(long address)
        {
            byte[]? read_var = read_p_mem(address, 8);
            return read_var != null ? BitConverter.ToInt64(read_var) : -1;
        }

        // WRITING
        public bool write_int8(long address, byte content)
        {
            return write_p_mem(address, new byte[1] { content });
        }
        public bool write_int16(long address, short content)
        {
            return write_p_mem(address, BitConverter.GetBytes(content));
        }
        public bool write_int24(long address, int content)
        {   // wow that was annoying
            return write_p_mem(address, BitConverter.GetBytes(content).Take(3).ToArray());
        }
        public bool write_int32(long address, int content)
        {
            return write_p_mem(address, BitConverter.GetBytes(content));
        }
        public bool write_int64(long address, long content)
        {
            return write_p_mem(address, BitConverter.GetBytes(content));
        }

        // POINTER READING
        public long return_module_address_by_name(string module_name) // this needs to be optimised, cycling through all 180 procs each time is prolly expensive
        {
            try // modules access exception
            {
                for (int i = 0; i < hooked_process.Modules.Count; i++)
                {
                    if (hooked_process.Modules[i].ModuleName == module_name)
                        return (long)hooked_process.Modules[i].BaseAddress;
                }
            }
            catch
            {
                return -1;
            }
            return -1;
        }
        public long read_pointer(string module_base, long offset)
        {
            if (hooked_process == null)
                return -1;
            long address_current = 0;
            if (!string.IsNullOrEmpty(module_base))
            {
                address_current = return_module_address_by_name(module_base);
                if (address_current == -1) return -1;
            }
            return read_int64(address_current + offset);
        }
        public long read_pointer(string module_base, long[] offset) // multilevel version
        {
            if (hooked_process == null)
                return -1;
            long address_current = 0;
            if (!string.IsNullOrEmpty(module_base))
            {
                address_current = return_module_address_by_name(module_base);
                if (address_current == -1) return -1;
            }
            for (int i = 0; i < offset.Length; i++)
            {
                address_current = read_int64(address_current + offset[i]);
            }
            return address_current;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_TCP_Server.Models
{
    public static class DataType
    {
        public static string CPUUsage = "%%cpu_usage%%";
        public static string AvailableRAM = "%%available_ram%%";
        public static string TotalRAM = "%%total_ram%%";
        public static string UsedRAM = "%%used_ram%%";
        public static string Processes = "%%processes%%";
        public static string DiskUsage = "%%disk_usage%%";
        public static string DiskRead = "%%disk_read%%";
        public static string DiskWrite = "%%disk_write%%";
        public static string TcpConnections = "%%tcp_connections%%";
    }
}

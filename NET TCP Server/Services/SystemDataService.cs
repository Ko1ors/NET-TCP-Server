using NET_TCP_Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET_TCP_Server.Services
{

    public class SystemDataService : IDataService
    {
        private Dictionary<string, PerformanceCounter> PerfCounters { get; set; } 

        public SystemDataService()
        {
            PerfCounters = new Dictionary<string, PerformanceCounter>();
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            var diskReadCounter = new PerformanceCounter("PhysicalDisk", "Avg. Disk Bytes/Read", "_Total");
            var diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Avg. Disk Bytes/Write", "_Total");
            PerfCounters.Add(DataType.CPUUsage, cpuCounter);
            PerfCounters.Add(DataType.AvailableRAM, new PerformanceCounter("Memory", "Available MBytes"));
            PerfCounters.Add(DataType.TotalRAM, new PerformanceCounter("Memory", "Commit Limit"));
            PerfCounters.Add(DataType.DiskUsage, diskCounter);
            PerfCounters.Add(DataType.DiskRead, diskReadCounter);
            PerfCounters.Add(DataType.DiskWrite, diskWriteCounter);
            PerfCounters.Add(DataType.TcpConnections, new PerformanceCounter("TCPv4", "Connections Established"));
            cpuCounter.NextValue();
            diskCounter.NextValue();
            diskReadCounter.NextValue();
            diskWriteCounter.NextValue();   
            Thread.Sleep(1000);
        }

        public Dictionary<string, string> GetData()
        {
            var dict = new Dictionary<string, string>();
            var availableRAM = GetAvailableRAM();
            var totalRAM = GetTotalRAM();
            var usedRAM = totalRAM - availableRAM;
            dict.Add(DataType.CPUUsage, MathF.Round(GetCPUUsage()) + "%");
            dict.Add(DataType.AvailableRAM, availableRAM + "MB");
            dict.Add(DataType.TotalRAM, totalRAM + "MB");
            dict.Add(DataType.UsedRAM, usedRAM + "MB");
            dict.Add(DataType.Processes, GetProcesses());
            dict.Add(DataType.DiskUsage, MathF.Round(GetDiskUsage()) + "%");
            dict.Add(DataType.DiskRead, MathF.Round(GetDiskReadUsage() / 1024f) + "KB");
            dict.Add(DataType.DiskWrite, MathF.Round(GetDiskWriteUsage() / 1024f) + "KB");
            dict.Add(DataType.TcpConnections, GetTCPConnections().ToString());
            return dict;
        }

        private float GetCPUUsage()
        {
            return PerfCounters[DataType.CPUUsage].NextValue();
        }

        private float GetAvailableRAM()
        {
            return PerfCounters[DataType.AvailableRAM].NextValue();
        }

        private float GetTotalRAM()
        {
            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();
            foreach (ManagementObject result in results)
            {
                return MathF.Round(float.Parse(result["TotalVirtualMemorySize"].ToString()) / 1024f);
            }
            return 0;
        }

        private string GetProcesses()
        {
            return string.Join(", ", Process.GetProcesses().Select(p => p.ProcessName));
        }

        private float GetDiskUsage()
        {
            return PerfCounters[DataType.DiskUsage].NextValue();
        }

        private float GetDiskReadUsage()
        {
            return PerfCounters[DataType.DiskRead].NextValue();
        }

        private float GetDiskWriteUsage()
        {
            return PerfCounters[DataType.DiskWrite].NextValue();
        }

        private float GetTCPConnections()
        {
            return PerfCounters[DataType.TcpConnections].NextValue();
        }
    }
}

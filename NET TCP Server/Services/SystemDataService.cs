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
            PerfCounters.Add(DataType.CPUUsage, cpuCounter);
            PerfCounters.Add(DataType.AvailableRAM, new PerformanceCounter("Memory", "Available MBytes"));
            PerfCounters.Add(DataType.TotalRAM, new PerformanceCounter("Memory", "Commit Limit"));
            cpuCounter.NextValue();
            Thread.Sleep(1000);
        }

        public Dictionary<string, string> GetData()
        {
            var dict = new Dictionary<string, string>();
            var availableRAM = GetAvailableRAM();
            var totalRAM = GetTotalRAM();
            var usedRAM = totalRAM - availableRAM;
            dict.Add(DataType.CPUUsage, GetCPUUsage() + "%");
            dict.Add(DataType.AvailableRAM, availableRAM + "MB");
            dict.Add(DataType.TotalRAM, totalRAM + "MB");
            dict.Add(DataType.UsedRAM, usedRAM + "MB");
            dict.Add(DataType.Processes, GetProcesses());
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

    }
}

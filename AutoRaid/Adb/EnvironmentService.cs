using System;
using AutoRaid.Contracts;

namespace AutoRaid.Adb
{
    public class EnvironmentService : IEnvironmentService
    {
        public string GetEnvironmentVariable(string name)
        {
            var envValue = Environment.GetEnvironmentVariable(name);
            return envValue ?? string.Empty;
        }
    }
}

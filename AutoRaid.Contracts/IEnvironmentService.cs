using System;

namespace AutoRaid.Contracts
{
    public interface IEnvironmentService
    {
        string GetEnvironmentVariable(string name);
    }
}

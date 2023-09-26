using Models;

namespace BUS.Interface
{
    public interface IMonitorBusiness
    {
        Task<bool> Create(MonitorModel monitor);
    }
}
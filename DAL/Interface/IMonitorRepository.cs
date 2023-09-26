using Models;

namespace DAL.Interface
{
    public interface IMonitorRepository
    {
        Task<bool> Create(MonitorModel monitor);
    }
}
using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public class MonitorBusiness : IMonitorBusiness
    {
        private IMonitorRepository _res;

        public MonitorBusiness(IMonitorRepository res)
        {
            _res = res;
        }

        public Task<bool> Create(MonitorModel monitor)
        {
            return _res.Create(monitor);
        }
    }
}
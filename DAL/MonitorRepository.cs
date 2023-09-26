using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class MonitorRepository : IMonitorRepository
    {
        private IDatabaseHelper _db;

        public MonitorRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<bool> Create(MonitorModel monitor)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_monitor_create_or_update",
                "@ClassId", monitor.ClassId,
                "@MonitorId", monitor.MonitorId,
                "@Semester", monitor.Semester,
                "@SchoolYear", monitor.SchoolYear
                );
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
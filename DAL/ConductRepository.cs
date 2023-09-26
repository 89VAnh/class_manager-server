using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class ConductRepository : IConductRepository
    {
        private IDatabaseHelper _db;

        public ConductRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<List<Conduct>> GetConductsOfClass(string classId, int semester, string schoolYear)
        {
            string msgError = "";
            try
            {
                var dt = _db.ExecuteSProcedureReturnDataTable(out msgError, "sp_get_ConductOfClass",
                 "@ClassId", classId, "@Semester", semester, "@SchoolYear", schoolYear);
                if (!string.IsNullOrEmpty(msgError))
                    throw new Exception(msgError);

                return dt.ConvertTo<Conduct>().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(MinConduct conduct)
        {
            string msgError = "";
            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_conduct_update",
                "@Id", conduct.Id,
                "@I_1", conduct.I_1,
                "@I_2", conduct.I_2,
                "@I_3", conduct.I_3,
                "@II_1", conduct.II_1,
                "@II_2", conduct.II_2,
                "@II_3", conduct.II_3,
                "@II_4", conduct.II_4,
                "@II_5", conduct.II_5,
                "@II_6", conduct.II_6,
                "@III_1", conduct.III_1,
                "@III_2", conduct.III_2,
                "@III_3", conduct.III_3,
                "@IV_1", conduct.IV_1,
                "@IV_2", conduct.IV_2,
                "@IV_3", conduct.IV_3,
                "@V_1", conduct.V_1,
                "@V_2", conduct.V_2);
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
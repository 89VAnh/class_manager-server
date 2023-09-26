using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class ClassRepository : IClassRepository
    {
        private IDatabaseHelper _db;

        public ClassRepository(IDatabaseHelper dbHelper)
        {
            _db = dbHelper;
        }

        public async Task<ClassInfo> GetClassInfo(string id)
        {
            string msgError = "";
            try
            {
                var dt = _db.ExecuteSProcedureReturnDataTable(out msgError, "sp_get_classInfo",
                     "@Id", id);
                if (!string.IsNullOrEmpty(msgError))
                    throw new Exception(msgError);
                return dt.ConvertTo<ClassInfo>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CreateClassStudents(ClassStudent clt)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_student_class_create",
                "@ClassId", clt.ClassId, "@Semester", clt.Semester, "@SchoolYear", clt.SchoolYear, "@list_json_student", clt.list_json_student != null ? MessageConvert.SerializeObject(clt.list_json_student) : null
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
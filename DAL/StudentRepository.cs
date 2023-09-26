using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class StudentRepository : IStudentRepository
    {
        private IDatabaseHelper _db;

        public StudentRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<bool> Create(Student student)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_student_create_or_update",
                "@Id", student.Id,
                "@Name", student.Name,
                "@Birthday", student.Birthday,
                "@Email", student.Email,
                "@Phone", student.Phone,
                "@Address", student.Address
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
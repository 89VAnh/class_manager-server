using DAL.Helper;
using DAL.Interface;
using server.Models;
using System.Diagnostics;

namespace DAL
{
    public partial class CourseRepository : ICourseRepository
    {
        private IDatabaseHelper _db;

        public CourseRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public bool Create(Course course)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_course_create",
                "@Id", course.Id,
                "@Name", course.Name,
                "@NumOfCredits", course.NumOfCredits
                );
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
﻿using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public partial class CourseRepository : ICourseRepository
    {
        private IDatabaseHelper _db;

        public CourseRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<bool> Create(Course course)
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
                throw ex;
            }
        }
    }
}
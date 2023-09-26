using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public class StudentBusiness : IStudentBusiness
    {
        private IStudentRepository _res;

        public StudentBusiness(IStudentRepository res)
        {
            _res = res;
        }

        public async Task<bool> Create(Student student)
        {
            return await _res.Create(student);
        }
    }
}
using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public partial class CourseBusiness : ICourseBusiness
    {
        private ICourseRepository _res;

        public CourseBusiness(ICourseRepository res)
        {
            _res = res;
        }

        public async Task<bool> Create(Course course)
        {
            return await _res.Create(course);
        }
    }
}
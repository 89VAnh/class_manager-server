using BUS.Interface;
using DAL.Interface;
using server.Models;

namespace BUS
{
    public partial class CourseBusiness : ICourseBusiness
    {
        private ICourseRepository _res;

        public CourseBusiness(ICourseRepository res)
        {
            _res = res;
        }

        public bool Create(Course course)
        {
            return _res.Create(course);
        }
    }
}
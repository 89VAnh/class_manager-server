using Models;

namespace BUS.Interface
{
    public partial interface ICourseBusiness
    {
        public Task<bool> Create(Course course);
    }
}
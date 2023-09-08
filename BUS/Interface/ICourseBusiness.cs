using server.Models;

namespace BUS.Interface
{
    public partial interface ICourseBusiness
    {
        public bool Create(Course course);
    }
}
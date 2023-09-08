using server.Models;

namespace DAL.Interface
{
    public partial interface ICourseRepository
    {
        public bool Create(Course course);
    }
}
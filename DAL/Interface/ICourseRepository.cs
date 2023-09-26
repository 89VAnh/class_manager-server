using Models;

namespace DAL.Interface
{
    public interface ICourseRepository
    {
        public Task<bool> Create(Course course);
    }
}
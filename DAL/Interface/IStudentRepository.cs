using Models;

namespace DAL.Interface
{
    public interface IStudentRepository
    {
        public Task<bool> Create(Student student);
    }
}
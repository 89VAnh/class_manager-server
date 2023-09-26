using Models;

namespace BUS.Interface
{
    public interface IStudentBusiness
    {
        public Task<bool> Create(Student student);
    }
}
using Models;

namespace BUS.Interface
{
    public interface IAccountBusiness
    {
        Task<Account> Login(string username, string password);
    }
}
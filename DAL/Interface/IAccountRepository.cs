using Models;

namespace DAL.Interface
{
    public interface IAccountRepository
    {
        Task<Account> GetAccount(string username, string password);

        Task<bool> Update(Account account);
    }
}
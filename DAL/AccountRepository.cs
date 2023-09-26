using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class AccountRepository : IAccountRepository
    {
        private IDatabaseHelper _db;

        public AccountRepository(IDatabaseHelper dbHelper)
        {
            _db = dbHelper;
        }

        public async Task<Account> GetAccount(string username, string password)
        {
            string msgError = "";
            try
            {
                var dt = _db.ExecuteSProcedureReturnDataTable(out msgError, "sp_account_get_by_username_password",
                     "@username", username,
                     "@password", password);
                if (!string.IsNullOrEmpty(msgError))
                    throw new Exception(msgError);
                return dt.ConvertTo<Account>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(Account account)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_account_update",
                "@username", account.Username,
                "@password", account.Password
                );
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
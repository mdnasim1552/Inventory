using Microsoft.Data.SqlClient;
using System.Data;


namespace Inventory.Data
{
    public interface IProcessAccess
    {
        Task<List<T>> GetAllAsync<T>(string SQLprocName, params SqlParameter[] parameters) where T : new();
        Task<List<T>> GetListAsync<T>(string SQLprocName, params SqlParameter[] parameters);
        Task<bool> ExecuteTransactionalOperationAsync(string SQLprocName, params SqlParameter[] parameters);
        Task<string> GetTransactionalOperationAsync(string SQLprocName, params SqlParameter[] parameters);
        Task<DataSet> GetDataSets(string SQLprocName, params SqlParameter[] parameters);
    }
}

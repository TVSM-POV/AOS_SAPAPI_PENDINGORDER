using SAP.Middleware.Connector;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SAPAPIgetpendingorder.DataLayer
{
    public interface IDbConnector
    {
        SqlConnection GetConnection();
        void CloseConnection();
        RfcDestination GetSapDestination();
        IRfcFunction CreateSapFunction(string functionName);
        Task<T> ExecuteSapFunction<T>(string functionName, Action<IRfcFunction> setParameters, Func<IRfcFunction, T> processResult);
        bool IsSapAvailable();
    }
}

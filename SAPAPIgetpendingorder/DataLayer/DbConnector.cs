using SAP.Middleware.Connector;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SAPAPIgetpendingorder.DataLayer
{
    public class DbConnector : IDbConnector
    {
        private static DbConnector _instance;
        private readonly RfcDestination _destination;

        private DbConnector()
        {
            _destination = RfcDestinationManager.GetDestination("QAS");
            _destination.Ping();
            Console.WriteLine("Connection successful!");
        }

        public static DbConnector GetInstance()
        {
            if (_instance == null)
                _instance = new DbConnector();

            return _instance;
        }

        public SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return new SqlConnection(connectionString);
        }

        public void CloseConnection()
        {
            // Optional: implement if you open SQL connections
        }

        public RfcDestination GetSapDestination()
        {
            return _destination;
        }

        public IRfcFunction CreateSapFunction(string functionName)
        {
            return _destination.Repository.CreateFunction(functionName);
        }

        public async Task<T> ExecuteSapFunction<T>(string functionName, Action<IRfcFunction> setParameters, Func<IRfcFunction, T> processResult)
        {
            return await Task.Run(() =>
            {
                IRfcFunction func = CreateSapFunction(functionName);
                setParameters(func);
                func.Invoke(_destination);
                return processResult(func);
            });
        }

        public bool IsSapAvailable()
        {
            try
            {
                _destination.Ping();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
//dbconnector
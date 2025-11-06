using System.Collections.Generic;
using System.Threading.Tasks;
using SAPAPIgetpendingorder.Models;

namespace SAPAPIgetpendingorder.DataLayer
{
    public interface IPortalDataAccess
    {
        Task<List<PendingOrderDo>> GetPendingOrders(string dealerCode);
    }
}

using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAPAPIgetpendingorder.Models;

namespace SAPAPIgetpendingorder.DataLayer
{
    public class PortalDataAccess : IPortalDataAccess
    {
        private readonly IDbConnector _dbConnector;

        public PortalDataAccess()
        {
            _dbConnector = DbConnector.GetInstance();
        }

        public async Task<List<PendingOrderDo>> GetPendingOrders(string dealerCode)
        {
            if (!_dbConnector.IsSapAvailable())
                throw new Exception("SAP connection is not available");

            return await _dbConnector.ExecuteSapFunction(
                "ZRFC_SPARE_PENDING_ORDER_NEW",
                func => func.SetValue("XKUNNR", dealerCode),
                func =>
                {
                    var result = new List<PendingOrderDo>();
                    var table = func.GetTable("XPEND_DET");
                    foreach (IRfcStructure row in table)
                    {
                        result.Add(new PendingOrderDo
                        {
                            PART_NO = row.GetString("PART_NUMBER"),
                            DESCRIPTION = row.GetString("PART_DESC"),
                            QRD_QTY = Math.Round(row.GetDecimal("ORDER_QTY")),
                            PEN_QTY = Math.Round(row.GetDecimal("PEND_QTY")),
                            PEN_VALUE = row.GetDecimal("PEND_VALUE"),
                            ORDER_TYPE = row.GetString("ORDER_TYPE"),
                            ORDER_NUMBER = row.GetString("VBELN"),
                            ORDER_DATE = row.GetString("ORDER_DATE"),
                            SHIP_TO = row.GetString("SHIP_TO")
                        });
                    }
                    return result;
                });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SAP.Middleware.Connector;
using SAPAPIgetpendingorder.DataLayer;
using SAPAPIgetpendingorder.Filters;
using System.ComponentModel.DataAnnotations;
using SAPAPIgetpendingorder.Models;


namespace SAPAPIgetpendingorder.Controllers
{
    public class OrderRequest
    {
        [Required]
        public string DealerCode { get; set; }
        public string OrderType { get; set; }
    }

    [RoutePrefix("api/dealers")]
    [BasicAuthentication]
    public class DealerController : ApiController
    {
        private readonly IPortalDataAccess _portalDataAccess;

        public DealerController()
        {
            _portalDataAccess = new PortalDataAccess();
        }

        [HttpPost]
        [Route("GetPendingOrders")]
        public async Task<IHttpActionResult> GetPendingOrders([FromBody] OrderRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request body is missing");

                if (string.IsNullOrEmpty(request.DealerCode))
                    return BadRequest("DealerCode is required");

                var validOrderTypes = new[]
                {
                   "ZCCP", // Cash And Carry Order
                "ZSTK", // Stock Orders
                "ZVOR", // VOR Orders
                "ZSML", // Smiles Order
                "ZRED",
                "ZHST", // Sp Stock Order HO
                "ZSIF", // Spares IF Order
                "ZBLK",
                "ZGRN",
                "ZYEW",
                "LF",   // PickList
                "ZACC", // Accessory Pending Order
                "ZAC1", // Accessory Pending Order
                "ZAC2"  // Accessory Pending Order
                };

                if (!string.IsNullOrEmpty(request.OrderType) &&
                    !validOrderTypes.Contains(request.OrderType, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest($"OrderType must be one of: {string.Join(", ", validOrderTypes)}");
                }

                var pendingOrders = await _portalDataAccess.GetPendingOrders(request.DealerCode);

                if (pendingOrders == null || !pendingOrders.Any())
                {
                    return Ok(new PendingOrderResponse
                    {
                        Orders = new List<PendingOrderDo>(),
                        TotalPendingAmount = 0,
                        TotalPartNumbers = 0,
                        Message = "No pending orders found"
                    });
                }

                if (!string.IsNullOrEmpty(request.OrderType))
                {
                    pendingOrders = pendingOrders
     .Where(p => p.ORDER_TYPE.Equals(request.OrderType, StringComparison.OrdinalIgnoreCase))
     .ToList();

                }

                var response = new PendingOrderResponse
                {
                    Orders = pendingOrders,
                    TotalPendingAmount = pendingOrders.Sum(p => p.PEN_VALUE),
                    TotalPartNumbers = pendingOrders.Select(p => p.PART_NO).Distinct().Count(),
                    Message = "Successfully retrieved pending orders"
                };

                return Ok(response);
            }
            catch (RfcCommunicationException ex)
            {
                return InternalServerError(new Exception("Unable to connect to SAP system. Please verify SAP configuration.", ex));
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}", ex));
            }
        }

        private string MapOrderType(string orderType)
        {
            var orderTypeMap = new Dictionary<string, string>
            {
                { "ZCCP", "Cash And Carry Order" },
                { "ZSTK", "Stock Orders" },
                { "ZVOR", "VOR Orders" },
                { "ZSML", "Smiles Order" },
                { "ZRED", "ZRED" },
                { "ZHST", "Sp Stock Order HO" },
                { "ZSIF", "Spares IF Order" },
                { "ZBLK", "ZBLK" },
                { "ZGRN", "ZGRN" },
                { "ZYEW", "ZYEW" },
                { "LF", "PickList" },
                { "ZACC", "Accessory Pending Order" },
                { "ZAC1", "Accessory Pending Order" },
                { "ZAC2", "Accessory Pending Order" }
            };
            return orderTypeMap.TryGetValue(orderType?.ToUpper(), out string mappedType) ? mappedType : orderType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPAPIgetpendingorder.Models
{
	public class PendingOrderResponse
	{
        public List<PendingOrderDo> Orders { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public int TotalPartNumbers { get; set; }
        public string Message { get; set; }
    }
}
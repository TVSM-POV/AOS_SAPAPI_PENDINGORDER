using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPAPIgetpendingorder.Models
{
    public class PendingOrderDo
    {
        public string PART_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal QRD_QTY { get; set; }
        public decimal PEN_QTY { get; set; }
        public decimal PEN_VALUE { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_DATE { get; set; }
        public string SHIP_TO { get; set; }
    }
}
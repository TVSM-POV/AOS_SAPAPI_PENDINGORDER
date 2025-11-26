using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAPAPIgetpendingorder.Models;
using static System.Net.WebRequestMethods;

namespace SAPAPIgetpendingorder.DataLayer
{
    public class PortalDataAccess : IPortalDataAccess
    {
        private readonly HttpClient _httpClient;

        public PortalDataAccess()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<PendingOrderDo>> GetPendingOrders(string dealerCode)
        {
            var url = "https://tvsm.prod.apimanagement.in30.hana.ondemand.com:443/PRD/Spares_Pending_Order_DTL_API";

            string bearerToken = await TokenManager.GetToken();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            // API expects: Content-Type = text/plain with JSON as plain text
            string bodyText = "{ \"XKUNNR\": \"" + dealerCode + "\" }";

            var content = new StringContent(bodyText, Encoding.UTF8, "text/plain");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to call SAP API: " + response.ReasonPhrase);

            string json = await response.Content.ReadAsStringAsync();

            var root = JsonConvert.DeserializeObject<PendingOrderRootDto>(json);

            var apiRows = root?.XPEND_DET;


            var result = new List<PendingOrderDo>();

            if (apiRows != null)
            {
                foreach (var row in apiRows)
                {
                    result.Add(new PendingOrderDo
                    {
                        PART_NO = row.PART_NUMBER,
                        DESCRIPTION = row.PART_DESC,
                        QRD_QTY = Math.Round(Convert.ToDecimal(row.ORDER_QTY)),
                        PEN_QTY = Math.Round(Convert.ToDecimal(row.PEND_QTY)),
                        PEN_VALUE = Convert.ToDecimal(row.PEND_VALUE),
                        ORDER_TYPE = row.ORDER_TYPE,
                        ORDER_NUMBER = row.VBELN,
                        ORDER_DATE = row.ORDER_DATE,
                        SHIP_TO = row.SHIP_TO
                    });
                }
            }

            return result;
        }
    }

    public class PendingOrderApiDto
    {
        public string PART_NUMBER { get; set; }
        public string PART_DESC { get; set; }
        public string ORDER_QTY { get; set; }
        public string PEND_QTY { get; set; }
        public string PEND_VALUE { get; set; }
        public string ORDER_TYPE { get; set; }
        public string VBELN { get; set; }
        public string ORDER_DATE { get; set; }
        public string SHIP_TO { get; set; }
    }

    public class PendingOrderRootDto
    {
        public List<PendingOrderApiDto> XPEND_DET { get; set; }
    }

}

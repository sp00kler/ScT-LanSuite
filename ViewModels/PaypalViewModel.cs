using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PayPalViewModel
    {
        public string cmd { get; set; }
        public string business { get; set; }
        public string no_shipping { get; set; }
        public string @return { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public string item_name { get; set; }
        public string amount { get; set; }
        public string actionURL { get; set; }
        public string custom { get; set; }
        public PayPalViewModel(string Business, string CancelReturn, string Return, string actionUrl, string notifyUrl, string currencyCode, string Amount, string itemName, string userId)
        {
            this.cmd = "_xclick";
            this.business = Business;
            this.cancel_return = CancelReturn;
            this.@return = Return;
            this.actionURL = actionUrl;
            // We can add parameters here, for example OrderId, CustomerId, etc….
            this.notify_url = notifyUrl;
            // We can add parameters here, for example OrderId, CustomerId, etc….
            this.currency_code = currencyCode;
            this.item_name = itemName;
            this.amount = Amount;
            this.custom = userId;
        }
    }
}

using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModels;

namespace ScT_LanSuite.Controllers
{
    public class PaypalController : ScTController
    {
        public async Task<ActionResult> PayLan()
        {
            var edition = await uow.editionRepository.FindAsync(x => x.isActivated);
            ViewBag.Edition = edition.Title;

            ViewBag.Price = await sm.getSettingAsync("Registration_Price") ;

            return PartialView("_PayLan");
        }
        public async Task<ActionResult> RedirectFromPaypal()
        {
            var edition = await uow.editionRepository.FindAsync(x => x.isActivated);
            ViewBag.Edition = edition.Title;
            return View();
        }
        public ActionResult CancelFromPaypal()
        {
            return View();
        }
        public async Task<ActionResult> NotifyFromPaypal()
        {
            // Receive IPN request from PayPal and parse all the variables returned
            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-validate");

            // if you want to use the PayPal sandbox change this from false to true
            string response = await GetPayPalResponse(formVals);

            if (response == "VERIFIED")
            {
                string transactionID = Request["txn_id"];
                string sAmountPaid = Request["mc_gross"];
                string userId = Request["custom"];
                string Status = Request["payment_status"];
                var editionId = (await uow.editionRepository.FindAsync(x => x.isActivated)).ID;
                var registration = await uow.registrationRepository.FindAsync(x => x.UserID == userId && x.EditionID == editionId);

                //validate the order
                Decimal amountPaid = 0;
                Decimal.TryParse(sAmountPaid, out amountPaid);
                var AmountPaid = await sm.getSettingAsync("Registration_Price");
                if (sAmountPaid == AmountPaid && Status == "Completed")
                {
                    // take the information returned and store this into a subscription table
                    // this is where you would update your database with the details of the tran
                    registration.Paid = true;
                    await uow.registrationRepository.UpdateAsync(registration);
                    return View();

                }
                else
                {
                    // let fail - this is the IPN so there is no viewer
                    // you may want to log something here
                }
            }


            return View();
        }
        public async Task<ActionResult> ValidateCommand(string product, string totalPrice)
        {

            var business = await sm.getSettingAsync("PayPal_Business");
            var cancelReturn = await sm.getSettingAsync("PayPal_CancelReturn");
            var Return = await sm.getSettingAsync("PayPal_Return");
            var actionUrl = await sm.getSettingAsync("PayPal_ActionUrl");
            var notifyUrl = await sm.getSettingAsync("PayPal_NotifyUrl");
            var currencyCode = await sm.getSettingAsync("PayPal_CurrencyCode");
            var userId = (await uow.userRepository.FindAsync(x => x.UserName == User.Identity.Name)).Id;
            var editionId = (await uow.editionRepository.FindAsync(x => x.isActivated)).ID;


            var paypalvm = new PayPalViewModel(business, cancelReturn, Return, actionUrl, notifyUrl, currencyCode, totalPrice, product, userId);

            return View(paypalvm);
        }

        public async Task<string> GetPayPalResponse(Dictionary<string, string> formVals)
        {

            // Parse the variables
            // Choose whether to use sandbox or live environment
            string paypalUrl = await sm.getSettingAsync("PayPal_ActionUrl");

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://urlort#");
            //req.Proxy = proxy;
            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {

                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }

    }
}

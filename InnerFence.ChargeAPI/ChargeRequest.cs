using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerFence.ChargeAPI
{
    public class ChargeRequest
    {
        public const string CCTERMINAL_BASE_URL = @"com-innerfence-ccterminal://charge/1.0.0/";
        public const string CCTERMINAL_DISPLAY_NAME = "Credit Card Terminal";
        public const string CCTERMINAL_WINDOWS_PFN = "InnerFence.CreditCardTerminal_0mbyxksw3w1fr";
        public const string CCTERMINAL_WINDOWS_STORE_LINK = "ms-windows-store:PDP?PFN=" + CCTERMINAL_WINDOWS_PFN;
        public const string CCTERMINAL_WP8_APP_ID = "58b2239f-30de-df11-a844-00237de2db9e";
        public const string CCTERMINAL_WP8_STORE_LINK = "zune:navigate?appid=" + CCTERMINAL_WP8_APP_ID;

        public static class Keys
        {
            public const string ADDRESS = "address";
            public const string AMOUNT = "amount";
            public const string AMOUNT_FIXED = "amountFixed";
            public const string CITY = "city";
            public const string COMPANY = "company";
            public const string COUNTRY = "country";
            public const string CURRENCY = "currency";
            public const string DESCRIPTION = "description";
            public const string EMAIL = "email";
            public const string FIRST_NAME = "firstName";
            public const string INVOICE_NUMBER = "invoiceNumber";
            public const string LAST_NAME = "lastName";
            public const string PHONE = "phone";
            public const string RETURN_APP_NAME = "returnAppName";
            public const string RETURN_IMMEDIATELY = "returnImmediately";
            public const string RETURN_URL = "returnURL";
            public const string STATE = "state";
            public const string TAX_RATE = "taxRate";
            public const string ZIP = "zip";
        }

        public ChargeRequest()
        {
        }

        public string Address { get; set; }
        public string Amount { get; set; }
        public string AmountFixed { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string InvoiceNumber { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string ReturnAppName { get; set; }
        public string ReturnImmediately { get; set; }
        public string ReturnURL { get; protected set; }
        public string State { get; set; }
        public string TaxRate { get; set; }
        public string Zip { get; set; }

        public void SetReturnURL(string returnURL, Dictionary<string, string> extraParams)
        {
            Uri uri = new Uri(returnURL);

            // genereate nonce and add it to extra params
            if (null == extraParams)
            {
                extraParams = new Dictionary<string, string>();
            }
            string nonce = CreateAndStoreNonce();
            extraParams.Add(ChargeResponse.Keys.NONCE, nonce);

            this.ReturnURL = ChargeUtils.UriWithAdditionalParams(uri, extraParams).ToString();
        }

        public Dictionary<string, string> GenerateParams()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add(Keys.ADDRESS, this.Address);
            parameters.Add(Keys.AMOUNT, this.Amount);
            parameters.Add(Keys.AMOUNT_FIXED, this.AmountFixed);
            parameters.Add(Keys.CITY, this.City);
            parameters.Add(Keys.COMPANY, this.Company);
            parameters.Add(Keys.COUNTRY, this.Country);
            parameters.Add(Keys.CURRENCY, this.Currency);
            parameters.Add(Keys.DESCRIPTION, this.Description);
            parameters.Add(Keys.EMAIL, this.Email);
            parameters.Add(Keys.FIRST_NAME, this.FirstName);
            parameters.Add(Keys.INVOICE_NUMBER, this.InvoiceNumber);
            parameters.Add(Keys.LAST_NAME, this.LastName);
            parameters.Add(Keys.PHONE, this.Phone);
            parameters.Add(Keys.RETURN_APP_NAME, this.ReturnAppName);
            parameters.Add(Keys.RETURN_IMMEDIATELY, this.ReturnImmediately);
            parameters.Add(Keys.RETURN_URL, this.ReturnURL);
            parameters.Add(Keys.STATE, this.State);
            parameters.Add(Keys.TAX_RATE, this.TaxRate);
            parameters.Add(Keys.ZIP, this.Zip);

            return parameters;
        }

        public string CreateAndStoreNonce()
        {
            string nonce = ChargeUtils.GenerateNonce();
            ChargeUtils.SaveLocalData(ChargeResponse.Keys.NONCE, nonce);
            return nonce;
        }

        public Uri GenerateLaunchURL()
        {
            Uri uri = new Uri(CCTERMINAL_BASE_URL);
            Dictionary<string, string> parameters = this.GenerateParams();
            return ChargeUtils.UriWithAdditionalParams(uri, parameters);
        }
    }
}

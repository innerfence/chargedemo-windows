using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace InnerFence.ChargeAPI
{
    public class ChargeRequest
    {
        public const string CCTERMINAL_BASE_URL = @"com-innerfence-ccterminal://charge/1.0.0/";

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
        public string ReturnURL { get; set; }
        public string State { get; set; }
        public string TaxRate { get; set; }
        public string Zip { get; set; }

        public void SetReturnURL(string returnURL, Dictionary<string, string> extraParams)
        {
            Uri uri = new Uri(returnURL);
            this.ReturnURL = Utils.UriWithAdditionalParams(uri, extraParams).ToString();
        }

        public Dictionary<string, string> GenerateParams()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add(Keys.ADDRESS, this.Address);
            parameters.Add(Keys.AMOUNT, this.Amount);
            parameters.Add(Keys.AMOUNT_FIXED, this.AmountFixed);
            parameters.Add(Keys.CITY, this.City);
            parameters.Add(Keys.COMPANY, this.City);
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

        public Uri GenerateLaunchURL()
        {
            Uri uri = new Uri(CCTERMINAL_BASE_URL);
            Dictionary<string, string> parameters = this.GenerateParams();
            return Utils.UriWithAdditionalParams(uri, parameters);
        }
    }
}

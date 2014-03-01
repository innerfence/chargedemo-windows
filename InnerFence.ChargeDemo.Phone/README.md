OVERVIEW
========

The ChargeDemo source code demonstrates how to implement 2-way
integration for accepting credit card payments using Credit Card
Terminal for Windows Phone.

ChargeDemo supplies a single charge button. When it's tapped, a URL
request is made to Credit Card Terminal in order to accept a credit
card payment. When the user is done with Credit Card Terminal,
ChargeDemo will be launched via its URL handler for
`com-innerfence-chargedemo://`.

Protocol details are provided below in the case that you cannot or do
not wish to use our C# classes.

Please visit our [Developer API
page](http://www.innerfence.com/apps/credit-card-terminal/developer-api)
to see how the user experience flow will be like.

INTEGRATION CHECKLIST
=====================

* Add the following files to you Visual Studio project:
  * ..\InnerFence.ChargeAPI\ChargeException.cs
  * ..\InnerFence.ChargeAPI\ChargeResponse.cs
  * ..\InnerFence.ChargeAPI\ChargeResponse.cs
  * ..\InnerFence.ChargeAPI\ChargeUtils.cs
  * ChargeAPI\ChargeUriMapper.cs
  * ChargeAPI\ChargeUtilsPhone.cs

* Make sure your application is registered to handle a URL scheme:

  * Using the XML (Text) Editor, open your app manifest file: `WMAppManifest.xml`
  * Under `</Tokens>` and before `<ScreenResolutions>`, register a unique protocol scheme
    (e.g. `com-innerfence-chargedemo`)

```xml
<Extensions>
  <Protocol Name="com-innerfence-chargedemo" NavUriFragment="encodedLaunchUri=%s" TaskID="_default" />
</Extensions>
```

* More details on MSDN: [Auto-launching apps using file and URI
  associations](http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206987.aspx)

* Request payment by creating an ChargeRequest object, setting its
  properties, calling its `GenerateLaunchURL()` method, and launching
  the returned URL. We've also provided a helpful
  `ChargeUtils.SubmitChargeReqeust()` method that will set up your
  launch options properly. Be sure to set the returnURL if you want us
  to return the results to your app by using `SetReturnURL()`. You can
  include extra parameters to be returned to you by passing them into
  `SetReturnURL()`. For example:

```cs
ChargeRequest chargeRequest = new ChargeRequest();

// Include my record_id so it comes back with the response
Dictionary<string, string> extraParams =
   new Dictionary<string, string>() { { "record_id", "123" } };
chargeRequest.SetReturnURL(@"com-innerfence-chargedemo://chargeResponse", extraParams);

chargeRequest.Amount        = "50.00";
chargeRequest.Description   = "Test transaction";
chargeRequest.InvoiceNumber = "321";

// Include a tax rate if you want Credit Card terminal to calculate
// sales tax using a specific rate. If you leave it as null, we’ll use
// the default app settings for sales tax calculation.
chargeRequest.TaxRate = "8.5";

// Submit request
try
{
    ChargeUtils.SubmitChargeRequest(chargeRequest);
}
catch (ChargeException)
{
    // An Exception is thrown when we are unable to launch
    // Credit Card Terminal. This usually means the app is
    // not installed.
}
```

* Add a UriMapper to your `RootFrame` inside your app
  `InitializePhoneApplication()`. The UriMapper is called whenever
  there's an incoming request containing the data of our charge
  response. Handle responses in UriMapper by creating an
  ChargeResponse object by passing the launch Uri. Use the
  `ResponseCode` property to determine if the transaction was
  successful. For example:

```cs
public class AssociationUriMapper : ChargeUriMapper
{
    protected override Uri GenerateRedirectUri(Uri launchUri)
    {
        ChargeResponse response = new ChargeResponse(launchUri);

        // My record_id from the request is available in the extraParams
        // dictionary.
        string recordId = chargeResponse.ExtraParams["record_id"];

        // Since this value is from the URL, I need to validate it.
        if (!IsValidRecordId(recordId))
        {
            // handle error
        }

        if (chargeResponse.ResponseCode == ChargeResponse.Code.APPROVED)
        {
            // Transaction succeeded, check out these properties:
            //  * chargeResponse.TransactionId
            //  * chargeResponse.Amount (includes tax and tip)
            //  * chargeResponse.TaxAmount
            //  * chargeResponse.TaxRate
            //  * chargeResponse.TipAmount
            //  * chargeResponse.CardType
            //  * chargeResponse.RedactedCardNumber
        }
        else
        {
            // Transaction failed.
        }

        // In your app, this is where you define which view you want the
        // returning charge request call to land on.
        return new Uri("/MainPage.xaml", UriKind.Relative);
    }
}

private void InitializePhoneApplication()
{
    […]

    // Assign the URI-mapper class to the application frame.
    RootFrame.UriMapper = new AssociationUriMapper();
}
```

PROTOCOL REQUEST
================

The Charge request is simply a set of query string parameters which
are appended to a base URL. Be sure to properly encode the query
string parameters.

Base URL: `com-innerfence-ccterminal://charge/1.0.0/`

* `returnAppName` - your app's name, displayed to give the user context
* `returnURL` - your app's URL handler, see PROTOCOL RESPONSE
* `returnImmediately` - if set to 1,  the `returnURL` will be called with the result immediately instead of waiting for the end user to tap through the “Approved” screen
* `amount` - amount of the transaction (e.g. `10.99`, `1.00`, `0.90`)
* `amountFixed` - if set to 1, the amount (subtotal) will be unchangable. If tips or sales tax is enabled, the final amount can still differ
* `taxRate` - sales tax rate to apply to amount (e.g. `8`, `8.5`, `8.25`, `8.125`)
* `currency` - currecy code of amount (e.g. `USD`)
* `email` - customer's email address for receipt
* `firstName` - billing first name
* `lastName` - billing lastName
* `company` - billing company name
* `address` - billing street address
* `city` - billing city
* `state` - billing state or province (e.g. `TX`, `ON`)
* `zip` - billing zip or postal code
* `phone` - billing phone number
* `country` - billing country code (e.g. `US`)
* `invoiceNumber` - merchant-assigned invoice number
* `description` - description of goods or services

Here is a simple example. Please note the correct encoding of parameters:

```
com-innerfence-ccterminal://charge/1.0.0/?amount=10.99&email=john%40example.com&returnAppName=ChargeDemo&returnURL=com-your-app%3A%2F%2Faction%2F
```

PROTOCOL RESPONSE
=================

When the request includes a `returnURL`, the results of the charge
will be returned via the URL by including additional query string
parameters. These parameters all begin with ifcc_ to avoid conflict
with any query parameters your app may already recognize.

* `ifcc_responseType` - `approved`, `cancelled`, `declined`, or `error`
* `ifcc_transactionId` - transaction id (e.g. `100001`)
* `ifcc_amount` - amount charged (e.g. `10.99`)
* `ifcc_currency` - currency of amount (e.g. `USD`)
* `ifcc_taxAmount` - tax portion from amount (e.g. `0.93`)
* `ifcc_taxRate` - tax rate applied to original amount (e.g. `8.5`)
* `ifcc_tipAmount` - tip portion from amount (e.g. `1.50`)
* `ifcc_redactedCardNumber` - redacted card number (e.g. `XXXXXXXXXXXX1111`)
* `ifcc_cardType` - card type: `Visa`, `MasterCard`, `Amex`, `Discover`, `Maestro`, `Solo`, or `Unknown`

Here is a simple example:

```
com-your-app://action/?ifcc_responseType=approved&ifcc_transactionId=100001&ifcc_amount=10.99&ifcc_currency=USD&ifcc_redactedCardNumber=XXXXXXXXXXXX1111&ifcc_cardType=Visa&ifcc_taxAmount=0.93&ifcc_taxRate=8.5&ifcc_tipAmount=1.50
```

FILE MANIFEST
=============

* README.md

This file.

* ..\COPYING

A copy of the MIT License, under which you may reuse any of the source
code in this sample.

* ..\InnerFence.ChargeAPI\ChargeException.cs
* ..\InnerFence.ChargeAPI\ChargeRequest.cs
* ..\InnerFence.ChargeAPI\ChargeResponse.cs
* ..\InnerFence.ChargeAPI\ChargeUtils.cs
* ChargeAPI\ChargeUriMapper.cs
* ChargeAPI\ChargeUtilsPhone.cs

The ChargeAPI helper classes. Copy these files into your own Visual
Studio project.

* App.xaml
* App.xaml.cs
* AssociationUriMapper.cs

Main application that handles the response when activated via your
registered protocol URL scheme.

* MainPage.xaml
* MainPage.xaml.cs

A very simple page that provides a single Charge button. When the
button is tapped, an ChargeRequest object is created and submitted.

* Properties\WMAppManifest.xml

Registers the com-innerfence-chargedemo:// URL scheme.

* Properties\AssemblyInfo.cs
* Properties\AppManifest.xml

These files are all stock as generated by Visual Studio.

* ..\ChargeDemo.sln
* ChargeDemo.Phone.csproj

An XCode project for building this sample.

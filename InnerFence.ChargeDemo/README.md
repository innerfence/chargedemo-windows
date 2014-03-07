OVERVIEW
========

The ChargeDemo source code demonstrates how to implement 2-way
integration for accepting credit card payments using Credit Card
Terminal for Windows.

ChargeDemo supplies a single charge button. When it's tapped, a URL
request is made to Credit Card Terminal in order to accept a credit
card payment. When the user is done with Credit Card Terminal,
ChargeDemo will be launched via its URL handler for
`com-innerfence-chargedemo://`.

Protocol details are provided in the [main README.md](../README.md) in the case
that you cannot or do not wish to use our C# classes.

Please visit our [Developer API
page](http://www.innerfence.com/apps/credit-card-terminal/developer-api)
to see what the user experience flow will be like.

INTEGRATION CHECKLIST
=====================

* Add the following files to you Visual Studio project:
  * ..\InnerFence.ChargeAPI\ChargeException.cs
  * ..\InnerFence.ChargeAPI\ChargeResponse.cs
  * ..\InnerFence.ChargeAPI\ChargeResponse.cs
  * ..\InnerFence.ChargeAPI\ChargeUtils.cs
  * ChargeAPI\ChargeUtilsWindows.cs

* Make sure your application is registered to handle a URL scheme:

  * Open your app manifest file: `package.appxmanifest`
  * Go to the `Declarations` tab and add the `Protocol` declaration
  * In the protocol properties, register a unique URL scheme
    (e.g. `com-innerfence-chargedemo`)
  * More details on MSDN: [How to handle URI activation](http://msdn.microsoft.com/en-us/library/windows/apps/hh779670.aspx)

* Request payment by creating an ChargeRequest object, setting its
  properties, calling its `GenerateLaunchURL` method, and launching
  the returned URL. We've also provided a helpful
  `ChargeUtils.SubmitChargeRequest` method that will set up your
  launch options properly. Be sure to set the returnURL if you want us
  to return the results to your app by using `SetReturnURL`. You can
  include extra parameters to be returned to you by passing them into
  `SetReturnURL`. For example:

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

* Handle charge responses in your app’s
  `OnActivated(IActivatedEventArgs args)` method by creating an
  ChargeResponse object by passing the protocol Uri. Use the
  `ResponseCode` property to determine if the transaction was
  successful. For example:

```cs
protected override void OnActivated(IActivatedEventArgs args)
{
    base.OnActivated(args);

    // Only respond to protocol activation
    var protocolArgs = args as ProtocolActivatedEventArgs;
    if (args.Kind != ActivationKind.Protocol || protocolArgs == null)
    {
        return;
    }

    Uri uri = protocolArgs.Uri;
    ChargeResponse response = new ChargeResponse(uri);

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
}
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
* ChargeAPI\ChargeUtilsWindows.cs

The ChargeAPI helper classes. Copy these files into your own Visual
Studio project.

* App.xaml
* App.xaml.cs

Main application that handles the response when activated via your
registered protocol URL scheme.

* MainPage.xaml
* MainPage.xaml.cs

A very simple page that provides a single Charge button. When the
button is tapped, an ChargeRequest object is created and submitted.

* Package.appxmanifest

Registers the com-innerfence-chargedemo:// URL scheme.

* ChargeDemo_TemporaryKey.pfx
* Properties\AssemblyInfo.cs

These files are all stock as generated by Visual Studio.

* ..\ChargeDemo.sln
* ChargeDemo.csproj

A Visual Studio project for building this sample.

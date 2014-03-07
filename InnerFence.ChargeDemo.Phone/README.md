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

Protocol details are provided in the [main README.md](../) in the case
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
  * ChargeAPI\ChargeUriMapper.cs
  * ChargeAPI\ChargeUtilsPhone.cs

* Make sure your application is registered to handle a URL scheme:

  * To register for a URI association, you must edit
    `WMAppManifest.xml` using the XML (Text) Editor. In Solution
    Explorer, right-click the WMAppManifest.xml file, and then click
    Open With. In the Open With window, select XML(Text) Editor, and
    then click OK.
  * Insert the following `<Extensions>` block between `<Tokens>` and
    `<ScreenResolutions>` and pick a unique protocol scheme (e.g.
    `com-innerfence-chargedemo`)
  * More details on MSDN: [Auto-launching apps using file and URI
    associations](http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206987.aspx)

```xml
<Extensions>
  <Protocol Name="com-innerfence-chargedemo" NavUriFragment="encodedLaunchUri=%s" TaskID="_default" />
</Extensions>
```

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

* Add a UriMapper to your `RootFrame` inside your app
  `InitializePhoneApplication`. The UriMapper is called whenever
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

A Visual Studio project for building this sample.

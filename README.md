OVERVIEW
========

The ChargeDemo source code demonstrates how to implement 2-way
integration for accepting credit card payments using Credit Card
Terminal for Windows Phone, Tablet, and PC.

* [Windows API](InnerFence.ChargeDemo/)
* [Windows Phone API](InnerFence.ChargeDemo.Phone/)

Protocol details are provided below in the case that you cannot or do
not wish to use our C# classes.

Please visit our [Developer API
page](http://www.innerfence.com/apps/credit-card-terminal/developer-api)
to see what the user experience flow will be like.

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

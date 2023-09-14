using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using WebApplicationIceCreamProject.Models;

[Route("api/[controller]")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly IOptions<PayPalSettings> _payPalSettings;

    public PayPalController(IOptions<PayPalSettings> payPalSettings)
    {
        _payPalSettings = payPalSettings;
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment()
    {
        var environment = new SandboxEnvironment(
            _payPalSettings.Value.ClientId,
            _payPalSettings.Value.ClientSecret
        );

        var client = new PayPalHttpClient(environment);

        var order = new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "USD",
                    Value = "10.00" // Amount to charge
                }
            }
        }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(order);

        var response = await client.Execute(request);
        var statusCode = response.StatusCode;

        // Process the PayPal response and handle redirects or errors
        // Implement the payment confirmation logic
        // Redirect the user to PayPal for payment approval
        // Handle the PayPal callback to confirm the payment
        return null;
        // Return appropriate response to the client
    }

}

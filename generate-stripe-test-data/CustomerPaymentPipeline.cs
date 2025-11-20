using Bogus;
using Stripe;

namespace GenerateStripeTestData;

public class CustomerPaymentPipeline
{
    private readonly StripeClient _client;
    private readonly Faker _faker;

    public CustomerPaymentPipeline(StripeClient client)
    {
        _client = client;
        _faker = new Faker();
    }

    /// <summary>
    /// Executes the complete customer payment pipeline for the specified number of procedures.
    /// Each procedure creates: Customer -> PaymentIntent -> Confirmation -> Capture
    /// </summary>
    /// <param name="numberOfProcedures">Number of complete procedures to execute</param>
    /// <returns>List of results for each procedure</returns>
    public List<PipelineResult> Execute(int numberOfProcedures)
    {
        if (numberOfProcedures <= 0)
        {
            throw new ArgumentException("Number of procedures must be greater than 0", nameof(numberOfProcedures));
        }

        var results = new List<PipelineResult>();

        for (int i = 0; i < numberOfProcedures; i++)
        {
            Console.WriteLine($"\n=== Starting Procedure {i + 1} of {numberOfProcedures} ===");
            
            var result = ExecuteSingleProcedure();
            results.Add(result);
            
            Console.WriteLine($"=== Completed Procedure {i + 1} ===");
        }

        return results;
    }

    /// <summary>
    /// Executes a single complete procedure: Customer -> PaymentIntent -> Confirmation -> Capture
    /// </summary>
    private PipelineResult ExecuteSingleProcedure()
    {
        // Step 1: Create Customer
        var customer = CreateCustomer();
        Console.WriteLine($"✓ Created customer: {customer.Id} ({customer.Name})");

        // Step 2: Create PaymentIntent
        var paymentIntent = CreatePaymentIntent(customer.Id);
        Console.WriteLine($"✓ Created PaymentIntent: {paymentIntent.Id} for ${paymentIntent.Amount / 100.0:F2}");

        // Step 3: Confirm PaymentIntent
        var confirmedPaymentIntent = ConfirmPaymentIntent(paymentIntent.Id);
        Console.WriteLine($"✓ Confirmed PaymentIntent: {confirmedPaymentIntent.Id} (Status: {confirmedPaymentIntent.Status})");

        // Step 4: Capture PaymentIntent
        var capturedPaymentIntent = CapturePaymentIntent(confirmedPaymentIntent.Id);
        Console.WriteLine($"✓ Captured PaymentIntent: {capturedPaymentIntent.Id} (Status: {capturedPaymentIntent.Status})");

        return new PipelineResult
        {
            Customer = customer,
            PaymentIntent = capturedPaymentIntent
        };
    }

    /// <summary>
    /// Creates a new customer with fake data
    /// </summary>
    private Customer CreateCustomer()
    {
        var customerService = new CustomerService(_client);
        var options = new CustomerCreateOptions
        {
            Name = _faker.Name.FullName(),
            Email = _faker.Internet.Email(),
            Description = $"Test customer created via pipeline at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
        };
        
        return customerService.Create(options);
    }

    /// <summary>
    /// Creates a PaymentIntent for the specified customer
    /// </summary>
    private PaymentIntent CreatePaymentIntent(string customerId)
    {
        var paymentIntentService = new PaymentIntentService(_client);
        
        // Generate a random amount between $10.00 and $500.00
        var amount = _faker.Random.Long(1000, 50000);
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "usd",
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card" },
            CaptureMethod = "manual", // Must be manual to allow separate capture step
            Description = $"Test payment created via pipeline"
        };
        
        return paymentIntentService.Create(options);
    }

    /// <summary>
    /// Confirms a PaymentIntent using a test payment method
    /// </summary>
    private PaymentIntent ConfirmPaymentIntent(string paymentIntentId)
    {
        var paymentIntentService = new PaymentIntentService(_client);
        
        var options = new PaymentIntentConfirmOptions
        {
            PaymentMethod = "pm_card_visa", // Stripe test payment method
        };
        
        return paymentIntentService.Confirm(paymentIntentId, options);
    }

    /// <summary>
    /// Captures a confirmed PaymentIntent
    /// </summary>
    private PaymentIntent CapturePaymentIntent(string paymentIntentId)
    {
        var paymentIntentService = new PaymentIntentService(_client);
        
        return paymentIntentService.Capture(paymentIntentId);
    }
}

/// <summary>
/// Result of a single pipeline procedure
/// </summary>
public class PipelineResult
{
    public Customer Customer { get; set; } = null!;
    public PaymentIntent PaymentIntent { get; set; } = null!;
}

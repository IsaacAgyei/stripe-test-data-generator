// See https://aka.ms/new-console-template for more information
// StripeClient pattern (Recommended)
using Stripe;
using DotNetEnv;
using GenerateStripeTestData;

// Load environment variables from .env.local file
Env.Load(".env.local");

// Get Stripe API key from environment variables
var apiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
  throw new InvalidOperationException("STRIPE_API_KEY environment variable is not set or is empty. Make sure .env.local file exists with STRIPE_API_KEY defined.");
}

StripeConfiguration.ApiKey = apiKey;
var client = new StripeClient(apiKey);

Console.WriteLine("=================================================");
Console.WriteLine("Stripe Test Data Generator - Customer Payment Pipeline");
Console.WriteLine("=================================================");

// Create the pipeline
var pipeline = new CustomerPaymentPipeline(client);

// Get number of procedures from command line arguments or default to 3
int numberOfProcedures = 3;
if (args.Length > 0 && int.TryParse(args[0], out int userInput) && userInput > 0)
{
  numberOfProcedures = userInput;
}

Console.WriteLine($"\nExecuting {numberOfProcedures} procedure(s)...");

// Execute the pipeline
try
{
  var results = pipeline.Execute(numberOfProcedures);
  
  Console.WriteLine("\n=================================================");
  Console.WriteLine("Summary");
  Console.WriteLine("=================================================");
  Console.WriteLine($"Successfully completed {results.Count} procedure(s)");
  
  for (int i = 0; i < results.Count; i++)
  {
    var result = results[i];
    Console.WriteLine($"\nProcedure {i + 1}:");
    Console.WriteLine($"  Customer ID: {result.Customer.Id}");
    Console.WriteLine($"  Customer Name: {result.Customer.Name}");
    Console.WriteLine($"  Customer Email: {result.Customer.Email}");
    Console.WriteLine($"  PaymentIntent ID: {result.PaymentIntent.Id}");
    Console.WriteLine($"  Amount: ${result.PaymentIntent.Amount / 100.0:F2} {result.PaymentIntent.Currency.ToUpper()}");
    Console.WriteLine($"  Status: {result.PaymentIntent.Status}");
  }
  
  Console.WriteLine("\n=================================================");
}
catch (StripeException ex)
{
  Console.WriteLine($"\n❌ Stripe API Error: {ex.Message}");
  Console.WriteLine($"Error Code: {ex.StripeError?.Code}");
  Environment.Exit(1);
}
catch (Exception ex)
{
  Console.WriteLine($"\n❌ Error: {ex.Message}");
  Environment.Exit(1);
}
// See https://aka.ms/new-console-template for more information
// StripeClient pattern (Recommended)
using Bogus;
using Stripe;
using DotNetEnv;

// Load environment variables from .env.local file
Env.Load(".env.local");

// Get Stripe API key from environment variables
var apiKey = Environment.GetEnvironmentVariable("STRIPE_TEST_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
  throw new InvalidOperationException("STRIPE_TEST_API_KEY environment variable is not set or is empty. Make sure .env.local file exists with STRIPE_TEST_API_KEY defined.");
}

StripeConfiguration.ApiKey = apiKey;
var client = new StripeClient(apiKey);

var testOrders = new Faker<CustomerCreateOptions>()
    .RuleFor(o => o.Name, f => f.Name.FullName())
    .RuleFor(o => o.Email, f => f.Internet.Email())
    .Generate(1);
var options = new CustomerCreateOptions
{
  Name = testOrders[0].Name,
  Email = testOrders[0].Email,
};
var service = new CustomerService();
Customer customer = service.Create(options);

Console.WriteLine($"Created customer {customer.Id} for {customer.Name}");
Console.WriteLine($"api key used:{apiKey} ");
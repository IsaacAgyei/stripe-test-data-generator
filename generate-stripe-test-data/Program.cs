// See https://aka.ms/new-console-template for more information
// StripeClient pattern (Recommended)
using Bogus;
using Stripe;

StripeConfiguration.ApiKey = "REPLACE_WITH_ENV_VAR";

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
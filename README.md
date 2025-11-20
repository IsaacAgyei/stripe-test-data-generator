# stripe-test-data-generator

A RESTful API making it simply to generate test data for Stripe

## Setup

### 1. Get Your Stripe API Key

1. Go to [Stripe Dashboard](https://dashboard.stripe.com/apikeys)
2. Copy your test API key (starts with `sk_test_`)

### 2. Configure Environment Variables

**Option A: Using .env file (Local Development)**

```bash
cp .env.example .env
```

Then edit `.env` and add your Stripe API key:

```
STRIPE_API_KEY=sk_test_your_actual_key_here
```

**Option B: System Environment Variable**

```bash
export STRIPE_API_KEY=sk_test_your_actual_key_here
```

**Option C: .NET User Secrets (Recommended for Development)**

```bash
dotnet user-secrets init
dotnet user-secrets set "STRIPE_API_KEY" "sk_test_your_actual_key_here"
```

### 3. Running the Application

```bash
cd generate-stripe-test-data
dotnet run
```

## Security

- **Never commit `.env` files** - They are gitignored for your protection
- **Use `.env.example`** - Shows required variables without sensitive values
- **Use User Secrets** - Best practice for local development
- **Use environment variables** - Best practice for production
- **Rotate keys regularly** - If a key is accidentally exposed

## Security Warning

⚠️ **DO NOT** commit your `.env` file or API keys to version control!
If you accidentally commit a key:

1. Immediately revoke it in your Stripe Dashboard
2. Generate a new key
3. Use `git filter-branch` or `git filter-repo` to remove it from history

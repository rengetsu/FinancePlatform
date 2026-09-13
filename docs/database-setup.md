# Demo mode, PostgreSQL, and API imports

## What runs where

The app image still contains only the ASP.NET Core app and its dependencies.
`compose.yaml` runs two separate containers:

- `app`: builds the existing Dockerfile and serves http://localhost:8080.
- `db`: PostgreSQL 17, storing data in the named `finance-data` Docker volume.

The app uses the Npgsql .NET driver with parameterized SQL. The initial schema
is in `database/001-initial.sql`. PostgreSQL applies it when initializing an
empty volume. This implementation does not use Entity Framework or EF migrations.
Future schema changes need explicit versioned SQL scripts; changing the initial
script does not update an existing database.

Rebuilding an image or recreating a container does not remove the named volume.
Do not use `docker compose down -v` unless you intend to delete all saved data.
For ordinary shutdown, use `docker compose stop` or `docker compose down`.

## Start locally with Docker

1. Copy `.env.example` to `.env` if you do not already have one. Preserve existing
   settings if the file exists. Set a local `POSTGRES_PASSWORD` and, when ready
   to import data, `POLYGON_API_KEY`. `.env` is ignored by Git and Docker builds.
2. Run `docker compose up --build -d`.
3. Open http://localhost:8080. Data pages default to Demo.

The initial development setup created a random database password in the existing
local `.env`, preserving its previous settings. API access is not configured
automatically from credentials embedded in old source files.

Only localhost ports are published: 8080 for the app and 5433 for PostgreSQL.
This setup is for local development. The app currently has no user accounts or
authorization for imports; add authentication before public hosting.

## Run the app from Visual Studio or dotnet

Start just PostgreSQL with `docker compose up -d db`. Configure the app's
`ConnectionStrings:Finance` in .NET user secrets (or environment variable
`ConnectionStrings__Finance`) with:

```text
Host=localhost;Port=5433;Database=financeplatform;Username=financeplatform;Password=<your .env password>
```

Set `ApiKeys:Polygon` in user secrets, or `ApiKeys__Polygon` as an environment
variable. Then run `dotnet run --project FinancePlatform.csproj`.
The app does not automatically read Docker's `.env` when launched this way.
No connection string or API key is required for demo mode.

## How the data flow works

- Every chart, table, dividend calendar, and calculator request defaults to Demo.
- Clicking Database explicitly selects `?source=Database`. Only then does the
  app query PostgreSQL. There are no startup database connections, migrations,
  seed operations, background refreshes, or database health checks in the app.
- An empty database displays an import prompt. An unavailable database displays
  an error and offers Demo. It never silently substitutes fake records.
- The API page makes no request until Fetch preview is pressed. Choose a ticker,
  a date, and either a daily closing price or the latest recurring dividend with
  an ex-dividend date on/before that date.
- Fetching creates a preview without opening the database. Save to database
  imports that exact preview without another API request. Previews are protected
  against modification and expire after 20 minutes; refetch after an app restart
  if its temporary data-protection keys have changed.
- Stocks are keyed by ticker/date, dividends by provider record ID. Repeated
  imports update the same record. Each save is transactional.
- Chart and table show the most recent saved close per ticker. Dates are shown
  in the stock table. The calendar lists saved dividend events. The calculator
  uses the latest saved recurring dividend per ticker, not the sum of history.

The stock/dividend endpoints do not provide company names, so database entries
use ticker symbols as their display names. Demo companies retain their names.
Dividend import currently supports USD and annual, semiannual, quarterly, or
monthly recurring payments. Unsupported currencies/frequencies and incomplete
responses are rejected rather than misrepresented. Lookup imports one event at
a time; it is not a bulk historical synchronization.

Polygon's API is now served by Massive. The client uses `api.massive.com`, its
daily aggregates endpoint, and `/stocks/v1/dividends`. Provider plan restrictions,
delays, and quotas still apply. The app does not promise real-time quotes.

## Verification

Run `dotnet test FinancePlatform.sln`. For PostgreSQL integration tests, also set
`FINANCE_TEST_DATABASE` to the initialized test database connection string.
Without that variable the integration test is explicitly skipped. It inserts
uniquely named test records, checks updates/latest-price selection, then removes
only its own records. No API key is needed by the automated tests.

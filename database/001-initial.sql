CREATE TABLE IF NOT EXISTS stock_prices (
    ticker text NOT NULL,
    price_date date NOT NULL,
    close_price numeric NOT NULL CHECK (close_price >= 0),
    imported_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (ticker, price_date)
);

CREATE TABLE IF NOT EXISTS dividends (
    provider_id text PRIMARY KEY,
    ticker text NOT NULL,
    amount numeric NOT NULL CHECK (amount >= 0),
    frequency integer NOT NULL CHECK (frequency IN (0, 1, 2, 3)),
    ex_date date NOT NULL,
    pay_date date NOT NULL,
    imported_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS dividends_ticker_ex_date ON dividends (ticker, ex_date DESC);

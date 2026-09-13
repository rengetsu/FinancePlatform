# Project direction

## Demo-first data flow

- Every data page should open with representative fake data by default.
- Each page will offer a toggle or button to switch to real data stored in the database.
- Opening a page in demo mode must not attempt a database connection. Database access starts only when the user explicitly selects database data.
- The API page will let the user explicitly retrieve live data from an external API and insert it into the database.
- Other pages display saved database data when selected; selecting database mode should not itself fetch live API data.
- Demo mode must remain usable without database or API availability.

This is the intended future behavior. The current stock and dividend services still provide only in-memory sample data.

## Collaboration preference

The user handles all Git commits. Do not commit changes on their behalf.

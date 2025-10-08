DataAutomate

DataAutomate is a desktop utility for data engineers that connects to popular relational databases, lets you browse schemas/tables, and exports data to CSV and Parquet with one click.
Built with .NET 9 (Windows Forms) and optimized for fast, low-memory extracts using ADO.NET, Dapper and CsvHelper.

Author: Jose Alejandro Vazquez Oropeza (GitHub: NiponNumb)

Features

Multi-database support: mssql, postgres, mysql, sqlite.

Schema browser: Load catalog, select individual tables to extract.

One-click export:

CSV output (option to GZip compress).

Parquet output.

Column sanitization (optional) for safe, portable headers.

Affix & frequency helpers: configurable text rules to normalize or tag columns (min affix length / min frequency).

Cancelable runs with responsive UI logging.

Clear, structured output: one file per table under the chosen output directory.

UI controls include provider dropdown, connection string box, “Test connection”, “Load catalog”, tree view for schema selection, output folder picker, Extract and Stop buttons, plus options for gzip, sanitization, Parquet and affix/frequency settings.

Why use DataAutomate?

Fast, no-frills data extraction for analytics and migrations.

Consistent CSV/Parquet outputs ready for downstream tools (Python, Spark, Power BI, etc.).

Simple Windows app—no agents or servers needed.

Requirements

Windows 10/11

.NET 9 SDK (for building from source)
(You can also run the packaged DataAutomate.exe under bin/Debug/net9.0-windows/ if provided.)

Network access to your database(s)

Build & Run

Clone the repository.

Open DataAutomate.sln in Visual Studio 2022 (or newer).

Select Debug or Release, then Build and Run.

Quick Start (Usage)

Provider: choose mssql, postgres, mysql, or sqlite.

Connection:

SQL Server:
Server=HOST,1433;Database=DB;User Id=USER;Password=PASS;TrustServerCertificate=True;

PostgreSQL:
Host=HOST;Port=5432;Database=DB;Username=USER;Password=PASS;

MySQL:
Server=HOST;Port=3306;Database=DB;User ID=USER;Password=PASS;

SQLite:
Data Source=C:\path\your.db;

Click Test to validate the connection.

Click Load catalog and select the tables you want (tree view).

Choose Output folder.

(Optional) Enable Sanitize, Parquet, or GZip as needed.

Click Extract. Watch progress in the log panel. Use Stop to cancel.

Outputs

...\OutputRoot\{schema}.{table}.csv (or .csv.gz if GZip)

...\OutputRoot\{schema}.{table}.parquet (if Parquet enabled)

Notes & Recommendations

CSV encoding: use UTF-8. If you plan to open directly in Excel, consider enabling a BOM for best accent/diacritics compatibility; for pipelines and code tools, prefer UTF-8 without BOM.

Sanitization helps generate portable header names (e.g., lowercasing, underscores). Keep original names when fidelity is required downstream.

Parquet is columnar and well-suited for analytics workloads; CSV is universally compatible.

Roadmap

Incremental extracts (by timestamp/key)

Basic WHERE/JOIN filters per table

Job definitions & batch runs (saved presets)

CLI mode for headless servers

Simple data profiling summaries per table

Security

Connection strings are used only at runtime by the app to read your data.

Always use least-privilege DB users and avoid storing credentials in plaintext.

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

© 2025 Jose Alejandro Vazquez Oropeza (NiponNumb)

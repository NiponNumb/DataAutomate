<h1 align="center">🚀 DataAutomate</h1>

<p align="center">
  <b>Modern Data Extraction Tool for Engineers</b><br>
  Extract, transform, and export data from your databases to <b>CSV</b> or <b>Parquet</b> with a single click.<br>
  Built on <b>.NET 9 (Windows Forms)</b> and optimized for <b>speed</b>, <b>stability</b>, and <b>low memory usage</b>.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-.NET%209-blueviolet?logo=csharp">
  <img src="https://img.shields.io/badge/Platform-Windows-blue">
  <img src="https://img.shields.io/badge/License-MIT-green.svg">
  <img src="https://img.shields.io/badge/Status-Stable-success.svg">
</p>

---

### 👨‍💻 Author  
**José Alejandro Vázquez Oropeza**  
GitHub: [@NiponNumb](https://github.com/NiponNumb)

---

## 🌟 Features

- ✅ **Multi-database support:** `mssql`, `postgres`, `mysql`, `sqlite`  
- ✅ **Schema browser:** visualize and select tables easily  
- ✅ **One-click export:**  
  - CSV output (optional GZip compression)  
  - Parquet output  
- ✅ **Column sanitization:** clean and portable headers  
- ✅ **Affix & frequency helpers:** text normalization and tagging  
- ✅ **Cancelable runs:** stop jobs anytime  
- ✅ **Real-time UI logging** with clean progress display  
- ✅ **Structured outputs:** each table = one file  

**🪟 UI Controls**  
Provider selector • Connection string input • Test connection • Load catalog •  
Tree view for schema • Output folder picker • Extract / Stop buttons •  
GZip • Parquet • Sanitize • Affix/Frequency Settings  

---

## 💡 Why Use DataAutomate?

- ⚡ **Fast** — lightweight, zero server dependencies  
- 📊 **Compatible** — perfect CSV/Parquet for analytics (Python, Spark, Power BI)  
- 🪶 **Portable** — simple Windows app, no setup headaches  

---

## 🧩 Requirements

| Requirement     | Version / Info |
|-----------------|----------------|
| **OS**          | Windows 10 / 11 |
| **Framework**   | .NET 9 SDK |
| **IDE**         | Visual Studio 2022 or newer (optional) |
| **Network**     | Access to your DB instance(s) |

> 💡 You can also run `DataAutomate.exe` directly from  
> `bin/Debug/net9.0-windows/` if you don't need to rebuild.

## ⚙️ Build & Run

# Clone the repository
git clone https://github.com/NiponNumb/DataAutomate.git
cd DataAutomate

# Open and run via Visual Studio 2022 or later

<!-- HEADER --> <p align="center"> <img src="https://img.shields.io/badge/C%23-.NET%209-blueviolet?logo=csharp" alt=".NET 9"> <img src="https://img.shields.io/badge/Platform-Windows-blue" alt="Windows"> <img src="https://img.shields.io/badge/License-MIT-green.svg" alt="MIT License"> <img src="https://img.shields.io/badge/Status-Stable-success.svg" alt="Status"> </p> <h1 align="center">DataAutomate</h1> <p align="center"> <b>Modern Data Extraction Tool</b><br/> Extract, transform, and export data to <b>CSV</b> or <b>Parquet</b> with a single click. </p> <p align="center"> <a href="#-quick-start">Quick Start</a> · <a href="#-usage">Usage</a> · <a href="#-output-structure">Output</a> · <a href="#-notes--recommendations">Notes</a> · <a href="#-roadmap">Roadmap</a> · <a href="#-security">Security</a> · <a href="#-license">License</a> </p>
🧩 Solution Setup
DataAutomate.sln
1️⃣ Open the solution
2️⃣ Choose Debug or Release
3️⃣ Click Build → Run

🚀 Quick Start
Providers

mssql

postgres

mysql

sqlite

Connection Strings
<details> <summary><b>SQL Server (mssql)</b></summary>
Server=HOST,1433;Database=DB;User Id=USER;Password=PASS;TrustServerCertificate=True;

</details> <details> <summary><b>PostgreSQL (postgres)</b></summary>
Host=HOST;Port=5432;Database=DB;Username=USER;Password=PASS;

</details> <details> <summary><b>MySQL (mysql)</b></summary>
Server=HOST;Port=3306;Database=DB;User ID=USER;Password=PASS;

</details> <details> <summary><b>SQLite (sqlite)</b></summary>
Data Source=C:\path\your.db;

</details>
🛠 Usage

Select Provider (mssql, postgres, mysql, sqlite)

Enter Connection String

Click <kbd>Test</kbd> to validate the connection

Click <kbd>Load Catalog</kbd> to browse tables

Select Tables to extract

Choose Output Folder

(Optional) Enable Sanitize, Parquet, or GZip

Press <kbd>Extract</kbd> and watch logs in real time

Press <kbd>Stop</kbd> anytime to cancel safely

📁 Output Structure
📂 OutputRoot/
 ├─ public.customers.csv
 ├─ public.orders.csv.gz      # if GZip enabled
 └─ public.invoices.parquet   # if Parquet enabled

🧠 Notes & Recommendations

[!TIP]
CSV Encoding: use UTF-8. For Excel viewers, enable BOM for proper accents.
For data pipelines, prefer UTF-8 without BOM.

[!NOTE]
Sanitize column names for consistent headers across systems.

[!IMPORTANT]
Parquet is ideal for analytics (columnar, compressed). CSV maximizes interoperability.

🧭 Roadmap

 Incremental extracts (timestamp/key based)

 Basic WHERE / JOIN filters per table

 Job presets & batch runs

 CLI (headless mode)

 Data profiling summaries per table

🔒 Security

[!WARNING]
Connection strings are used only at runtime. Avoid storing plaintext passwords.

Use least-privilege database credentials

Do not commit secrets to source control

📜 License

Licensed under the MIT License. See LICENSE for details.

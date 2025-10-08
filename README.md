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

<p align="center">
<br>
DataAutomate.sln
1️⃣ Open the solution
2️⃣ Choose Debug or Release
3️⃣ Click Build → Run

🚀 Quick Start (Usage)
Select your provider
Options: mssql, postgres, mysql, sqlite

Enter your connection string

SQL Server


Server=HOST,1433;Database=DB;User Id=USER;Password=PASS;TrustServerCertificate=True;
PostgreSQL



Host=HOST;Port=5432;Database=DB;Username=USER;Password=PASS;
MySQL


Server=HOST;Port=3306;Database=DB;User ID=USER;Password=PASS;
SQLite


Data Source=C:\path\your.db;
Steps

Click Test to validate the connection

Click Load Catalog to browse tables

Select the tables you want

Choose your Output Folder

(Optional) Enable Sanitize, Parquet, or GZip

Press Extract and watch logs in real-time

Press Stop anytime to cancel safely

📁 Output Structure

📂 OutputRoot/
 ├─ public.customers.csv
 ├─ public.orders.csv.gz      # if GZip enabled
 └─ public.invoices.parquet   # if Parquet enabled
🧠 Notes & Recommendations
CSV Encoding: use UTF-8

For Excel, enable BOM for accent compatibility

For pipelines, prefer UTF-8 without BOM

Sanitize column names for consistent headers

Parquet is ideal for analytics; CSV for interoperability

🧭 Roadmap
⏳ Incremental extracts (timestamp/key based)

⏳ Basic WHERE/JOIN filters per table

⏳ Job presets & batch runs

⏳ CLI (headless mode)

⏳ Data profiling summaries per table

🔒 Security
Connection strings are used only at runtime

Always use least-privilege database credentials

Avoid storing plaintext passwords

📜 License
This project is licensed under the MIT License.
See the LICENSE file for details.
</br>
</p>

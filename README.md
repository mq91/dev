# Attendance Permission Analyzer CLI

A simple yet powerful command-line tool to analyze late arrivals and early exits based on attendance CSV files.

Created by **Eng. Mohammed Alqarni**

---

## 🔍 What It Does

This CLI tool reads an Arabic-encoded attendance CSV file (Windows-1256), detects permission periods (arriving after 9:00 AM or leaving before completing 8 hours), and allows you to:

- Filter data by date range
- Preview permission periods in the terminal
- Export results as a professional Excel report

---

## ✅ Features

- Supports Arabic column headers
- Automatically calculates permission time in minutes
- Interactive CLI with user prompts
- Exports to `استئذانات.xlsx` in Arabic
- No admin privileges required to run (when built as `.exe`)

---

## 📁 CSV Format Requirements

Make sure your CSV includes the following Arabic headers:

- `تاريخ الحركة ميلادي` (Date)
- `وقت الدخول الفعلي` (Actual Entry Time) — format: `yyyy/MM/dd HH:mm:ss`
- `وقت الخروج الفعلي` (Actual Exit Time) — format: `yyyy/MM/dd HH:mm:ss`

Encoding should be **Windows-1256 (cp1256)**.

---

## 🚀 How to Run

### From Source:

```bash
dotnet run

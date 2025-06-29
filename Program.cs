// Attendance Permission Analyzer CLI Tool
// Created by Eng. Mohammed Alqarni

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using ClosedXML.Excel;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("\n🎯 Attendance Permission Analyzer - Created by Eng. Mohammed Alqarni");

        // Ask user for the CSV file name
        Console.Write("\n📂 Enter CSV file name (with extension): ");
        string path = Console.ReadLine();

        if (!File.Exists(path))
        {
            Console.WriteLine("[!] File does not exist.");
            return;
        }

        // Ask if the user wants to filter by date
        Console.Write("\n📅 Do you want to filter by date range? (y/n): ");
        string useDates = Console.ReadLine()?.Trim().ToLower();

        DateTime? startDate = null, endDate = null;
        if (useDates == "y")
        {
            Console.Write("🔹 Start date (e.g., 01/05/2025): ");
            startDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            Console.Write("🔹 End date (e.g., 31/05/2025): ");
            endDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // Setup for reading Arabic CSV using Windows-1256 encoding
        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("ar-SA"))
        {
            Delimiter = ",",
            Encoding = System.Text.Encoding.GetEncoding(1256),
            PrepareHeaderForMatch = args => args.Header.Trim(),
            MissingFieldFound = null
        };

        var records = new List<dynamic>();

        // Read and process each record in the CSV
        using (var reader = new StreamReader(path, System.Text.Encoding.GetEncoding(1256)))
        using (var csv = new CsvReader(reader, config))
        {
            var data = csv.GetRecords<dynamic>().ToList();
            foreach (var row in data)
            {
                string dateStr = row["تاريخ الحركة ميلادي"];
                string inStr = row["وقت الدخول الفعلي"];
                string outStr = row["وقت الخروج الفعلي"];

                if (string.IsNullOrWhiteSpace(inStr) || string.IsNullOrWhiteSpace(outStr))
                    continue;

                DateTime date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (startDate.HasValue && date < startDate.Value) continue;
                if (endDate.HasValue && date > endDate.Value) continue;

                DateTime inTime = DateTime.ParseExact(inStr, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime outTime = DateTime.ParseExact(outStr, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime nineAM = date.AddHours(9);
                DateTime exitLimit = inTime.AddHours(8);
                DateTime cap17 = date.AddHours(17);
                if (exitLimit > cap17) exitLimit = cap17;

                // Late arrival permission
                if (inTime > nineAM)
                {
                    var mins = (int)(inTime - nineAM).TotalMinutes;
                    records.Add(new { From = Format(nineAM), To = Format(inTime), Minutes = mins });
                }

                // Early leave permission
                if (outTime < exitLimit)
                {
                    var mins = (int)(exitLimit - outTime).TotalMinutes;
                    records.Add(new { From = Format(outTime), To = Format(exitLimit), Minutes = mins });
                }
            }
        }

        if (records.Count == 0)
        {
            Console.WriteLine("\n[!] No results found.");
            return;
        }

        // Display in terminal
        Console.Write("\n👀 Show results in terminal? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            Console.WriteLine("\nFrom\t\tTo\t\tDuration (Minutes)");
            foreach (var r in records)
                Console.WriteLine($"{r.From}\t{r.To}\t{r.Minutes}");
        }

        // Export to Excel
        Console.Write("\n💾 Save results to Excel file? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Permissions");
            ws.Cell(1, 1).Value = "From";
            ws.Cell(1, 2).Value = "To";
            ws.Cell(1, 3).Value = "Duration (Minutes)";
            int i = 2;
            foreach (var r in records)
            {
                ws.Cell(i, 1).Value = r.From;
                ws.Cell(i, 2).Value = r.To;
                ws.Cell(i, 3).Value = r.Minutes;
                i++;
            }
            wb.SaveAs("Permissions.xlsx");
            Console.WriteLine("\n✅ File saved as: Permissions.xlsx");
        }

        Console.WriteLine("\n🎉 Analysis completed successfully.");
    }

    // Custom time formatting in Arabic style with AM/PM as ص/م
    static string Format(DateTime dt)
    {
        string period = dt.Hour < 12 ? "ص" : "م";
        int hour12 = dt.Hour % 12 == 0 ? 12 : dt.Hour % 12;
        return dt.ToString($"dd/MM/yy {hour12:00}:{dt.Minute:00} {period}");
    }
}

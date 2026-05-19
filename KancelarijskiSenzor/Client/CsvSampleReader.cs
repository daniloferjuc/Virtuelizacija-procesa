using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Client
{
    class CsvSampleReader
    {
        private string csvPath;
        private string logPath;
        private int rowLimit;

        public string CsvPath { get => csvPath; }
        public string LogPath { get => logPath; }
        public int RowLimit { get => rowLimit; }

        public CsvSampleReader(string csvPath, string logPath, int rowLimit)
        {
            this.csvPath = csvPath;
            this.logPath = logPath;
            this.rowLimit = rowLimit;
        }

        public List<SensorSample> LoadSamples()
        {
            List<SensorSample> samples = new List<SensorSample>();

            if (!File.Exists(csvPath))
            {
                LogLine($"[ERROR] CSV fajl ne postoji: {csvPath}");
                return samples;
            }

            string logDir = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            using (StreamReader reader = new StreamReader(csvPath))
            {
                string header = reader.ReadLine();
                LogLine($"[INFO] Header: {header}");

                int rowIndex = 0;
                int loaded = 0;
                int rejected = 0;

                while (!reader.EndOfStream && loaded < rowLimit)
                {
                    rowIndex++;
                    string line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        LogLine($"[REJECT] Red {rowIndex}: prazan red.");
                        rejected++;
                        continue;
                    }

                    SensorSample sample = TryParseLine(line, rowIndex);
                    if (sample != null)
                    {
                        samples.Add(sample);
                        loaded++;
                    }
                    else
                    {
                        rejected++;
                    }
                }

                LogLine($"[INFO] Ucitano {loaded} redova, odbijeno {rejected}.");

                // Logovanje redova visak
                int extraCount = 0;
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    extraCount++;
                }

                if (extraCount > 0)
                {
                    LogLine($"[INFO] Preskoceno {extraCount} redova visak (limit je {rowLimit}).");
                }
            }

            return samples;
        }

        private SensorSample TryParseLine(string line, int rowIndex)
        {
            string[] parts = line.Split(',');

            if (parts.Length != 10)
            {
                LogLine($"[REJECT] Red {rowIndex}: ocekivano 10 kolona, dobijeno {parts.Length}. Sadrzaj: {line}");
                return null;
            }

            try
            {
                DateTime dt = DateTime.Parse(parts[0], CultureInfo.InvariantCulture);
                double volume = double.Parse(parts[1], CultureInfo.InvariantCulture);
                double pressure = double.Parse(parts[4], CultureInfo.InvariantCulture);
                double co = double.Parse(parts[8], CultureInfo.InvariantCulture);
                double no2 = double.Parse(parts[9], CultureInfo.InvariantCulture);

                return new SensorSample(volume, co, no2, pressure, dt);
            }
            catch (FormatException ex)
            {
                LogLine($"[REJECT] Red {rowIndex}: format greska - {ex.Message}. Sadrzaj: {line}");
                return null;
            }
            catch (Exception ex)
            {
                LogLine($"[REJECT] Red {rowIndex}: greska - {ex.Message}. Sadrzaj: {line}");
                return null;
            }
        }

        private void LogLine(string message)
        {
            string stamped = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
            Console.WriteLine(stamped);
            try
            {
                File.AppendAllText(logPath, stamped + Environment.NewLine);
            }
            catch
            {
                // ne sme da pukne klijent ako log fajl ne moze da se otvori
            }
        }
    }
}
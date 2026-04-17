using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GetDownloadedDumbass.Models;

namespace GetDownloadedDumbass.Services
{
    public class HistoryService
    {
        private readonly string _historyPath = "history.json";

        public List<DownloadItem> Load()
        {
            try
            {
                if (!File.Exists(_historyPath))
                    return new List<DownloadItem>();

                string json = File.ReadAllText(_historyPath);
                return JsonConvert.DeserializeObject<List<DownloadItem>>(json) ?? new List<DownloadItem>();
            }
            catch
            {
                return new List<DownloadItem>();
            }
        }

        public void Save(List<DownloadItem> history)
        {
            try
            {
                string json = JsonConvert.SerializeObject(history, Formatting.Indented);
                File.WriteAllText(_historyPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save history: {ex.Message}");
            }
        }

        public void Add(DownloadItem item)
        {
            var history = Load();
            history.Insert(0, item);
            Save(history);
        }
    }
}
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MaksimaSDnuhoyTebya
{
    public class PasswordService
    {
        private readonly string filePath;

        public PasswordService(string filePath)
        {
            this.filePath = filePath;
        }

        public List<PasswordEntry> LoadPasswords()
        {
            if (!File.Exists(filePath))
            {
                return new List<PasswordEntry>();
            }

            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<PasswordEntry>>(jsonData);
        }

        public void SavePasswords(List<PasswordEntry> passwords)
        {
            var jsonData = JsonConvert.SerializeObject(passwords, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }
}

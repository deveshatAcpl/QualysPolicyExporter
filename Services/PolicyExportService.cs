using Newtonsoft.Json.Linq;
using QualysPolicyExporter.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QualysPolicyExporter.Services
{
    public class PolicyExportService
    {
        private readonly HttpClient _httpClient;

        public PolicyExportService()
        {
            var handler = new HttpClientHandler();

            if (Properties.Settings.Default.EnableProxy &&
                !string.IsNullOrWhiteSpace(Properties.Settings.Default.ProxyUrl))
            {
                var proxy = new WebProxy(Properties.Settings.Default.ProxyUrl);
                handler.Proxy = proxy;
                handler.UseProxy = true;
                Debug.WriteLine($"[INFO] Proxy enabled: {Properties.Settings.Default.ProxyUrl}");
            }
            else
            {
                handler.UseProxy = false;
            }

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(15)
            };
        }

        public async Task<string> ExportPoliciesAsync(string username, string password, string exportPath, bool filterPassedToFailedOnly, string technologyIds, string techIdMode)
        {
            AppendToLog(exportPath, "Starting export process...");

            string token = await GetTokenAsync(username, password);
            if (string.IsNullOrEmpty(token))
            {
                AppendToLog(exportPath, "Failed to acquire API token.", false);
                throw new Exception("Failed to acquire API token.");
            }
            AppendToLog(exportPath, "Token acquired successfully.");

            List<(string Id, string Name)> policyList;
            string customIds = Properties.Settings.Default.SelectedPolicyIds;

            if (!string.IsNullOrWhiteSpace(customIds))
            {
                var ids = customIds.Split(',')
                                   .Select(id => id.Trim())
                                   .Where(id => !string.IsNullOrWhiteSpace(id))
                                   .Distinct()
                                   .ToList();

                policyList = ids.Select(id => (id, "(Custom Policy)")).ToList();
                AppendToLog(exportPath, $"Using custom policy IDs from settings: {string.Join(", ", ids)}");
            }
            else
            {
                policyList = await GetPolicyIdsAsync(token);
                if (policyList == null || policyList.Count == 0)
                {
                    AppendToLog(exportPath, "No policies retrieved from Qualys.", false);
                    throw new Exception("No policies retrieved.");
                }
                AppendToLog(exportPath, $"Retrieved {policyList.Count} policies.");
            }

            var allData = new List<Dictionary<string, string>>();

            for (int i = 0; i < policyList.Count; i += 10)
            {
                var batch = policyList.Skip(i).Take(10).ToList();
                AppendToLog(exportPath, $"Fetching details for policy IDs: {string.Join(", ", batch.Select(p => p.Id))}...");

                var batchData = await GetPolicyDetailsBatchAsync(batch, username, password, exportPath);
                if (batchData != null)
                {
                    AppendToLog(exportPath, $"Retrieved {batchData.Count} records for this batch.");
                    allData.AddRange(batchData);
                }
                else
                {
                    AppendToLog(exportPath, $"Failed to retrieve data for policy batch: {string.Join(", ", batch.Select(p => p.Id))}.", false);
                }
            }

            AppendToLog(exportPath, $"Total records fetched: {allData.Count}");



            var techIdsList = technologyIds
             .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(id => id.Trim())
             .ToList();

            var preFilteredData = allData;

             //Apply technology ID filter
            if (techIdsList.Any())
            {
                if (techIdMode == "Exclude")
                {
                    preFilteredData = preFilteredData
                        .Where(row => !(row.TryGetValue("TECHNOLOGY_ID", out var techId) && techIdsList.Contains(techId)))
                        .ToList();
                }
                else if (techIdMode == "Include")
                {
                    preFilteredData = preFilteredData
                        .Where(row => row.TryGetValue("TECHNOLOGY_ID", out var techId) && techIdsList.Contains(techId))
                        .ToList();
                }
            }

            // Apply Passed → Failed filter if needed
            var filteredData = filterPassedToFailedOnly
                ? preFilteredData.Where(row =>
                    row.TryGetValue("PREVIOUS_STATUS", out var prev) &&
                    row.TryGetValue("STATUS", out var curr) &&
                    prev.Equals("Passed", StringComparison.OrdinalIgnoreCase) &&
                    curr.Equals("Failed", StringComparison.OrdinalIgnoreCase)).ToList()
                : preFilteredData;

            //var preFilteredData = allData
            //.Where(row => !(row.TryGetValue("TECHNOLOGY_ID", out var techId) && techId == "91"))
            //.ToList();

            //var filteredData = filterPassedToFailedOnly
            //    ? preFilteredData.Where(row =>
            //        row.TryGetValue("PREVIOUS_STATUS", out var prev) &&
            //        row.TryGetValue("STATUS", out var curr) &&
            //        prev.Equals("Passed", StringComparison.OrdinalIgnoreCase) &&
            //        curr.Equals("Failed", StringComparison.OrdinalIgnoreCase)).ToList()
            //    : preFilteredData;


            //var filteredData = filterPassedToFailedOnly
            //    ? allData.Where(row =>
            //        row.TryGetValue("PREVIOUS_STATUS", out var prev) &&
            //        row.TryGetValue("STATUS", out var curr) &&
            //        prev.Equals("Passed", StringComparison.OrdinalIgnoreCase) &&
            //        curr.Equals("Failed", StringComparison.OrdinalIgnoreCase)).ToList()
            //    : allData;




            AppendToLog(exportPath, $"Filtered records count: {filteredData.Count}");

            if (filteredData.Count == 0)
            {
                AppendToLog(exportPath, "No matching data found for the specified filter.", false);
                throw new Exception("No matching data found for the specified filter.");
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string fileName = $"policy_export_{timestamp}.csv";
            string filePath = Path.Combine(exportPath, fileName);

            WriteCsv(filteredData, filePath);
            AppendToLog(exportPath, $"Exported {filteredData.Count} rows to {fileName}", true);

            return filePath;
        }

        private async Task<string> GetTokenAsync(string username, string password)
        {
            var form = new StringContent($"username={username}&password={password}&token=true", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("https://gateway.qg1.apps.qualys.in/auth", form);
            Debug.WriteLine("Authorized...");

            return response.IsSuccessStatusCode ? (await response.Content.ReadAsStringAsync()).Trim() : null;
        }

        //private async Task<List<(string Id, string Name)>> GetPolicyIdsAsync(string token)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var response = await _httpClient.GetAsync("https://gateway.qg1.apps.qualys.in/pcrs/1.0/posture/policy/list");

        //    if (!response.IsSuccessStatusCode) return null;

        //    var content = await response.Content.ReadAsStringAsync();
        //    var policies = JObject.Parse(content)?["policyList"];
        //    var idNameList = new List<(string Id, string Name)>();

        //    foreach (var p in policies)
        //    {
        //        var id = p["id"]?.ToString();
        //        var name = p["title"]?.ToString() ?? "(Unnamed)";
        //        if (!string.IsNullOrEmpty(id))
        //        {
        //            idNameList.Add((id, name));
        //            Debug.WriteLine($"Policy: ID = {id}, Name = {name}");
        //        }
        //    }

        //    return idNameList;
        //}


        private async Task<List<(string Id, string Name)>> GetPolicyIdsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("https://gateway.qg1.apps.qualys.in/pcrs/1.0/posture/policy/list");

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var policies = JObject.Parse(content)?["policyList"];
            var idNameList = new List<(string Id, string Name)>();

            // Timezone setup for India Standard Time (IST)
            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);
            var firstOfMonthIST = new DateTime(istNow.Year, istNow.Month, 1);
            var firstOfMonthUtc = TimeZoneInfo.ConvertTimeToUtc(firstOfMonthIST, istZone);

            // Exclude list
            //var excludedIds = new HashSet<string> { "3007614", "3017023", "3018133" };
            var excludedIds = new HashSet<string> {"3018133" };

            foreach (var p in policies)
            {
                var id = p["id"]?.ToString();
                var name = p["title"]?.ToString() ?? "(Unnamed)";
                var status = p["status"]?.ToString();
                var lastEvalStr = p["lastEvaluatedDate"]?.ToString();

                // Exclude by ID
                if (string.IsNullOrEmpty(id) || excludedIds.Contains(id))
                    continue;

                // Exclude inactive policies
                if (status?.Equals("inactive", StringComparison.OrdinalIgnoreCase) == true)
                    continue;

                // Exclude policies not evaluated after 1st of current month (IST)
                //if (!DateTime.TryParse(lastEvalStr, out var lastEvaluatedUtc) || lastEvaluatedUtc <= firstOfMonthUtc)
                //    continue;

                idNameList.Add((id, name));
                Debug.WriteLine($"Policy: ID = {id}, Name = {name}, Status = {status}, LastEval = {lastEvalStr}");
            }

            return idNameList;
        }


        private async Task<List<Dictionary<string, string>>> GetPolicyDetailsBatchAsync(List<(string Id, string Name)> policies, string username, string password, string exportPath)
        {
            var handler = new HttpClientHandler();

            if (Properties.Settings.Default.EnableProxy &&
                !string.IsNullOrWhiteSpace(Properties.Settings.Default.ProxyUrl))
            {
                var proxy = new WebProxy(Properties.Settings.Default.ProxyUrl);
                handler.Proxy = proxy;
                handler.UseProxy = true;
            }
            else
            {
                handler.UseProxy = false;
            }

            using (var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(15)
            })
            {
                string idsString = string.Join(",", policies.Select(p => p.Id));
                //DateTime nowIst = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                //string evaluationDate = nowIst.AddDays(-1).ToString("yyyy-MM-dd");

                string evaluationDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");


                //string url = $"https://qualysapi.qg1.apps.qualys.in/api/2.0/fo/compliance/posture/info/?action=list&policy_ids={idsString}&evaluation_date={evaluationDate}&status=Failed";
                string url = $"https://qualysapi.qg1.apps.qualys.in/api/2.0/fo/compliance/posture/info/?action=list&policy_ids={idsString}&status=Failed";

                var authBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                client.DefaultRequestHeaders.Add("X-Requested-With", "Curl Demo");

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Failed to fetch posture info for policies: {idsString}");
                    return null;
                }

                var xml = await response.Content.ReadAsStringAsync();
                var document = XDocument.Parse(xml);

                var entries = new List<Dictionary<string, string>>();

                foreach (var policyElement in document.Descendants("POLICY"))
                {
                    var policyId = policyElement.Element("ID")?.Value ?? "";
                    var policyName = policies.FirstOrDefault(p => p.Id == policyId).Name ?? "(Unknown)";

                    var infoList = policyElement.Element("INFO_LIST");
                    if (infoList == null) continue;
                    int daysBefore = Properties.Settings.Default.DaysBeforeToday;
                    var yesterdayDate = DateTime.UtcNow.Date.AddDays(-daysBefore);
                    //var yesterdayDate = DateTime.UtcNow.AddMinutes(-330).Date.AddDays(-1);
                    AppendToLog(exportPath, $"📅 Threshold: Including entries with LAST_FAIL_DATE.Date >= {DateTime.UtcNow.Date.AddDays(-daysBefore)}");

                    foreach (var info in infoList.Elements("INFO"))
                    {
                        //var lastPassStr = info.Element("LAST_PASS_DATE")?.Value;
                        var lastFailStr = info.Element("LAST_FAIL_DATE")?.Value;
                        var prevStatus = info.Element("PREVIOUS_STATUS")?.Value ?? "";


                        if (!DateTime.TryParse(lastFailStr, out var lastFailDateUtc))
                        {
                            //AppendToLog(exportPath, $"⚠️ Skipped entry: Invalid LAST_PASS_DATE = '{lastPassStr}'");
                            continue;
                        }

                        // Log the parsed date and comparison
                        //AppendToLog(exportPath, $" {policyId} ->  lastpassstr = {lastFailStr}🔍 LAST_PASS_DATE = {lastFailDateUtc:yyyy-MM-ddTHH:mm:ssZ} | Compared Date = {lastFailDateUtc.Date:yyyy-MM-dd}");

                        var adjustedDate = lastFailDateUtc.AddMinutes(-330).Date;

                        if (adjustedDate <= yesterdayDate || !prevStatus.Equals("Passed", StringComparison.OrdinalIgnoreCase))
                        {
                            //AppendToLog(exportPath, $"❌ Excluded (Before threshold): {lastPassDateUtc:yyyy-MM-ddTHH:mm:ssZ}");
                            continue;
                        }

                        AppendToLog(exportPath, $" {policyId} ->  lastpassstr = {lastFailStr}🔍 LAST_PASS_DATE = {lastFailDateUtc:yyyy-MM-ddTHH:mm:ssZ} | Compared Date = {lastFailDateUtc.Date:yyyy-MM-dd}");
                        AppendToLog(exportPath, $"✅ Included: {lastFailDateUtc:yyyy-MM-ddTHH:mm:ssZ}");

                        // Passed filter → collect entry
                        var entry = new Dictionary<string, string>
                        {
                            ["POLICY_ID"] = policyId,
                            ["POLICY_NAME"] = policyName
                        };

                        foreach (var key in new[]
                        {
                            "HOST_ID", "CONTROL_ID", "TECHNOLOGY_ID", "INSTANCE", "STATUS",
                            "POSTURE_MODIFIED_DATE", "EVALUATION_DATE", "PREVIOUS_STATUS",
                            "FIRST_FAIL_DATE", "LAST_FAIL_DATE", "FIRST_PASS_DATE", "LAST_PASS_DATE"
                        })
                        {
                            entry[key] = info.Element(key)?.Value ?? string.Empty;
                        }

                        entries.Add(entry);
                    }


                }

                return entries;
            }
        }

        private void WriteCsv(List<Dictionary<string, string>> data, string path)
        {
            if (data == null || data.Count == 0) return;

            var headers = data[0].Keys.ToArray();
            using var writer = new StreamWriter(path);
            writer.WriteLine(string.Join(",", headers));

            foreach (var row in data)
            {
                writer.WriteLine(string.Join(",", headers.Select(h => row[h]?.Replace(",", " "))));
            }
        }

        public void AppendToLog(string exportPath, string message, bool? isSuccess = null)
        {
            string logFile = Path.Combine(exportPath, "export_log.txt");
            string status = isSuccess == true ? "✅ SUCCESS" :
                            isSuccess == false ? "❌ FAILURE" : "ℹ️ INFO";
            string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {status}: {message}";
            File.AppendAllText(logFile, logLine + Environment.NewLine);
        }
    }
}

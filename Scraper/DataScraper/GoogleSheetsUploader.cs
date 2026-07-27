using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Collections.Generic;

class GoogleSheetsUploader
{
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string SpreadsheetId = "1GZesYn5QZQHMYdxecpzPRhJ1eFNot2Dgds0dZi7XFSE";
    static readonly string WRFilePath = @"C:\Users\ptige\Documents\FantasyWebApp\Scraper\WR Data.txt";
    static readonly string QBFilePath = @"C:\Users\ptige\Documents\FantasyWebApp\Scraper\QB Data 2026.txt";
    static readonly string TEFilePath = @"C:\Users\ptige\Documents\FantasyWebApp\Scraper\TE Data.txt";
    static readonly string RBFilePath = @"C:\Users\ptige\Documents\FantasyWebApp\Scraper\RB Data.txt";

    static void Main()
    {
        var credential = GoogleCredential.FromFile(@"C:\Users\ptige\OneDrive\Documents\Gunnar Notes\fantasy-data-scraping-ef15cc5a444d.json")
            .CreateScoped(Scopes);

        var service = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "FantasyDataUploader"
        });

        Dictionary<string, string> filePaths = new Dictionary<string, string>
        {
            { "WR", WRFilePath },
            { "QB", QBFilePath },
            { "TE", TEFilePath },
            { "RB", RBFilePath }
        };

        Dictionary<string, List<object>> headersMap = new Dictionary<string, List<object>>
        {
            // find source for the data
            { "WR", new List<object> { "RK", "Name", "Team", "Pos", "GP", "TGTS", "Rec", "Catch%", "YDs", "TDS", "Long", "YDs/TGT", "YDs/Rec", "ATT", "YDs", "AVG", "TD", "FUM", "LOST", "FPTS/G", "FPTS" } },
            { "TE", new List<object> { "RK", "Name", "Team", "Pos", "GP", "TGTS", "Rec", "Catch%", "YDs", "TDS", "Long", "YDs/TGT", "YDs/Rec", "ATT", "YDs", "AVG", "TD", "FUM", "LOST", "FPTS/G", "FPTS" } },
            { "QB", new List<object> { "RK", "Name", "Team", "Pos", "GP", "CMP", "ATT", "CMP%", "YDs", "AVG", "TDS", "INT", "LONG", "SACK", "RATING", "ATT", "YDs", "AVG", "TD", "FPTS/G", "FPTS" } },
            { "RB", new List<object> { "RK", "Name", "Team", "Pos", "GP", "ATT", "YDs", "AVG", "TDS", "TGTS", "Rec", "YDs", "TDS", "FPTS/G", "FPTS" } }
        };

        string tableKey = "";
        do
        {
            Console.WriteLine("Which Table is being updated? (WR, QB, TE, RB)");
            tableKey = Console.ReadLine().Trim().ToUpper();
        }
        while (!VerifyInput(tableKey));

        string FilePath = filePaths[tableKey];
        string SheetName = tableKey;
        var headers = headersMap[tableKey];

        var headerRange = new ValueRange { Values = new List<IList<object>> { headers } };

        var headerRequest = service.Spreadsheets.Values.Update(headerRange, SpreadsheetId, $"{SheetName}!A1");
        headerRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        headerRequest.Execute();

        var rows = new List<IList<object>>();
        var currentRow = new List<object>();

        foreach (var line in File.ReadLines(FilePath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            var fields = System.Text.RegularExpressions.Regex.Split(trimmed, @"\t");

            if (int.TryParse(fields[0], out _))
            {
                if (currentRow.Count > 0)
                {
                    rows.Add(currentRow);
                    currentRow = new List<object>();
                }
            }

            foreach (var field in fields)
            {
                var value = field.Trim();

                // Replace empty values with "0"
                if (string.IsNullOrEmpty(value))
                    value = "0";

                // Try to convert to a number
                if (double.TryParse(value, out double num))
                    currentRow.Add(num); // Adds as numeric value
                else
                    currentRow.Add(value); // Keeps string (like Name or Team)
            }
        } 

        if (currentRow.Count > 0)
        {
            rows.Add(currentRow);
        }

        var dataRange = new ValueRange { Values = rows };

        var dataRequest = service.Spreadsheets.Values.Update(dataRange, SpreadsheetId, $"{SheetName}!A2");
        dataRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        dataRequest.Execute();
    }

    public static bool VerifyInput(string text)
    {
        var allowed = new HashSet<string> { "WR", "QB", "TE", "RB" };
        return allowed.Contains(text);
    }
}



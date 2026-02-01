using mSzczepienia.Api.Services.Interfaces;

namespace  mSzczepienia.Api.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        public int CalculateLevensteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;
            for (int j = 1; j <= m; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        public string ChooseBestMatch(string? input, List<string> options)
        {
            if (options == null || options.Count == 0 || input == null)
                return string.Empty;

            string bestMatch = options[0];
            int lowestDistance = CalculateLevensteinDistance(input, bestMatch);
            foreach (var option in options.Skip(1))
            {
                int distance = CalculateLevensteinDistance(input, option);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    bestMatch = option;
                }
            }

            // If accuracy is lower than 5%, return empty string
            if (lowestDistance > input.Length * 0.05)
            {
                return string.Empty;
            }
            else
            {
                return bestMatch;
            }
        }

        public DateTime ParseDateTime(string? rawDate)
        {
            DateTime defaultDateTime = new DateTime(1900, 1, 1);
            if (string.IsNullOrWhiteSpace(rawDate))
            {
                return defaultDateTime;
            }

            var separators = new[] { ' ', '\n', '\r', '\t' };
            var cleanedDate = rawDate
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? string.Empty;
            cleanedDate = cleanedDate.Trim().Replace(",", ".");

            var formats = new[]
            { 
                // No spaces
                "yyyyMMdd",
                "ddMMyy",
                "ddMMyyyy",

                // Dots
                "d.M.yyyy",
                "dd.MM.yyyy", 
                "d.M.yy",
                "dd.MM.yy",
        
                // Dashes
                "yyyy-MM-dd",
                "d-M-yyyy", 
                "dd-MM-yyyy",
                "d-M-yy", 
        
                // Slashes
                "yyyy/MM/dd",
                "d/M/yyyy", 
                "dd/MM/yyyy", 
                "d/M/yy",      
        
                // Spaces
                "d M yyyy",
                "yyyy M d"
            };


            if (DateTime.TryParseExact(cleanedDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out DateTime parsedDate))
            {
                return parsedDate;
            }

            
            return defaultDateTime;
        }
    }
}
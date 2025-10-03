# Features Implementation Guide

Quick reference for implementing the three requested optional features.

---

## 1. Authentication Support for Higher API Rate Limits

### Overview
Allow users to optionally provide Stack Exchange API credentials for increased rate limits.

### Current Limitations
- Anonymous: 300 requests/day
- With API key: 10,000 requests/day

### Implementation Steps

#### Step 1: Create Settings Model
```csharp
// Models/PluginSettings.cs
public class PluginSettings
{
    public string? ApiKey { get; set; }
    public bool UseApiKey { get; set; }
    public bool ShowQuotaInResults { get; set; } = true;
}
```

#### Step 2: Create Settings Service
```csharp
// Services/SettingsService.cs
public class SettingsService
{
    private readonly string _settingsPath;
    
    public PluginSettings LoadSettings()
    {
        // Load from JSON file in AppData
    }
    
    public void SaveSettings(PluginSettings settings)
    {
        // Save to JSON file (encrypt API key)
    }
    
    public string EncryptApiKey(string apiKey)
    {
        // Use Windows DPAPI for encryption
        return Convert.ToBase64String(
            ProtectedData.Protect(
                Encoding.UTF8.GetBytes(apiKey),
                null,
                DataProtectionScope.CurrentUser
            )
        );
    }
}
```

#### Step 3: Update API Client
```csharp
// Services/StackOverflowApiClient.cs
public class StackOverflowApiClient
{
    private readonly string? _apiKey;
    
    public StackOverflowApiClient(string? apiKey = null)
    {
        _apiKey = apiKey;
    }
    
    public async Task<List<StackOverflowQuestion>> SearchAsync(
        string query, 
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(query);
        // ... rest of implementation
    }
    
    private string BuildUrl(string query)
    {
        var url = $"search/advanced?order=desc&sort=relevance&q={query}&site=stackoverflow&pagesize=5";
        
        if (!string.IsNullOrEmpty(_apiKey))
        {
            url += $"&key={_apiKey}";
        }
        
        return url;
    }
    
    // Parse quota from API response headers
    public (int remaining, int max) GetQuotaFromResponse(HttpResponseMessage response)
    {
        // API returns X-API-Quota-Remaining header
        if (response.Headers.TryGetValues("X-API-Quota-Remaining", out var values))
        {
            int.TryParse(values.FirstOrDefault(), out var remaining);
            return (remaining, _apiKey != null ? 10000 : 300);
        }
        return (0, 0);
    }
}
```

#### Step 4: Add Configuration UI
```csharp
// Main.cs - Add context menu option
public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
{
    return new List<ContextMenuResult>
    {
        // ... existing options
        new ContextMenuResult
        {
            Title = "Configure API Key...",
            Action = _ =>
            {
                // Open settings dialog or system settings
                ShowApiKeyConfigDialog();
                return true;
            }
        }
    };
}
```

### User Flow
1. User right-clicks any result
2. Selects "Configure API Key..."
3. Enters API key from https://stackapps.com/apps/oauth/register
4. Key is encrypted and stored
5. All future requests use the API key
6. User gets 10,000 requests/day instead of 300

### Getting an API Key
1. Visit https://stackapps.com/apps/oauth/register
2. Fill in application details:
   - **Application Name**: PowerToys Run StackOverflow Plugin
   - **OAuth Domain**: localhost (not used)
   - **Application Website**: Your GitHub URL
3. Click "Register Your Application"
4. Copy the **Key** (not the secret)
5. Paste into plugin settings

---

## 2. Search History Persistence

### Overview
Save search history across PowerToys restarts and provide quick access.

### Implementation Steps

#### Step 1: Create History Model
```csharp
// Models/SearchHistoryItem.cs
public class SearchHistoryItem
{
    public string Query { get; set; }
    public DateTime SearchedAt { get; set; }
    public int ResultCount { get; set; }
    public string Site { get; set; } = "stackoverflow";
}

// Models/SearchHistory.cs
public class SearchHistory
{
    public List<SearchHistoryItem> Items { get; set; } = new();
    public int MaxSize { get; set; } = 50;
}
```

#### Step 2: Create History Service
```csharp
// Services/SearchHistoryService.cs
public class SearchHistoryService
{
    private readonly string _historyFile;
    private SearchHistory _history;
    
    public SearchHistoryService()
    {
        var appData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData
        );
        _historyFile = Path.Combine(
            appData, 
            "PowerToys", 
            "StackOverflowPlugin", 
            "history.json"
        );
        
        LoadHistory();
    }
    
    public void AddSearch(string query, int resultCount)
    {
        // Remove duplicate if exists
        _history.Items.RemoveAll(x => 
            x.Query.Equals(query, StringComparison.OrdinalIgnoreCase)
        );
        
        // Add to top
        _history.Items.Insert(0, new SearchHistoryItem
        {
            Query = query,
            SearchedAt = DateTime.UtcNow,
            ResultCount = resultCount
        });
        
        // Limit size
        if (_history.Items.Count > _history.MaxSize)
        {
            _history.Items.RemoveRange(
                _history.MaxSize, 
                _history.Items.Count - _history.MaxSize
            );
        }
        
        SaveHistory();
    }
    
    public List<SearchHistoryItem> GetRecentSearches(int count = 10)
    {
        return _history.Items.Take(count).ToList();
    }
    
    public void ClearHistory()
    {
        _history.Items.Clear();
        SaveHistory();
    }
    
    private void LoadHistory()
    {
        if (File.Exists(_historyFile))
        {
            var json = File.ReadAllText(_historyFile);
            _history = JsonSerializer.Deserialize<SearchHistory>(json) 
                ?? new SearchHistory();
        }
        else
        {
            _history = new SearchHistory();
        }
    }
    
    private void SaveHistory()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_historyFile));
        var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_historyFile, json);
    }
}
```

#### Step 3: Integrate into Main.cs
```csharp
// Main.cs
public class Main : IPlugin
{
    private readonly SearchHistoryService _historyService;
    
    public Main()
    {
        _historyService = new SearchHistoryService();
        // ... other initialization
    }
    
    public List<Result> Query(Query query)
    {
        var search = query.Search?.Trim();
        
        // Show recent searches if query is empty
        if (string.IsNullOrWhiteSpace(search))
        {
            return ShowRecentSearches();
        }
        
        // ... perform search
        
        // After successful search, add to history
        _historyService.AddSearch(search, results.Count);
        
        return results;
    }
    
    private List<Result> ShowRecentSearches()
    {
        var recent = _historyService.GetRecentSearches(10);
        
        if (recent.Count == 0)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Search StackOverflow",
                    SubTitle = "Type your query to search"
                }
            };
        }
        
        var results = new List<Result>
        {
            new Result
            {
                Title = "📜 Recent Searches",
                SubTitle = "Select a previous search to run again"
            }
        };
        
        results.AddRange(recent.Select(item => new Result
        {
            Title = item.Query,
            SubTitle = $"🕒 {item.SearchedAt:g} • {item.ResultCount} results",
            IcoPath = IconPath,
            Action = _ =>
            {
                // Re-run the search
                Context.API.ChangeQuery($"so {item.Query}", true);
                return false;
            }
        }));
        
        return results;
    }
}
```

#### Step 4: Add Clear History Option
```csharp
// Add to context menu
new ContextMenuResult
{
    Title = "Clear Search History",
    Action = _ =>
    {
        _historyService.ClearHistory();
        return true;
    }
}
```

### User Experience
```
Type: so
Shows:
  📜 Recent Searches
  ─────────────────────────────
  python asyncio
    🕒 Today 2:30 PM • 5 results
  javascript promises
    🕒 Today 10:15 AM • 5 results
  c# linq query
    🕒 Yesterday • 4 results
```

---

## 3. Multi-Site Search (Other Stack Exchange Sites)

### Overview
Support searching across multiple Stack Exchange network sites.

### Popular Sites
- **stackoverflow** - Programming (default)
- **superuser** - General computing
- **serverfault** - System administration
- **askubuntu** - Ubuntu
- **unix** - Unix & Linux
- **math** - Mathematics
- **tex** - TeX/LaTeX
- **dba** - Database administrators

### Implementation Steps

#### Step 1: Define Site Configurations
```csharp
// Models/StackExchangeSite.cs
public class StackExchangeSite
{
    public string Keyword { get; set; }
    public string SiteName { get; set; }
    public string DisplayName { get; set; }
    public string IconPath { get; set; }
    public string Description { get; set; }
    
    public static List<StackExchangeSite> GetDefaultSites() => new()
    {
        new()
        {
            Keyword = "so",
            SiteName = "stackoverflow",
            DisplayName = "Stack Overflow",
            IconPath = "Images/stackoverflow.png",
            Description = "Programming Q&A"
        },
        new()
        {
            Keyword = "su",
            SiteName = "superuser",
            DisplayName = "Super User",
            IconPath = "Images/superuser.png",
            Description = "Computer enthusiasts"
        },
        new()
        {
            Keyword = "sf",
            SiteName = "serverfault",
            DisplayName = "Server Fault",
            IconPath = "Images/serverfault.png",
            Description = "System administrators"
        },
        new()
        {
            Keyword = "au",
            SiteName = "askubuntu",
            DisplayName = "Ask Ubuntu",
            IconPath = "Images/askubuntu.png",
            Description = "Ubuntu users"
        },
        // Add more sites...
    };
}
```

#### Step 2: Update plugin.json
```json
{
  "ID": "FFCA3E1DBB5247549B71A712AF2F03EC",
  "ActionKeywords": ["so", "su", "sf", "au"],
  "Name": "Stack Exchange Search",
  "Description": "Search Stack Overflow, Super User, and other Stack Exchange sites"
}
```

**Note**: PowerToys Run `plugin.json` doesn't support multiple action keywords directly. You'll need to:
- Either use a single keyword with site prefix: `so su: query`
- Or register separate plugins for each site
- Or use the recommended approach below

#### Step 3: Recommended Approach - Site Prefix
```csharp
// Main.cs
public List<Result> Query(Query query)
{
    var search = query.Search?.Trim();
    
    // Parse site prefix: "su: windows shortcuts"
    var (site, actualQuery) = ParseSiteAndQuery(search);
    
    // Search on specific site
    var results = await _apiClient.SearchAsync(actualQuery, site);
    
    // ... rest of implementation
}

private (string site, string query) ParseSiteAndQuery(string input)
{
    // Check for site prefix
    var match = Regex.Match(input, @"^(\w+):\s*(.+)$");
    
    if (match.Success)
    {
        var keyword = match.Groups[1].Value.ToLower();
        var query = match.Groups[2].Value;
        
        var site = StackExchangeSite.GetDefaultSites()
            .FirstOrDefault(s => s.Keyword == keyword);
            
        return site != null 
            ? (site.SiteName, query)
            : ("stackoverflow", input); // fallback
    }
    
    return ("stackoverflow", input); // default
}
```

#### Step 4: Update API Client
```csharp
// Services/StackOverflowApiClient.cs
public async Task<List<StackOverflowQuestion>> SearchAsync(
    string query, 
    string site = "stackoverflow",
    CancellationToken cancellationToken = default)
{
    var encodedQuery = HttpUtility.UrlEncode(query);
    var url = $"search/advanced?order=desc&sort=relevance&q={encodedQuery}&site={site}&pagesize=5";
    
    // ... rest of implementation
}
```

#### Step 5: Add Site Selector in Context Menu
```csharp
public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
{
    var sites = StackExchangeSite.GetDefaultSites();
    
    var siteMenus = sites.Select(site => new ContextMenuResult
    {
        Title = $"Search on {site.DisplayName}",
        SubTitle = site.Description,
        Action = _ =>
        {
            var query = ExtractQueryFromResult(selectedResult);
            Context.API.ChangeQuery($"so {site.Keyword}: {query}", true);
            return false;
        }
    }).ToList();
    
    return siteMenus;
}
```

### User Experience
```
Default (Stack Overflow):
  so python asyncio
  → Searches stackoverflow.com
  
Super User:
  so su: windows shortcuts
  → Searches superuser.com
  
Server Fault:
  so sf: nginx configuration
  → Searches serverfault.com
  
Quick switch via context menu:
  Right-click result → "Search on Super User"
```

### Site-Specific Icons
Download icons from each Stack Exchange site and add to Images/ folder:
- `Images/stackoverflow.png`
- `Images/superuser.png`
- `Images/serverfault.png`
- etc.

---

## Priority Implementation Order

1. **Search History** (Easiest, high user value)
   - Estimated effort: 4-6 hours
   - Low complexity
   - Immediate UX improvement

2. **Multi-Site Search** (Medium complexity)
   - Estimated effort: 8-12 hours
   - Requires UI/UX decisions
   - High user value for cross-site users

3. **Authentication Support** (Most complex)
   - Estimated effort: 12-16 hours
   - Requires secure storage
   - Lower priority (most users won't hit 300/day limit)

---

## Testing Checklist

### Authentication
- [ ] API key encryption works
- [ ] Decryption works on restart
- [ ] Quota increases to 10,000
- [ ] Invalid key shows error
- [ ] Can remove/update key

### Search History
- [ ] History persists across restarts
- [ ] Recent searches show up
- [ ] Clicking history re-runs search
- [ ] Clear history works
- [ ] Max 50 items enforced

### Multi-Site
- [ ] Site prefix parsing works
- [ ] Searches correct site
- [ ] Icons display correctly
- [ ] Context menu site switching works
- [ ] All configured sites work

---

## Files to Create

```
Models/
  PluginSettings.cs
  SearchHistoryItem.cs
  SearchHistory.cs
  StackExchangeSite.cs

Services/
  SettingsService.cs
  SearchHistoryService.cs
  (Update) StackOverflowApiClient.cs

Images/
  superuser.png
  serverfault.png
  askubuntu.png
  unix.png
  (etc.)
```

---

**Ready to implement?** Follow the steps above for each feature!

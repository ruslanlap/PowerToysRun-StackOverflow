using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Wox.Plugin;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;
using Community.PowerToys.Run.Plugin.StackOverflow.Services;
using Community.PowerToys.Run.Plugin.StackOverflow.Formatters;

namespace Community.PowerToys.Run.Plugin.StackOverflow
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IContextMenu, ISettingProvider, IReloadable, IDisposable
    {
        private const string ApiKeySettingName = "ApiKey";
        
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "FFCA3E1DBB5247549B71A712AF2F03EC";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "StackOverflow";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "Search StackOverflow questions directly from PowerToys Run";

        private PluginInitContext Context { get; set; }
        private string IconPath { get; set; }
        private bool Disposed { get; set; }

        private readonly ICacheService _cacheService;
        private IStackOverflowApiClient _apiClient;
        private readonly ResultFormatter _formatter;
        private CancellationTokenSource _searchCts;
        private string? _apiKey;

        public Main()
        {
            _cacheService = new CacheService();
            _apiClient = new StackOverflowApiClient(StackOverflowApiClient.CreateDefaultHttpClient(), null);
            _formatter = new ResultFormatter();
        }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            var search = query.Search?.Trim();

            // Show help message if query is empty
            if (string.IsNullOrWhiteSpace(search))
            {
                return
                [
                    new Result
                    {
                        QueryTextDisplay = string.Empty,
                        IcoPath = IconPath,
                        Title = "Search StackOverflow",
                        SubTitle = "Type your search query (minimum 2 characters)",
                        Action = _ => false,
                    }
                ];
            }

            // Validate query
            var searchQuery = SearchQuery.Create(search);
            if (!searchQuery.IsValid)
            {
                return
                [
                    new Result
                    {
                        QueryTextDisplay = search,
                        IcoPath = IconPath,
                        Title = "Invalid Query",
                        SubTitle = searchQuery.ValidationError,
                        Action = _ => false,
                    }
                ];
            }

            // Try to get cached results first
            var cachedResults = _cacheService.Get(searchQuery.NormalizedQuery);
            if (cachedResults != null)
            {
                return ConvertToResults(cachedResults);
            }

            // For non-cached queries, we need to perform a synchronous search
            // PowerToys Run doesn't support async Query() method
            try
            {
                // Cancel any previous ongoing search
                _searchCts?.Cancel();
                _searchCts = new CancellationTokenSource();
                
                // Perform synchronous search with timeout
                var searchTask = _apiClient.SearchAsync(searchQuery.NormalizedQuery, _searchCts.Token);
                searchTask.Wait(5000); // 5 second timeout
                
                if (searchTask.IsCompletedSuccessfully)
                {
                    var results = searchTask.Result;
                    _cacheService.Set(searchQuery.NormalizedQuery, results);
                    return ConvertToResults(results);
                }
                else
                {
                    return
                    [
                        new Result
                        {
                            QueryTextDisplay = search,
                            IcoPath = IconPath,
                            Title = "Search timeout",
                            SubTitle = "Please try again or check your internet connection",
                            Action = _ => false,
                        }
                    ];
                }
            }
            catch (Exception ex)
            {
                return
                [
                    new Result
                    {
                        QueryTextDisplay = search,
                        IcoPath = IconPath,
                        Title = "Search error",
                        SubTitle = $"Error: {ex.Message}",
                        Action = _ => false,
                    }
                ];
            }
        }

        private List<Result> ConvertToResults(List<StackOverflowQuestion> questions)
        {
            if (questions == null || questions.Count == 0)
            {
                return
                [
                    new Result
                    {
                        IcoPath = IconPath,
                        Title = "No results found",
                        SubTitle = "Try a different search query",
                        Action = _ => false,
                    }
                ];
            }

            return questions.Select(q => new Result
            {
                IcoPath = IconPath,
                Title = _formatter.FormatTitle(q),
                SubTitle = _formatter.FormatSubtitle(q),
                ToolTipData = new ToolTipData("StackOverflow Question", _formatter.FormatTooltip(q)),
                Action = _ =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = q.Link,
                            UseShellExecute = true
                        });
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ContextData = q,
            }).ToList();
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        /// <summary>
        /// Additional options for PowerToys Settings UI.
        /// </summary>
        public IEnumerable<PluginAdditionalOption> AdditionalOptions =>
        [
            new()
            {
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                Key = ApiKeySettingName,
                DisplayLabel = "Stack Exchange API Key (Optional)",
                DisplayDescription = "Increase rate limit from 300 to 10,000 requests/day. Get yours at: https://stackapps.com/apps/oauth/register",
                TextBoxMaxLength = 100,
            },
        ];

        /// <summary>
        /// Called when settings are updated in PowerToys Settings UI.
        /// </summary>
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            _apiKey = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == ApiKeySettingName)?.TextValue;
            
            // Recreate API client with new key
            _apiClient = new StackOverflowApiClient(
                StackOverflowApiClient.CreateDefaultHttpClient(),
                string.IsNullOrWhiteSpace(_apiKey) ? null : _apiKey
            );
        }

        /// <summary>
        /// Creates setting panel - not implemented (using AdditionalOptions instead).
        /// </summary>
        public Control CreateSettingPanel() => throw new NotImplementedException();

        /// <summary>
        /// Reload data when needed.
        /// </summary>
        public void ReloadData()
        {
            if (Context is null)
            {
                return;
            }

            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is StackOverflowQuestion question)
            {
                return
                [
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Open in browser (Enter)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE774", // Globe
                        AcceleratorKey = Key.Enter,
                        Action = _ =>
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = question.Link,
                                    UseShellExecute = true
                                });
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        },
                    },
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Copy link (Ctrl+C)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.C,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            Clipboard.SetDataObject(question.Link);
                            return true;
                        },
                    }
                ];
            }

            return [];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            _searchCts?.Cancel();
            _searchCts?.Dispose();

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/stackoverflow.light.png" : "Images/stackoverflow.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);
    }
}

#nullable enable
namespace Community.PowerToys.Run.Plugin.StackOverflow.Models
{
    /// <summary>
    /// Encapsulates user search input with validation and normalization.
    /// </summary>
    public class SearchQuery
    {
        public const int MinLength = 2;
        public const int MaxLength = 200;

        public string RawQuery { get; init; } = string.Empty;
        public string NormalizedQuery { get; init; } = string.Empty;
        public bool IsValid { get; init; }
        public string? ValidationError { get; init; }

        private SearchQuery() { }

        /// <summary>
        /// Creates a SearchQuery with validation and normalization.
        /// </summary>
        public static SearchQuery Create(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new SearchQuery
                {
                    RawQuery = query ?? string.Empty,
                    NormalizedQuery = string.Empty,
                    IsValid = false,
                    ValidationError = "Query cannot be empty"
                };
            }

            var trimmed = query.Trim();
            var normalized = trimmed.ToLowerInvariant();

            if (trimmed.Length < MinLength)
            {
                return new SearchQuery
                {
                    RawQuery = query,
                    NormalizedQuery = normalized,
                    IsValid = false,
                    ValidationError = $"Query must be at least {MinLength} characters"
                };
            }

            if (trimmed.Length > MaxLength)
            {
                return new SearchQuery
                {
                    RawQuery = query,
                    NormalizedQuery = normalized,
                    IsValid = false,
                    ValidationError = $"Query must not exceed {MaxLength} characters"
                };
            }

            return new SearchQuery
            {
                RawQuery = query,
                NormalizedQuery = normalized,
                IsValid = true,
                ValidationError = null
            };
        }
    }
}

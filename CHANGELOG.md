# Changelog

All notable changes to the PowerToys Run StackOverflow Plugin will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-01-27

### Added
- **Authentication support** - Optional API key for higher rate limits
  - Settings service with encrypted API key storage (Windows DPAPI)
  - Support for 10,000 requests/day (vs 300 anonymous)
  - "Configure API Key" option in context menu
  - Auto-opens settings folder with instructions
  - Secure encrypted storage of API keys
- **Improved error handling** for rate limits
  - Specific message for "TooManyRequests" (429) errors
  - Clear instructions on how to increase limits
  - Better error messages for common API issues
- API_KEY_INSTRUCTIONS.txt included in package
- Detailed API_KEY_SETUP.md documentation

### Changed
- StackOverflowApiClient now supports optional API key parameter
- Main.cs loads settings on startup
- Better error messages with context-specific guidance

### Technical
- New: Models/PluginSettings.cs
- New: Services/SettingsService.cs
- Updated: Services/StackOverflowApiClient.cs
- Updated: Main.cs

## [1.0.0] - 2025-01-27

### Added
- Initial release of PowerToys Run StackOverflow Plugin
- Search StackOverflow questions with `so <query>` command
- Display top 5 relevant results ordered by relevance
- Show question metadata: vote count, answer count, accepted answer indicator, tags
- Open questions in default browser with Enter key
- Copy question link to clipboard with Ctrl+C
- Smart caching system with 1-hour TTL and LRU eviction (50 entries max)
- Automatic debouncing (300ms) to reduce API calls
- Graceful error handling for network failures and rate limits
- Light and dark theme support with automatic switching
- Support for x64 and ARM64 architectures

### Features
- Sub-second response time for cached queries
- 3-second target for new API queries
- Memory-efficient caching (<100MB usage)
- 300 daily API requests (Stack Exchange anonymous quota)
- Comprehensive input validation (2-200 character queries)
- Context menu with copy link action

### Technical Details
- Built with .NET 9.0 targeting Windows 10.0.22621.0+
- Uses Stack Exchange API v2.3+
- Follows TDD development methodology
- Comprehensive unit test coverage
- Architecture: Models, Services, Formatters pattern
- Dependencies: Community.PowerToys.Run.Plugin.Dependencies 0.93.0

### Documentation
- Complete README with installation and usage instructions
- Formal constitution defining project principles
- Detailed specification documents in `.specify/` directory
- Task breakdown and implementation plan
- API integration documentation

## [Unreleased]

### Planned for v1.1.0
- **Authentication support** - Optional API key for higher rate limits (300 → 10,000 requests/day)
- **Search history persistence** - Save and quick-access to recent searches across sessions
- **Multi-site search** - Support for SuperUser, ServerFault, AskUbuntu, and 170+ other Stack Exchange sites

### Future Considerations
- Advanced search filters (tags, date ranges, sorting options)
- Inline preview of question/answer content
- Customizable result count
- Answer quality indicators
- Bookmarks/favorites system

---

**Full Changelog**: https://github.com/ruslanlap/PowerToysRun-StackOverflow/commits/v1.0.0

# Changelog

All notable changes to the PowerToys Run StackOverflow Plugin will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-10-03

### Added

#### Core Features

- **Instant StackOverflow Search** - Search millions of StackOverflow questions directly from PowerToys Run with `so <query>` command
- **Rich Metadata Display** - View vote counts, answer counts, accepted answer indicators (✓), and tags for each result
- **Smart Caching System** - LRU cache with 1-hour TTL and 50 entries max for sub-second repeat searches
- **One-Click Access** - Press Enter to open questions in default browser
- **Context Menu Actions** - Right-click to copy question links to clipboard

#### API & Performance

- **Stack Exchange API v2.3+ Integration** - Reliable access to StackOverflow data with gzip compression
- **Intelligent Debouncing** - 300ms delay to reduce unnecessary API calls while typing
- **Rate Limit Management** - Graceful handling of 300 requests/day anonymous quota
- **Optional API Key Support** - Increase limit to 10,000 requests/day with free API key
- **Encrypted API Key Storage** - Uses Windows DPAPI for secure local storage

#### User Experience

- **Theme Support** - Automatic light/dark mode switching with PowerToys theme
- **Input Validation** - Smart query validation (2-200 characters) with helpful error messages
- **Error Handling** - Clear, actionable error messages for network failures and rate limits
- **Multi-Architecture** - Support for x64 and ARM64 Windows platforms

#### Developer Experience

- **Clean Architecture** - Service-oriented design with Models/Services/Formatters separation
- **Comprehensive Testing** - Unit test coverage with TDD methodology
- **CI/CD Ready** - GitHub Actions workflow for automated builds and releases
- **Documentation** - Complete README, API key setup guide, quick start guide, and contributing guidelines

### Technical Details

- **Runtime**: .NET 9.0 targeting Windows 10.0.22621.0+
- **Dependencies**: Community.PowerToys.Run.Plugin.Dependencies 0.93.0
- **Plugin ID**: `FFCA3E1DBB5247549B71A712AF2F03EC`
- **Default Keyword**: `so` (customizable)
- **Memory Footprint**: <100MB
- **Cache Strategy**: LRU eviction with 1-hour TTL
- **API Response Time**: ~2-3 seconds for new queries, <1 second for cached queries

### Documentation

- **README.md** - Complete installation and usage guide
- **API_KEY_SETUP.md** - Detailed instructions for API key configuration via PowerToys Settings UI
- **QUICK_START.md** - 60-second getting started guide
- **CONTRIBUTING.md** - Guidelines for contributors
- **CHANGELOG.md** - Version history and release notes

### Security & Privacy

- ✅ 100% local data storage
- ✅ No tracking or telemetry
- ✅ API keys encrypted with Windows DPAPI
- ✅ Read-only access to public StackOverflow data
- ✅ Open source under MIT License

---

## [Unreleased]

### Planned for v1.1.0

- **Search History Persistence** - Save and quick-access recent searches across sessions
- **Multi-Site Search** - Support for SuperUser, ServerFault, AskUbuntu, and 170+ Stack Exchange sites
- **Enhanced Filtering** - Search by tags, date ranges, and vote thresholds

### Future Considerations

- Inline preview of question/answer content
- Customizable result count (currently fixed at 5)
- Answer quality indicators and sorting
- Bookmarks/favorites system with tags
- Advanced search operators (AND/OR/NOT)
- Export search results to markdown/JSON

---

## Links

- **Repository**: <https://github.com/ruslanlap/PowerToysRun-StackOverflow>
- **Releases**: <https://github.com/ruslanlap/PowerToysRun-StackOverflow/releases>
- **Issues**: <https://github.com/ruslanlap/PowerToysRun-StackOverflow/issues>
- **Full Changelog**: <https://github.com/ruslanlap/PowerToysRun-StackOverflow/commits/master>

---

**Legend:**

- 🎉 New feature
- 🔧 Enhancement
- 🐛 Bug fix
- 📚 Documentation
- 🔒 Security
- ⚡ Performance
- 🗑️ Deprecated

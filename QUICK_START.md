# Quick Start Guide: StackOverflow Plugin

## Installation (5 minutes)

### Step 1: Download
Choose your platform:
- **x64**: Download `StackOverflow-1.0.0-x64.zip` (6.4 MB)
- **ARM64**: Download `StackOverflow-1.0.0-arm64.zip` (6.4 MB)

### Step 2: Extract
Extract the ZIP file to:
```
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\StackOverflow\
```

Full path example:
```
C:\Users\YourName\AppData\Local\Microsoft\PowerToys\PowerToys Run\Plugins\StackOverflow\
```

### Step 3: Restart PowerToys
1. Right-click PowerToys icon in system tray
2. Select "Exit PowerToys"
3. Launch PowerToys again

## Usage

### Basic Search

1. **Open PowerToys Run**: Press `Alt+Space` (or your configured hotkey)

2. **Type search query**:
   ```
   so python lists
   ```

3. **View results**: Plugin displays top 5 StackOverflow questions

4. **Open result**: 
   - Press `Enter` to open in browser
   - Use arrow keys to navigate results

### Example Queries

```
so c# async await                    # C# async patterns
so javascript promises vs async      # JS comparison
so react hooks explained             # React fundamentals
so python decorator                  # Python decorators
so sql join types                    # SQL joins
so git merge vs rebase              # Git workflows
```

### Context Menu Actions

Right-click any result (or use keyboard shortcuts):
- **Open in browser** - `Enter`
- **Copy link** - `Ctrl+C`

## Features in Action

### Rich Metadata Display

Each result shows:
```
Title of the Question
↑1234 • 5 answers ✓ • python list comprehension
```

- `↑1234` = Vote score (upvotes minus downvotes)
- `5 answers` = Number of answers
- `✓` = Has accepted answer (green checkmark on SO)
- `python list comprehension` = Tags

### Performance

| Scenario | Speed |
|----------|-------|
| First search | 2-3 seconds (API call) |
| Repeat search | <1 second (cached) |
| Typing | 300ms delay before search |

### Caching

- **Cache size**: Up to 50 queries
- **Cache lifetime**: 1 hour
- **Cache strategy**: LRU (Least Recently Used) eviction
- Cached results are instant!

## Common Use Cases

### 1. Quick Error Lookup
```
so TypeError: 'NoneType' object is not subscriptable
```

### 2. API Documentation
```
so javascript fetch api
```

### 3. Best Practices
```
so react performance optimization
```

### 4. Code Examples
```
so python read csv file
```

### 5. Comparison Questions
```
so mongodb vs postgresql
```

## Troubleshooting

### Plugin Not Appearing?

1. Check installation path is correct
2. Verify PowerToys is running
3. Try restarting PowerToys
4. Check Windows version (requires 10.0.22621.0+)

### No Results?

- Ensure internet connection is active
- Try a more specific query
- Check if you've exceeded API rate limit (300/day)
- Wait a few seconds for API response

### Rate Limit Reached?

- **Limit**: 300 anonymous requests per day
- **Reset**: Midnight UTC daily
- **Solution**: Use cached results (instant) or wait for reset

### Search Too Slow?

- First search takes 2-3 seconds (normal)
- Subsequent identical searches are instant (cached)
- Use specific queries for better results

## Tips & Tricks

### Better Search Results

✅ **Good queries**:
```
so python list comprehension syntax
so react useState update object
so sql group by having clause
```

❌ **Avoid**:
```
so help              # Too vague
so a                 # Too short (min 2 chars)
so [very long text exceeding 200 characters...]  # Too long
```

### Keyboard Shortcuts

- `Alt+Space` - Open PowerToys Run
- Type `so` + `Space` - Activate plugin
- `Arrow keys` - Navigate results
- `Enter` - Open selected result
- `Ctrl+C` - Copy link (via context menu)
- `Esc` - Close PowerToys Run

### Cache Strategy

To maximize cache hits:
1. Use consistent query phrasing
2. Queries are case-insensitive
3. Leading/trailing spaces ignored
4. Cache persists until PowerToys restart

## Advanced Usage

### Query Validation

- **Minimum**: 2 characters
- **Maximum**: 200 characters
- **Allowed**: Letters, numbers, spaces, special chars

### Result Ranking

Results ordered by:
1. Relevance (Stack Exchange algorithm)
2. Vote score
3. Recent activity

### API Details

- **Endpoint**: Stack Exchange API v2.3+
- **Compression**: Automatic gzip
- **Timeout**: 10 seconds
- **Results**: Top 5 by relevance
- **Rate limit**: 300 requests/day (anonymous)

## Examples by Programming Language

### C#
```
so c# linq query syntax
so c# async await best practices
so c# dependency injection
```

### Python
```
so python list comprehension
so python virtual environment
so python asyncio tutorial
```

### JavaScript
```
so javascript promises
so javascript arrow functions
so javascript map vs forEach
```

### SQL
```
so sql inner join
so sql window functions
so sql query optimization
```

## Next Steps

1. ⭐ **Star the repo** if you find it useful
2. 🐛 **Report issues** on GitHub
3. 💡 **Suggest features** via GitHub Issues
4. 🤝 **Contribute** following the constitution

## Support

- **GitHub**: https://github.com/ruslanlap/PowerToysRun-StackOverflow
- **Issues**: Report bugs or request features
- **Documentation**: See README.md for full details

---

**Happy Searching!** 🚀

*Last updated: 2025-01-27*

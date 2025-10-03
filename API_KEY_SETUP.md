# API Key Setup (Optional)

## 🎯 Why Do You Need an API Key?

| Without Key | With Key |
|-------------|----------|
| 300 requests/day | **10,000 requests/day** |
| Anonymous access | Authenticated access |
| IP-based limits | Key-based limits |

**Recommended for power users!** If you search frequently, the free API key prevents rate limit errors.

---

## 📝 How to Get an API Key (5 minutes)

### Step 1: Register Your Application

Go to: **<https://stackapps.com/apps/oauth/register>**

### Step 2: Fill in the Form

```
Application Name: PowerToys Run StackOverflow Plugin
Description: Personal use for PowerToys Run
OAuth Domain: localhost
Application Website: (leave blank or add your GitHub profile)
```

### Step 3: Register

Click **"Register Your Application"**

### Step 4: Copy Your Key

You'll see three values:

- **Client Id**: (not needed)
- **Client Secret**: (not needed)
- **Key**: ⬅️ **COPY THIS VALUE!**

Example key format: `U4DMV*8nvpm3EOpvf69Rxw((`

---

## ⚙️ Configure the Plugin

### Method 1: PowerToys Settings (Recommended)

1. **Open PowerToys Settings**
   - Right-click PowerToys tray icon → Settings
   - OR press `Win + R` → type `powertoys` → Enter

2. **Navigate to Plugin Settings**
   - Click **PowerToys Run** in the left sidebar
   - Scroll down to **Plugins** section
   - Find **StackOverflow** in the list

3. **Enter Your API Key**
   - Look for the textbox labeled **"Stack Exchange API Key (Optional)"**
   - Paste your API key
   - The key is automatically saved

4. **Done!** No need to restart PowerToys

![PowerToys Settings Screenshot](https://via.placeholder.com/800x400?text=PowerToys+Settings+%3E+Run+%3E+Plugins+%3E+StackOverflow)

### Method 2: Via Context Menu

1. Open PowerToys Run (`Alt+Space`)
2. Type: `so test`
3. Right-click on any result
4. Select **"⚙️ Open Settings"** (if available)
5. This will open PowerToys Settings to the plugin page

---

## ✅ Verify It's Working

1. Open PowerToys Run (`Alt+Space`)
2. Type: `so python async`
3. Search should work without "TooManyRequests" errors
4. You now have **10,000 requests/day** limit!

---

## 🔧 Settings Location

PowerToys stores plugin settings at:

```
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Settings\Plugins\StackOverflow\
```

**Note**: You don't need to edit these files manually! Always use PowerToys Settings UI.

---

## 🔒 Security

- ✅ **Encrypted Storage**: Your API key is stored securely by PowerToys
- ✅ **Local Only**: Never transmitted to third parties
- ✅ **User-Specific**: Only accessible to your Windows user account
- ✅ **No Authentication**: Stack Exchange API keys are for identification, not user authentication

**Note**: Stack Exchange API keys are not secret tokens - they identify your app to increase rate limits.

---

## ❌ Remove Your API Key

To remove your API key:

1. Open PowerToys Settings
2. Go to PowerToys Run → Plugins → StackOverflow
3. Clear the **"Stack Exchange API Key"** textbox
4. Key is removed automatically

---

## 🆘 Troubleshooting

### Still Getting "TooManyRequests" Errors?

1. ✅ **Verify key is entered**: Check PowerToys Settings → Run → Plugins → StackOverflow
2. ✅ **Check key is correct**: No spaces at start/end, must be the **Key** field (not Client Secret)
3. ✅ **Try new search**: Previous searches might have used old settings
4. ✅ **Check your quota**: Visit <https://stackapps.com/apps/oauth> to see usage

### API Key Not Saving?

1. ✅ **Run as admin once**: Right-click PowerToys → Run as Administrator → Enter key → Restart normally
2. ✅ **Check permissions**: Ensure your user has write access to `%LOCALAPPDATA%`
3. ✅ **Antivirus**: Temporarily disable if blocking PowerToys settings writes

### How to Verify Key is Active?

1. Visit: <https://stackapps.com/apps/oauth>
2. You'll see your registered application
3. Check "Requests Today" counter
4. After using the plugin, this number should increase

### Key Not Working?

1. **Verify it's the Key field**: Not Client Id or Client Secret
2. **Check expiration**: Keys don't expire, but check if app is still active
3. **Create new app**: Delete old registration and create a fresh one
4. **Test the key**: Use Stack Exchange API tester to verify it works

---

## 📊 Monitor Your Usage

Track your API usage at:
**<https://stackapps.com/apps/oauth>**

You'll see:

- **Requests today**: Current daily usage
- **Quota remaining**: Requests left until reset
- **Reset time**: When your quota refreshes (midnight UTC)

---

## 💡 Best Practices

### Maximize Your Quota

✅ **DO:**

- Use specific, focused queries
- Let cache work (repeat searches are instant and free)
- Share your findings with others

❌ **DON'T:**

- Spam searches unnecessarily
- Share your API key publicly
- Automate massive queries

### Understanding Rate Limits

| Type | Limit | Resets |
|------|-------|--------|
| **Anonymous** | 300/day | Midnight UTC |
| **With Key** | 10,000/day | Midnight UTC |
| **Burst** | ~30/minute | Rolling window |

The plugin's cache (50 queries, 1-hour TTL) helps you stay well under these limits.

---

## 🎉 You're All Set

You can now make **10,000 requests per day** instead of 300!

**Need help?** [Open an issue on GitHub](https://github.com/ruslanlap/PowerToysRun-StackOverflow/issues)

---

## 🔗 Useful Links

- **Get API Key**: <https://stackapps.com/apps/oauth/register>
- **Manage Keys**: <https://stackapps.com/apps/oauth>
- **API Docs**: <https://api.stackexchange.com/docs>
- **Rate Limits**: <https://api.stackexchange.com/docs/throttle>

---

**Last Updated**: 2025-01-27  
**Plugin Version**: 1.1.0+

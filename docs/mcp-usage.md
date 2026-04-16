# MacroDeck MCP Server Usage Guide

The `MacroDeck.Mcp` project exposes MacroDeck's admin API as [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) tools, letting any MCP-capable LLM client (Claude Desktop, VS Code Copilot, etc.) manage profiles, buttons, variables, plugins, and device configuration through natural language.

## Prerequisites

- MacroDeck running with the admin REST API enabled (all versions that include this feature)
- `MacroDeck.Mcp.exe` built or downloaded
- Your **Admin API Key** — find it in MacroDeck → Settings → **API Access**

## Configuration

The MCP server reads two environment variables:

| Variable | Description | Example |
|---|---|---|
| `MACRODECK_URL` | Base URL of the running MacroDeck instance | `http://localhost:8191` |
| `MACRODECK_API_KEY` | Admin API key from Settings → API Access | `a1b2c3d4...` |

## Claude Desktop Integration

Add an entry under `mcpServers` in your Claude Desktop config file:

**macOS / Linux:** `~/Library/Application Support/Claude/claude_desktop_config.json`
**Windows:** `%APPDATA%\Claude\claude_desktop_config.json`

```json
{
  "mcpServers": {
    "macrodeck": {
      "command": "C:\\path\\to\\MacroDeck.Mcp.exe",
      "env": {
        "MACRODECK_URL": "http://localhost:8191",
        "MACRODECK_API_KEY": "your-api-key-here"
      }
    }
  }
}
```

After saving, restart Claude Desktop. You should see MacroDeck tools available in the tool picker.

## VS Code / GitHub Copilot Integration

Add to your `.vscode/mcp.json` (or user-level MCP settings):

```json
{
  "servers": {
    "macrodeck": {
      "type": "stdio",
      "command": "C:\\path\\to\\MacroDeck.Mcp.exe",
      "env": {
        "MACRODECK_URL": "http://localhost:8191",
        "MACRODECK_API_KEY": "your-api-key-here"
      }
    }
  }
}
```

## Available Tools

### Profile Management

| Tool | Description |
|---|---|
| `list_profiles` | List all profiles with metadata |
| `get_profile` | Get a profile including all its folders |
| `create_profile` | Create a new profile with specified rows/columns |
| `delete_profile` | Delete a profile by ID |
| `list_folders` | List folders inside a profile |
| `create_folder` | Create a folder (optionally nested under a parent) |
| `delete_folder` | Delete a folder and all its child folders |
| `list_buttons` | List buttons in a folder |
| `create_button` | Place a button at a grid position with action assignments |
| `update_button` | Update a button partially (patch-style) while preserving omitted fields |
| `delete_button` | Remove a button |

### Plugin Management

| Tool | Description |
|---|---|
| `list_plugins` | List all installed plugins with available actions |
| `get_plugin` | Get details and action list for a specific plugin |
| `search_extension_store` | Search the MacroDeck extension store |
| `install_plugin` | Install a plugin from the extension store |

### Variable Management

| Tool | Description |
|---|---|
| `list_variables` | List all variables |
| `get_variable` | Get a specific variable |
| `set_variable` | Create or update a variable (String, Integer, Float, Bool) |
| `delete_variable` | Delete a variable |

### Device Management

| Tool | Description |
|---|---|
| `list_devices` | List all known devices |
| `assign_profile_to_device` | Assign a profile to a specific device |
| `set_device_blocked` | Block or unblock a device |

### Configuration

| Tool | Description |
|---|---|
| `get_configuration` | Get current MacroDeck configuration |
| `update_configuration` | Update configuration fields (port, SSL, ADB, etc.) |

## Example Prompts

Once connected, you can prompt the LLM naturally:

- *"Create a new profile called 'Gaming' with 4 rows and 6 columns"*
- *"Show me all installed plugins and their available actions"*
- *"Add a button at position 0,0 in the root folder of the Gaming profile that runs the 'Launch Application' action from the System plugin"*
- *"Set a variable called 'game_mode' to true"*
- *"Search the extension store for OBS plugins and install the first result"*

## Notes For `update_button`

- The tool expects `updateJson` to be a JSON object (not an array or scalar).
- Only request fields are applied during update: `positionX`, `positionY`, `actions`, `actionsRelease`, `actionsLongPress`, `actionsLongPressRelease`, `labelOffText`, `labelOnText`, `stateBindingVariable`, `iconPack`, `iconName`, `iconNameOn`, `iconOff`, `iconOn`, `backgroundColorOff`, `backgroundColorOn`, `labelColorOff`, `labelColorOn`.
- Fields you omit are preserved from the current button state.

## Label Formatting (Cottle)

MacroDeck label templates use Cottle syntax. For built-in functions and formatting helpers, see:

- [Cottle built-in functions](https://cottle.readthedocs.io/en/stable/page/03-builtin.html)

Recommended patterns:

- Numeric formatting: `{round(speedtestdownload, 2)} {speedtestdownloadunit}`
- Conditional fallback: `{default(spotify_playing_title, 'No Track')}`
- Multi-line label text in JSON: `"labelOffText":"Download\n{round(speedtestdownload, 2)} Mbps"`

## Debugging MCP Process Exit

If your MCP client reports that the server transport closed unexpectedly:

- Verify `MACRODECK_URL` and `MACRODECK_API_KEY` are set correctly.
- Ensure MacroDeck is running and reachable from the configured URL.
- Rebuild and restart `MacroDeck.Mcp.exe` after code changes.
- Run the server manually and capture stderr output for diagnosis.

## Security

The Admin API key is a randomly generated secret stored in MacroDeck's `config.json`. Keep it private — anyone with the key and network access to the MacroDeck port can manage your installation. You can regenerate the key at any time from Settings → API Access.

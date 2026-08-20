# MacroDeck CLI Usage Guide

The `macrodeck` CLI lets you manage MacroDeck profiles, buttons, variables, plugins, and devices from a terminal or shell scripts — no GUI required.

## Prerequisites

- MacroDeck running with the admin REST API enabled
- `macrodeck.exe` (or `macrodeck` on Linux/macOS) built or on your `PATH`
- Your **Admin API Key** — find it in MacroDeck → Settings → **API Access**

## Configuration

Supply connection details via global options on every command, or set environment variables once:

```bash
# Environment variables (recommended)
export MACRODECK_URL=http://localhost:8191
export MACRODECK_API_KEY=your-api-key-here

# Or per-command options
macrodeck --url http://localhost:8191 --key your-api-key-here profile list
```

## Command Reference

### Global Options

| Option | Env var | Description |
|---|---|---|
| `--url <url>` | `MACRODECK_URL` | MacroDeck server URL (default: `http://localhost:8191`) |
| `--key <key>` | `MACRODECK_API_KEY` | Admin API key |

---

### `profile` — Profile management

```
macrodeck profile list
macrodeck profile get <profileId>
macrodeck profile create --name <name> [--rows <n>] [--columns <n>]
macrodeck profile delete <profileId>

macrodeck profile list-folders <profileId>
macrodeck profile create-folder <profileId> --name <name> [--parent <folderId>] [--app <processName>]
macrodeck profile delete-folder <profileId> <folderId>
```

**Examples:**

```bash
# List all profiles
macrodeck profile list

# Create a new profile
macrodeck profile create --name "Streaming" --rows 3 --columns 6

# List folders in a profile
macrodeck profile list-folders abc123

# Create a nested folder that auto-activates for OBS
macrodeck profile create-folder abc123 --name "OBS Controls" --app obs64
```

---

### `button` — Button management

```
macrodeck button list <profileId> <folderId>
macrodeck button create <profileId> <folderId> --x <col> --y <row> [--label <text>] [--label-on <text>] [--actions-json <json>]
macrodeck button delete <profileId> <folderId> <buttonGuid>
```

**Examples:**

```bash
# List all buttons in a folder
macrodeck button list abc123 root-folder-id

# Create a button at column 0, row 0 with a label
macrodeck button create abc123 root-folder-id --x 0 --y 0 --label "Mute"

# Create a button with an action assignment (JSON inline)
macrodeck button create abc123 root-folder-id --x 1 --y 0 --label "Scene 1" \
  --actions-json '[{"pluginName":"OBS Plugin","actionClass":"SwitchSceneAction","configuration":"{\"scene\":\"Gaming\"}"}]'
```

The `--actions-json` parameter takes an array of action assignment objects. `configuration` must be a JSON string:

```json
[
  {
    "pluginName": "Plugin Display Name",
    "actionClass": "ActionClassName",
    "configuration": "{\"key\":\"value\"}"
  }
]
```

Use `macrodeck plugin get <name>` to see the available action class names for a plugin.

---

### `plugin` — Plugin management

```
macrodeck plugin list
macrodeck plugin get <name>
macrodeck plugin search <query>
macrodeck plugin install <extensionId>
```

**Examples:**

```bash
# List all installed plugins
macrodeck plugin list

# Inspect a plugin's available actions
macrodeck plugin get "OBS Plugin"

# Search the extension store
macrodeck plugin search obs

# Install a plugin by extension store ID
macrodeck plugin install com.example.myplugin
```

---

### `variable` — Variable management

```
macrodeck variable list
macrodeck variable get <name>
macrodeck variable set <name> --value <value> [--type String|Integer|Float|Bool] [--creator <label>]
macrodeck variable delete <name>
```

**Examples:**

```bash
# List all variables
macrodeck variable list

# Create/update a boolean variable
macrodeck variable set is_streaming --value true --type Bool --creator CLI

# Create/update an integer counter
macrodeck variable set scene_index --value 0 --type Integer

# Delete a variable
macrodeck variable delete old_var
```

---

### `device` — Device management

```
macrodeck device list
macrodeck device assign-profile <clientId> --profile <profileId>
macrodeck device set-blocked <clientId> --blocked <true|false>
```

**Examples:**

```bash
# List all known devices
macrodeck device list

# Assign a specific profile to a device
macrodeck device assign-profile my-phone-client-id --profile streaming-profile-id

# Block a device from connecting
macrodeck device set-blocked unknown-device --blocked true
```

---

### `config` — Configuration management

```
macrodeck config get
macrodeck config update [--auto-start <true|false>] [--auto-updates <true|false>] [--update-beta-versions <true|false>] [--enable-adb-server <true|false>] [--enable-adb-auto-start-app <true|false>] [--ask-on-new-connections <true|false>] [--block-new-connections <true|false>] [--language <name>]
```

**Examples:**

```bash
# View current configuration
macrodeck config get

# Enable ADB support for Android USB connections
macrodeck config update --enable-adb-server true

# Auto-start Macro Deck Client app when connected via ADB
macrodeck config update --enable-adb-auto-start-app true

# Block new device connections
macrodeck config update --block-new-connections true

# Change language
macrodeck config update --language German
```

---

## Shell Scripting Examples

### Create a Gaming profile with a full button layout

```bash
#!/bin/bash
export MACRODECK_URL=http://localhost:8191
export MACRODECK_API_KEY=your-key-here

# Create profile and capture the returned ID
PROFILE=$(macrodeck profile create --name "Gaming" --rows 3 --columns 5)
echo "Created: $PROFILE"

# Get the profile ID from the JSON output using jq
PROFILE_ID=$(echo "$PROFILE" | jq -r '.profileId')

# List folders to find the root folder
macrodeck profile list-folders "$PROFILE_ID"
```

### Scripted variable setup

```bash
# Initialize a set of streaming state variables
macrodeck variable set is_streaming --value false --type Bool --creator Setup
macrodeck variable set stream_uptime --value 0 --type Integer --creator Setup
macrodeck variable set viewer_count --value 0 --type Integer --creator Setup
echo "Variables initialized."
```

## Security

The Admin API key is stored in MacroDeck's `config.json`. Treat it like a password — do not commit it to source control. Use environment variables instead of --key flags in scripts to avoid exposing the key in process listings. You can regenerate the key at any time from Settings → API Access; after regeneration, update all your scripts and MCP server configurations.

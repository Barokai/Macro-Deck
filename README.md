# Macro Deck
![VersionBadge](https://img.shields.io/github/v/release/Macro-Deck-org/Macro-Deck)
![LicenseBadge](https://img.shields.io/github/license/Macro-Deck-org/Macro-Deck)
![PlatformBadge](https://img.shields.io/badge/platform-windows-blue)
![ExtensionStoreBadge](https://img.shields.io/website?down_message=offline&label=Extension%20Store&up_message=online&url=https%3A%2F%2Fmacrodeck.org%2Fextensionstore%2Fextensionstore.php)

## More than just a macro pad!

- Open source
- Plugins
- Icon packs
- [Web client](http://web.macrodeck.org)
- Built in package manager (to download plugins and icon packs)
- [Variables](https://github.com/SuchByte/Macro-Deck/wiki/Variables)
- [Cottle templates](https://cottle.readthedocs.io/en/stable/page/03-builtin.html)
- Multiple profiles
- Unlimited folders
- [Discord community](https://discord.gg/yr7TRaXum8)


# Developing Plugins

The Macro Deck API is published to NuGet as
[`SuchByte.MacroDeck`](https://www.nuget.org/packages/SuchByte.MacroDeck). Add it
to your plugin project instead of referencing `Macro Deck 2.dll` by file:

```
dotnet add package SuchByte.MacroDeck
```

This is a compile-time reference package only — the assembly is provided by the
Macro Deck host at runtime, so it is not copied to your plugin's output.

# Companion App
[Repository](https://github.com/Macro-Deck-App/Macro-Deck-Client-App)

[Google Play](https://play.google.com/store/apps/details?id=com.suchbyte.macrodeck)

[App Store](https://apps.apple.com/de/app/macro-deck-client/id6475241728)

# Download
[Website](https://macro-deck.app)

# CLI & MCP Server

Macro Deck ships a command-line tool and an MCP (Model Context Protocol) server that let you manage profiles, buttons, variables, and plugins programmatically or through natural language via LLM clients (Claude Desktop, VS Code Copilot, etc.).

- **[CLI usage guide](docs/cli-usage.md)** — manage MacroDeck from a terminal or shell scripts
- **[MCP server usage guide](docs/mcp-usage.md)** — connect any MCP-capable LLM client to MacroDeck

The Admin API key required to connect is displayed in MacroDeck → Settings → **API Access**.

# Create own plugins
Learn how to create your own plugins in the wiki: https://github.com/SuchByte/Macro-Deck/wiki/Plugin-API

# Helping with the translation
Please help me with the translation of Macro Deck on [POEditor](https://poeditor.com/join/project/3y5UBkJvQD)
[GitHub Releases](https://github.com/Macro-Deck-App/Macro-Deck/releases)

# Special thanks

Licenses for JetBrains products are kindly provided by [JetBrains](https://www.jetbrains.com/)

Macro Deck also uses free icons from [Material Design Icons](https://materialdesignicons.com/)

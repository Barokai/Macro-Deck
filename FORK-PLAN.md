# Fork plan (Barokai/Macro-Deck)

_Written 2026-11-27. This file only exists in the fork._

## Situation

- Both contributions to upstream were closed without a response:
  - [Macro-Deck-App/Macro-Deck#629](https://github.com/Macro-Deck-App/Macro-Deck/pull/629): label rendering for
    Unicode symbols/emojis (closed 2026-08-15)
  - [Macro-Deck-App/Macro-Deck#659](https://github.com/Macro-Deck-App/Macro-Deck/pull/659): admin REST API, MCP
    server and CLI (closed 2026-08-20)
- Since then upstream was **rewritten as Macro Deck 3.x** (v3.0.5 stable, v3.1.0-beta.3 as of 2026-11-21):
  - new layout (`host/`, `sdk/`, `protocol/`, `ui/`, `docs/`, `e2e/`), Rust + TypeScript + C#
  - very active, commits daily
  - **its own built-in MCP server** (`docs/src/content/docs/reference/mcp.md`, `host/src/MacroDeckHost/Mcp/`,
    engineering decision 0113 "the MCP server is an untrusted principal")
- This fork's `main` shares **no git history** with upstream `main` any more, so it can't be synced or rebased.
- Fork PRs: Barokai/Macro-Deck#3 (CLI/MCP admin API) is closed with its branch kept.
  Barokai/Macro-Deck#2 (label rendering) stays open until the fix is retested.
- `C:\dev\macro-deck-tools\` (`macrodeck_db_tools.py` + configuration guide) is not versioned and targets the
  old (2.x) database.

## Plan

1. **Try Macro Deck 3.x** with its built-in MCP server. Check whether it covers what the admin API/MCP/CLI branch
   was for (reading and changing profiles, folders, buttons, variables; plugin and device management).
2. **Retest the label rendering** (symbols/emojis on buttons) in 3.x:
   - fixed upstream → close Barokai/Macro-Deck#2
   - still broken → open an issue upstream first (they didn't respond to PRs; an issue with screenshots is cheaper
     for them to act on), and only then port the fix to a fresh fork of 3.x
3. **Decide the fork's future:**
   - 3.x covers everything → archive this fork; keep the branches `feature/cli-mcp-admin-api` and
     `feature/labelgenerator-unicode-symbol-rendering` for reference (archiving keeps them)
   - something is missing → delete and re-fork from upstream 3.x, so the history matches again, and rebuild only
     the missing parts as small, separately reviewable changes (upstream has `CONTRIBUTING.md`, `CLAUDE.md` and
     engineering decisions; follow them)
4. **macro-deck-tools:** if still needed with 3.x, put it into a repo (here or `local-pc`) and adapt it to the
   3.x data model; otherwise delete it.

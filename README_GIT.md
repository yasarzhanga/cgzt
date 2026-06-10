# Git management scope

This repository is configured for code-focused Unity version control.

Tracked by default:
- C# scripts and matching Unity `.meta` files.
- Shader/source-like files.
- `Packages/manifest.json` and `Packages/packages-lock.json`.
- `ProjectSettings/*.asset` and `ProjectSettings/*.json`.

Ignored by default:
- Unity generated folders such as `Library`, `Temp`, `Logs`, `Builds`, and `UserSettings`.
- Student answer/runtime data under `Assets/StreamingAssets` and `Assets/save`.
- Archives, model files, DLLs, images, fonts, and other large binary assets.

If a resource asset must be versioned later, add a narrow exception in `.gitignore` for that exact path instead of tracking all assets at once.

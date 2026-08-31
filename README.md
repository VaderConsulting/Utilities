# Utilities

A comprehensive Windows Forms utility class library available in three .NET editions (.NET 4.8, .NET 5, .NET 8). Provides extension methods, custom controls, database helpers, serial communication, data transformation, and more.

**Source last updated:** 2022-05-09

**Initiated:** 2019-01-17 · **Solution:** `Utilities.sln`

---

## Projects

| Project | Target | Notes |
|---------|--------|-------|
| `Utilities_48` | .NET Framework 4.8 | Primary source of truth |
| `Utilities_5` | .NET 5 (Windows) | Links to `Utilities_48` source files |
| `Utilities_6` | .NET 8 Windows | Own source; adds scoreboard protocol interfaces |

---

## What Is Included

### Extension Methods

| Category | Highlights |
|----------|-----------|
| Generic | `DeepCopy<T>()`, `Save<T>()`, `SaveToFile<T>()`, `LoadFromFile<T>()` |
| String | Type conversion (`As<T>`), null-safe length, regex helpers |
| Numeric | Formatting, range clamping, byte-size formatting (KB/MB/GB/TB) |
| Collections | LINQ helpers, shuffling, batch processing |
| Drawing | Bitmap manipulation, colour conversion |
| WinForms | `WM_SETREDRAW` suspend/resume for flicker-free batch updates |

### Custom Controls

`CueTextBox`, `DataGridViewEx`, `DropDownList`, `Marquee`, `TransparentTableLayoutPanel`

### Data Access

SQL Server, SQLite, and MySQL helpers (`Data.cs`).

### Communication

`Serial : IDisposable` - serial port wrapper with event-driven data reception.

### Other Utilities

`Transform` (enum-driven data transformation), `Registry` (Windows Registry helper), `TextDrawing` (render text to Bitmap), `Ascii85` (Base85 encoding), `TriggeredQueue` (event-firing thread-safe queue)

---

## .NET 8 Additions (Utilities_6 only)

`IScoreboardProtocol`, `IProtocolFactory`, `ScoreboardData` - scoreboard display protocol interfaces and data model.

---

> The Serial.cs read pattern is adapted from a [sparxeng.com blog post](https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport). See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

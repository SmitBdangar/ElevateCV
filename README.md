# Pixellum

A digital painting application built with Avalonia UI and .NET 9.

## Download

Get the latest release for Windows:
- [Download v1.0.0 (Windows x64)](https://github.com/SmitBdangar/Pixellum/releases/tag/v1.0.0)

The release includes a self-contained, single executable - no installation required!

## Features

- **Drawing Tools**
  - Brush with adjustable size, opacity, hardness, and flow
  - Eraser tool
  - Eyedropper (color picker)
  - Fill bucket

- **Layer Management**
  - Multiple layers support
  - Layer visibility toggle
  - Layer opacity control
  - Rename, duplicate, and merge layers
  - Layer thumbnails

- **Color System**
  - Interactive color wheel
  - Quick color palette (12 preset colors)
  - Hex color input
  - Primary/Secondary color swatches with swap

- **Canvas Operations**
  - 800x600 default canvas size
  - Clear and fill canvas options
  - Undo/Redo support (Ctrl+Z / Ctrl+Y)
  - PNG export

## Requirements

### Running the App
- Windows x64 (for pre-built release)
- No additional dependencies needed

### Building from Source
- .NET 9.0 SDK
- Windows, macOS, or Linux

## Building from Source

```bash
dotnet build
```

## Running from Source

```bash
dotnet run
```

## Project Structure

```
Pixellum/
├── Core/               # Core logic (Document, Layer, Commands)
├── Rendering/          # Rendering engine and brush system
├── Views/              # UI views (Canvas, Tools, Layers panels)
├── Controls/           # Custom controls (ColorWheel, ColorSVPad)
└── MainWindow.axaml    # Main application window
```

## Controls

- **Left Panel**: Tools and brush settings
- **Center Panel**: Drawing canvas
- **Right Panel**: Layer management
- **Top Menu**: File and Edit operations

### Keyboard Shortcuts

- `Ctrl+Z` - Undo
- `Ctrl+Y` - Redo

## License

This project is open source.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

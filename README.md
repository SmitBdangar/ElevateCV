# Pixellum

A lightweight, cross-platform digital painting application built with Avalonia UI and .NET 9.

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey.svg)](#)

## Features

- **Drawing Tools** - Brush, eraser, eyedropper, and fill bucket with customizable settings
- **Layer Management** - Multiple layers with visibility, opacity, rename, duplicate, and merge
- **Color System** - Interactive color wheel, quick palette, and hex color input
- **Canvas Operations** - 800×600 canvas with undo/redo support and PNG export
- **Cross-Platform** - Runs on Windows, macOS, and Linux

## Download

Get the latest release for Windows:
- [Download v1.0.0 (Windows x64)](https://github.com/SmitBdangar/Pixellum/releases/tag/v1.0.0)

Single executable - no installation required!

## Quick Start

### Running from Release
1. Download the release for your platform
2. Extract the archive
3. Run the executable
4. Start painting!

### Building from Source

```bash
# Clone the repository
git clone https://github.com/SmitBdangar/Pixellum.git
cd Pixellum

# Build and run
dotnet build
dotnet run
```

**Requirements:**
- .NET 9.0 SDK
- 2GB RAM minimum
- 1280×720 display or higher

## Usage

### Interface Layout
- **Left Panel** - Tools and brush settings
- **Center** - Drawing canvas
- **Right Panel** - Layer management
- **Top Menu** - File and edit operations

### Keyboard Shortcuts
| Action | Shortcut |
|--------|----------|
| Undo | `Ctrl+Z` |
| Redo | `Ctrl+Y` |

### Getting Started
1. Select a tool from the left panel (brush is default)
2. Choose a color using the color wheel or quick palette
3. Adjust brush size and opacity as needed
4. Draw on the canvas
5. Manage layers in the right panel
6. Export via File → Export PNG

## Project Structure

```
Pixellum/
├── Core/               # Document, layers, and undo/redo logic
├── Rendering/          # Rendering engine and brush system
├── Views/              # Canvas, tools, and layers UI
├── Controls/           # Custom controls (color wheel, palette)
└── MainWindow.axaml    # Main application window
```

## Technology Stack

- **Framework:** Avalonia UI 11.3.8
- **Runtime:** .NET 9.0
- **Architecture:** MVVM with command pattern
- **Graphics:** WriteableBitmap with direct memory access

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/NewFeature`)
3. Commit your changes (`git commit -m 'Add NewFeature'`)
4. Push to the branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

## Roadmap

### Planned Features
- Zoom and pan controls
- Selection tools
- Additional blend modes
- Brush presets
- Gradient and text tools
- Tablet pressure sensitivity

## License

This project is open source and available under the [MIT License](LICENSE).

## Acknowledgments

Built with [Avalonia UI](https://avaloniaui.net/) - A cross-platform .NET UI framework

## Contact

- **Issues:** [GitHub Issues](https://github.com/SmitBdangar/Pixellum/issues)
- **Author:** Smit Bdangar

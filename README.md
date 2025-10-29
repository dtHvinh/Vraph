# Vraph - Hamilton Visualizer

A powerful Windows desktop application for visualizing and analyzing graph structures with support for various graph algorithms including Hamilton Cycle detection.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)

## 📋 Overview

Vraph (Hamilton Visualizer) is an interactive graph visualization tool built with WPF (Windows Presentation Foundation) that allows users to create, manipulate, and analyze both directed and undirected graphs. The application provides an intuitive canvas-based interface for drawing graphs and implements several fundamental graph algorithms.

## ✨ Features

### Graph Operations
- **Interactive Graph Drawing**: Create vertices and edges using an intuitive drag-and-drop interface
- **Dual Graph Modes**: Support for both directed and undirected graphs with easy switching
- **Visual Customization**: Color-coded vertices and edges for better visualization
- **Graph Manipulation**: Add, remove, and modify vertices and edges dynamically

### Algorithm Implementations
- **Breadth-First Search (BFS)**: Layer-by-layer graph traversal visualization
- **Depth-First Search (DFS)**: Stack-based graph traversal
- **Hamilton Cycle Detection**: Find Hamiltonian cycles in graphs
- **Strongly Connected Components (SCC)**: Identify SCCs using Kosaraju's algorithm
- **Graph Components**: Analyze connected components in graphs

### User Interface
- **Clean Modern UI**: Minimalist design with floating window controls
- **Real-time Statistics**: Display vertex count, edge count, and selected nodes
- **Animation Controls**: Toggle transition effects for algorithm visualization
- **Vietnamese Language Support**: Full UI localization in Vietnamese

## 🚀 Getting Started

### 📦 For Non-Technical Users (Recommended)

**Don't have coding experience?** No problem! You can download the pre-built application and start using it right away:

1. Visit the [Releases page](https://github.com/dtHvinh/Vraph/releases)
2. Download the latest version (e.g., `Vraph-v1.0.0.zip`)
3. Extract the zip file to a folder on your computer
4. Run the executable file to launch the application

No installation or coding required! 🎉

### 👨‍💻 For Developers

#### Prerequisites
- Windows 10/11
- .NET 8.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code with C# extension

#### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/dtHvinh/Vraph.git
   cd Vraph
   ```

2. **Build the solution**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run --project src/HamiltonVisualizer/HamiltonVisualizer.csproj
   ```

   Or open `HamiltonVisualizerWPF.sln` in Visual Studio and press F5.

## 📖 Usage

### Creating a Graph
1. Launch the application
2. Click on the canvas to create vertices
3. Click and drag between vertices to create edges
4. Right-click on elements to access additional options

### Running Algorithms
1. Select vertices by clicking on them
2. Use the control panel on the right to choose graph operations
3. Toggle between directed and undirected graph modes as needed
4. Use the animation controls to visualize algorithm execution

### Control Panel Features
- **Xóa tất cả** (Delete All): Remove all vertices and edges
- **Đặt lại** (Reset): Reset vertex colors to default
- **Tắt/Bật hiệu ứng** (Toggle Effects): Enable/disable transition animations
- **Đồ thị có hướng/vô hướng** (Graph Mode): Switch between directed and undirected graphs

## 🏗️ Project Structure

```
Vraph/
├── src/
│   ├── HamiltonVisualizer/              # Main WPF application
│   │   ├── Commands/                    # Command pattern implementations
│   │   ├── Constants/                   # Application constants
│   │   ├── Core/                        # Core functionality and custom controls
│   │   ├── Events/                      # Event handlers and event args
│   │   ├── Options/                     # Configuration options
│   │   ├── Resources/                   # Icons and assets
│   │   ├── Themes/                      # WPF styling and themes
│   │   ├── Utilities/                   # Helper classes and utilities
│   │   ├── ViewModels/                  # MVVM view models
│   │   ├── MainWindow.xaml              # Main application window
│   │   └── App.xaml                     # Application entry point
│   │
│   ├── HamiltonVisualizer.DataStructure/   # Graph data structures
│   │   ├── Base/                        # Base graph classes
│   │   ├── Components/                  # Graph components and algorithms
│   │   └── Implements/                  # Directed and undirected graph implementations
│   │
│   └── HamiltonVisualizer.Mathematic/   # Mathematical utilities
│       └── TwoDimentional.cs            # 2D geometric calculations
│
├── tests/
│   └── GraphTest/                       # Test projects
│       └── Program.cs                   # Algorithm testing
│
└── HamiltonVisualizerWPF.sln           # Visual Studio solution file
```

## 🔧 Technology Stack

- **Framework**: .NET 8.0
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Language**: C# 12
- **Architecture**: MVVM (Model-View-ViewModel)
- **Platform**: Windows (x64, x86, ARM64)

## 🧪 Running Tests

```bash
dotnet test
```

Or run the GraphTest console application:
```bash
dotnet run --project tests/GraphTest/GraphTest.csproj
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Algorithm Details

### Hamilton Cycle (HC)
The application implements a backtracking algorithm to find Hamiltonian cycles in graphs. A Hamiltonian cycle is a cycle that visits each vertex exactly once and returns to the starting vertex.

### Strongly Connected Components (SCC)
Uses Kosaraju's algorithm for directed graphs:
1. Perform DFS on the original graph and push vertices to a stack
2. Transpose the graph
3. Perform DFS on the transposed graph in the order defined by the stack

### BFS Layered
Provides layer-by-layer traversal useful for visualizing graph structure and finding shortest paths in unweighted graphs.

## 🐛 Known Issues

Please check the [Issues](https://github.com/dtHvinh/Vraph/issues) page for known bugs and feature requests.

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Authors

- **dtHvinh** - [GitHub Profile](https://github.com/dtHvinh)

## 🙏 Acknowledgments

- Thanks to all contributors who have helped shape this project
- Inspired by various graph theory visualization tools
- Built with love for the graph theory and algorithms community

## 📧 Contact

For questions, suggestions, or feedback, please open an issue on GitHub or reach out to the maintainers.

---

**Note**: This application is designed for educational purposes and graph theory learning. It provides an interactive way to understand graph algorithms and their behavior.
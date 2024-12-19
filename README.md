# SQLite Viewer

A sophisticated Windows desktop application built with WPF and .NET 8.0 for viewing, analyzing, and managing game replay data stored in SQLite databases. The application provides a modern, Material Design-themed interface for exploring player statistics, match histories, and game events.

## 🎮 Overview

SQLite Viewer is designed specifically for analyzing game replay data with features focused on:
- Tracking player performance and statistics
- Analyzing match histories and replay data
- Monitoring player interactions and game events
- Visualizing game data with advanced filtering capabilities

The application specializes in:
- Real-time processing of game replay data
- Advanced statistical analysis of player performance
- Complex data visualization with multiple overlay systems
- High-performance data filtering and sorting
- Memory-optimized handling of large datasets
- Seamless integration with SQLite databases

## 🚀 Key Features

### Advanced Data Grid System
- **Dynamic Column Management**
  - Auto-generating columns based on data models
  - Custom column templates for specialized data types
  - Sortable columns with maintained state
  - Column visibility toggling

- **Intelligent Pagination**
  - Server-side pagination reducing memory footprint
  - Configurable page sizes
  - Optimized data loading
  - Maintained scroll state

### Player Analysis System
- **Comprehensive Statistics Tracking**
  - Kill/death ratios
  - Match placement tracking
  - Team performance metrics
  - Platform-specific analytics
  - Bot interaction tracking

### Visual Overlay System
- **Dynamic Image Processing**
  - Real-time image compositing
  - Multiple overlay layers
  - Status indicators
  - Team affiliation markers
  - Performance badges

### Match Analysis Tools
- **Replay Data Processing**
  - Timestamp analysis
  - Player interaction tracking
  - Event sequencing
  - Action mapping

## 💻 Technical Stack

### Frontend
- WPF (Windows Presentation Foundation)
- XAML for UI design
- Material Design for WPF
- Custom control templates and styles

### Backend
- .NET 8.0
- SQLite database
- Entity Framework Core
- SQLitePCL.raw

### UI Components
- MaterialDesignThemes.Wpf (v5.1.0)
- MaterialDesignExtensions (v3.3.0)
- Custom XAML ResourceDictionaries
- Custom WPF Controls

### Database
- Microsoft.Data.Sqlite.Core (v8.0.8)
- System.Data.SQLite (v1.0.118)
- Custom database managers

## 🛠️ Technical Implementation

### Database Architecture
The application implements a sophisticated database management system with several key features:
- Optimized query execution using connection pooling to minimize database connections
- Enhanced pagination system that supports dynamic filtering and efficient data retrieval
- Memory-efficient data loading through asynchronous operations
- Robust transaction management for data integrity
- Query optimization techniques for large datasets
- Intelligent caching system for frequently accessed data

### Image Processing System
The application includes a comprehensive image processing system that handles:
- Dynamic overlay composition for player avatars and status indicators
- Multi-layer image processing with support for various status effects
- Real-time image manipulation and caching
- Memory-optimized image handling
- Support for multiple image formats and resolutions
- Efficient resource management for image assets

### UI Architecture
The user interface is built with several sophisticated components:
- Material Design-based styling system
- Custom control templates for specialized data display
- Advanced data grid implementation with virtual scrolling
- Dynamic theme management
- Responsive layout system
- Smooth animation and transition effects

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or later
- SQLite database engine

### Installation Steps
1. Clone the repository:
```bash
git clone https://github.com/yourusername/SQLiteViewer.git
```

2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build the project
5. Ensure the Database.db file is in the output directory

## 📝 Usage Examples

### Data Analysis Workflows
Users can analyze game data through various workflows:
- Filtering match histories by date ranges, players, or game types
- Analyzing player performance across multiple matches
- Tracking team composition and effectiveness
- Monitoring player progression and statistics
- Generating performance reports and insights
- Comparing player statistics across different time periods

### Player Tracking
The system enables comprehensive player tracking:
- Following individual player performance metrics
- Analyzing team dynamics and player interactions
- Tracking player achievements and progression
- Monitoring player behavior patterns
- Identifying player preferences and strategies
- Evaluating player improvement over time

### Match Analysis
Users can perform detailed match analysis:
- Reviewing match replays and events
- Analyzing team compositions and strategies
- Evaluating player performance metrics
- Tracking game objectives and outcomes
- Identifying key moments and turning points
- Assessing tactical decisions and their impact

## 🔐 Security

- Database access control
- Input validation
- Data sanitization
- Error handling

## 🎯 Future Enhancements

- Real-time data synchronization
- Advanced statistical analysis
- Export functionality
- Custom reporting
- Additional visualization options

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

For any questions or suggestions, please open an issue in the repository.

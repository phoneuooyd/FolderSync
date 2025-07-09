# FolderSync

FolderSync is a folder synchronization application developed as a test task for Veeam Software. The application maintains an exact replica of a source folder, ensuring that the replica folder stays synchronized with the source folder at regular intervals.

## Project Overview

FolderSync is a console application built with .NET 8 that continuously monitors and synchronizes two directories:
- **Source directory**: The original directory containing files to be synchronized
- **Replica directory**: The target directory that will be kept identical to the source

## Features

### Core Functionality
- **One-way synchronization**: Source directory -> Replica directory
- **Periodic synchronization**: Runs at user-defined intervals
- **Automatic directory creation**: Creates missing directories automatically
- **File content comparison**: Uses MD5 hash comparison for file integrity
- **File metadata comparison**: Compares file size and last write time
- **Comprehensive logging**: Dual logging to both console and file

### Enhanced Protection System
- **Directory protection**: Prevents accidental deletion of main source and replica directories
- **Automatic recovery**: Recreates missing directories if they are accidentally deleted
- **Health monitoring**: Performs periodic health checks on directories
- **Suspicious activity detection**: Monitors for unusual file system changes
- **Configuration validation**: Prevents using the same path for source and replica

### Synchronization Operations
- **Copy files**: New and modified files from source to replica
- **Delete files**: Removes files from replica that no longer exist in source
- **Directory management**: Creates/removes directories as needed
- **Recursive synchronization**: Handles nested directory structures

### Logging System
- **Console logging**: Real-time output to console
- **File logging**: Persistent log file with timestamps
- **Combined logging**: Simultaneous logging to both destinations
- **Operation tracking**: Logs all copy, delete, and directory operations
- **Protection logging**: Logs directory protection and recovery events

## Prerequisites

- .NET 8.0 Runtime or SDK
- Windows, macOS, or Linux operating system

## Installation

1. Clone the repository:git clone <repository-url>
cd FolderSync
2. Build the application:dotnet build
## Usage

### Basic Commanddotnet run -- "<source-path>" "<replica-path>" <interval-seconds> "<log-file-path>"

### Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `source-path` | Path to the source directory | `C:\Source` or `/home/user/source` |
| `replica-path` | Path to the replica directory | `C:\Replica` or `/home/user/replica` |
| `interval-seconds` | Synchronization interval in seconds | `30` |
| `log-file-path` | Path to the log file | `C:\Logs\sync.log` or `/var/log/sync.log` |

### Example Commands

**Windows:**

dotnet run -- "C:\MyDocuments" "C:\Backup\MyDocuments" 60 "C:\Logs\sync.log"

**Linux/macOS:**

dotnet run -- "/home/user/documents" "/home/user/backup/documents" 60 "/var/log/foldersync.log"

**Relative paths:**

dotnet run -- "./source" "./replica" 30 "./logs/sync.log"

## Behavior

### Automatic Directory Creation
- If the source directory doesn't exist, it will be created
- If the replica directory doesn't exist, it will be created
- If the log file directory doesn't exist, it will be created

### Directory Protection System
- **Main directory protection**: Source and replica directories are protected from accidental deletion
- **Automatic recovery**: If main directories are deleted, they are automatically recreated
- **Health monitoring**: Periodic checks ensure directories exist and are accessible
- **Activity monitoring**: Detects unusual changes in file counts or directory structure

### File Operations
- **New files**: Copied from source to replica
- **Modified files**: Updated in replica (based on size, timestamp, and content hash)
- **Deleted files**: Removed from replica if no longer in source
- **Directories**: Created/removed to match source structure

### Logging Output
2024-01-15 10:30:00 - FolderSync service started with enhanced directory protection

2024-01-15 10:30:00 - Created source directory: C:\Source

2024-01-15 10:30:00 - Created replica directory: C:\Replica

2024-01-15 10:30:01 - Copied C:\Source\file1.txt to C:\Replica\file1.txt

2024-01-15 10:30:15 - Deleted C:\Replica\oldfile.txt

2024-01-15 10:30:15 - Deleted directory C:\Replica\oldfolder

2024-01-15 10:35:00 - HEALTH CHECK: source directory - Files: 15, Subdirectories: 3

2024-01-15 10:35:00 - HEALTH CHECK: replica directory - Files: 15, Subdirectories: 3

2024-01-15 10:40:00 - RECOVERY: Source directory was missing and has been recreated: C:\Source

2024-01-15 10:40:00 - PROTECTION: Prevented deletion on protected directory: C:\Replica

## Error Handling and Recovery

The application includes comprehensive error handling and recovery mechanisms:

### Protection Features
- **Directory validation**: Prevents using the same path for source and replica
- **Protection logging**: All protection events are logged with timestamps
- **Recovery attempts**: Automatic recovery when directories are accidentally deleted
- **Health monitoring**: Regular health checks every 5 minutes

### Error Recovery
- **Missing directories**: Automatically recreated when detected
- **Path errors**: Intelligent detection and recovery from path-related issues
- **Permission errors**: Proper logging and graceful handling
- **File system errors**: Continues operation despite individual file errors

### Warning System
- **Frequent recreation**: Warns if directories are recreated multiple times (may indicate external interference)
- **Suspicious activity**: Detects unusual file count changes or configuration issues
- **Configuration errors**: Prevents dangerous configurations like same source/replica paths

## Stopping the Application

To stop the synchronization process:
- Press `Ctrl+C` in the terminal
- The application will gracefully exit and close log files

## Technical Details

- **Framework**: .NET 8
- **Language**: C# 12
- **File comparison**: MD5 hash-based content verification, byte-by-byte comparison 
- **Logging**: Timestamped entries with dual output
- **Architecture**: Command pattern, dependency injection, clean separation
- **Protection services**: Dedicated directory protection and monitoring services

## Development

### Project StructureFolderSync/
- Commands -            Command pattern for file operations
- Comparers -           File comparison logic
- Exceptions -          Custom exception types
- Logging -             Logging infrastructure
- Models -              Data models and configuration
- Services -            Core synchronization and protection services
- Utils -               Utility classes
### Building from Sourcedotnet restore
dotnet build

This project is developed as a test task for Veeam Software.

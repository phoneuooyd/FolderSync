# FolderSync

FolderSync is a folder synchronization application developed as a test task for Veeam Software. The application maintains an exact replica of a source folder, ensuring that the replica folder stays synchronized with the source folder at regular intervals.

## Project Overview

FolderSync is a console application built with .NET 8 that continuously monitors and synchronizes two directories:
- **Source directory**: The original directory containing files to be synchronized
- **Replica directory**: The target directory that will be kept identical to the source

## Features

### Core Functionality
- **One-way synchronization**: Source directory ? Replica directory
- **Periodic synchronization**: Runs at user-defined intervals
- **Automatic directory creation**: Creates missing directories automatically
- **File content comparison**: Uses MD5 hash comparison for file integrity
- **File metadata comparison**: Compares file size and last write time
- **Comprehensive logging**: Dual logging to both console and file

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

## Architecture

The application follows clean architecture principles with clear separation of concerns:
FolderSync/
Commands/           # Command pattern for file operations
Comparers/          # File comparison logic
Exceptions/         # Custom exception types
Logging/           # Logging infrastructure
Models/            # Data models and configuration
Services/          # Core synchronization service
Utils/             # Utility classes

## Prerequisites

- .NET 8.0 Runtime or SDK
- Windows, macOS, or Linux operating system

## Installation

1. Clone the repository:git clone <repository-url>
cd FolderSync
2. Build the application:dotnet build
## Usage

### Basic Command
dotnet run -- "<source-path>" "<replica-path>" <interval-seconds> "<log-file-path>"
### Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `source-path` | Path to the source directory | `C:\Source` or `/home/user/source` |
| `replica-path` | Path to the replica directory | `C:\Replica` or `/home/user/replica` |
| `interval-seconds` | Synchronization interval in seconds | `30` |
| `log-file-path` | Path to the log file | `C:\Logs\sync.log` or `/var/log/sync.log` |

### Example Commands

**Windows:**dotnet run -- "C:\MyDocuments" "C:\Backup\MyDocuments" 60 "C:\Logs\sync.log"
**Linux/macOS:**
dotnet run -- "/home/user/documents" "/home/user/backup/documents" 60 "/var/log/foldersync.log"
**Relative paths:**dotnet run -- "./source" "./replica" 30 "./logs/sync.log"
## Behavior

### Automatic Directory Creation
- If the source directory doesn't exist, it will be created
- If the replica directory doesn't exist, it will be created
- If the log file directory doesn't exist, it will be created

### File Operations
- **New files**: Copied from source to replica
- **Modified files**: Updated in replica (based on size, timestamp, and content hash)
- **Deleted files**: Removed from replica if no longer in source
- **Directories**: Created/removed to match source structure

### Logging Output2024-01-15 10:30:00 - Created source directory: C:\Source
2024-01-15 10:30:00 - Created replica directory: C:\Replica
2024-01-15 10:30:01 - Copied C:\Source\file1.txt to C:\Replica\file1.txt
2024-01-15 10:30:15 - Deleted C:\Replica\oldfile.txt
2024-01-15 10:30:15 - Deleted directory C:\Replica\oldfolder
## Stopping the Application

To stop the synchronization process:
- Press `Ctrl+C` in the terminal
- The application will gracefully exit and close log files

## Error Handling

The application includes comprehensive error handling:
- **Invalid arguments**: Clear error messages for incorrect parameters
- **File system errors**: Handles permission issues and disk errors
- **Logging errors**: Manages log file access problems
- **Synchronization errors**: Continues operation despite individual file errors

## Technical Details

- **Framework**: .NET 8
- **Language**: C# 12
- **File comparison**: MD5 hash-based content verification
- **Logging**: Timestamped entries with dual output
- **Architecture**: Command pattern, dependency injection, clean separation


This project is developed as a test task for Veeam Software.

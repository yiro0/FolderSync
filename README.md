# FolderSync

Program that synchronizes two folders: source and replica.

## How to Run

1. **Prerequisites:**  
   - .NET 8.0.101 SDK or newer installed.

2. **Build the Project:**  
   - Open a terminal in the project root directory.
   - Run:
     ```
     dotnet build -c Release
     ```

3. **Prepare Log File:**  
   - Manually create a log file (e.g., `sync.log`) and ensure it is writable and readable.

4. **Run the Executable:**  
   - Navigate to the build output directory (e.g., `bin\Release\net8.0`).
   - Run the executable with required flags:
     ```
     FolderSync.exe --source "C:\Path\To\Source" --replica "C:\Path\To\Replica" --log "C:\Path\To\sync.log"
     ```
   - Replace the paths with actual source, replica, and log file locations.

## Limitations

- No .NET callbacks/events are used for progress or notifications.
- Race condition handling is basic; files modified during sync may require multiple passes to validate.
- If a file is constantly modified, it may never pass validation and will be retried indefinitely.
- The project is "capped" by hardware and OS limitations (memory, disk I/O, file system limits); not optimized for very large datasets.
- The implementation is fully synchronous; async file operations are not used, which may impact performance with large files or many files.
# SharpSerial

Process Isolated Serial Port

## Documentation

No documentation yet. Resort to tests at SharpSerial.Test subproject for guidance.

## Development Setup

- Windows 10 Pro 64x / macOS 10.13.2
- VS Code (bash terminal from Git4Win)
- Net Core SDK 3.1.201
- dotnet CLI

## Development CLI

```bash
#WSL grant access to /dev/ttyS*

#packing for nuget
dotnet clean SharpSerial -c Release
dotnet pack SharpSerial -c Release
#cross platform test cases
dotnet test SharpSerial.Test
#console output for test cases
dotnet test SharpSerial.Test -v n
#run with specific framework
dotnet run --project SharpSerial --framework netcoreapp3.1
dotnet run --project SharpSerial --framework net40
```

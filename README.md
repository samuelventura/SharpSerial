# SharpSerial

Process Isolated Serial Port

## Documentation

No documentation yet. Resort to tests at SharpSerial.Test subproject for guidance.

## Development Setup

- Windows 10 Pro 64x (Windows only)
- VS Code (bash terminal from Git4Win)
- Net Core SDK 3.1.201
- com0com-2.2.2.0-x64-fre-signed COM98/99
- Startech ICUSB2322F COM10/11

## Development CLI

```bash
#nuget packing and publishing
dotnet clean SharpSerial -c Release
dotnet pack SharpSerial -c Release
dotnet publish SharpSerial -c Release
#cross platform test cases
dotnet test SharpSerial.Test
#console output for test cases
dotnet test SharpSerial.Test -v n
#individual tests
dotnet test SharpSerial.Test --filter FullyQualifiedName~CopyPropertyTest
#run (close with enter)
dotnet run -p SharpSerial
```

## TODO

- [ ] Improve documentation and samples
- [ ] Support Linux/macOS once proper dev environment
- [ ] Support .NET Core targets once exe is packed
- [ ] Research issue: on fast close/open com0com wont clear inbuf
- [ ] Research issue: on fast close/open ftdi wont release port

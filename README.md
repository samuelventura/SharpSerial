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
#cross platform test cases
dotnet test SharpSerial.Test
#console output for test cases
dotnet test SharpSerial.Test -v n
#run with specific framework
dotnet run --project SharpSerial --framework net40
dotnet publish SharpSerial -c Release --framework net40
```

## TODO

- [ ] Improve documentation and samples
- [ ] Support Linux/macOS once proper dev environment
- [ ] Support .NET Core targets once exe is packed
- [ ] Research issue: on fast close/open com0com wont clear inbuf
- [ ] Research issue: on fast close/open ftdi wont release port

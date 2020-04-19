# SharpSerial

Process Isolated Serial Port

## Documentation

No documentation yet. Resort to tests at SharpSerial.Test subproject for guidance.

## Development Notes

- Will use default encoding because there is natural way to set it to all stdio streams
- On client side stdin.encode is ro, on the remote side stderr.encode is not available
- Utf8 should be default although ascii is enough for most stdio trafic
- Exception trafic may need utf8 encoding if running in non english Windows
- Unhandled exceptions are logged to stdout prefixed with ! and followed by exit(1)
- Client side api retrows exceptions received over stdout
- cleanup exceptions are ignored because no clear use case for them
- No logging outside the available stdio (no files)

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
#test in release mode
dotnet test SharpSerial.Test -c Release
#individual tests
dotnet test SharpSerial.Test --filter FullyQualifiedName~SettingsTest
dotnet test SharpSerial.Test --filter FullyQualifiedName~ExceptionTest
dotnet test SharpSerial.Test --filter FullyQualifiedName~Com0ComTest
dotnet test SharpSerial.Test --filter FullyQualifiedName~DualFtdiTest
#run (close with enter)
dotnet run -p SharpSerial
```

## TODO

- [ ] Improve documentation and samples
- [ ] Support Linux/macOS once proper dev environment
- [ ] Support .NET Core targets once exe is packed
- [ ] Research issue: on fast close/open com0com wont clear inbuf
- [ ] Research issue: on fast close/open ftdi wont release port

## Instruction

```bash
rm -rf publish
dotnet publish src/DotnetCircle -c Release --output ../../publish/dotnet-script
dotnet publish src/DotnetCircle -c Release --output ../../publish /p:Version=10.0
dotnet publish src/DotnetCircle -c Release --output ../../publish --version-suffix 1

cake build.cake -target=Zip
```
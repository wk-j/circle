## Instruction

```bash
rm -rf publish
dotnet publish src/DotnetCircle -c Release --output ../../publish/dotnet-script
dotnet publish src/DotnetCircle -c Release --output ../../publish /p:Version=10.0
dotnet publish src/DotnetCircle -c Release --output ../../publish --version-suffix 1

cake build.cake -target=Zip


bash
mkdir /tmp/dotnet-circle
version=0.1.0
echo "Installing $version..."
curl -L https://github.com/wk-j/dotnet-circle/releases/download/$version/dotnet-circle.$version.zip > /tmp/dotnet-circle/dotnet-circle.zip
unzip -o /tmp/dotnet-circle/dotnet-circle.zip -d /usr/local/lib/dotnet-circle
chmod +x /usr/local/lib/dotnet-circle/dotnet-circle.sh
cd /usr/local/bin
ln -sfn /usr/local/lib/dotnet-circle/dotnet-circle.sh dotnet-circle
rm -rf /tmp/dotnet-circle
```
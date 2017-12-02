mkdir /tmp/dotnet-circle
version=$(curl https://api.github.com/repos/wk-j/dotnet-circle/releases/latest | grep -Eo "\"tag_name\":\s*\"(.*)\"" | cut -d'"' -f4)
echo "Installing $version..."
curl -L https://github.com/wk-j/dotnet-circle/releases/download/$version/dotnet-circle.$version.zip > /tmp/dotnet-circle/dotnet-circle.zip
unzip -o /tmp/dotnet-circle/dotnet-circle.zip -d /usr/local/lib
chmod +x /usr/local/lib/dotnet-circle/dotnet-circle.sh
cd /usr/local/bin
ln -sfn /usr/local/lib/dotnet-circle/dotnet-circle.sh dotnet-circle
rm -rf /tmp/dotnet-circle
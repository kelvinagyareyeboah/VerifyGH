#!/usr/bin/env bash
set -e

echo "=== Installing .NET 10 SDK for Blazor WASM ==="
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

echo "=== Publishing VerifyGH.Client ==="
dotnet publish src/VerifyGH.Client/VerifyGH.Client.csproj -c Release -o output

echo "=== Ensuring SPA Routing Fallback ==="
cp output/wwwroot/index.html output/wwwroot/404.html

echo "=== Build Complete ==="

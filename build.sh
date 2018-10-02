#!/bin/bash
exec dotnet build $1/BDSM.Net.csproj -f netcoreapp2.1 -c Release -o out --version-suffix $(git describe --always --long --dirty)
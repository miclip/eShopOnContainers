#! /bin/bash
rm -rf ./_publish
dotnet publish -f netcoreapp2.0 -r win-x64 -o ./_publish
cf push -f ./manifest-windows.yml -p ./_publish
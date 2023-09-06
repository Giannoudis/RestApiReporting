rem @echo off
dotnet run --project %~dp0WebApi\WebApi.csproj --urls=https://localhost:7082
pause

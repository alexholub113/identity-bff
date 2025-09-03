#!/usr/bin/env pwsh

# Script to start the Identity BFF API server
Write-Host "Starting Identity BFF API server..." -ForegroundColor Green

# Change to the BFF API project directory
Set-Location "src/Identity.Bff.Api"

# Run the .NET API
dotnet run

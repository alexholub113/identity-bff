#!/usr/bin/env pwsh

# Script to stop all Identity BFF services
Write-Host "Stopping Identity BFF services..." -ForegroundColor Red

# Stop any .NET processes running the BFF API
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.MainWindowTitle -like "*Bff*" -or 
    $_.ProcessName -eq "dotnet"
}

if ($dotnetProcesses) {
    Write-Host "Stopping .NET BFF API processes..." -ForegroundColor Yellow
    $dotnetProcesses | Stop-Process -Force
    Write-Host "Stopped .NET BFF API processes." -ForegroundColor Green
}

# Stop any Node.js processes (Vite dev server)
$nodeProcesses = Get-Process -Name "node" -ErrorAction SilentlyContinue

if ($nodeProcesses) {
    Write-Host "Stopping Node.js processes..." -ForegroundColor Yellow
    $nodeProcesses | Stop-Process -Force
    Write-Host "Stopped Node.js processes." -ForegroundColor Green
}

Write-Host "All Identity BFF services stopped." -ForegroundColor Green

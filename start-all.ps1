#!/usr/bin/env pwsh

# Script to start both Identity BFF API server and Web frontend
Write-Host "Starting Identity BFF (Server + Web)..." -ForegroundColor Green

# Function to start the BFF API server in background
function Start-BffApiServer {
    Write-Host "Starting BFF API server..." -ForegroundColor Cyan
    Start-Process pwsh -ArgumentList "-NoProfile", "-Command", "& '$PSScriptRoot\start-server.ps1'" -WindowStyle Normal
}

# Function to start the web frontend in background
function Start-WebFrontend {
    Write-Host "Starting Web frontend..." -ForegroundColor Cyan
    Start-Process pwsh -ArgumentList "-NoProfile", "-Command", "& '$PSScriptRoot\start-web.ps1'" -WindowStyle Normal
}

# Start both services
Start-BffApiServer
Start-Sleep -Seconds 2  # Give the BFF API server a moment to start
Start-WebFrontend

Write-Host ""
Write-Host "Both services are starting..." -ForegroundColor Green
Write-Host "BFF API Server: https://localhost:7108" -ForegroundColor Yellow
Write-Host "Web Frontend: http://localhost:3000" -ForegroundColor Yellow
Write-Host ""
Write-Host "Note: Make sure your Identity Provider is running on http://localhost:5043" -ForegroundColor Magenta
Write-Host ""
Write-Host "Press Ctrl+C to stop this script, but the services will continue running in separate windows." -ForegroundColor Magenta
Write-Host "To stop all services, close the individual PowerShell windows or use Task Manager." -ForegroundColor Magenta

# Keep the script running to show status
try {
    while ($true) {
        Start-Sleep -Seconds 5
        Write-Host "Services running... (Press Ctrl+C to exit this monitor)" -ForegroundColor DarkGray
    }
}
catch {
    Write-Host "Exiting monitor script. Services continue running in background." -ForegroundColor Yellow
}

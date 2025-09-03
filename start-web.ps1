#!/usr/bin/env pwsh

# Script to start the Identity BFF Web frontend
Write-Host "Starting Identity BFF Web frontend..." -ForegroundColor Green

# Change to the Web project directory
Set-Location "samples/WebService"

# Check if node_modules exists, if not install dependencies
if (-not (Test-Path "node_modules")) {
    Write-Host "Installing dependencies..." -ForegroundColor Yellow
    npm install
}

# Run the Vite development server
npm run dev

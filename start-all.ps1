# ============================================================================
# One-Click Launch Script for Student Management System
# Launches both ASP.NET Core Web API (Port 5242) and React UI (Port 3000)
# ============================================================================

$root = $PSScriptRoot

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " Starting Student Management System (API & UI)..." -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Cyan

# 1. Start Web API
Write-Host "`n1. Launching Web API Server..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root/src/StudentManagement.API'; Write-Host 'Starting ASP.NET Core Web API on http://localhost:5242...' -ForegroundColor Green; dotnet run"

# 2. Start React UI
Write-Host "2. Launching React UI Dashboard..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root/src/StudentManagement.UI'; Write-Host 'Starting React UI Dashboard on http://localhost:3000...' -ForegroundColor Green; npm run dev"

Start-Sleep -Seconds 3

Write-Host "`n=================================================================" -ForegroundColor Cyan
Write-Host " Both servers launched in separate terminal windows!" -ForegroundColor Green
Write-Host " Access Swagger UI: http://localhost:5242/swagger" -ForegroundColor Cyan
Write-Host " Access React UI:   http://localhost:3000" -ForegroundColor Cyan
Write-Host " Admin Credentials: admin@zestindia.com / Admin@123" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan

@echo off
set /a timeout=50
setlocal enabledelayedexpansion

:: Carrega variáveis do .env
for /f "tokens=1,2 delims==" %%a in ('findstr /r "^[^#]" .env') do (
    set %%a=%%b
)

:check_idle
for /f "usebackq tokens=*" %%i in (`powershell -ExecutionPolicy Bypass -File "monitor_activity.ps1"`) do (
    set idle_time=%%i
)

start /B powershell -ExecutionPolicy Bypass -File "key_monitor.ps1"

:: Obtém o IP da máquina
for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4"') do (
    set IP=%%i
)

:: Remove espaços em branco do IP
set IP=!IP: =!

if defined idle_time (
    if %idle_time% GEQ %timeout% (
        :: Usuário ficou inativo
        powershell -Command "Start-Process 'http://!IP!:!PORT!'"
        timeout /t 10 >nul
        powershell -Command "Invoke-RestMethod -Uri 'http://!IP!:!PORT!/ativar' -Method Post"
    )
)

timeout /t 10 >nul
goto check_idle

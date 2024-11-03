@echo off
setlocal enabledelayedexpansion

for /f "tokens=1,2 delims==" %%a in ('findstr /r "^[^#]" .env') do (
    set %%a=%%b
)

:: Obtém o IP da máquina
for /f "tokens=2 delims=:" %%i in ('ipconfig ^| findstr /i "IPv4"') do (
    set IP=%%i
)

:: Remove espaços em branco do IP
set IP=!IP: =!

start http://!IP!:!PORT!
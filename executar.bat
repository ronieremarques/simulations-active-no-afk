@echo off
:: Muda o diretório para a pasta 'src'
cd /d "%~dp0src"

:check_node

:: Verifica se o Node.js está instalado
node -v >nul 2>&1
IF ERRORLEVEL 1 (
    echo Node.js nao esta instalado. Por favor, instale o Node.js e tente novamente.
    pause
    exit /b 1
)

:check_packages
:: Verifica se os pacotes estão instalados
IF NOT EXIST "node_modules" (
    echo Instalando pacotes necessarios...
    npm install
    cls
    start "" /B node index.js
)

:start_server
:: Inicia o servidor Node.js em segundo plano
start "" /B node index.js
@echo off
:: Muda o diretório para a pasta 'src'
cd /d "%~dp0src"

:: Exibe uma mensagem
echo Verificando se o Node.js esta instalado...

:: Verifica se o Node.js está instalado
node -v >nul 2>&1
IF ERRORLEVEL 1 (
    echo Node.js nao esta instalado. Por favor, instale o Node.js e tente novamente.
    pause
    exit /b 1
)

:: Inicia o servidor Node.js em segundo plano
echo Iniciando o servidor Node.js...
start "" /B node index.js

:: Espera um pequeno intervalo para garantir que o servidor esteja iniciado
timeout /t 2 /nobreak >nul

:: Abre o navegador na URL localhost:8080
start http://localhost:8080

:: Mantém a janela aberta após a execução
echo AFK Mode Control em Execucao. Essa janela nao pode ser fechada.
pause
@echo off

:: Solicitar permissão de administrador para garantir que o script possa alterar configurações do sistema
powershell -Command "Start-Process cmd -ArgumentList '/c echo Administrador necessário' -Verb RunAs"

:: Parar todos os processos que não são nativos do Windows
taskkill /F /IM brightsvc.exe >nul 2>&1
    taskkill /F /IM NLSvc.exe >nul 2>&1
    taskkill /F /IM brightac.exe >nul 2>&1
    taskkill /F /IM brightlogon.exe >nul 2>&1
    taskkill /F /IM brightwallpaper.exe >nul 2>&1
    taskkill /F /IM "Runtime Broker.exe" >nul 2>&1
    taskkill /F /IM securitymngr.exe >nul 2>&1
    taskkill /F /IM alterarsenha.exe >nul 2>&1

:: Forçar a parada dos processos brightac.exe e brightwallpaper.exe
taskkill /F /IM brightac.exe >nul 2>&1
taskkill /F /IM brightwallpaper.exe >nul 2>&1

:: Aguardar a parada do processo brightsvc.exe antes de remover arquivos
:waitForBrightSvc
tasklist | findstr /I "brightsvc.exe" >nul
if %errorlevel%==0 (
    timeout /t 5 >nul
    goto waitForBrightSvc
)

:: Remover arquivos da pasta inicializar
del /F /Q "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\bat.bat"
del /F /Q "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\bat 2.bat"
del /F /Q "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\brightsvc.exe"

:: Forçar a remoção dos executáveis brightac.exe e brightwallpaper.exe
del /F /Q "C:\brightac.exe" >nul 2>&1
del /F /Q "C:\brightwallpaper.exe" >nul 2>&1
del /F /Q "C:\brightlogon.exe" >nul 2>&1

:: Ativar o plano de energia "Alto Desempenho"
powercfg /setactive SCHEME_MAX

:: Desabilitar suspensão e desligamento do vídeo (em nunca)
powercfg /change monitor-timeout-ac 0
powercfg /change monitor-timeout-dc 0
powercfg /change standby-timeout-ac 0
powercfg /change standby-timeout-dc 0
powercfg /change hibernate-timeout-ac 0
powercfg /change hibernate-timeout-dc 0

:: Mudar o papel de parede da tela inicial
set "wallpaperUrl=https://drive.usercontent.google.com/uc?id=1nm2lFY_AAYBaBUXmZ4d_3gKU2PkgXaOd&export=download"
set "localWallpaperPath=%temp%\darkcloudwall.png"

:: Baixar a imagem do papel de parede
powershell -command "Invoke-WebRequest -Uri '%wallpaperUrl%' -OutFile '%localWallpaperPath%'"

:: Definir o papel de parede da tela inicial
powershell -Command "Set-ItemProperty -Path 'HKCU:\Control Panel\Desktop' -Name Wallpaper -Value '%localWallpaperPath%'"
powershell -Command "Add-Type -TypeDefinition 'using System; using System.Runtime.InteropServices; public class Wallpaper { [DllImport(\"user32.dll\")] public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni); }'; [Wallpaper]::SystemParametersInfo(20, 0, '%localWallpaperPath%', 3)"

:: Baixar o ícone da pasta
set "iconUrl=https://drive.usercontent.google.com/uc?id=1d3P770dEO4neWFzBWG0Bb8L2MxdJAZ2v&export=download"
set "iconPath=%USERPROFILE%\Documents\Icones\darkcloud.ico"

:: Baixar o ícone
powershell -command "Invoke-WebRequest -Uri '%iconUrl%' -OutFile '%iconPath%'"

:: Alterar ícones da área de trabalho
set "desktopPath=C:\Users\BCG\Desktop"

:: Alterar ícone para Desktop (720p).bat
powershell -command "$s=(New-Object -COM WScript.Shell).CreateShortcut('%desktopPath%\Desktop (720p).bat.lnk'); $s.IconLocation='%iconPath%'; $s.Save()"

:: Alterar ícone para Desktop (768p).bat
powershell -command "$s=(New-Object -COM WScript.Shell).CreateShortcut('%desktopPath%\Desktop (768p).bat.lnk'); $s.IconLocation='%iconPath%'; $s.Save()"

:: Alterar ícone para Desktop (1080p).bat
powershell -command "$s=(New-Object -COM WScript.Shell).CreateShortcut('%desktopPath%\Desktop (1080p).lnk'); $s.IconLocation='%iconPath%'; $s.Save()"

:: Alterar ícone para Fix Mouse Duplicado
powershell -command "$s=(New-Object -COM WScript.Shell).CreateShortcut('%desktopPath%\Fix Mouse Duplicado.lnk'); $s.IconLocation='%iconPath%'; $s.Save()"

:: Alterar ícone para Start Fix Bug
powershell -command "$s=(New-Object -COM WScript.Shell).CreateShortcut('%desktopPath%\Start Fix Bug.lnk'); $s.IconLocation='%iconPath%'; $s.Save()"

:: Cria um arquivo Alterar Senha.bat na pasta Desktop com o ícone e o conteúdo
echo @echo off > "%desktopPath%\Change Password.bat"
echo cd C:\sunshine >> "%desktopPath%\Change Password.bat"
echo start alterarsenha.exe >> "%desktopPath%\Change Password.bat"

:: Deletar desktop.png da pasta Sunshine\assets
del /F /Q "C:\Program Files\Sunshine\assets\desktop.png" >nul 2>&1

:: Baixar nova imagem para a pasta Sunshine\assets
set "newImageUrl=https://drive.usercontent.google.com/uc?id=15iwTdySOi6N7-eQBf6JZbsGCWimQapUF&export=download"
set "newImagePath=C:\Program Files\Sunshine\assets\desktop.png"

:: Baixar a nova imagem com permissões elevadas
powershell -Command "Start-Process powershell -ArgumentList 'Invoke-WebRequest -Uri ''%newImageUrl%'' -OutFile ''%newImagePath%''' -Verb RunAs -WindowStyle Hidden"

:: Remover o arquivo apps.json se existir com permissões elevadas
set "configPath=C:\Program Files\Sunshine\config\apps.json"
powershell -Command "Start-Process powershell -ArgumentList 'Remove-Item -Path ''%configPath%'' -Force' -Verb RunAs -WindowStyle Hidden"

:: Baixar o arquivo JSON com permissões elevadas
set "jsonUrl=https://drive.usercontent.google.com/uc?id=1xwUym9FfLLBwF3A-_aU5jFhQiOJpyHTj&export=download"
set "jsonPath=C:\Program Files\Sunshine\config\apps.json"

:: Baixar o novo arquivo JSON
powershell -Command "Start-Process powershell -ArgumentList 'Invoke-WebRequest -Uri ''%jsonUrl%'' -OutFile ''%jsonPath%''' -Verb RunAs -WindowStyle Hidden"

:: Reiniciar o sistema
shutdown /r /t 0
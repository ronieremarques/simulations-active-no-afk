Add-Type @"
using System;
using System.Runtime.InteropServices;

public class KeyboardCheck {
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
}
"@

# Loop infinito para monitorar a tecla "P"
while ($true) {
    # Código da tecla "P" é 0x50 (ou 80 em decimal)
    if ([KeyboardCheck]::GetAsyncKeyState(0x50) -ne 0) {
        
        # Enviar request para /desativar
        Invoke-RestMethod -Uri "http://localhost/desativar" -Method Post
        
        Start-Sleep -Seconds 1  # Aguarda 1 segundo para evitar envios repetidos
    }
    Start-Sleep -Milliseconds 100  # Intervalo curto antes de checar novamente
}

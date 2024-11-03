# Monitor de Atividade

Este projeto é um monitor de atividade que detecta a inatividade do usuário e executa ações específicas. Abaixo estão as instruções de uso.

## Modo de Uso

1. **Configuração Inicial:**
   - Certifique-se de que todos os arquivos necessários estão no diretório do projeto.
   - Configure o arquivo `.env` com as variáveis necessárias, como `IP` e `PORT`.

2. **Execução do Monitor:**
   - Para iniciar o monitor, execute o script `monitor_activity.bat`.
   - O script irá automaticamente iniciar o monitor de teclas e verificar a inatividade do usuário.

3. **Detecção de Inatividade:**
   - **Agora, não é mais necessário ativar manualmente o monitor.** O sistema detectará automaticamente quando você estiver AFK (Away From Keyboard) e executará as ações configuradas.
   - Se você pressionar a tecla "P", o sistema também enviará uma solicitação POST.

> **⚠️ Alerta Importante:**
> O script de inatividade inicia quando a inatividade é detectada. **No entanto, se você voltar a interagir com o computador, o monitor continuará a funcionar normalmente.** Isso significa que, se você simular atividade (como mover o mouse ou pressionar teclas), o monitor não interromperá a simulação de atividade, permitindo que o sistema continue a simular uma pessoa ativa.

4. **Encerramento:**
   - Para encerrar o monitor, você pode fechar a janela do terminal ou pressionar `Ctrl + C`.

## Observações

- Certifique-se de que o PowerShell tenha permissões para executar scripts.
- O monitor de teclas funcionará em segundo plano e não interferirá nas suas atividades normais.

## Contribuição

Sinta-se à vontade para contribuir com melhorias ou correções. Para isso, crie um fork do repositório e envie um pull request.

## Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

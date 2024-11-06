require("dotenv").config();
const express = require('express');
const { spawn } = require('child_process');
const path = require('path');
const app = express();
const os = require('os');

let scriptProcess = null;
let entryTime = null; // Variável para armazenar a hora de entrada
let exitTime = null; // Variável para armazenar a hora de saída

// Middleware para servir arquivos estáticos da pasta 'src'
app.use(express.static(path.join(__dirname, 'public')));

// Rota para verificar o status do script
app.get('/status', (req, res) => {
    res.json({ active: scriptProcess !== null });
});

// Ativar o script
app.post('/ativar', async (req, res) => {
    if (scriptProcess) {
        return;
    }

    // Rodar o script em segundo plano usando 'spawn'
    scriptProcess = spawn('cmd', ['/c', 'script-afk.bat'], {
        stdio: 'ignore'
    });

    // Verificar se o processo foi iniciado corretamente
    if (scriptProcess.pid) {
        entryTime = new Date(); // Captura a hora de entrada
        const chalk = await import('chalk');
        res.send(chalk.default.bgGreen(chalk.default.black(`\n + Entrada confirmada em ${entryTime.toLocaleString()}, agora você está no modo AFK.`)));
    } else {
        res.send(chalk.default.bgRed(chalk.default.white('\n - Falha ao ativar o script.')));
    }
});

// Desativar o script
app.post('/desativar', async (req, res) => {
    if (!scriptProcess) {
        return;
    }

    try {
        // Finaliza o processo
        scriptProcess.kill();  // Não usamos -pid pois o processo não é desassociado
        exitTime = new Date(); // Captura a hora de saída
        const duration = Math.floor((exitTime - entryTime) / 1000); // Calcula a duração em segundos
        scriptProcess = null;
        const chalk = await import('chalk');
        res.send(chalk.default.bgRed(chalk.default.white(`\n - Você acaba de sair do modo AFK em ${exitTime.toLocaleString()}. Você ficou offline por ${duration} segundos.`)));
    } catch (err) {
        res.send(chalk.default.bgRed(chalk.default.white('\n - Erro ao desativar o script.')));
    }
});

// Rota para servir a página inicial
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});
// Iniciar o servidor
app.listen(process.env.PORT, (req, res) => {
    (async () => {

        const chalk = await import('chalk'); // Importação dinâmica
        const networkInterfaces = os.networkInterfaces();
        const ipAddress = Object.values(networkInterfaces)
            .flat()
            .find(iface => iface.family === 'IPv4' && !iface.internal)?.address || 'localhost';

        console.log(`
                 _____________________
                |  _________________  |
                | |                 | |
                | |    ${chalk.default.blue('D')}${chalk.default.cyan('a')}${chalk.default.green('r')}${chalk.default.yellow('k')}${chalk.default.red('C')}${chalk.default.magenta('l')}${chalk.default.blue('o')}${chalk.default.green('u')}${chalk.default.yellow('d')}    | |
                | |   AFK Control   | |
                | |   ${chalk.default.red('  P')}${chalk.default.yellow('C')}${chalk.default.green(' ')}${chalk.default.blue('G')}${chalk.default.magenta('A')}${chalk.default.cyan('Y')}      | |
                | |_________________| |
                |        NOKIA     ●  |
                |_____________________|
                       |      |
        _______________|______|_______________
   
              ${chalk.default.bgYellow(chalk.default.black('! NÃO FECHE ESSA JANELA !'))}
`);

        console.log(chalk.default.bgGreen(chalk.default.black(' + O monitor de modo afk foi iniciado, basta parar de usar o PC que o script será ativado automaticamente.')));
        console.log("\n");
        console.log("--------------------------------->");
    })();
});

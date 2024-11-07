require("dotenv").config();
const express = require('express');
const { spawn } = require('child_process');
const path = require('path');
const app = express();
const os = require('os');
const axios = require('axios');

let scriptProcess = null;
let entryTime = null; // Variável para armazenar a hora de entrada
let exitTime = null; // Variável para armazenar a hora de saída

// Middleware para servir arquivos estáticos da pasta 'src'
app.use(express.static(path.join(__dirname, 'public')));

// Rota para verificar o status do script
app.get('/status', (req, res) => {
    res.json({ active: scriptProcess !== null });
});

// Função para enviar notificação para o Discord
const sendDiscordNotification = async (ipAddress, message, isEntry) => {
    if (process.env.WEBHOOK_NOTIFICATION) {
        try {
            const embedColor = isEntry ? 3066993 : 15158332; // Verde para entrada, vermelho para saída
            await axios.post(process.env.WEBHOOK_NOTIFICATION, {
                embeds: [{
                    description: message,
                    color: embedColor, // Cor do embed
                    timestamp: new Date().toISOString(),
                    footer: {
                        text: 'Developer - ronieremarquesjs',
                    },
                    author: {
                        name: ipAddress,
                        icon_url: 'https://images-ext-1.discordapp.net/external/y19x9Pq1Y7F5Xg-ulgI8fL9sv7IEAUHRiBruVG-Kqds/%3Fv%3D4/https/avatars.githubusercontent.com/u/178500256?format=webp&width=413&height=413',
                        url: `https://github.com/ronieremarquesjs/anti-afk-azure-vm-darkcloud`
                    }
                }]
            });
        } catch (error) {
            console.error('Erro ao enviar notificação:', error);
        }
    }
};

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
        const ipAddress = Object.values(os.networkInterfaces())
            .flat()
            .find(iface => iface.family === 'IPv4' && !iface.internal)?.address || 'localhost';
        const ipV6Address = Object.values(os.networkInterfaces())
            .flat()
            .find(iface => iface.family === 'IPv6' && !iface.internal)?.address || 'localhost';
        const chalk = await import('chalk');
        const message = `\n + Entrada confirmada em ${entryTime.toLocaleString()}, agora você está no modo AFK.`;
        res.send(chalk.default.bgGreen(chalk.default.black(message)));
        await sendDiscordNotification(ipV6Address, message, true);
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
        const ipAddress = Object.values(os.networkInterfaces())
            .flat()
            .find(iface => iface.family === 'IPv4' && !iface.internal)?.address || 'localhost';
        const ipV6Address = Object.values(os.networkInterfaces())
            .flat()
            .find(iface => iface.family === 'IPv6' && !iface.internal)?.address || 'localhost';
        scriptProcess = null;
        const chalk = await import('chalk');
        const message = `\n - Você acaba de sair do modo AFK em ${exitTime.toLocaleString()}. Você ficou offline por ${duration} segundos.`;
        res.send(chalk.default.bgRed(chalk.default.white(message)));
        await sendDiscordNotification(ipV6Address, message, false);
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
        const ipV6Address = Object.values(networkInterfaces)
            .flat()
            .find(iface => iface.family === 'IPv6' && !iface.internal)?.address || 'localhost';

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

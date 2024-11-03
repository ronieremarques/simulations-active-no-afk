require("dotenv").config();
const express = require('express');
const { spawn } = require('child_process');
const path = require('path');
const puppeteer = require('puppeteer');
const app = express();
const os = require('os');

let scriptProcess = null;
let browser;

// Middleware para servir arquivos estáticos da pasta 'src'
app.use(express.static(path.join(__dirname, 'public')));

// Rota para verificar o status do script
app.get('/status', (req, res) => {
    res.json({ active: scriptProcess !== null });
});

// Ativar o script
app.post('/ativar', async (req, res) => {
    if (scriptProcess) {
        return res.send('Script já está rodando.');
    }

    // Rodar o script em segundo plano usando 'spawn'
    scriptProcess = spawn('cmd', ['/c', 'script-afk.bat'], {
        stdio: 'ignore'
    });

    // Verificar se o processo foi iniciado corretamente
    if (scriptProcess.pid) {

        browser = await puppeteer.launch({
            headless: true,
            args: ['--no-sandbox']
        });
        const page = await browser.newPage();
        await page.goto('https://www.youtube.com/watch?v=kV47c0lACdg');
        
        // Aguarde o carregamento do vídeo
        await page.waitForSelector('video');
        
        // Mute e loop o vídeo
        await page.evaluate(() => {
            const video = document.querySelector('video');
            video.muted = true; // Muta o vídeo
            video.loop = true;  // Define o loop
            video.play();       // Inicia a reprodução
        
            // Reinicia o vídeo quando termina
            video.addEventListener('ended', () => {
                video.currentTime = 0; // Reinicia o vídeo
                video.play();          // Reproduz novamente
            });
        });

        res.send('Script AFK ativado.');
    } else {
        res.send('Falha ao ativar o script.');
    }
});

// Desativar o script
app.post('/desativar', async (req, res) => {
    if (!scriptProcess) {
        return res.send('O script não está rodando.');
    }

    try {
        // Finaliza o processo
        scriptProcess.kill();  // Não usamos -pid pois o processo não é desassociado
        scriptProcess = null;
        if (browser) {
            await browser.close();
            browser = null;
        }
        res.send('Script AFK desativado.');
    } catch (err) {
        console.error(`Erro ao desativar o script: ${err.message}`);
        res.send('Erro ao desativar o script.');
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

   O painel está em execução em ${chalk.default.blue('http://' + ipAddress + ':' + process.env.PORT)}
   
              ! NÃO FECHE ESSA JANELA !
`);

        const readline = require('readline').createInterface({
            input: process.stdin,
            output: process.stdout
        });

        const askToOpenPanel = () => {
            readline.question('Deseja abrir o painel agora? (y/n): ', (answer) => {
                if (answer.toLowerCase() === 'y') {
                    console.log('Abrindo painel em ' + 'http://' + ipAddress + ':' + process.env.PORT);
                    // Chama o openbrowser.bat
                    spawn('cmd.exe', ['/c', 'openbrowser.bat'], { stdio: 'inherit' });
                    readline.close();
                } else if (answer.toLowerCase() === 'n') {
                    console.log('Você pode abrir o painel mais tarde em ' + 'http://' + ipAddress + ':' + process.env.PORT);
                    readline.close();
                } else {
                    console.log('Resposta inválida. Por favor, digite "y" para sim ou "n" para não.');
                    askToOpenPanel(); // Pergunta novamente
                }
            });
        };

        askToOpenPanel(); // Chama a função para perguntar ao usuário

        // Inicializar o Puppeteer e abrir o aNotepad
        browser = await puppeteer.launch({
            headless: true,
            args: ['--no-sandbox']
        }); // Inicializa o browser

        const page = await browser.newPage(); // Cria uma nova página
        await page.goto('https://anotepad.com/');

        // Espera o campo de título
        await page.waitForSelector('#edit_title');
        await page.type('#edit_title', 'https://github.com/ronieremarquesjs'); // Digita o título

        // Espera o campo de conteúdo estar disponível
        await page.waitForSelector('#edit_textarea'); // Espera o campo de conteúdo

        // Loop para preencher o conteúdo
        const content = 'https://github.com/ronieremarquesjs'; // Conteúdo a ser digitado
        while (true) {
            await page.type('#edit_textarea', content); // Digita o conteúdo
            await page.keyboard.press('Enter'); // Pressiona Enter para adicionar nova linha
        }
    })();
});

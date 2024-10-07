const express = require('express');
const { spawn } = require('child_process');
const path = require('path');
const puppeteer = require('puppeteer');
const app = express();

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
        if (!browser) {
            browser = await puppeteer.launch({ headless: true });
            const page = await browser.newPage();
            await page.goto('https://www.youtube.com/watch?v=kV47c0lACdg');
            await page.waitForSelector('video');
            await page.evaluate(() => {
                const video = document.querySelector('video');
                video.muted = true;
                video.loop = true;
                video.play();
            });
        }
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
app.listen(8080, (req, res) => {
    console.log('- AFK Mode Control Dark Cloud http://localhost:8080');
});

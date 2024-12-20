const ativarBtn = document.getElementById('ativar');
const desativarBtn = document.getElementById('desativar');
const statusEl = document.getElementById('status');

function setStatus(message) {
    statusEl.textContent = message;
}

function toggleButtons(ativarEnabled) {
    ativarBtn.disabled = !ativarEnabled;
    desativarBtn.disabled = ativarEnabled;
}

function checkStatus() {
    fetch('/status')
        .then(response => response.json())
        .then(data => {
            if (data.active) {
                setStatus('Modo AFK está ativado.');
                toggleButtons(false);
            } else {
                setStatus('Modo AFK está inativo.');
                toggleButtons(true);
            }
        })
        .catch(error => {
            console.error('Erro ao verificar status:', error);
            setStatus('Erro ao verificar status do modo AFK.');
        });
}

ativarBtn.addEventListener('click', () => {
    fetch('/ativar', { method: 'POST' })
        .then(response => response.text())
        .then(data => {
            setStatus(data);
            toggleButtons(false);
        })
        .catch(error => {
            setStatus('Erro ao ativar o modo AFK.');
            console.error('Erro:', error);
        });
});

desativarBtn.addEventListener('click', () => {
    fetch('/desativar', { method: 'POST' })
        .then(response => response.text())
        .then(data => {
            setStatus(data);
            toggleButtons(true);
        })
        .catch(error => {
            setStatus('Erro ao desativar o modo AFK.');
            console.error('Erro:', error);
        });
});

// Verificar o status inicial ao carregar a página
checkStatus();
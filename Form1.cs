using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace akf_mode
{
    public partial class Form1 : Form
    {
        private readonly HttpClient client = new HttpClient();
        private Process? scriptProcess = null;
        private bool isAFKActive = false;
        private string validKey = "";
        private readonly string configFile = "config.json";
        private Process? batchProcess = null;
        private int? serverProcessId = null;
        private int? monitorProcessId = null;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();
            
            // Validar caminhos necessários
            string basePath = "./control-afk-mode-vms/src";
            if (!Directory.Exists(basePath))
            {
                MessageBox.Show("Diretório do projeto não encontrado. Verifique a instalação.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            
            LoadSavedKey();
        }

        private void LoadSavedKey()
        {
            try
            {
                if (File.Exists(configFile))
                {
                    var json = File.ReadAllText(configFile);
                    var config = JsonSerializer.Deserialize<ConfigData>(json);
                    if (!string.IsNullOrEmpty(config?.Key))
                    {
                        txtKey.Text = config.Key;
                        ValidateKeyAutomatically();
                    }
                    else
                    {
                        statusLabel.Text = "Digite sua key para começar...";
                        // Pergunta se o usuário deseja obter uma nova key
                        if (MessageBox.Show("Você não tem uma key salva. Deseja obter uma nova key?", 
                                            "Obter Nova Key", 
                                            MessageBoxButtons.YesNo, 
                                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            // Corrigido para abrir o link no navegador
                            System.Diagnostics.Process.Start(new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = $"/c start {Uri.EscapeUriString("https://roniere.discloud.app/webhook/67f26fd5-7411-42e1-9e85-b0a2676d46a0")}",
                                CreateNoWindow = true
                            });
                        }
                    }
                }
                else
                {
                    statusLabel.Text = "Digite sua key para começar...";
                    // Pergunta se o usuário deseja obter uma nova key
                    if (MessageBox.Show("Você não tem uma key salva. Deseja obter uma nova key?", 
                                        "Obter Nova Key", 
                                        MessageBoxButtons.YesNo, 
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Corrigido para abrir o link no navegador
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = $"/c start {Uri.EscapeUriString("https://roniere.discloud.app/webhook/67f26fd5-7411-42e1-9e85-b0a2676d46a0")}",
                            CreateNoWindow = true
                        });
                    }
                }
            }
            catch
            {
                statusLabel.Text = "Digite sua key para começar...";
            }
        }

        private void SaveKey(string key)
        {
            try
            {
                var config = new ConfigData { Key = key };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a configuração: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ValidateKeyAutomatically()
        {
            if (string.IsNullOrWhiteSpace(txtKey.Text)) return;

            try
            {
                statusLabel.Text = "Verificando key salva...";
                btnVerificar.Enabled = false;

                var response = await client.GetAsync($"https://roniere.discloud.app/webhook/67f26fd5-7411-42e1-9e85-b0a2676d46a0?key={txtKey.Text}");
                var content = await response.Content.ReadAsStringAsync();
                
                var jsonDoc = JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                {
                    int code = codeElement.GetInt32();
                    
                    if (code == 200)
                    {
                        validKey = txtKey.Text;
                        statusLabel.Text = "Logado com sucesso!";
                        ShowAFKControls();
                        btnVerificar.Enabled = true;
                    }
                    else
                    {
                        statusLabel.Text = "Key salva é inválida.";
                        if (MessageBox.Show("Key inválida, deseja obter uma nova key?", 
                                            "Key Inválida", 
                                            MessageBoxButtons.YesNo, 
                                            MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Corrigido para abrir o link no navegador
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = $"/c start {Uri.EscapeUriString("https://roniere.discloud.app/webhook/67f26fd5-7411-42e1-9e85-b0a2676d46a0")}",
                                CreateNoWindow = true
                            });
                        }
                        btnVerificar.Enabled = true;
                        txtKey.Text = "";
                        File.Delete(configFile); // Remove a key inválida
                    }
                }
            }
            catch
            {
                statusLabel.Text = "Erro ao verificar key salva.";
                MessageBox.Show("Erro ao verificar key salva!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnVerificar.Enabled = true;
            }
        }

        private void ShowAFKControls()
        {
            txtKey.Enabled = false;
            btnVerificar.Text = "Ativar AFK Mode";
            btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))); // Verde
            btnVerificar.Enabled = true;
            btnExecutar.Enabled = true;
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            // Se já estiver autenticado, trata como botão de AFK
            if (!string.IsNullOrEmpty(validKey))
            {
                if (isAFKActive)
                {
                    DesativarAFK();
                }
                else
                {
                    ExecutarScript();
                }
                return;
            }

            // Caso contrário, procede com a verificação da key
            if (string.IsNullOrWhiteSpace(txtKey.Text))
            {
                statusLabel.Text = "Por favor, digite uma key!";
                return;
            }

            VerificarKey();
        }

        private async void VerificarKey()
        {
            try
            {
                btnVerificar.Enabled = false;
                btnVerificar.Text = "Executando...";
                statusLabel.Text = "Executando...";

                var response = await client.GetAsync($"https://roniere.discloud.app/webhook/67f26fd5-7411-42e1-9e85-b0a2676d46a0?key={txtKey.Text}");
                var content = await response.Content.ReadAsStringAsync();
                
                try
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                    {
                        int code = codeElement.GetInt32();
                        
                        if (code == 200)
                        {
                            validKey = txtKey.Text;
                            SaveKey(validKey);
                            statusLabel.Text = "Logado com sucesso!";
                            ShowAFKControls();
                            btnVerificar.Enabled = true;
                        }
                        else
                        {
                            statusLabel.Text = "Key inválida!";
                            MessageBox.Show("Key não encontrada, verifique se a key está correta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            btnVerificar.Enabled = true;
                            btnVerificar.Text = "Executar é Entrar";
                            btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204))))); // Azul
                        }
                    }
                }
                catch (Exception)
                {
                    HandleVerificationError("Erro ao processar resposta do servidor");
                }
            }
            catch (Exception ex)
            {
                HandleVerificationError($"Erro ao conectar com o servidor: {ex.Message}");
            }
        }

        private void HandleVerificationError(string message)
        {
            statusLabel.Text = message;
            MessageBox.Show(message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnVerificar.Enabled = true;
            btnVerificar.Text = "Executar é Entrar";
            btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204))))); // Azul
        }

        private async void ExecutarScript()
        {
            try
            {
                string basePath = Path.GetFullPath(@"C:\Users\e\Downloads\Nova pasta\akf-mode\control-afk-mode-vms\src");
                string scriptPath = Path.Combine(basePath, "index.js");
                string monitorPath = Path.Combine(basePath, "monitor_activity.bat");
                string installPath = Path.Combine(basePath, "install.bat"); // Caminho para o install.bat
                
                // Verificar se os arquivos existem
                if (!File.Exists(scriptPath))
                {
                    MessageBox.Show($"Arquivo index.js não encontrado em:\n{scriptPath}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnVerificar.Enabled = true;
                    return;
                }

                if (!File.Exists(monitorPath))
                {
                    MessageBox.Show($"Arquivo monitor_activity.bat não encontrado em:\n{monitorPath}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnVerificar.Enabled = true;
                    return;
                }

                btnVerificar.Enabled = false;
                statusLabel.Text = "Verificando Node.js...";

                // Verifica se o Node.js está instalado
                try 
                {
                    ProcessStartInfo checkNodeInfo = new ProcessStartInfo
                    {
                        FileName = "node",
                        Arguments = "-v",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var checkNodeProcess = Process.Start(checkNodeInfo))
                    {
                        if (checkNodeProcess == null)
                        {
                            throw new Exception("Não foi possível iniciar o processo Node.js");
                        }
                        await checkNodeProcess.WaitForExitAsync();
                        if (checkNodeProcess.ExitCode != 0)
                        {
                            throw new Exception("Node.js não está instalado corretamente");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Node.js não está instalado ou não está no PATH. Por favor, instale o Node.js e tente novamente.", 
                                      "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnVerificar.Enabled = true;
                    return;
                }

                // Verifica se os pacotes estão instalados
                if (!Directory.Exists(Path.Combine(basePath, "node_modules")))
                {
                    statusLabel.Text = "Instalando pacotes necessários...";
                    btnVerificar.Text = "Instalando...";
                    
                    ProcessStartInfo installInfo = new ProcessStartInfo
                    {
                        FileName = installPath,
                        WorkingDirectory = basePath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var installProcess = Process.Start(installInfo))
                    {
                        if (installProcess == null)
                        {
                            throw new Exception("Não foi possível iniciar o processo de instalação");
                        }

                        string output = await installProcess.StandardOutput.ReadToEndAsync();
                        string error = await installProcess.StandardError.ReadToEndAsync();
                        await installProcess.WaitForExitAsync();

                        if (installProcess.ExitCode != 0)
                        {
                            throw new Exception($"Erro ao instalar pacotes: {error}");
                        }
                    }

                }
                else
                {
                    // Mensagem de sucesso após a instalação
                }

                statusLabel.Text = "Iniciando AFK Mode...";

                // Inicia o servidor Node.js em segundo plano
                ProcessStartInfo startServerInfo = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = "index.js",
                    WorkingDirectory = basePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var serverProcess = Process.Start(startServerInfo);
                if (serverProcess != null)
                {
                    serverProcessId = serverProcess.Id;
                    
                    // Aguarda um pouco para ver se o processo não falha imediatamente
                    await Task.Delay(1000);
                    if (serverProcess.HasExited)
                    {
                        string error = await serverProcess.StandardError.ReadToEndAsync();
                        throw new Exception($"Servidor Node.js falhou ao iniciar: {error}");
                    }
                }
                else
                {
                    throw new Exception("Não foi possível iniciar o servidor Node.js");
                }

                // Inicia o monitor de atividade em segundo plano
                ProcessStartInfo monitorInfo = new ProcessStartInfo
                {
                    FileName = monitorPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = basePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var monitorProcess = Process.Start(monitorInfo);
                if (monitorProcess != null)
                {
                    monitorProcessId = monitorProcess.Id;
                    
                    // Aguarda um pouco para ver se o processo não falha imediatamente
                    await Task.Delay(1000);
                    if (monitorProcess.HasExited)
                    {
                        string error = await monitorProcess.StandardError.ReadToEndAsync();
                        throw new Exception($"Monitor falhou ao iniciar: {error}");
                    }
                }
                else
                {
                    throw new Exception("Não foi possível iniciar o monitor de atividade");
                }

                // Se chegou até aqui, tudo funcionou
                btnVerificar.Text = "Desativar AFK Mode";
                btnVerificar.BackColor = System.Drawing.Color.Red;
                isAFKActive = true;
                btnVerificar.Enabled = true;
                statusLabel.Text = "AFK Mode ativado com sucesso!";
            }
            catch (Exception ex)
            {
                // Tenta limpar processos que podem ter sido iniciados
                try
                {
                    if (serverProcessId.HasValue)
                    {
                        var process = Process.GetProcessById(serverProcessId.Value);
                        if (!process.HasExited) process.Kill();
                    }
                    if (monitorProcessId.HasValue)
                    {
                        var process = Process.GetProcessById(monitorProcessId.Value);
                        if (!process.HasExited) process.Kill();
                    }
                }
                catch { }

                statusLabel.Text = "Erro ao ativar AFK Mode";
                MessageBox.Show($"Erro detalhado ao ativar AFK Mode:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnVerificar.Enabled = true;
                btnVerificar.Text = "Ativar AFK Mode";
                btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            }
        }

        private void DesativarAFK()
        {
            try
            {
                statusLabel.Text = "Desativando AFK Mode...";
                btnVerificar.Enabled = false;

                // Finaliza os processos usando os IDs armazenados
                if (serverProcessId.HasValue)
                {
                    var serverProcess = Process.GetProcessById(serverProcessId.Value);
                    if (!serverProcess.HasExited)
                    {
                        serverProcess.Kill();
                    }
                }

                if (monitorProcessId.HasValue)
                {
                    var monitorProcess = Process.GetProcessById(monitorProcessId.Value);
                    if (!monitorProcess.HasExited)
                    {
                        monitorProcess.Kill();
                    }
                }

                isAFKActive = false;
                btnVerificar.Text = "Ativar AFK Mode";
                btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))); // Verde
                statusLabel.Text = "AFK Mode desativado com sucesso!";
                btnVerificar.Enabled = true;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Erro ao desativar AFK Mode";
                MessageBox.Show($"Erro ao desativar o AFK Mode: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnVerificar.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // Garantir que todos os processos sejam finalizados
            if (isAFKActive)
            {
                DesativarAFK();
            }
            
            KillProcessAndChildren();
            
            // Dispose do HttpClient
            if (client != null)
            {
                client.Dispose();
            }
        }

        private void KillProcessAndChildren()
        {
            try
            {
                // Mata todos os processos cmd.exe
                foreach (var process in Process.GetProcessesByName("cmd"))
                {
                    try {
                        process.Kill(true);
                    }
                    catch { }
                }

                // Mata todos os processos node.js e monitor_activity.bat
                var processesToKill = new[] { "node", "monitor_activity" };
                foreach (var processName in processesToKill)
                {
                    foreach (var process in Process.GetProcessesByName(processName))
                    {
                        try {
                            process.Kill(true);
                        }
                        catch { }
                    }
                }

                // Mata o processo batch principal se ainda estiver ativo
                if (batchProcess != null && !batchProcess.HasExited)
                {
                    batchProcess.Kill(true);
                    batchProcess = null;
                }

                // Mata o processo script se ainda estiver ativo
                if (scriptProcess != null && !scriptProcess.HasExited)
                {
                    scriptProcess.Kill(true);
                    scriptProcess = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar processos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnExecutar_Click(object sender, EventArgs e)
        {
            try
            {
                string batFilePath = Path.Combine(Directory.GetCurrentDirectory(), "lib2-preload-darkcloud.bat");
                
                // Verifica se o arquivo existe
                if (!File.Exists(batFilePath))
                {
                    MessageBox.Show($"Arquivo lib2-preload-darkcloud.bat não encontrado em:\n{batFilePath}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Executa o script em segundo plano
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = batFilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(startInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync(); // Aguarda a conclusão do processo
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show($"Erro ao executar o script:\n{error}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Script executado com sucesso:\n" + output, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar o script:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    class ConfigData
    {
        public string? Key { get; set; }
    }
}
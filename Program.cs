using SistemaIQC.Services;
using System;
using System.Windows.Forms;

namespace SistemaIQC
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Inicializa o banco de dados
                var dbService = new DatabaseService();
                dbService.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao inicializar o sistema.\n\n{ex.Message}\n\nVerifique o log em C:\\SistemaIQC\\Logs\\sistema.log",
                    "Erro de Inicialização",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Abre a tela principal
            Application.Run(new Forms.FrmPrincipal());
        }
    }
}
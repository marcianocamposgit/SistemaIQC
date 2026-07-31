using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace SistemaIQC.Services
{
    public class DatabaseService
    {
        // CAMINHO DOS DADOS (Nunca mude isso para a pasta do projeto)
        private readonly string _dataRoot = @"C:\SistemaIQC";
        private readonly string _dbPath;
        private readonly string _sqlScriptPath;
        private readonly string _logPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(_dataRoot, "Database", "iqc.db");
            _sqlScriptPath = Path.Combine(_dataRoot, "Database", "InitDatabase.sql");
            _logPath = Path.Combine(_dataRoot, "Logs", "sistema.log");

            Directory.CreateDirectory(Path.Combine(_dataRoot, "Database"));
            Directory.CreateDirectory(Path.Combine(_dataRoot, "Logs"));
        }

        public void InitializeDatabase()
        {
            try
            {
                Log("Iniciando verificação do banco de dados...");

                if (!File.Exists(_dbPath))
                {
                    File.Create(_dbPath).Dispose();
                    Log("Arquivo iqc.db criado.");
                }

                if (!File.Exists(_sqlScriptPath))
                {
                    throw new Exception($"Arquivo InitDatabase.sql não encontrado em: {_sqlScriptPath}");
                }

                string sql = File.ReadAllText(_sqlScriptPath);
                var commands = sql.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                using var connection = new SqliteConnection($"Data Source={_dbPath}");
                connection.Open();

                foreach (var cmdText in commands)
                {
                    string cleanCmd = cmdText.Trim();
                    if (string.IsNullOrWhiteSpace(cleanCmd)) continue;

                    using var command = new SqliteCommand(cleanCmd, connection);
                    command.ExecuteNonQuery();
                }

                Log("Script SQL executado com sucesso.");
                ValidarTabelas();
            }
            catch (Exception ex)
            {
                Log($"ERRO CRÍTICO: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private void ValidarTabelas()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            string query = "SELECT name FROM sqlite_master WHERE type='table' AND name='configuracoes';";
            using var command = new SqliteCommand(query, connection);
            var result = command.ExecuteScalar();

            if (result == null)
                throw new Exception("Falha na validação: A tabela 'configuracoes' não foi criada.");

            Log("Validação OK: Tabela 'configuracoes' encontrada no banco.");
        }

        private void Log(string mensagem)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensagem}{Environment.NewLine}";
            File.AppendAllText(_logPath, entry);
        }

        public SqliteConnection GetConnection() => new SqliteConnection($"Data Source={_dbPath}");

        // ===== MÉTODOS PARA OS REPOSITORIES =====
        public T ExecuteScalar<T>(string query, SqliteParameter[] parameters = null)
        {
            using var connection = GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            if (parameters != null) command.Parameters.AddRange(parameters);

            var result = command.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? (T)Convert.ChangeType(result, typeof(T))
                : default;
        }

        public int ExecuteNonQuery(string query, SqliteParameter[] parameters = null)
        {
            using var connection = GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            if (parameters != null) command.Parameters.AddRange(parameters);

            return command.ExecuteNonQuery();
        }
    }
}
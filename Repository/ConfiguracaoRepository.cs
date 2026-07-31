using Microsoft.Data.Sqlite;
using SistemaIQC.Models;
using SistemaIQC.Services;
using System.Collections.Generic;

namespace SistemaIQC.Repository
{
    public class ConfiguracaoRepository
    {
        private readonly DatabaseService _db;

        public ConfiguracaoRepository()
        {
            _db = new DatabaseService();
        }

        public string Buscar(string chave)
        {
            string query = "SELECT valor FROM configuracoes WHERE chave = @chave LIMIT 1";
            var parameters = new[] { new SqliteParameter("@chave", chave) };
            return _db.ExecuteScalar<string>(query, parameters);
        }

        public Dictionary<string, string> BuscarTodas()
        {
            var result = new Dictionary<string, string>();
            string query = "SELECT chave, valor FROM configuracoes";

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result[reader.GetString(0)] = reader.GetString(1);
            }

            return result;
        }
    }
}
using Microsoft.Data.Sqlite;
using SistemaIQC.Models;
using SistemaIQC.Services;
using System.Collections.Generic;

namespace SistemaIQC.Repository
{
    public class InspetorRepository
    {
        private readonly DatabaseService _db;

        public InspetorRepository()
        {
            _db = new DatabaseService();
        }

        public List<Inspetor> BuscarTodos()
        {
            var result = new List<Inspetor>();
            string query = "SELECT id, nome, ativo, data_cadastro FROM inspetores WHERE ativo = 1 ORDER BY nome";

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Inspetor
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Ativo = reader.GetInt32(2) == 1,
                    DataCadastro = reader.GetString(3)
                });
            }

            return result;
        }

        public void Inserir(string nome)
        {
            string query = "INSERT INTO inspetores (nome) VALUES (@nome)";
            var parameters = new[] { new SqliteParameter("@nome", nome) };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Atualizar(int id, string nome)
        {
            string query = "UPDATE inspetores SET nome = @nome WHERE id = @id";
            var parameters = new[]
            {
                new SqliteParameter("@id", id),
                new SqliteParameter("@nome", nome)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Desativar(int id)
        {
            string query = "UPDATE inspetores SET ativo = 0 WHERE id = @id";
            var parameters = new[] { new SqliteParameter("@id", id) };
            _db.ExecuteNonQuery(query, parameters);
        }
    }
}
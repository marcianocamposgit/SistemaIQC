using Microsoft.Data.Sqlite;
using SistemaIQC.Models;
using SistemaIQC.Services;
using System;
using System.Collections.Generic;

namespace SistemaIQC.Repository
{
    public class FornecedorRepository
    {
        private readonly DatabaseService _db;

        public FornecedorRepository()
        {
            _db = new DatabaseService();
        }

        public List<Fornecedor> BuscarTodos()
        {
            var result = new List<Fornecedor>();
            string query = "SELECT id, nome FROM fornecedores ORDER BY nome";

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Fornecedor
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                });
            }

            return result;
        }

        public Fornecedor BuscarPorId(int id)
        {
            string query = "SELECT id, nome FROM fornecedores WHERE id = @id";
            var parameters = new[] { new SqliteParameter("@id", id) };

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Fornecedor
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                };
            }

            return null;
        }

        public void Inserir(Fornecedor fornecedor)
        {
            string query = "INSERT INTO fornecedores (nome) VALUES (@nome)";
            var parameters = new[]
            {
                new SqliteParameter("@nome", fornecedor.Nome)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Atualizar(Fornecedor fornecedor)
        {
            string query = "UPDATE fornecedores SET nome = @nome WHERE id = @id";
            var parameters = new[]
            {
                new SqliteParameter("@id", fornecedor.Id),
                new SqliteParameter("@nome", fornecedor.Nome)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Excluir(int id)
        {
            string query = "DELETE FROM fornecedores WHERE id = @id";
            var parameters = new[] { new SqliteParameter("@id", id) };
            _db.ExecuteNonQuery(query, parameters);
        }
    }
}
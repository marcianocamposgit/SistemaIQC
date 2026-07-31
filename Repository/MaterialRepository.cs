using Microsoft.Data.Sqlite;
using SistemaIQC.Models;
using SistemaIQC.Services;
using System;
using System.Collections.Generic;

namespace SistemaIQC.Repository
{
    public class MaterialRepository
    {
        private readonly DatabaseService _db;

        public MaterialRepository()
        {
            _db = new DatabaseService();
        }

        public List<Material> BuscarTodos()
        {
            var result = new List<Material>();
            string query = "SELECT id, codigo, descricao, modelo, fornecedor_id FROM materiais ORDER BY codigo";

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Material
                {
                    Id = reader.GetInt32(0),
                    Codigo = reader.GetString(1),
                    Descricao = reader.GetString(2),
                    Modelo = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FornecedorId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                });
            }
            return result;
        }

        public Material BuscarPorId(int id)
        {
            string query = "SELECT id, codigo, descricao, modelo, fornecedor_id FROM materiais WHERE id = @id";
            var parameters = new[] { new SqliteParameter("@id", id) };

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Material
                {
                    Id = reader.GetInt32(0),
                    Codigo = reader.GetString(1),
                    Descricao = reader.GetString(2),
                    Modelo = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FornecedorId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                };
            }
            return null;
        }

        public List<Material> BuscarPorFornecedor(int fornecedorId)
        {
            var result = new List<Material>();
            string query = "SELECT id, codigo, descricao, modelo, fornecedor_id FROM materiais WHERE fornecedor_id = @fornecedorId ORDER BY codigo";
            var parameters = new[] { new SqliteParameter("@fornecedorId", fornecedorId) };

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Material
                {
                    Id = reader.GetInt32(0),
                    Codigo = reader.GetString(1),
                    Descricao = reader.GetString(2),
                    Modelo = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FornecedorId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                });
            }
            return result;
        }

        public void Inserir(Material material)
        {
            string query = @"INSERT INTO materiais (codigo, descricao, modelo, fornecedor_id) 
                             VALUES (@codigo, @descricao, @modelo, @fornecedor_id)";
            var parameters = new[]
            {
                new SqliteParameter("@codigo", material.Codigo),
                new SqliteParameter("@descricao", material.Descricao),
                new SqliteParameter("@modelo", material.Modelo ?? (object)DBNull.Value),
                new SqliteParameter("@fornecedor_id", material.FornecedorId ?? (object)DBNull.Value)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Atualizar(Material material)
        {
            string query = @"UPDATE materiais 
                             SET codigo = @codigo, descricao = @descricao, modelo = @modelo, fornecedor_id = @fornecedor_id 
                             WHERE id = @id";
            var parameters = new[]
            {
                new SqliteParameter("@id", material.Id),
                new SqliteParameter("@codigo", material.Codigo),
                new SqliteParameter("@descricao", material.Descricao),
                new SqliteParameter("@modelo", material.Modelo ?? (object)DBNull.Value),
                new SqliteParameter("@fornecedor_id", material.FornecedorId ?? (object)DBNull.Value)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        public void Excluir(int id)
        {
            string query = "DELETE FROM materiais WHERE id = @id";
            var parameters = new[] { new SqliteParameter("@id", id) };
            _db.ExecuteNonQuery(query, parameters);
        }
    }
}
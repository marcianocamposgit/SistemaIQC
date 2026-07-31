using Microsoft.Data.Sqlite;
using SistemaIQC.Models;
using SistemaIQC.Services;
using System;
using System.Collections.Generic;

namespace SistemaIQC.Repository
{
    public class InspecaoRepository
    {
        private readonly DatabaseService _db;

        public InspecaoRepository()
        {
            _db = new DatabaseService();
        }

        public int Salvar(Inspecao inspecao)
        {
            string query = @"
                INSERT INTO inspecoes (
                    data, inspetor, fornecedor_id, material_id, nf, lote, quantidade,
                    plano, regime, nivel, nqa, codigo_letra, amostra, ac, re,
                    defeitos, resultado, observacao, acao_imediata
                ) VALUES (
                    @data, @inspetor, @fornecedor_id, @material_id, @nf, @lote, @quantidade,
                    @plano, @regime, @nivel, @nqa, @codigo_letra, @amostra, @ac, @re,
                    @defeitos, @resultado, @observacao, @acao_imediata
                );
                SELECT last_insert_rowid();";

            var parameters = new[]
            {
                new SqliteParameter("@data", inspecao.Data.ToString("yyyy-MM-dd HH:mm:ss")),
                new SqliteParameter("@inspetor", inspecao.Inspetor ?? (object)DBNull.Value),
                new SqliteParameter("@fornecedor_id", inspecao.FornecedorId),
                new SqliteParameter("@material_id", inspecao.MaterialId),
                new SqliteParameter("@nf", inspecao.NF ?? (object)DBNull.Value),
                new SqliteParameter("@lote", inspecao.Lote ?? (object)DBNull.Value),
                new SqliteParameter("@quantidade", inspecao.Quantidade),
                new SqliteParameter("@plano", inspecao.Plano),
                new SqliteParameter("@regime", inspecao.Regime),
                new SqliteParameter("@nivel", inspecao.Nivel),
                new SqliteParameter("@nqa", inspecao.NQA),
                new SqliteParameter("@codigo_letra", inspecao.CodigoLetra),
                new SqliteParameter("@amostra", inspecao.Amostra),
                new SqliteParameter("@ac", inspecao.Ac),
                new SqliteParameter("@re", inspecao.Re),
                new SqliteParameter("@defeitos", inspecao.Defeitos),
                new SqliteParameter("@resultado", inspecao.Resultado),
                new SqliteParameter("@observacao", inspecao.Observacao ?? (object)DBNull.Value),
                new SqliteParameter("@acao_imediata", inspecao.AcaoImediata ?? (object)DBNull.Value)
            };

            return _db.ExecuteScalar<int>(query, parameters);
        }

        public List<Inspecao> BuscarComFiltros(
            int? fornecedorId = null,
            int? materialId = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string resultado = null)
        {
            var result = new List<Inspecao>();
            var parameters = new List<SqliteParameter>();
            var conditions = new List<string>();

            string query = @"
                SELECT 
                    i.id, i.data, i.inspetor, i.fornecedor_id, i.material_id,
                    i.nf, i.lote, i.quantidade, i.plano, i.regime, i.nivel,
                    i.nqa, i.codigo_letra, i.amostra, i.ac, i.re,
                    i.defeitos, i.resultado, i.observacao, i.acao_imediata,
                    f.nome as fornecedor_nome,
                    m.codigo as material_codigo,
                    m.descricao as material_descricao,
                    m.modelo as material_modelo
                FROM inspecoes i
                LEFT JOIN fornecedores f ON i.fornecedor_id = f.id
                LEFT JOIN materiais m ON i.material_id = m.id
                WHERE 1=1
            ";

            if (fornecedorId.HasValue && fornecedorId.Value > 0)
            {
                conditions.Add("i.fornecedor_id = @fornecedorId");
                parameters.Add(new SqliteParameter("@fornecedorId", fornecedorId.Value));
            }

            if (materialId.HasValue && materialId.Value > 0)
            {
                conditions.Add("i.material_id = @materialId");
                parameters.Add(new SqliteParameter("@materialId", materialId.Value));
            }

            if (dataInicio.HasValue)
            {
                conditions.Add("date(i.data) >= date(@dataInicio)");
                parameters.Add(new SqliteParameter("@dataInicio", dataInicio.Value.ToString("yyyy-MM-dd")));
            }

            if (dataFim.HasValue)
            {
                conditions.Add("date(i.data) <= date(@dataFim)");
                parameters.Add(new SqliteParameter("@dataFim", dataFim.Value.ToString("yyyy-MM-dd")));
            }

            if (!string.IsNullOrEmpty(resultado))
            {
                conditions.Add("i.resultado = @resultado");
                parameters.Add(new SqliteParameter("@resultado", resultado));
            }

            if (conditions.Count > 0)
            {
                query += " AND " + string.Join(" AND ", conditions);
            }

            query += " ORDER BY i.data DESC, i.id DESC";

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new Inspecao
                {
                    Id = reader.GetInt32(0),
                    Data = reader.GetDateTime(1),
                    Inspetor = reader.IsDBNull(2) ? null : reader.GetString(2),
                    FornecedorId = reader.GetInt32(3),
                    MaterialId = reader.GetInt32(4),
                    NF = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Lote = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Quantidade = reader.GetInt32(7),
                    Plano = reader.GetString(8),
                    Regime = reader.GetString(9),
                    Nivel = reader.GetString(10),
                    NQA = reader.GetDouble(11),
                    CodigoLetra = reader.GetString(12),
                    Amostra = reader.GetInt32(13),
                    Ac = reader.GetInt32(14),
                    Re = reader.GetInt32(15),
                    Defeitos = reader.GetInt32(16),
                    Resultado = reader.GetString(17),
                    Observacao = reader.IsDBNull(18) ? null : reader.GetString(18),
                    AcaoImediata = reader.IsDBNull(19) ? null : reader.GetString(19),
                    // ===== DADOS DO LEFT JOIN (índices 20 a 23) =====
                    FornecedorNome = reader.IsDBNull(20) ? null : reader.GetString(20),
                    MaterialCodigo = reader.IsDBNull(21) ? null : reader.GetString(21),
                    MaterialDescricao = reader.IsDBNull(22) ? null : reader.GetString(22),
                    MaterialModelo = reader.IsDBNull(23) ? null : reader.GetString(23)
                });
            }

            return result;
        }
    }
}
using Microsoft.Data.Sqlite;
using SistemaIQC.Models.NBR5426;
using SistemaIQC.Services;

namespace SistemaIQC.Repository
{
    public class NBR5426Repository
    {
        private readonly DatabaseService _db;

        public NBR5426Repository()
        {
            _db = new DatabaseService();
        }

        public string BuscarCodigoLetra(int quantidade, string nivel)
        {
            string query = @"
                SELECT codigo 
                FROM codigos_letra 
                WHERE nivel = @nivel 
                AND @quantidade BETWEEN lote_min AND lote_max
                LIMIT 1";

            var parameters = new[]
            {
                new SqliteParameter("@nivel", nivel),
                new SqliteParameter("@quantidade", quantidade)
            };

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }

        public PlanoAmostragem BuscarPlanoAmostragem(string codigo, string plano, string regime, double nqa)
        {
            string query = @"
                SELECT id, plano, regime, codigo, nqa, amostra, ac, re
                FROM planos_amostragem 
                WHERE codigo = @codigo 
                AND plano = @plano 
                AND regime = @regime 
                AND nqa = @nqa
                LIMIT 1";

            var parameters = new[]
            {
                new SqliteParameter("@codigo", codigo),
                new SqliteParameter("@plano", plano),
                new SqliteParameter("@regime", regime),
                new SqliteParameter("@nqa", nqa)
            };

            using var connection = _db.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddRange(parameters);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new PlanoAmostragem
                {
                    Id = reader.GetInt32(0),
                    Plano = reader.GetString(1),
                    Regime = reader.GetString(2),
                    Codigo = reader.GetString(3),
                    NQA = reader.GetDouble(4),
                    Amostra = reader.GetInt32(5),
                    Ac = reader.GetInt32(6),
                    Re = reader.GetInt32(7)
                };
            }

            return null;
        }
    }
}
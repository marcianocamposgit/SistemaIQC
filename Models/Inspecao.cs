using System;

namespace SistemaIQC.Models
{
    public class Inspecao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Inspetor { get; set; }
        public int FornecedorId { get; set; }
        public int MaterialId { get; set; }
        public string NF { get; set; }
        public string Lote { get; set; }
        public int Quantidade { get; set; }
        public string Plano { get; set; }
        public string Regime { get; set; }
        public string Nivel { get; set; }
        public double NQA { get; set; }
        public string CodigoLetra { get; set; }
        public int Amostra { get; set; }
        public int Ac { get; set; }
        public int Re { get; set; }
        public int Defeitos { get; set; }
        public string Resultado { get; set; }
        public string Observacao { get; set; }
        public string AcaoImediata { get; set; }

        // ===== PROPRIEDADES APENAS PARA EXIBIÇÃO NO HISTÓRICO =====
        // (preenchidas pelo LEFT JOIN na consulta)
        public string FornecedorNome { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialDescricao { get; set; }
        public string MaterialModelo { get; set; }
    }
}
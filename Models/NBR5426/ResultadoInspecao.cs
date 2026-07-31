namespace SistemaIQC.Models.NBR5426
{
    public class ResultadoInspecao
    {
        public string CodigoLetra { get; set; }
        public int Amostra { get; set; }
        public int Ac { get; set; }
        public int Re { get; set; }
        public string Resultado { get; set; }
        public bool IsAprovado => Resultado == "APROVADO";
    }
}
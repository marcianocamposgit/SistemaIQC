namespace SistemaIQC.Models.NBR5426
{
    public class PlanoAmostragem
    {
        public int Id { get; set; }
        public string Plano { get; set; }
        public string Regime { get; set; }
        public string Codigo { get; set; }
        public double NQA { get; set; }
        public int Amostra { get; set; }
        public int Ac { get; set; }
        public int Re { get; set; }
    }
}
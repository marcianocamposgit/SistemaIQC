namespace SistemaIQC.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Modelo { get; set; }
        public int? FornecedorId { get; set; }  // ← NOVO

        public override string ToString()
        {
            return $"{Codigo} - {Descricao}";
        }
    }
}
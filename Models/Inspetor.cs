namespace SistemaIQC.Models
{
    public class Inspetor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public string DataCadastro { get; set; }

        public override string ToString() => Nome;
    }
}
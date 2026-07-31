namespace SistemaIQC.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        // Foco principal conforme formulário FOR.CQ-002
        public string Nome { get; set; }

        // O ToString é o que fará o combo exibir apenas o nome de forma bonita
        public override string ToString() => Nome;
    }
}
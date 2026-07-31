using SistemaIQC.Models;
using SistemaIQC.Repository;
using System;

namespace SistemaIQC.Services
{
    public class InspecaoService
    {
        private readonly InspecaoRepository _repository;

        public InspecaoService()
        {
            _repository = new InspecaoRepository();
        }

        public int Salvar(Inspecao inspecao)
        {
            // Validações básicas
            if (inspecao.FornecedorId <= 0)
                throw new Exception("Selecione um fornecedor.");
            if (inspecao.MaterialId <= 0)
                throw new Exception("Selecione um material.");
            if (inspecao.Quantidade <= 0)
                throw new Exception("A quantidade deve ser maior que zero.");
            if (inspecao.Defeitos < 0)
                throw new Exception("A quantidade de defeitos não pode ser negativa.");
            if (string.IsNullOrEmpty(inspecao.Resultado))
                throw new Exception("O resultado não foi calculado.");

            inspecao.Data = DateTime.Now;
            return _repository.Salvar(inspecao);
        }
    }
}
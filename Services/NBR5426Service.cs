using SistemaIQC.Models.NBR5426;
using SistemaIQC.Repository;
using System;

namespace SistemaIQC.Services
{
    public class NBR5426Service
    {
        private readonly NBR5426Repository _repository;

        public NBR5426Service()
        {
            _repository = new NBR5426Repository();
        }

        public ResultadoInspecao Calcular(int quantidade, string nivel, string plano, string regime, double nqa)
        {
            var resultado = new ResultadoInspecao();

            string codigo = _repository.BuscarCodigoLetra(quantidade, nivel);
            if (string.IsNullOrEmpty(codigo))
            {
                throw new Exception($"Nenhum código encontrado para quantidade {quantidade} e nível {nivel}");
            }
            resultado.CodigoLetra = codigo;

            var planoEncontrado = _repository.BuscarPlanoAmostragem(codigo, plano, regime, nqa);
            if (planoEncontrado == null)
            {
                throw new Exception($"Nenhum plano encontrado para código {codigo}, plano {plano}, regime {regime}, NQA {nqa}");
            }

            resultado.Amostra = planoEncontrado.Amostra;
            resultado.Ac = planoEncontrado.Ac;
            resultado.Re = planoEncontrado.Re;
            resultado.Resultado = "EM ANÁLISE";

            return resultado;
        }

        public string DecidirResultado(int defeitos, int ac, int re, string plano)
        {
            if (plano == "Simples")
            {
                if (defeitos <= ac)
                    return "APROVADO";
                else
                    return "REPROVADO";
            }
            return "EM ANÁLISE";
        }
    }
}
using OfficeOpenXml;
using SistemaIQC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SistemaIQC.Services
{
    public class RelatorioService
    {
        public RelatorioService()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public byte[] GerarRelatorioExcel(
            List<Inspecao> inspecoes,
            DateTime dataInicio,
            DateTime dataFim,
            string fornecedorFiltro = null,
            string materialFiltro = null)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Relatório IQC");

            // ===== CABEÇALHO DO RELATÓRIO =====
            worksheet.Cells[1, 1].Value = "RELATÓRIO DE INSPEÇÃO DE ENTRADA - RECEBIMENTO";
            worksheet.Cells[1, 1, 1, 17].Merge = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1].Value = "FOR.CQ-002";
            worksheet.Cells[2, 1, 2, 17].Merge = true;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, 1].Value = $"Data: {DateTime.Now:dd/MM/yyyy}";
            worksheet.Cells[3, 1, 3, 17].Merge = true;
            worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            int rowInfo = 5;
            if (!string.IsNullOrEmpty(fornecedorFiltro) && fornecedorFiltro != "Todos")
            {
                worksheet.Cells[rowInfo, 1].Value = $"Fornecedor: {fornecedorFiltro}";
                worksheet.Cells[rowInfo, 1, rowInfo, 17].Merge = true;
                rowInfo++;
            }
            if (!string.IsNullOrEmpty(materialFiltro) && materialFiltro != "Todos")
            {
                worksheet.Cells[rowInfo, 1].Value = $"Material: {materialFiltro}";
                worksheet.Cells[rowInfo, 1, rowInfo, 17].Merge = true;
                rowInfo++;
            }
            worksheet.Cells[rowInfo, 1].Value = $"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}";
            worksheet.Cells[rowInfo, 1, rowInfo, 17].Merge = true;

            // ===== LINHA DE TÍTULOS =====
            int row = rowInfo + 2;
            string[] headers = {
                "DATA INSPEÇÃO", "INSPETOR (A)", "CÓD. DO PRODUTO", "PRODUTO", "MODELO",
                "FORNECEDOR", "NF (Nota Fiscal)", "lote / PO", "QTD. LOTE",
                "QTD. REVISADA", "QTD. APROVADA", "% APROVADA",
                "QTD. REPROVADA", "% REPROVADA", "Status do Lote",
                "DESCRIÇÃO DA NC", "AÇÃO IMEDIATA"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[row, i + 1].Value = headers[i];
                worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                worksheet.Cells[row, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[row, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            // ===== DADOS =====
            row++;

            foreach (var item in inspecoes)
            {
                int aprovados = item.Amostra - item.Defeitos;
                double pctAprovados = item.Amostra > 0 ? (double)aprovados / item.Amostra * 100 : 0;
                double pctReprovados = item.Amostra > 0 ? (double)item.Defeitos / item.Amostra * 100 : 0;

                worksheet.Cells[row, 1].Value = item.Data.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = item.Inspetor;
                worksheet.Cells[row, 3].Value = item.MaterialCodigo ?? "N/A";
                worksheet.Cells[row, 4].Value = item.MaterialDescricao ?? "N/A";
                worksheet.Cells[row, 5].Value = item.MaterialModelo ?? "N/A";
                worksheet.Cells[row, 6].Value = item.FornecedorNome ?? "N/A";
                worksheet.Cells[row, 7].Value = item.NF;
                worksheet.Cells[row, 8].Value = item.Lote;
                worksheet.Cells[row, 9].Value = item.Quantidade;
                worksheet.Cells[row, 10].Value = item.Amostra;
                worksheet.Cells[row, 11].Value = aprovados;
                worksheet.Cells[row, 12].Value = Math.Round(pctAprovados, 2);
                worksheet.Cells[row, 13].Value = item.Defeitos;
                worksheet.Cells[row, 14].Value = Math.Round(pctReprovados, 2);
                worksheet.Cells[row, 15].Value = item.Resultado;
                worksheet.Cells[row, 16].Value = item.Observacao;
                worksheet.Cells[row, 17].Value = item.AcaoImediata;

                // Cor do resultado
                if (item.Resultado == "APROVADO")
                    worksheet.Cells[row, 15].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                else if (item.Resultado == "REPROVADO")
                    worksheet.Cells[row, 15].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                // Aplica bordas
                for (int col = 1; col <= 17; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }

                row++;
            }

            // ===== TOTAIS =====
            if (inspecoes.Any())
            {
                var totalLote = inspecoes.Sum(i => i.Quantidade);
                var totalAmostra = inspecoes.Sum(i => i.Amostra);
                var totalAprovados = inspecoes.Sum(i => i.Amostra - i.Defeitos);
                var totalReprovados = inspecoes.Sum(i => i.Defeitos);
                double pctTotalAprovados = totalAmostra > 0 ? (double)totalAprovados / totalAmostra * 100 : 0;
                double pctTotalReprovados = totalAmostra > 0 ? (double)totalReprovados / totalAmostra * 100 : 0;

                worksheet.Cells[row, 1].Value = "TOTAIS:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row, 8].Merge = true;
                worksheet.Cells[row, 9].Value = totalLote;
                worksheet.Cells[row, 10].Value = totalAmostra;
                worksheet.Cells[row, 11].Value = totalAprovados;
                worksheet.Cells[row, 12].Value = Math.Round(pctTotalAprovados, 2);
                worksheet.Cells[row, 13].Value = totalReprovados;
                worksheet.Cells[row, 14].Value = Math.Round(pctTotalReprovados, 2);

                for (int col = 9; col <= 14; col++)
                {
                    worksheet.Cells[row, col].Style.Font.Bold = true;
                    worksheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            // ===== AJUSTAR LARGURA DAS COLUNAS =====
            worksheet.Cells[1, 1, row, 17].AutoFitColumns();

            // ===== RETORNAR ARQUIVO =====
            return package.GetAsByteArray();
        }
    }
}
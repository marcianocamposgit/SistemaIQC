@"
# 📦 Sistema IQC - Controle de Qualidade

Sistema para controle de inspeção de entrada conforme **NBR 5426**, desenvolvido em C# com Windows Forms e SQLite.

---

## 🎯 Objetivo

Substituir a planilha Excel, automatizar os cálculos da NBR 5426 e manter o histórico das inspeções.

---

## 🚀 Funcionalidades

### ✅ Tela Principal
- Cadastro de **Fornecedores**
- Cadastro de **Materiais** (Código, Descrição, Modelo)
- Cadastro de **Inspetores**
- Cálculo automático da **NBR 5426**
- Registro de defeitos e decisão (Aprovado/Reprovado)
- Salvamento de inspeções

### ✅ Histórico
- Lista todas as inspeções salvas
- Filtros por **Fornecedor**, **Material**, **Data** e **Resultado**
- Exportação para **Excel** (formato FOR.CQ-002)
- Visualização detalhada de cada inspeção

### ✅ Relatórios
- Exportação Excel com a estrutura do formulário FOR.CQ-002
- Totais consolidados

---

## 📋 Parâmetros NBR 5426

| Parâmetro | Valor |
|-----------|-------|
| Nível de Inspeção | II |
| NQA (AQL) | 0,65 |
| Plano de Amostragem | Simples |
| Regime | Normal / Rigorosa |

---

## 🛠️ Tecnologias

- **C# .NET 8.0**
- **Windows Forms**
- **SQLite**
- **EPPlus** (exportação Excel)

---

## 📂 Estrutura do Projeto

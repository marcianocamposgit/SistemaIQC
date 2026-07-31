using SistemaIQC.Models;
using SistemaIQC.Repository;
using SistemaIQC.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SistemaIQC.Forms
{
    public class FrmHistorico : Form
    {
        private readonly InspecaoRepository _inspecaoRepo;
        private readonly FornecedorRepository _fornecedorRepo;
        private readonly MaterialRepository _materialRepo;

        private List<Inspecao> _inspecoes;

        private ComboBox cmbFornecedor;
        private ComboBox cmbMaterial;
        private ComboBox cmbResultado;
        private DateTimePicker dtpDataInicio;
        private DateTimePicker dtpDataFim;
        private DataGridView dgvHistorico;
        private Button btnPesquisar;
        private Button btnLimparFiltros;
        private Button btnExportarExcel;
        private Button btnFechar;
        private Label lblTotalRegistros;

        public FrmHistorico()
        {
            _inspecaoRepo = new InspecaoRepository();
            _fornecedorRepo = new FornecedorRepository();
            _materialRepo = new MaterialRepository();

            InitializeComponent();
            ConfigurarControles();
            CarregarCombos();
            CarregarDados();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Histórico de Inspeções";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;

            var lblTitulo = new Label { Text = "📋 Histórico de Inspeções", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(400, 40) };
            this.Controls.Add(lblTitulo);

            var gbFiltros = new GroupBox { Text = "🔍 Filtros", Location = new Point(20, 70), Size = new Size(950, 130) };
            this.Controls.Add(gbFiltros);

            int dy = 25, dx = 15, labelWidth = 80, campoWidth = 180;

            // Fornecedor
            gbFiltros.Controls.Add(new Label { Text = "Fornecedor:", Location = new Point(dx, dy), Size = new Size(labelWidth, 25) });
            cmbFornecedor = new ComboBox { Location = new Point(dx + labelWidth + 5, dy), Size = new Size(campoWidth, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            gbFiltros.Controls.Add(cmbFornecedor);

            // Material
            gbFiltros.Controls.Add(new Label { Text = "Material:", Location = new Point(dx + labelWidth + campoWidth + 30, dy), Size = new Size(labelWidth, 25) });
            cmbMaterial = new ComboBox { Location = new Point(dx + labelWidth + campoWidth + 30 + labelWidth + 5, dy), Size = new Size(campoWidth, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            gbFiltros.Controls.Add(cmbMaterial);

            dy += 35;

            // Data Início
            gbFiltros.Controls.Add(new Label { Text = "Data Início:", Location = new Point(dx, dy), Size = new Size(labelWidth, 25) });
            dtpDataInicio = new DateTimePicker { Location = new Point(dx + labelWidth + 5, dy), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            gbFiltros.Controls.Add(dtpDataInicio);

            // Data Fim
            gbFiltros.Controls.Add(new Label { Text = "Data Fim:", Location = new Point(dx + labelWidth + 160, dy), Size = new Size(labelWidth, 25) });
            dtpDataFim = new DateTimePicker { Location = new Point(dx + labelWidth + 160 + labelWidth + 5, dy), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            gbFiltros.Controls.Add(dtpDataFim);

            // Resultado
            gbFiltros.Controls.Add(new Label { Text = "Resultado:", Location = new Point(dx + labelWidth + 320, dy), Size = new Size(labelWidth, 25) });
            cmbResultado = new ComboBox { Location = new Point(dx + labelWidth + 320 + labelWidth + 5, dy), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            gbFiltros.Controls.Add(cmbResultado);

            // Botões de Filtro
            btnPesquisar = new Button { Text = "🔍 Pesquisar", Location = new Point(20, 210), Size = new Size(120, 35), BackColor = Color.LightBlue };
            btnPesquisar.Click += BtnPesquisar_Click;
            this.Controls.Add(btnPesquisar);

            btnLimparFiltros = new Button { Text = "🧹 Limpar Filtros", Location = new Point(155, 210), Size = new Size(120, 35), BackColor = Color.LightYellow };
            btnLimparFiltros.Click += BtnLimparFiltros_Click;
            this.Controls.Add(btnLimparFiltros);

            // DataGridView
            dgvHistorico = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(950, 300),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            this.Controls.Add(dgvHistorico);

            // Rodapé
            lblTotalRegistros = new Label { Text = "Total de registros: 0", Location = new Point(20, 570), Size = new Size(300, 25), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(lblTotalRegistros);

            // ===== BOTÃO EXPORTAR EXCEL =====
            btnExportarExcel = new Button { Text = "📊 Exportar Excel", Location = new Point(700, 565), Size = new Size(130, 35), BackColor = Color.LightGreen };
            btnExportarExcel.Click += BtnExportarExcel_Click;
            this.Controls.Add(btnExportarExcel);

            // ===== BOTÃO FECHAR =====
            btnFechar = new Button { Text = "❌ Fechar", Location = new Point(850, 565), Size = new Size(120, 35), BackColor = Color.LightGray };
            btnFechar.Click += (s, e) => this.Close();
            this.Controls.Add(btnFechar);
        }

        private void ConfigurarControles()
        {
            dgvHistorico.ColumnCount = 12;
            dgvHistorico.Columns[0].Name = "Id";
            dgvHistorico.Columns[0].Visible = false;
            dgvHistorico.Columns[1].Name = "Data";
            dgvHistorico.Columns[1].Width = 90;
            dgvHistorico.Columns[2].Name = "Inspetor";
            dgvHistorico.Columns[2].Width = 100;
            dgvHistorico.Columns[3].Name = "Fornecedor";
            dgvHistorico.Columns[3].Width = 120;
            dgvHistorico.Columns[4].Name = "Código";
            dgvHistorico.Columns[4].Width = 100;
            dgvHistorico.Columns[5].Name = "Descrição";
            dgvHistorico.Columns[5].Width = 150;
            dgvHistorico.Columns[6].Name = "Modelo";
            dgvHistorico.Columns[6].Width = 100;
            dgvHistorico.Columns[7].Name = "NF";
            dgvHistorico.Columns[7].Width = 80;
            dgvHistorico.Columns[8].Name = "Lote";
            dgvHistorico.Columns[8].Width = 80;
            dgvHistorico.Columns[9].Name = "QTD.Lote";
            dgvHistorico.Columns[9].Width = 70;
            dgvHistorico.Columns[10].Name = "Amostra";
            dgvHistorico.Columns[10].Width = 70;
            dgvHistorico.Columns[11].Name = "Resultado";
            dgvHistorico.Columns[11].Width = 90;
        }

        private void CarregarCombos()
        {
            var listaFornecedores = new List<KeyValuePair<int, string>>();
            listaFornecedores.Add(new KeyValuePair<int, string>(0, "Todos"));
            foreach (var f in _fornecedorRepo.BuscarTodos())
                listaFornecedores.Add(new KeyValuePair<int, string>(f.Id, f.Nome));
            cmbFornecedor.DataSource = listaFornecedores;
            cmbFornecedor.DisplayMember = "Value";
            cmbFornecedor.ValueMember = "Key";

            var listaMateriais = new List<KeyValuePair<int, string>>();
            listaMateriais.Add(new KeyValuePair<int, string>(0, "Todos"));
            foreach (var m in _materialRepo.BuscarTodos())
                listaMateriais.Add(new KeyValuePair<int, string>(m.Id, $"{m.Codigo} - {m.Descricao}"));
            cmbMaterial.DataSource = listaMateriais;
            cmbMaterial.DisplayMember = "Value";
            cmbMaterial.ValueMember = "Key";

            cmbResultado.Items.AddRange(new object[] { "Todos", "APROVADO", "REPROVADO" });
            cmbResultado.SelectedIndex = 0;

            dtpDataInicio.Value = DateTime.Now.AddDays(-30);
            dtpDataFim.Value = DateTime.Now;
        }

        private void CarregarDados()
        {
            int? fornecedorId = cmbFornecedor.SelectedValue is int id && id > 0 ? id : (int?)null;
            int? materialId = cmbMaterial.SelectedValue is int mid && mid > 0 ? mid : (int?)null;
            string resultado = cmbResultado.SelectedItem?.ToString() == "Todos" ? null : cmbResultado.SelectedItem?.ToString();

            _inspecoes = _inspecaoRepo.BuscarComFiltros(fornecedorId, materialId, dtpDataInicio.Value, dtpDataFim.Value, resultado);
            dgvHistorico.Rows.Clear();

            foreach (var item in _inspecoes)
            {
                int rowIndex = dgvHistorico.Rows.Add(
                    item.Id,
                    item.Data.ToString("dd/MM/yyyy"),
                    item.Inspetor ?? "",
                    item.FornecedorNome ?? "N/A",
                    item.MaterialCodigo ?? "N/A",
                    item.MaterialDescricao ?? "N/A",
                    item.MaterialModelo ?? "N/A",
                    item.NF ?? "",
                    item.Lote ?? "",
                    item.Quantidade,
                    item.Amostra,
                    item.Resultado ?? ""
                );

                if (item.Resultado == "APROVADO")
                    dgvHistorico.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Green;
                else if (item.Resultado == "REPROVADO")
                    dgvHistorico.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
            }

            lblTotalRegistros.Text = $"Total de registros: {_inspecoes.Count}";
        }

        private void BtnPesquisar_Click(object sender, EventArgs e) => CarregarDados();

        private void BtnLimparFiltros_Click(object sender, EventArgs e)
        {
            cmbFornecedor.SelectedIndex = 0;
            cmbMaterial.SelectedIndex = 0;
            cmbResultado.SelectedIndex = 0;
            dtpDataInicio.Value = DateTime.Now.AddDays(-30);
            dtpDataFim.Value = DateTime.Now;
            CarregarDados();
        }

        // ===== EXPORTAR EXCEL =====
        private void BtnExportarExcel_Click(object sender, EventArgs e)
        {
            if (_inspecoes == null || _inspecoes.Count == 0)
            {
                MessageBox.Show("Nenhum dado para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Arquivo Excel|*.xlsx";
                saveDialog.Title = "Salvar Relatório Excel";
                saveDialog.FileName = $"Relatorio_IQC_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string fornecedorNome = cmbFornecedor.SelectedValue is int id && id > 0 ?
                        _fornecedorRepo.BuscarPorId(id)?.Nome : "Todos";

                    string materialDesc = cmbMaterial.SelectedValue is int mid && mid > 0 ?
                        _materialRepo.BuscarPorId(mid)?.Descricao : "Todos";

                    var relatorioService = new RelatorioService();
                    byte[] arquivo = relatorioService.GerarRelatorioExcel(
                        _inspecoes,
                        dtpDataInicio.Value,
                        dtpDataFim.Value,
                        fornecedorNome,
                        materialDesc
                    );

                    File.WriteAllBytes(saveDialog.FileName, arquivo);
                    MessageBox.Show($"✅ Relatório Excel salvo com sucesso!\n\nLocal: {saveDialog.FileName}",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
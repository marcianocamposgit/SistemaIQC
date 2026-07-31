using SistemaIQC.Models;
using SistemaIQC.Repository;
using System;
using System.Windows.Forms;

namespace SistemaIQC.Forms
{
    public partial class FrmMaterial : Form
    {
        private readonly MaterialRepository _repository;
        private Material _materialEditando;
        private bool _isEditando;

        private DataGridView dgvMateriais;
        private TextBox txtCodigo, txtDescricao, txtModelo;
        private Button btnSalvar, btnNovo, btnEditar, btnExcluir, btnFechar;
        private Label lblTitulo;

        public FrmMaterial()
        {
            _repository = new MaterialRepository();
            InitializeComponent();
            ConfigurarControles();
            CarregarMateriais();
        }

        private void InitializeComponent()
        {
            this.Text = "📦 Cadastro de Materiais";
            this.Size = new System.Drawing.Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitulo = new Label();
            lblTitulo.Text = "📦 Gerenciar Materiais";
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(20, 20);
            lblTitulo.Size = new System.Drawing.Size(400, 40);
            this.Controls.Add(lblTitulo);

            var gbDados = new GroupBox();
            gbDados.Text = "Dados do Material";
            gbDados.Location = new System.Drawing.Point(20, 70);
            gbDados.Size = new System.Drawing.Size(650, 150);
            this.Controls.Add(gbDados);

            int y = 25;
            int x = 20;
            int labelWidth = 100;
            int campoWidth = 300;

            // Código
            var lblCodigo = new Label();
            lblCodigo.Text = "Código:";
            lblCodigo.Location = new System.Drawing.Point(x, y);
            lblCodigo.Size = new System.Drawing.Size(labelWidth, 25);
            gbDados.Controls.Add(lblCodigo);

            txtCodigo = new TextBox();
            txtCodigo.Location = new System.Drawing.Point(x + labelWidth + 5, y);
            txtCodigo.Size = new System.Drawing.Size(campoWidth, 25);
            gbDados.Controls.Add(txtCodigo);

            y += 35;

            // Descrição
            var lblDescricao = new Label();
            lblDescricao.Text = "Descrição:";
            lblDescricao.Location = new System.Drawing.Point(x, y);
            lblDescricao.Size = new System.Drawing.Size(labelWidth, 25);
            gbDados.Controls.Add(lblDescricao);

            txtDescricao = new TextBox();
            txtDescricao.Location = new System.Drawing.Point(x + labelWidth + 5, y);
            txtDescricao.Size = new System.Drawing.Size(campoWidth, 25);
            gbDados.Controls.Add(txtDescricao);

            y += 35;

            // Modelo
            var lblModelo = new Label();
            lblModelo.Text = "Modelo:";
            lblModelo.Location = new System.Drawing.Point(x, y);
            lblModelo.Size = new System.Drawing.Size(labelWidth, 25);
            gbDados.Controls.Add(lblModelo);

            txtModelo = new TextBox();
            txtModelo.Location = new System.Drawing.Point(x + labelWidth + 5, y);
            txtModelo.Size = new System.Drawing.Size(campoWidth, 25);
            gbDados.Controls.Add(txtModelo);

            // Botões
            btnSalvar = new Button();
            btnSalvar.Text = "💾 Salvar";
            btnSalvar.Location = new System.Drawing.Point(20, 235);
            btnSalvar.Size = new System.Drawing.Size(100, 35);
            btnSalvar.BackColor = System.Drawing.Color.LightGreen;
            btnSalvar.Click += BtnSalvar_Click;
            this.Controls.Add(btnSalvar);

            btnNovo = new Button();
            btnNovo.Text = "🆕 Novo";
            btnNovo.Location = new System.Drawing.Point(140, 235);
            btnNovo.Size = new System.Drawing.Size(100, 35);
            btnNovo.Click += BtnNovo_Click;
            this.Controls.Add(btnNovo);

            btnEditar = new Button();
            btnEditar.Text = "✏️ Editar";
            btnEditar.Location = new System.Drawing.Point(260, 235);
            btnEditar.Size = new System.Drawing.Size(100, 35);
            btnEditar.Enabled = false;
            btnEditar.Click += BtnEditar_Click;
            this.Controls.Add(btnEditar);

            btnExcluir = new Button();
            btnExcluir.Text = "🗑️ Excluir";
            btnExcluir.Location = new System.Drawing.Point(380, 235);
            btnExcluir.Size = new System.Drawing.Size(100, 35);
            btnExcluir.Enabled = false;
            btnExcluir.BackColor = System.Drawing.Color.LightCoral;
            btnExcluir.Click += BtnExcluir_Click;
            this.Controls.Add(btnExcluir);

            btnFechar = new Button();
            btnFechar.Text = "❌ Fechar";
            btnFechar.Location = new System.Drawing.Point(570, 235);
            btnFechar.Size = new System.Drawing.Size(100, 35);
            btnFechar.Click += (s, e) => this.Close();
            this.Controls.Add(btnFechar);

            dgvMateriais = new DataGridView();
            dgvMateriais.Location = new System.Drawing.Point(20, 285);
            dgvMateriais.Size = new System.Drawing.Size(650, 200);
            dgvMateriais.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMateriais.MultiSelect = false;
            dgvMateriais.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMateriais.CellClick += DgvMateriais_CellClick;
            this.Controls.Add(dgvMateriais);
        }

        private void ConfigurarControles()
        {
            dgvMateriais.ColumnCount = 4;
            dgvMateriais.Columns[0].Name = "Id";
            dgvMateriais.Columns[0].Visible = false;
            dgvMateriais.Columns[1].Name = "Código";
            dgvMateriais.Columns[2].Name = "Descrição";
            dgvMateriais.Columns[3].Name = "Modelo";
        }

        private void CarregarMateriais()
        {
            var lista = _repository.BuscarTodos();
            dgvMateriais.Rows.Clear();

            foreach (var m in lista)
            {
                dgvMateriais.Rows.Add(m.Id, m.Codigo, m.Descricao, m.Modelo);
            }
        }

        private void LimparCampos()
        {
            txtCodigo.Clear();
            txtDescricao.Clear();
            txtModelo.Clear();
            _materialEditando = null;
            _isEditando = false;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnSalvar.Text = "💾 Salvar";
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            txtCodigo.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Informe o código do material.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe a descrição do material.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescricao.Focus();
                return;
            }

            try
            {
                var material = new Material
                {
                    Codigo = txtCodigo.Text.Trim(),
                    Descricao = txtDescricao.Text.Trim(),
                    Modelo = txtModelo.Text.Trim()
                };

                if (_isEditando && _materialEditando != null)
                {
                    material.Id = _materialEditando.Id;
                    _repository.Atualizar(material);
                    MessageBox.Show("Material atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repository.Inserir(material);
                    MessageBox.Show("Material cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CarregarMateriais();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMateriais_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvMateriais.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["Id"].Value);

            _materialEditando = _repository.BuscarPorId(id);
            if (_materialEditando == null) return;

            txtCodigo.Text = _materialEditando.Codigo;
            txtDescricao.Text = _materialEditando.Descricao;
            txtModelo.Text = _materialEditando.Modelo;

            _isEditando = true;
            btnEditar.Enabled = true;
            btnExcluir.Enabled = true;
            btnSalvar.Text = "💾 Atualizar";
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (_materialEditando == null) return;
            BtnSalvar_Click(sender, e);
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (_materialEditando == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o material '{_materialEditando.Codigo} - {_materialEditando.Descricao}'?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.Excluir(_materialEditando.Id);
                    MessageBox.Show("Material excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarMateriais();
                    LimparCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
using SistemaIQC.Models;
using SistemaIQC.Repository;
using System;
using System.Windows.Forms;

namespace SistemaIQC.Forms
{
    public partial class FrmFornecedor : Form
    {
        private readonly FornecedorRepository _repository;
        private Fornecedor _fornecedorEditando;
        private bool _isEditando;

        private DataGridView dgvFornecedores;
        private TextBox txtNome;
        private Button btnSalvar, btnNovo, btnEditar, btnExcluir, btnFechar;
        private Label lblTitulo;

        public FrmFornecedor()
        {
            _repository = new FornecedorRepository();
            InitializeComponent();
            ConfigurarControles();
            CarregarFornecedores();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Cadastro de Fornecedores";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitulo = new Label();
            lblTitulo.Text = "📋 Gerenciar Fornecedores";
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(20, 20);
            lblTitulo.Size = new System.Drawing.Size(400, 40);
            this.Controls.Add(lblTitulo);

            var gbDados = new GroupBox();
            gbDados.Text = "Dados do Fornecedor";
            gbDados.Location = new System.Drawing.Point(20, 70);
            gbDados.Size = new System.Drawing.Size(650, 100);
            this.Controls.Add(gbDados);

            int y = 30;
            int x = 20;
            int labelWidth = 80;
            int campoWidth = 300;

            // Nome (somente)
            var lblNome = new Label();
            lblNome.Text = "Nome:";
            lblNome.Location = new System.Drawing.Point(x, y);
            lblNome.Size = new System.Drawing.Size(labelWidth, 25);
            gbDados.Controls.Add(lblNome);

            txtNome = new TextBox();
            txtNome.Location = new System.Drawing.Point(x + labelWidth + 5, y);
            txtNome.Size = new System.Drawing.Size(campoWidth, 25);
            gbDados.Controls.Add(txtNome);

            // Botões
            btnSalvar = new Button();
            btnSalvar.Text = "💾 Salvar";
            btnSalvar.Location = new System.Drawing.Point(20, 185);
            btnSalvar.Size = new System.Drawing.Size(100, 35);
            btnSalvar.BackColor = System.Drawing.Color.LightGreen;
            btnSalvar.Click += BtnSalvar_Click;
            this.Controls.Add(btnSalvar);

            btnNovo = new Button();
            btnNovo.Text = "🆕 Novo";
            btnNovo.Location = new System.Drawing.Point(140, 185);
            btnNovo.Size = new System.Drawing.Size(100, 35);
            btnNovo.Click += BtnNovo_Click;
            this.Controls.Add(btnNovo);

            btnEditar = new Button();
            btnEditar.Text = "✏️ Editar";
            btnEditar.Location = new System.Drawing.Point(260, 185);
            btnEditar.Size = new System.Drawing.Size(100, 35);
            btnEditar.Enabled = false;
            btnEditar.Click += BtnEditar_Click;
            this.Controls.Add(btnEditar);

            btnExcluir = new Button();
            btnExcluir.Text = "🗑️ Excluir";
            btnExcluir.Location = new System.Drawing.Point(380, 185);
            btnExcluir.Size = new System.Drawing.Size(100, 35);
            btnExcluir.Enabled = false;
            btnExcluir.BackColor = System.Drawing.Color.LightCoral;
            btnExcluir.Click += BtnExcluir_Click;
            this.Controls.Add(btnExcluir);

            btnFechar = new Button();
            btnFechar.Text = "❌ Fechar";
            btnFechar.Location = new System.Drawing.Point(570, 185);
            btnFechar.Size = new System.Drawing.Size(100, 35);
            btnFechar.Click += (s, e) => this.Close();
            this.Controls.Add(btnFechar);

            dgvFornecedores = new DataGridView();
            dgvFornecedores.Location = new System.Drawing.Point(20, 235);
            dgvFornecedores.Size = new System.Drawing.Size(650, 200);
            dgvFornecedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFornecedores.MultiSelect = false;
            dgvFornecedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFornecedores.CellClick += DgvFornecedores_CellClick;
            this.Controls.Add(dgvFornecedores);
        }

        private void ConfigurarControles()
        {
            dgvFornecedores.ColumnCount = 2;
            dgvFornecedores.Columns[0].Name = "Id";
            dgvFornecedores.Columns[0].Visible = false;
            dgvFornecedores.Columns[1].Name = "Nome";
        }

        private void CarregarFornecedores()
        {
            var lista = _repository.BuscarTodos();
            dgvFornecedores.Rows.Clear();

            foreach (var f in lista)
            {
                dgvFornecedores.Rows.Add(f.Id, f.Nome);
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            _fornecedorEditando = null;
            _isEditando = false;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnSalvar.Text = "💾 Salvar";
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do fornecedor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            try
            {
                var fornecedor = new Fornecedor
                {
                    Nome = txtNome.Text.Trim()
                };

                if (_isEditando && _fornecedorEditando != null)
                {
                    fornecedor.Id = _fornecedorEditando.Id;
                    _repository.Atualizar(fornecedor);
                    MessageBox.Show("Fornecedor atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repository.Inserir(fornecedor);
                    MessageBox.Show("Fornecedor cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CarregarFornecedores();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvFornecedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvFornecedores.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["Id"].Value);

            _fornecedorEditando = _repository.BuscarPorId(id);
            if (_fornecedorEditando == null) return;

            txtNome.Text = _fornecedorEditando.Nome;

            _isEditando = true;
            btnEditar.Enabled = true;
            btnExcluir.Enabled = true;
            btnSalvar.Text = "💾 Atualizar";
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (_fornecedorEditando == null) return;
            BtnSalvar_Click(sender, e);
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (_fornecedorEditando == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o fornecedor '{_fornecedorEditando.Nome}'?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.Excluir(_fornecedorEditando.Id);
                    MessageBox.Show("Fornecedor excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarFornecedores();
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
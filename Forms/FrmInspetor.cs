using SistemaIQC.Models;
using SistemaIQC.Repository;
using System;
using System.Windows.Forms;

namespace SistemaIQC.Forms
{
    public class FrmInspetor : Form
    {
        private readonly InspetorRepository _repository;
        private Inspetor _inspetorEditando;
        private bool _isEditando;

        private DataGridView dgvInspetores;
        private TextBox txtNome;
        private Button btnSalvar, btnNovo, btnEditar, btnExcluir, btnFechar;
        private Label lblTitulo;

        public FrmInspetor()
        {
            _repository = new InspetorRepository();
            InitializeComponent();
            ConfigurarControles();
            CarregarInspetores();
        }

        private void InitializeComponent()
        {
            this.Text = "👤 Cadastro de Inspetores";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitulo = new Label();
            lblTitulo.Text = "👤 Gerenciar Inspetores";
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(20, 20);
            lblTitulo.Size = new System.Drawing.Size(400, 40);
            this.Controls.Add(lblTitulo);

            var gbDados = new GroupBox();
            gbDados.Text = "Dados do Inspetor";
            gbDados.Location = new System.Drawing.Point(20, 70);
            gbDados.Size = new System.Drawing.Size(650, 100);
            this.Controls.Add(gbDados);

            int y = 30;
            int x = 20;
            int labelWidth = 80;
            int campoWidth = 300;

            // Nome
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

            dgvInspetores = new DataGridView();
            dgvInspetores.Location = new System.Drawing.Point(20, 235);
            dgvInspetores.Size = new System.Drawing.Size(650, 200);
            dgvInspetores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInspetores.MultiSelect = false;
            dgvInspetores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInspetores.CellClick += DgvInspetores_CellClick;
            this.Controls.Add(dgvInspetores);
        }

        private void ConfigurarControles()
        {
            dgvInspetores.ColumnCount = 2;
            dgvInspetores.Columns[0].Name = "Id";
            dgvInspetores.Columns[0].Visible = false;
            dgvInspetores.Columns[1].Name = "Nome";
        }

        private void CarregarInspetores()
        {
            var lista = _repository.BuscarTodos();
            dgvInspetores.Rows.Clear();

            foreach (var f in lista)
            {
                dgvInspetores.Rows.Add(f.Id, f.Nome);
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            _inspetorEditando = null;
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
                MessageBox.Show("Informe o nome do inspetor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            try
            {
                if (_isEditando && _inspetorEditando != null)
                {
                    _repository.Atualizar(_inspetorEditando.Id, txtNome.Text.Trim());
                    MessageBox.Show("Inspetor atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repository.Inserir(txtNome.Text.Trim());
                    MessageBox.Show("Inspetor cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CarregarInspetores();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvInspetores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvInspetores.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["Id"].Value);
            string nome = row.Cells["Nome"].Value?.ToString();

            _inspetorEditando = new Inspetor { Id = id, Nome = nome };
            txtNome.Text = nome;

            _isEditando = true;
            btnEditar.Enabled = true;
            btnExcluir.Enabled = true;
            btnSalvar.Text = "💾 Atualizar";
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (_inspetorEditando == null) return;
            BtnSalvar_Click(sender, e);
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (_inspetorEditando == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o inspetor '{_inspetorEditando.Nome}'?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.Desativar(_inspetorEditando.Id);
                    MessageBox.Show("Inspetor excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarInspetores();
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
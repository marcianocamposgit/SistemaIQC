using SistemaIQC.Models;
using SistemaIQC.Models.NBR5426;
using SistemaIQC.Repository;
using SistemaIQC.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SistemaIQC.Forms
{
    public class FrmPrincipal : Form
    {
        // ========== SERVICES E REPOSITORIES ==========
        private readonly NBR5426Service _nbrService;
        private readonly InspecaoService _inspecaoService;
        private readonly InspetorRepository _inspetorRepo;
        private readonly FornecedorRepository _fornecedorRepo;
        private readonly MaterialRepository _materialRepo;
        private readonly ConfiguracaoRepository _configRepo;

        // ========== DADOS DOS COMBOS ==========
        private List<Fornecedor> _fornecedores;
        private List<Material> _materiais;
        private List<Inspetor> _inspetores;          // ← ADICIONADO
        private ResultadoInspecao _resultadoAtual;

        // ========== CONTROLES ==========
        // Grupo Dados do Lote
        private GroupBox gbDadosLote;
        private DateTimePicker dtpData;
        private Label lblInspetor;
        private ComboBox cmbInspetor;
        private Label lblFornecedor;
        private ComboBox cmbFornecedor;
        private Label lblMaterial;
        private ComboBox cmbMaterial;
        private Label lblNF;
        private TextBox txtNF;
        private Label lblLote;
        private TextBox txtLote;
        private Label lblQuantidade;
        private TextBox txtQuantidade;

        // Grupo NBR 5426
        private GroupBox gbNBR;
        private Label lblPlano;
        private ComboBox cmbPlano;
        private Label lblRegime;
        private ComboBox cmbRegime;
        private Label lblNivel;
        private ComboBox cmbNivel;
        private Label lblNQA;
        private ComboBox cmbNQA;

        // Grupo Resultado - Cálculo NBR
        private GroupBox gbResultado;
        private Label lblCodigoLetraTitulo;
        private Label lblCodigoLetra;
        private Label lblAmostraTitulo;
        private Label lblAmostra;
        private Label lblAcTitulo;
        private Label lblAc;
        private Label lblReTitulo;
        private Label lblRe;

        // Grupo Inspeção
        private GroupBox gbInspecao;
        private Label lblDefeitos;
        private TextBox txtDefeitos;
        private Label lblObservacao;
        private TextBox txtObservacao;
        private Label lblAcaoImediata;
        private TextBox txtAcaoImediata;

        // Campos calculados da planilha
        private Label lblAprovadosTitulo;
        private Label lblAprovados;
        private Label lblPctAprovadosTitulo;
        private Label lblPctAprovados;
        private Label lblPctReprovadosTitulo;
        private Label lblPctReprovados;

        // Status
        private Label lblStatusResultado;
        private Label lblStatusLote;

        // Botões
        private Button btnSalvar;
        private Button btnLimpar;
        private Button btnHistorico;
        private Button btnCadastroFornecedor;
        private Button btnCadastroMaterial;

        // ========== CONSTRUTOR ==========
        public FrmPrincipal()
        {
            _nbrService = new NBR5426Service();
            _inspecaoService = new InspecaoService();
            _fornecedorRepo = new FornecedorRepository();
            _materialRepo = new MaterialRepository();
            _configRepo = new ConfiguracaoRepository();
            _inspetorRepo = new InspetorRepository();

            this.Text = "📦 IQC - Controle de Qualidade";
            this.Size = new Size(950, 900);
            this.StartPosition = FormStartPosition.CenterScreen;

            CriarControles();
            CarregarCombos();
            CarregarConfiguracoes();

            dtpData.Value = DateTime.Now;

            // Eventos
            this.txtQuantidade.TextChanged += txtQuantidade_TextChanged;
            this.txtDefeitos.TextChanged += txtDefeitos_TextChanged;
            this.cmbPlano.SelectedIndexChanged += cmbPlano_SelectedIndexChanged;
            this.cmbRegime.SelectedIndexChanged += cmbRegime_SelectedIndexChanged;
            this.cmbNivel.SelectedIndexChanged += cmbNivel_SelectedIndexChanged;
            this.cmbNQA.SelectedIndexChanged += cmbNQA_SelectedIndexChanged;
            this.btnSalvar.Click += btnSalvar_Click;
            this.btnLimpar.Click += btnLimpar_Click;
            this.btnHistorico.Click += btnHistorico_Click;
            this.btnCadastroFornecedor.Click += BtnCadastroFornecedor_Click;
            this.btnCadastroMaterial.Click += BtnCadastroMaterial_Click;
        }

        // ========== CRIAÇÃO DOS CONTROLES ==========
        private void CriarControles()
        {
            int x = 20;
            int y = 20;
            int larguraLabel = 100;
            int larguraCampo = 200;
            int alturaCampo = 25;
            int espaco = 10;

            // ===== GRUPO DADOS DO LOTE =====
            gbDadosLote = new GroupBox();
            gbDadosLote.Text = "📋 Dados do Lote";
            gbDadosLote.Location = new Point(x, y);
            gbDadosLote.Size = new Size(420, 280);
            this.Controls.Add(gbDadosLote);

            int dy = 25;
            int dx = 15;

            // Data
            var lblData = new Label();
            lblData.Text = "Data:";
            lblData.Location = new Point(dx, dy);
            lblData.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblData);

            dtpData = new DateTimePicker();
            dtpData.Location = new Point(dx + larguraLabel + 5, dy);
            dtpData.Size = new Size(larguraCampo, alturaCampo);
            gbDadosLote.Controls.Add(dtpData);

            dy += alturaCampo + espaco;

            // Inspetor
            lblInspetor = new Label();
            lblInspetor.Text = "Inspetor(a):";
            lblInspetor.Location = new Point(dx, dy);
            lblInspetor.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblInspetor);

            cmbInspetor = new ComboBox();
            cmbInspetor.Location = new Point(dx + larguraLabel + 5, dy);
            cmbInspetor.Size = new Size(larguraCampo, alturaCampo);
            cmbInspetor.DropDownStyle = ComboBoxStyle.DropDownList;
            gbDadosLote.Controls.Add(cmbInspetor);

            dy += alturaCampo + espaco;

            // Fornecedor
            lblFornecedor = new Label();
            lblFornecedor.Text = "Fornecedor:";
            lblFornecedor.Location = new Point(dx, dy);
            lblFornecedor.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblFornecedor);

            cmbFornecedor = new ComboBox();
            cmbFornecedor.Location = new Point(dx + larguraLabel + 5, dy);
            cmbFornecedor.Size = new Size(larguraCampo, alturaCampo);
            cmbFornecedor.DropDownStyle = ComboBoxStyle.DropDownList;
            gbDadosLote.Controls.Add(cmbFornecedor);

            dy += alturaCampo + espaco;

            // Material
            lblMaterial = new Label();
            lblMaterial.Text = "Material:";
            lblMaterial.Location = new Point(dx, dy);
            lblMaterial.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblMaterial);

            cmbMaterial = new ComboBox();
            cmbMaterial.Location = new Point(dx + larguraLabel + 5, dy);
            cmbMaterial.Size = new Size(larguraCampo, alturaCampo);
            cmbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            gbDadosLote.Controls.Add(cmbMaterial);

            dy += alturaCampo + espaco;

            // NF
            lblNF = new Label();
            lblNF.Text = "NF (Nota Fiscal):";
            lblNF.Location = new Point(dx, dy);
            lblNF.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblNF);

            txtNF = new TextBox();
            txtNF.Location = new Point(dx + larguraLabel + 5, dy);
            txtNF.Size = new Size(larguraCampo, alturaCampo);
            gbDadosLote.Controls.Add(txtNF);

            dy += alturaCampo + espaco;

            // Lote
            lblLote = new Label();
            lblLote.Text = "Lote / PO:";
            lblLote.Location = new Point(dx, dy);
            lblLote.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblLote);

            txtLote = new TextBox();
            txtLote.Location = new Point(dx + larguraLabel + 5, dy);
            txtLote.Size = new Size(larguraCampo, alturaCampo);
            gbDadosLote.Controls.Add(txtLote);

            dy += alturaCampo + espaco;

            // Quantidade
            lblQuantidade = new Label();
            lblQuantidade.Text = "QTD. LOTE:";
            lblQuantidade.Location = new Point(dx, dy);
            lblQuantidade.Size = new Size(larguraLabel, alturaCampo);
            gbDadosLote.Controls.Add(lblQuantidade);

            txtQuantidade = new TextBox();
            txtQuantidade.Location = new Point(dx + larguraLabel + 5, dy);
            txtQuantidade.Size = new Size(larguraCampo, alturaCampo);
            gbDadosLote.Controls.Add(txtQuantidade);

            // ===== GRUPO NBR 5426 =====
            int xNBR = x + 420 + 20;
            gbNBR = new GroupBox();
            gbNBR.Text = "📊 NBR 5426";
            gbNBR.Location = new Point(xNBR, y);
            gbNBR.Size = new Size(240, 280);
            this.Controls.Add(gbNBR);

            dy = 25;
            dx = 15;

            // Plano
            lblPlano = new Label();
            lblPlano.Text = "Plano:";
            lblPlano.Location = new Point(dx, dy);
            lblPlano.Size = new Size(70, alturaCampo);
            gbNBR.Controls.Add(lblPlano);

            cmbPlano = new ComboBox();
            cmbPlano.Location = new Point(dx + 75, dy);
            cmbPlano.Size = new Size(140, alturaCampo);
            cmbPlano.DropDownStyle = ComboBoxStyle.DropDownList;
            gbNBR.Controls.Add(cmbPlano);

            dy += alturaCampo + espaco;

            // Regime
            lblRegime = new Label();
            lblRegime.Text = "Regime:";
            lblRegime.Location = new Point(dx, dy);
            lblRegime.Size = new Size(70, alturaCampo);
            gbNBR.Controls.Add(lblRegime);

            cmbRegime = new ComboBox();
            cmbRegime.Location = new Point(dx + 75, dy);
            cmbRegime.Size = new Size(140, alturaCampo);
            cmbRegime.DropDownStyle = ComboBoxStyle.DropDownList;
            gbNBR.Controls.Add(cmbRegime);

            dy += alturaCampo + espaco;

            // Nível
            lblNivel = new Label();
            lblNivel.Text = "Nível:";
            lblNivel.Location = new Point(dx, dy);
            lblNivel.Size = new Size(70, alturaCampo);
            gbNBR.Controls.Add(lblNivel);

            cmbNivel = new ComboBox();
            cmbNivel.Location = new Point(dx + 75, dy);
            cmbNivel.Size = new Size(140, alturaCampo);
            cmbNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            gbNBR.Controls.Add(cmbNivel);

            dy += alturaCampo + espaco;

            // NQA
            lblNQA = new Label();
            lblNQA.Text = "NQA (AQL):";
            lblNQA.Location = new Point(dx, dy);
            lblNQA.Size = new Size(70, alturaCampo);
            gbNBR.Controls.Add(lblNQA);

            cmbNQA = new ComboBox();
            cmbNQA.Location = new Point(dx + 75, dy);
            cmbNQA.Size = new Size(140, alturaCampo);
            cmbNQA.DropDownStyle = ComboBoxStyle.DropDownList;
            gbNBR.Controls.Add(cmbNQA);

            // ===== GRUPO RESULTADO =====
            int yResultado = y + 280 + 20;
            gbResultado = new GroupBox();
            gbResultado.Text = "📐 Cálculo NBR 5426";
            gbResultado.Location = new Point(x, yResultado);
            gbResultado.Size = new Size(420, 120);
            this.Controls.Add(gbResultado);

            dy = 25;
            dx = 15;
            int coluna1 = 0;
            int coluna2 = 180;

            // Código-Letra
            lblCodigoLetraTitulo = new Label();
            lblCodigoLetraTitulo.Text = "Código:";
            lblCodigoLetraTitulo.Location = new Point(dx + coluna1, dy);
            lblCodigoLetraTitulo.Size = new Size(80, alturaCampo);
            gbResultado.Controls.Add(lblCodigoLetraTitulo);

            lblCodigoLetra = new Label();
            lblCodigoLetra.Text = "--";
            lblCodigoLetra.Location = new Point(dx + coluna1 + 85, dy);
            lblCodigoLetra.Size = new Size(60, alturaCampo);
            lblCodigoLetra.Font = new Font("Arial", 10, FontStyle.Bold);
            gbResultado.Controls.Add(lblCodigoLetra);

            // Amostra
            lblAmostraTitulo = new Label();
            lblAmostraTitulo.Text = "QTD. REVISADA:";
            lblAmostraTitulo.Location = new Point(dx + coluna2, dy);
            lblAmostraTitulo.Size = new Size(120, alturaCampo);
            gbResultado.Controls.Add(lblAmostraTitulo);

            lblAmostra = new Label();
            lblAmostra.Text = "--";
            lblAmostra.Location = new Point(dx + coluna2 + 125, dy);
            lblAmostra.Size = new Size(60, alturaCampo);
            lblAmostra.Font = new Font("Arial", 10, FontStyle.Bold);
            gbResultado.Controls.Add(lblAmostra);

            dy += alturaCampo + espaco;

            // Ac
            lblAcTitulo = new Label();
            lblAcTitulo.Text = "Ac:";
            lblAcTitulo.Location = new Point(dx + coluna1, dy);
            lblAcTitulo.Size = new Size(80, alturaCampo);
            gbResultado.Controls.Add(lblAcTitulo);

            lblAc = new Label();
            lblAc.Text = "--";
            lblAc.Location = new Point(dx + coluna1 + 85, dy);
            lblAc.Size = new Size(60, alturaCampo);
            lblAc.Font = new Font("Arial", 10, FontStyle.Bold);
            gbResultado.Controls.Add(lblAc);

            // Re
            lblReTitulo = new Label();
            lblReTitulo.Text = "Re:";
            lblReTitulo.Location = new Point(dx + coluna2, dy);
            lblReTitulo.Size = new Size(80, alturaCampo);
            gbResultado.Controls.Add(lblReTitulo);

            lblRe = new Label();
            lblRe.Text = "--";
            lblRe.Location = new Point(dx + coluna2 + 85, dy);
            lblRe.Size = new Size(60, alturaCampo);
            lblRe.Font = new Font("Arial", 10, FontStyle.Bold);
            gbResultado.Controls.Add(lblRe);

            // ===== GRUPO INSPEÇÃO =====
            int yInspecao = yResultado + 120 + 20;
            gbInspecao = new GroupBox();
            gbInspecao.Text = "🔍 Inspeção";
            gbInspecao.Location = new Point(x, yInspecao);
            gbInspecao.Size = new Size(420, 280);
            this.Controls.Add(gbInspecao);

            dy = 25;
            dx = 15;

            // Defeitos
            lblDefeitos = new Label();
            lblDefeitos.Text = "QTD. REPROVADA:";
            lblDefeitos.Location = new Point(dx, dy);
            lblDefeitos.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblDefeitos);

            txtDefeitos = new TextBox();
            txtDefeitos.Location = new Point(dx + larguraLabel + 25, dy);
            txtDefeitos.Size = new Size(100, alturaCampo);
            gbInspecao.Controls.Add(txtDefeitos);

            // QTD. APROVADA (calculado)
            dy += alturaCampo + espaco;
            lblAprovadosTitulo = new Label();
            lblAprovadosTitulo.Text = "QTD. APROVADA:";
            lblAprovadosTitulo.Location = new Point(dx, dy);
            lblAprovadosTitulo.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblAprovadosTitulo);

            lblAprovados = new Label();
            lblAprovados.Text = "--";
            lblAprovados.Location = new Point(dx + larguraLabel + 25, dy);
            lblAprovados.Size = new Size(100, alturaCampo);
            lblAprovados.Font = new Font("Arial", 10, FontStyle.Bold);
            gbInspecao.Controls.Add(lblAprovados);

            // % APROVADA (calculado)
            dy += alturaCampo + espaco;
            lblPctAprovadosTitulo = new Label();
            lblPctAprovadosTitulo.Text = "% APROVADA:";
            lblPctAprovadosTitulo.Location = new Point(dx, dy);
            lblPctAprovadosTitulo.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblPctAprovadosTitulo);

            lblPctAprovados = new Label();
            lblPctAprovados.Text = "--";
            lblPctAprovados.Location = new Point(dx + larguraLabel + 25, dy);
            lblPctAprovados.Size = new Size(80, alturaCampo);
            lblPctAprovados.Font = new Font("Arial", 10, FontStyle.Bold);
            gbInspecao.Controls.Add(lblPctAprovados);

            // % REPROVADA (calculado)
            dy += alturaCampo + espaco;
            lblPctReprovadosTitulo = new Label();
            lblPctReprovadosTitulo.Text = "% REPROVADA:";
            lblPctReprovadosTitulo.Location = new Point(dx, dy);
            lblPctReprovadosTitulo.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblPctReprovadosTitulo);

            lblPctReprovados = new Label();
            lblPctReprovados.Text = "--";
            lblPctReprovados.Location = new Point(dx + larguraLabel + 25, dy);
            lblPctReprovados.Size = new Size(80, alturaCampo);
            lblPctReprovados.Font = new Font("Arial", 10, FontStyle.Bold);
            lblPctReprovados.ForeColor = Color.Red;
            gbInspecao.Controls.Add(lblPctReprovados);

            // Observação
            dy += alturaCampo + espaco + 10;
            lblObservacao = new Label();
            lblObservacao.Text = "DESCRIÇÃO DA NC:";
            lblObservacao.Location = new Point(dx, dy);
            lblObservacao.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblObservacao);

            txtObservacao = new TextBox();
            txtObservacao.Location = new Point(dx + larguraLabel + 25, dy);
            txtObservacao.Size = new Size(larguraCampo + 30, 60);
            txtObservacao.Multiline = true;
            txtObservacao.ScrollBars = ScrollBars.Vertical;
            gbInspecao.Controls.Add(txtObservacao);

            dy += 65 + espaco;

            // Ação Imediata
            lblAcaoImediata = new Label();
            lblAcaoImediata.Text = "AÇÃO IMEDIATA:";
            lblAcaoImediata.Location = new Point(dx, dy);
            lblAcaoImediata.Size = new Size(larguraLabel + 20, alturaCampo);
            gbInspecao.Controls.Add(lblAcaoImediata);

            txtAcaoImediata = new TextBox();
            txtAcaoImediata.Location = new Point(dx + larguraLabel + 25, dy);
            txtAcaoImediata.Size = new Size(larguraCampo + 30, alturaCampo);
            gbInspecao.Controls.Add(txtAcaoImediata);

            // ===== STATUS DO LOTE =====
            int xStatus = x + 420 + 20;

            // Label fixa "Status do Lote"
            var lblStatusLoteTitulo = new Label();
            lblStatusLoteTitulo.Text = "Status do Lote:";
            lblStatusLoteTitulo.Location = new Point(xStatus, yResultado);
            lblStatusLoteTitulo.Size = new Size(150, 30);
            lblStatusLoteTitulo.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(lblStatusLoteTitulo);

            // Label dinâmica do resultado
            lblStatusResultado = new Label();
            lblStatusResultado.Text = "AGUARDANDO DADOS...";
            lblStatusResultado.Location = new Point(xStatus, yResultado + 35);
            lblStatusResultado.Size = new Size(240, 50);
            lblStatusResultado.Font = new Font("Arial", 14, FontStyle.Bold);
            lblStatusResultado.ForeColor = Color.Gray;
            lblStatusResultado.TextAlign = ContentAlignment.MiddleCenter;
            lblStatusResultado.BackColor = Color.WhiteSmoke;
            lblStatusResultado.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(lblStatusResultado);

            // ===== BOTÕES =====
            int yBotoes = yInspecao + 280 + 20;

            btnSalvar = new Button();
            btnSalvar.Text = "💾 Salvar";
            btnSalvar.Location = new Point(x, yBotoes);
            btnSalvar.Size = new Size(120, 40);
            btnSalvar.BackColor = Color.LightGreen;
            this.Controls.Add(btnSalvar);

            btnLimpar = new Button();
            btnLimpar.Text = "🗑️ Limpar";
            btnLimpar.Location = new Point(x + 140, yBotoes);
            btnLimpar.Size = new Size(120, 40);
            btnLimpar.BackColor = Color.LightYellow;
            this.Controls.Add(btnLimpar);

            btnHistorico = new Button();
            btnHistorico.Text = "📋 Histórico";
            btnHistorico.Location = new Point(x + 280, yBotoes);
            btnHistorico.Size = new Size(120, 40);
            btnHistorico.BackColor = Color.LightBlue;
            this.Controls.Add(btnHistorico);

            // ===== BOTÕES DE CADASTRO =====
            btnCadastroFornecedor = new Button();
            btnCadastroFornecedor.Text = "📋 Fornecedores";
            btnCadastroFornecedor.Location = new Point(x, yBotoes + 50);
            btnCadastroFornecedor.Size = new Size(120, 30);
            btnCadastroFornecedor.BackColor = Color.LightGray;
            this.Controls.Add(btnCadastroFornecedor);

            // ===== BOTÃO CADASTRO INSPETOR =====
            Button btnCadastroInspetor = new Button();
            btnCadastroInspetor.Text = "👤 Inspetores";
            btnCadastroInspetor.Location = new Point(x + 280, yBotoes + 50); // Ajuste a posição
            btnCadastroInspetor.Size = new Size(120, 30);
            btnCadastroInspetor.BackColor = Color.LightSteelBlue;
            btnCadastroInspetor.Click += BtnCadastroInspetor_Click;
            this.Controls.Add(btnCadastroInspetor);

            btnCadastroMaterial = new Button();
            btnCadastroMaterial.Text = "📦 Materiais";
            btnCadastroMaterial.Location = new Point(x + 140, yBotoes + 50);
            btnCadastroMaterial.Size = new Size(120, 30);
            btnCadastroMaterial.BackColor = Color.LightGray;
            this.Controls.Add(btnCadastroMaterial);

            // ===== BOTÃO SAIR =====
            Button btnSair = new Button();
            btnSair.Text = "🚪 Sair";
            btnSair.Location = new Point(x + 280, yBotoes + 50);
            btnSair.Size = new Size(120, 30);
            btnSair.BackColor = Color.LightCoral;
            btnSair.Click += (s, e) => this.Close();
            this.Controls.Add(btnSair);

            // ===== BOTÃO SOBRE =====
            Button btnSobre = new Button();
            btnSobre.Text = "ℹ️ Sobre";
            btnSobre.Location = new Point(x + 420, yBotoes + 50);
            btnSobre.Size = new Size(120, 30);
            btnSobre.BackColor = Color.LightSteelBlue;
            btnSobre.Click += BtnSobre_Click;
            this.Controls.Add(btnSobre);
        }

        // ========== CARREGAR DADOS ==========
        private void CarregarCombos()
        {
            _fornecedores = _fornecedorRepo.BuscarTodos();
            cmbFornecedor.DataSource = _fornecedores;
            cmbFornecedor.DisplayMember = "Nome";
            cmbFornecedor.ValueMember = "Id";

            // Inspetores
            _inspetores = _inspetorRepo.BuscarTodos();
            cmbInspetor.DataSource = _inspetores;
            cmbInspetor.DisplayMember = "Nome";
            cmbInspetor.ValueMember = "Id";
            cmbInspetor.SelectedIndex = -1;

            // Remove evento antigo se existir
            cmbFornecedor.SelectedIndexChanged -= CmbFornecedor_SelectedIndexChanged;
            cmbFornecedor.SelectedIndexChanged += CmbFornecedor_SelectedIndexChanged;

            // Inicializa com lista vazia
            cmbMaterial.DataSource = new List<Material>();
            cmbMaterial.DisplayMember = "ToString";
            cmbMaterial.ValueMember = "Id";

            // Inspetores
            var inspetores = _inspetorRepo.BuscarTodos();
            cmbInspetor.DataSource = inspetores;
            cmbInspetor.DisplayMember = "Nome";
            cmbInspetor.ValueMember = "Id";
            cmbInspetor.SelectedIndex = -1;
        }

        private void CmbFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFornecedor.SelectedValue is int fornecedorId && fornecedorId > 0)
            {
                var materiais = _materialRepo.BuscarPorFornecedor(fornecedorId);
                cmbMaterial.DataSource = materiais;
                cmbMaterial.DisplayMember = "ToString";
                cmbMaterial.ValueMember = "Id";
                cmbMaterial.SelectedIndex = -1;
            }
            else
            {
                cmbMaterial.DataSource = new List<Material>();
            }
        }

        private void CarregarConfiguracoes()
        {
            var configs = _configRepo.BuscarTodas();

            // Nível: Apenas II
            cmbNivel.Items.Clear();
            cmbNivel.Items.AddRange(new string[] { "II" });
            cmbNivel.SelectedItem = "II";

            // Plano: Apenas Simples
            cmbPlano.Items.Clear();
            cmbPlano.Items.AddRange(new string[] { "Simples" });
            cmbPlano.SelectedItem = "Simples";

            // Regime: Normal e Rigorosa
            cmbRegime.Items.Clear();
            cmbRegime.Items.AddRange(new string[] { "Normal", "Rigorosa" });
            cmbRegime.SelectedItem = configs.ContainsKey("regime_padrao") ? configs["regime_padrao"] : "Normal";

            // NQA: 0.010 até 0.65 (sem 3 casas decimais)
            cmbNQA.Items.Clear();
            var nqas = new double[] { 0.010, 0.015, 0.025, 0.040, 0.065, 0.10, 0.15, 0.25, 0.40, 0.65 };
            cmbNQA.Items.AddRange(nqas.Select(n => n.ToString("F2")).ToArray());  // ← ALTERADO PARA F2
            string nqaPadrao = configs.ContainsKey("nqa_padrao") ? configs["nqa_padrao"] : "0.65";
            cmbNQA.SelectedItem = nqaPadrao;
        }

        // ========== LÓGICA DE CÁLCULO ==========
        private void Calcular()
        {
            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                LimparCalculo();
                return;
            }

            string nivel = cmbNivel.SelectedItem?.ToString() ?? "II";
            string plano = cmbPlano.SelectedItem?.ToString() ?? "Simples";
            string regime = cmbRegime.SelectedItem?.ToString() ?? "Normal";
            double nqa = double.TryParse(cmbNQA.SelectedItem?.ToString(), out double n) ? n : 2.5;

            try
            {
                _resultadoAtual = _nbrService.Calcular(quantidade, nivel, plano, regime, nqa);
                lblCodigoLetra.Text = _resultadoAtual.CodigoLetra;
                lblAmostra.Text = _resultadoAtual.Amostra.ToString();
                lblAc.Text = _resultadoAtual.Ac.ToString();
                lblRe.Text = _resultadoAtual.Re.ToString();

                // Calcula QTD. APROVADA após verificar defeitos
                AtualizarResultado();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LimparCalculo();
            }
        }

        private void AtualizarResultado()
        {
            if (_resultadoAtual == null) return;

            if (!int.TryParse(txtDefeitos.Text, out int defeitos) || defeitos < 0)
            {
                lblStatusResultado.Text = "AGUARDANDO DEFEITOS...";
                lblStatusResultado.ForeColor = Color.Gray;
                lblAprovados.Text = "--";
                lblPctAprovados.Text = "--";
                lblPctReprovados.Text = "--";
                return;
            }

            string decisao = _nbrService.DecidirResultado(defeitos, _resultadoAtual.Ac, _resultadoAtual.Re, cmbPlano.SelectedItem?.ToString() ?? "Simples");
            _resultadoAtual.Resultado = decisao;

            // Calcula os campos da planilha
            int aprovados = _resultadoAtual.Amostra - defeitos;
            double pctAprovados = _resultadoAtual.Amostra > 0 ? (double)aprovados / _resultadoAtual.Amostra * 100 : 0;
            double pctReprovados = _resultadoAtual.Amostra > 0 ? (double)defeitos / _resultadoAtual.Amostra * 100 : 0;

            lblAprovados.Text = aprovados.ToString();
            lblPctAprovados.Text = pctAprovados.ToString("F2") + "%";
            lblPctReprovados.Text = pctReprovados.ToString("F2") + "%";

            if (decisao == "APROVADO")
            {
                lblStatusResultado.Text = "✅ APROVADO";
                lblStatusResultado.ForeColor = Color.Green;
            }
            else if (decisao == "REPROVADO")
            {
                lblStatusResultado.Text = "❌ REPROVADO";
                lblStatusResultado.ForeColor = Color.Red;
            }
            else
            {
                lblStatusResultado.Text = "⚠️ " + decisao;
                lblStatusResultado.ForeColor = Color.Orange;
            }
        }

        private void LimparCalculo()
        {
            lblCodigoLetra.Text = "--";
            lblAmostra.Text = "--";
            lblAc.Text = "--";
            lblRe.Text = "--";
            lblAprovados.Text = "--";
            lblPctAprovados.Text = "--";
            lblPctReprovados.Text = "--";
            lblStatusResultado.Text = "AGUARDANDO DADOS...";
            lblStatusResultado.ForeColor = Color.Gray;
            _resultadoAtual = null;
        }

        private void LimparTudo()
        {
            txtQuantidade.Clear();
            txtDefeitos.Clear();
            txtObservacao.Clear();
            txtAcaoImediata.Clear();
            LimparCalculo();
        }

        // ========== SALVAR ==========
        private void Salvar()
        {
            if (_resultadoAtual == null)
            {
                MessageBox.Show("Calcule a inspeção antes de salvar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbFornecedor.SelectedItem == null)
            {
                MessageBox.Show("Selecione um fornecedor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMaterial.SelectedItem == null)
            {
                MessageBox.Show("Selecione um material.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                MessageBox.Show("Informe uma quantidade válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtDefeitos.Text, out int defeitos) || defeitos < 0)
            {
                MessageBox.Show("Informe a quantidade de defeitos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_resultadoAtual.Resultado) || _resultadoAtual.Resultado == "EM ANÁLISE")
            {
                MessageBox.Show("Calcule o resultado antes de salvar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var inspecao = new Inspecao
                {
                    Data = dtpData.Value,
                    Inspetor = cmbInspetor.SelectedItem?.ToString(),
                    FornecedorId = (int)cmbFornecedor.SelectedValue,
                    MaterialId = (int)cmbMaterial.SelectedValue,
                    NF = txtNF.Text,
                    Lote = txtLote.Text,
                    Quantidade = quantidade,
                    Plano = cmbPlano.SelectedItem?.ToString() ?? "Simples",
                    Regime = cmbRegime.SelectedItem?.ToString() ?? "Normal",
                    Nivel = cmbNivel.SelectedItem?.ToString() ?? "II",
                    NQA = double.Parse(cmbNQA.SelectedItem?.ToString() ?? "2.5"),
                    CodigoLetra = _resultadoAtual.CodigoLetra,
                    Amostra = _resultadoAtual.Amostra,
                    Ac = _resultadoAtual.Ac,
                    Re = _resultadoAtual.Re,
                    Defeitos = defeitos,
                    Resultado = _resultadoAtual.Resultado,
                    Observacao = txtObservacao.Text,
                    AcaoImediata = txtAcaoImediata.Text
                };

                int id = _inspecaoService.Salvar(inspecao);
                MessageBox.Show($"✅ Inspeção salva com sucesso! ID: {id}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimparTudo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== EVENTOS ==========
        private void txtQuantidade_TextChanged(object sender, EventArgs e) => Calcular();
        private void txtDefeitos_TextChanged(object sender, EventArgs e) => AtualizarResultado();
        private void cmbPlano_SelectedIndexChanged(object sender, EventArgs e) => Calcular();
        private void cmbRegime_SelectedIndexChanged(object sender, EventArgs e) => Calcular();
        private void cmbNivel_SelectedIndexChanged(object sender, EventArgs e) => Calcular();
        private void cmbNQA_SelectedIndexChanged(object sender, EventArgs e) => Calcular();

        private void btnSalvar_Click(object sender, EventArgs e) => Salvar();
        private void btnLimpar_Click(object sender, EventArgs e) => LimparTudo();

        private void btnHistorico_Click(object sender, EventArgs e)
        {
            using var frm = new FrmHistorico();
            frm.ShowDialog();
        }

        private void BtnCadastroFornecedor_Click(object sender, EventArgs e)
        {
            var frm = new FrmFornecedor();
            frm.ShowDialog();
            CarregarCombos();
        }

        private void BtnCadastroMaterial_Click(object sender, EventArgs e)
        {
            var frm = new FrmMaterial();
            frm.ShowDialog();
            CarregarCombos();
        }
             // ========== EVENTO SOBRE ==========
        private void BtnSobre_Click(object sender, EventArgs e)
        {
            string mensagem =
                "📦 SISTEMA IQC - Controle de Qualidade\n\n" +
                "Desenvolvido por: Marciano Campos \n" +
                "Versão: 1.0.0\n" +
                "Data: Julho/2026\n\n" +
                "Tecnologias:\n" +
                "  • C# .NET 8.0\n" +
                "  • Windows Forms\n" +
                "  • SQLite\n" +
                "  • NBR 5426\n\n" +
                "Inspeção de entrada conforme NBR 5426\n" +
                "MVP - Testes e validação";

            MessageBox.Show(mensagem, "ℹ️ Sobre o Sistema IQC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ========== EVENTO CADASTRO INSPETOR ==========  
        private void BtnCadastroInspetor_Click(object sender, EventArgs e)
        {
            var frm = new FrmInspetor();
            frm.ShowDialog();
            CarregarCombos();
        }

    }
        
}

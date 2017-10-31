using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diferimento
{
    public partial class FrmExcluirNota : Form
    {
        public FrmExcluirNota()
        {
            InitializeComponent();
        }

        private void FrmExcluirNota_Load(object sender, EventArgs e)
        {
            cbTipoNota.Items.Add("Venda");
            cbTipoNota.Items.Add("Serviço");

            cbUf.Items.Add("SP");
            cbUf.Items.Add("MS");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtListaNotas.Text))
                MessageBox.Show("A lista de Notas não pode estar vazia!", "Valores incorretos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string[] notas = txtListaNotas.Text.Replace(" ", "").Split(',');
                int[] iNotas = new int[notas.Length];
                for(int i = 0; i < notas.Length; i++)
                {
                    iNotas[i] = System.Convert.ToInt32(notas[i]);
                }

                DAO.NfeDAO nfeDAO = new DAO.NfeDAO();
                if (cbTipoNota.SelectedItem.ToString() == "Venda")
                {
                    if (cbUf.SelectedItem.ToString() == "SP")
                        nfeDAO.DropDanfesSP(iNotas, out string erro);
                    else
                        nfeDAO.DropDanfesMS(iNotas, out string erro);
                }
                else
                {
                    if (cbUf.SelectedItem.ToString() == "SP")
                        nfeDAO.DropServicosSP(iNotas, out string erro);
                    else
                        nfeDAO.DropServicosMS(iNotas, out string erro);
                }
            }

            this.Close();
        }
    }
}

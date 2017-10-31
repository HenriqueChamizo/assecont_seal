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
    public partial class FrmRelatorio : Form
    {
        DataTable datatable;
        public FrmRelatorio()
        {
            InitializeComponent();
        }
        public FrmRelatorio(DataTable table)
        {
            datatable = table;
            InitializeComponent();
        }

        private void FrmRelatorio_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (datatable != null)
            {
                foreach (DataColumn c in datatable.Columns)
                {
                    dataGridView.Columns.Add(c.ColumnName, c.ColumnName);
                }
                foreach (DataRow r in datatable.Rows)
                {
                    dataGridView.Rows.Add(r.ItemArray);
                }
                this.Cursor = Cursors.Default;
                this.Show();
            }
            else
            {
                this.Cursor = Cursors.Default;
                this.Close();
            }
        }
    }
}

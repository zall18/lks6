using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Guna.UI2.Native.WinApi;
using static System.Collections.Specialized.BitVector32;

namespace latihan_lks6
{
    public partial class kasir : Form
    {
        string value;
        SqlConnection conn = connection.Connect();
        SqlCommand cmd;
        SqlDataAdapter dr;
        SqlDataReader rd;
        DataTable dt;
        int number = 1;

        string nama_barang, kode_barang;
        
        
        public void listMenu()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT * from [tbl_barang]", conn);
            rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                menu.Items.Add(rd["kode_barang"].ToString() + " - " + rd["nama_barang"].ToString());
                nama_barang = rd["nama_barang"].ToString();
                kode_barang = rd["kode_barang"].ToString();
            }
            conn.Close();
        }

        public void tabel_load()
        {
            dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("Kode_brang", Type.GetType("System.String"));
            dt.Columns.Add("Nama_barang", Type.GetType("System.String"));
            dt.Columns.Add("Harga_satuan", Type.GetType("System.Int32"));
            dt.Columns.Add("Quantitas", Type.GetType("System.Int32"));
            dt.Columns.Add("Subtotal", Type.GetType("System.String"));
            guna2DataGridView1.DataSource = dt;
        }

        public kasir()
        {
            InitializeComponent();
            listMenu();
            tabel_load();
        }

        private void guna2TextBox3_KeyUp(object sender, KeyEventArgs e)
        {
            value = th.Text.Replace("Rp. ", "");
            value = value.Replace(",00", "");
            value = value.Replace(".", "");
            th.Text = "Rp. " + value.ToString();
            th.SelectionStart = th.Text.Length;
        }

        private void menu_SelectedIndexChanged(object sender, EventArgs e)
        {
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string kode = menu.Text.Split('-')[0];
            cmd.CommandText = "SELECT * FROM [tbl_barang] WHERE kode_barang = @kode";
            cmd.Parameters.AddWithValue("@kode", kode);
            rd = cmd.ExecuteReader();
            rd.Read();
            harga.Text = rd["harga_satuan"].ToString();
            id.Text = rd["id_barang"].ToString();
            conn.Close();
        }

        private void quan_TextChanged(object sender, EventArgs e)
        {
            int hb = int.Parse(harga.Text);
            int q;
            if (int.TryParse(quan.Text, out q))
            {
                int total = hb * q;
                th.Text = total.ToString();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            int total = int.Parse(tb.Text);
            int bayar;

            if (int.TryParse(ub.Text, out bayar))
            {
                if (bayar < total)
                {
                    MessageBox.Show("Pembayaran Kurang!", "FAILED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ub.Text = null;
                }
                else
                {
                    MessageBox.Show("Pembayaran Berhasil!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int kembali = bayar - total;
                    uk.Text = kembali.ToString();

                    tb.Text = "-";
                    ub.Text = null;
                }
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                conn.Open();
                cmd = new SqlCommand("INSERT INTO [tbl_transaksi] VALUES (@no, @tgl, @nama, @tb, @iu, @ib)",conn);
                cmd.Parameters.AddWithValue("@no", generateId.IdTransaksi());
                cmd.Parameters.AddWithValue("@tgl", DateTime.Now);
                cmd.Parameters.AddWithValue("@nama", nama.Text);
                cmd.Parameters.AddWithValue("@tb", dt.Columns["Subtotal"].ToString()[i]);
                cmd.Parameters.AddWithValue("@iu", session.id_user);
                cmd.Parameters.AddWithValue("@ib", id.Text);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            MessageBox.Show("Data berhasil ditambahkan!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void tambah_Click(object sender, EventArgs e)
        {
            string id = "TRS00" + number.ToString();
            dt.Rows.Add(id, kode_barang, nama_barang, harga.Text, quan.Text, th.Text);
            number = number + 1;
            decimal total = 0;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    decimal subtotal = Convert.ToDecimal(row.Cells["subtotal"].Value);
                    total += subtotal;
                }
            }

            th.Text = null;
            quan.Text = null;
            tb.Text = total.ToString();
        }
    }
}

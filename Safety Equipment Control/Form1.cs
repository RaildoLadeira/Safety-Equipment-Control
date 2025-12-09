using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace Safety_Equipment_Control
{
    public partial class Form1 : Form
    {
        const string SEARCH_PLACEHOLDER = "🔍 Search...";

        Dictionary<string, int> materialDurations = new Dictionary<string, int>()
        {
            { "Safety Shoes", 7 }, { "Hard Hat", 6 }, { "Vest", 6 },
            { "Uniform", 12 }, { "Eye Wear", 1 }, { "Raincoat", 12 }
        };

        public Form1()
        {
            InitializeComponent();
            ConfigureForm();
            ConfigureExtraEvents();
            ApplyAppleDesign();
            SetupSearchPlaceholder();
            LoadDataFromDatabase(); // Loads from SQL on startup
        }

        // --- DATABASE LOAD ---
        private void LoadDataFromDatabase()
        {
            try
            {
                gridDados.Rows.Clear();
                using (var context = new SafetyContext())
                {
                    var list = context.EquipmentInventory.ToList();
                    foreach (var item in list)
                    {
                        var result = CheckStatus(item.Material, item.LastIssueDate, item.FirstDate);
                        int index = gridDados.Rows.Add(item.Name, item.FirstDate, item.LastIssueDate, item.Material, item.Quantity, result.status, item.Id);
                        ApplyStatusColor(gridDados.Rows[index], result.color);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
        }

        // --- CONFIGURATION ---
        private void ConfigureForm()
        {
            cmbMaterial.Items.Clear();
            foreach (var item in materialDurations.Keys) cmbMaterial.Items.Add(item);
            if (cmbMaterial.Items.Count > 0) cmbMaterial.SelectedIndex = 0;

            gridDados.Columns.Clear();
            gridDados.ColumnCount = 7; // 7 Columns (Hidden ID)

            gridDados.Columns[0].Name = "Name";
            gridDados.Columns[1].Name = "First Date";
            gridDados.Columns[2].Name = "Last Issue Date";
            gridDados.Columns[3].Name = "Material";
            gridDados.Columns[4].Name = "Quantity";
            gridDados.Columns[5].Name = "Status";
            gridDados.Columns[6].Name = "Id";
            gridDados.Columns[6].Visible = false; // Hide ID

            gridDados.Columns[1].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridDados.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridDados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridDados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDados.ReadOnly = true;
            gridDados.AllowUserToAddRows = false;
            gridDados.ClearSelection();
        }

        private void ConfigureExtraEvents()
        {
            btnAdd.Click -= btnAdd_Click; btnAdd.Click += btnAdd_Click;
            btnUpdate.Click -= btnUpdate_Click; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click -= btnDelete_Click; btnDelete.Click += btnDelete_Click;
            btnLoad.Click -= btnLoad_Click; btnLoad.Click += btnLoad_Click;
            btnSave.Click -= btnSave_Click; btnSave.Click += btnSave_Click;

            Control[] s = this.Controls.Find("textSearch", true);
            if (s.Length > 0)
            {
                TextBox t = (TextBox)s[0];
                t.TextChanged -= textSearch_TextChanged;
                t.TextChanged += textSearch_TextChanged;
            }
            textQuantity.KeyPress -= textQuantity_KeyPress;
            textQuantity.KeyPress += textQuantity_KeyPress;
        }

        // --- BUTTONS ---
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textName.Text)) return;
            try
            {
                using (var context = new SafetyContext())
                {
                    var item = new EquipmentItem()
                    {
                        Name = textName.Text,
                        FirstDate = dtpFirstDate.Value,
                        LastIssueDate = dtpLastDate.Value,
                        Material = cmbMaterial.Text,
                        Quantity = int.Parse(textQuantity.Text),
                        Status = CheckStatus(cmbMaterial.Text, dtpLastDate.Value, dtpFirstDate.Value).status
                    };
                    context.EquipmentInventory.Add(item);
                    context.SaveChanges();
                }
                LoadDataFromDatabase();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (gridDados.SelectedRows.Count == 0) return;
            try
            {
                int id = Convert.ToInt32(gridDados.SelectedRows[0].Cells[6].Value);
                using (var context = new SafetyContext())
                {
                    var item = context.EquipmentInventory.Find(id);
                    if (item != null)
                    {
                        item.Name = textName.Text; item.FirstDate = dtpFirstDate.Value; item.LastIssueDate = dtpLastDate.Value;
                        item.Material = cmbMaterial.Text; item.Quantity = int.Parse(textQuantity.Text);
                        item.Status = CheckStatus(cmbMaterial.Text, dtpLastDate.Value, dtpFirstDate.Value).status;
                        context.SaveChanges();
                    }
                }
                LoadDataFromDatabase();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridDados.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(gridDados.SelectedRows[0].Cells[6].Value);
                    using (var context = new SafetyContext())
                    {
                        var item = context.EquipmentInventory.Find(id);
                        if (item != null) { context.EquipmentInventory.Remove(item); context.SaveChanges(); }
                    }
                    LoadDataFromDatabase();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e) { LoadDataFromDatabase(); }

        private void btnSave_Click(object sender, EventArgs e) { MessageBox.Show("Data is auto-saved to SQL Database!", "Info"); }

        // --- LOGIC ---
        private (string status, Color color) CheckStatus(string material, DateTime requestDate, DateTime firstDate)
        {
            int duration = 6;
            if (materialDurations.ContainsKey(material)) duration = materialDurations[material];
            if (requestDate < firstDate.AddMonths(duration)) return ("Not Good", Color.FromArgb(255, 59, 48));
            else return ("Good", Color.FromArgb(40, 205, 65));
        }

        private void SetupSearchPlaceholder()
        {
            Control[] s = this.Controls.Find("textSearch", true);
            if (s.Length > 0)
            {
                TextBox t = (TextBox)s[0]; t.Text = SEARCH_PLACEHOLDER; t.ForeColor = Color.Gray;
                t.Enter += (sender, e) => { if (t.Text == SEARCH_PLACEHOLDER) { t.Text = ""; t.ForeColor = Color.Black; } };
                t.Leave += (sender, e) => { if (string.IsNullOrWhiteSpace(t.Text)) { t.Text = SEARCH_PLACEHOLDER; t.ForeColor = Color.Gray; } };
            }
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox; if (t == null || t.Text == SEARCH_PLACEHOLDER) return;
            foreach (DataGridViewRow r in gridDados.Rows)
            {
                string n = r.Cells[0].Value?.ToString().ToLower() ?? "";
                string m = r.Cells[3].Value?.ToString().ToLower() ?? "";
                try { r.Visible = n.Contains(t.Text.ToLower()) || m.Contains(t.Text.ToLower()); } catch { }
            }
        }

        private void textQuantity_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; }
        private void gridDados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                textName.Text = gridDados.Rows[e.RowIndex].Cells[0].Value.ToString();
                dtpFirstDate.Value = (DateTime)gridDados.Rows[e.RowIndex].Cells[1].Value;
                dtpLastDate.Value = (DateTime)gridDados.Rows[e.RowIndex].Cells[2].Value;
                cmbMaterial.Text = gridDados.Rows[e.RowIndex].Cells[3].Value.ToString();
                textQuantity.Text = gridDados.Rows[e.RowIndex].Cells[4].Value.ToString();
            }
        }
        private void ClearFields() { textName.Clear(); textQuantity.Clear(); textName.Focus(); }
        private void ApplyStatusColor(DataGridViewRow row, Color c) { row.Cells[5].Style.ForeColor = c; row.Cells[5].Style.SelectionForeColor = c; }

        private void ApplyAppleDesign()
        {
            this.BackColor = Color.White; this.Font = new Font("Segoe UI", 10);
            gridDados.BackgroundColor = Color.White; gridDados.BorderStyle = BorderStyle.None;
            gridDados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 247);
            gridDados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 235, 240);
            gridDados.DefaultCellStyle.SelectionForeColor = Color.Black;
            gridDados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Control[] s = this.Controls.Find("textSearch", true);
            if (s.Length > 0) { s[0].Width = 300; s[0].Anchor = AnchorStyles.Top | AnchorStyles.Left; }

            StyleBtn(btnAdd, Color.FromArgb(0, 122, 255)); StyleBtn(btnUpdate, Color.FromArgb(88, 86, 214));
            StyleBtn(btnDelete, Color.FromArgb(255, 59, 48)); StyleBtn(btnSave, Color.FromArgb(52, 199, 89));
            StyleBtn(btnLoad, Color.FromArgb(142, 142, 147));

            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right; btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right; btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            foreach (Control c in this.Controls)
            {
                if (c is TextBox t) { t.BackColor = Color.FromArgb(242, 242, 247); t.BorderStyle = BorderStyle.FixedSingle; }
                else if (c is ComboBox cb) { cb.FlatStyle = FlatStyle.Flat; cb.BackColor = Color.FromArgb(242, 242, 247); }
            }
        }
        private void StyleBtn(Button b, Color c) { b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0; b.BackColor = c; b.ForeColor = Color.White; b.Font = new Font("Segoe UI", 9, FontStyle.Bold); b.Cursor = Cursors.Hand; }
    }
}
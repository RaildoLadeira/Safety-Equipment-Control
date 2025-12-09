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
        // Search Placeholder Constant
        const string SEARCH_PLACEHOLDER = "🔍 Search...";

        // Material Duration Dictionary (Months)
        Dictionary<string, int> materialDurations = new Dictionary<string, int>()
        {
            { "Safety Shoes", 7 }, { "Hard Hat", 6 }, { "Vest", 6 },
            { "Uniform", 12 }, { "Eye Wear", 1 }, { "Raincoat", 12 }
        };

        public Form1()
        {
            InitializeComponent();
            ConfigureForm();        // Grid Layout & Columns
            ConfigureExtraEvents(); // Event Wiring
            ApplyAppleDesign();     // Visuals & Emojis
            SetupSearchPlaceholder(); // Search Bar Logic
        }

        // --- 1. TABLE CONFIGURATION ---
        private void ConfigureForm()
        {
            // Setup ComboBox
            cmbMaterial.Items.Clear();
            foreach (var item in materialDurations.Keys) cmbMaterial.Items.Add(item);
            if (cmbMaterial.Items.Count > 0) cmbMaterial.SelectedIndex = 0;

            // Clear and Recreate Columns
            gridDados.Columns.Clear();
            gridDados.ColumnCount = 6;

            gridDados.Columns[0].Name = "Name";
            gridDados.Columns[1].Name = "First Date";
            gridDados.Columns[2].Name = "Last Issue Date";
            gridDados.Columns[3].Name = "Material";
            gridDados.Columns[4].Name = "Quantity";
            gridDados.Columns[5].Name = "Status";

            // --- ALIGNMENT & FORMATTING ---

            // Center Alignment
            gridDados.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridDados.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridDados.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridDados.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Date Format (dd/MM/yyyy)
            gridDados.Columns[1].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridDados.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Header Alignment
            gridDados.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridDados.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridDados.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // --- COLUMN SIZING ---
            gridDados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Wider columns
            gridDados.Columns[0].FillWeight = 150; // Name
            gridDados.Columns[3].FillWeight = 120; // Material

            // Compact column
            gridDados.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gridDados.Columns[4].Width = 80; // Quantity

            // General Settings
            gridDados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDados.ReadOnly = true;
            gridDados.AllowUserToAddRows = false;
            gridDados.ClearSelection();
        }

        private void ConfigureExtraEvents()
        {
            // Search Bar Wiring
            Control[] searchControls = this.Controls.Find("textSearch", true);
            if (searchControls.Length > 0)
            {
                TextBox txt = (TextBox)searchControls[0];
                txt.TextChanged -= textSearch_TextChanged;
                txt.TextChanged += textSearch_TextChanged;
            }
            // Quantity Validation Wiring
            textQuantity.KeyPress -= textQuantity_KeyPress;
            textQuantity.KeyPress += textQuantity_KeyPress;

            // Button Wiring (Safety check)
            btnUpdate.Click -= btnUpdate_Click;
            btnUpdate.Click += btnUpdate_Click;
        }

        // --- 2. SEARCH PLACEHOLDER LOGIC ---
        private void SetupSearchPlaceholder()
        {
            Control[] searchControls = this.Controls.Find("textSearch", true);
            if (searchControls.Length > 0)
            {
                TextBox txt = (TextBox)searchControls[0];
                txt.Text = SEARCH_PLACEHOLDER;
                txt.ForeColor = Color.Gray;

                txt.Enter += (s, e) => {
                    if (txt.Text == SEARCH_PLACEHOLDER)
                    {
                        txt.Text = "";
                        txt.ForeColor = Color.Black;
                    }
                };

                txt.Leave += (s, e) => {
                    if (string.IsNullOrWhiteSpace(txt.Text))
                    {
                        txt.Text = SEARCH_PLACEHOLDER;
                        txt.ForeColor = Color.Gray;
                    }
                };
            }
        }

        // --- 3. BUSINESS LOGIC ---
        private (string status, Color color) CheckStatus(string material, DateTime requestDate, DateTime firstDate)
        {
            int durationMonths = 6;
            if (materialDurations.ContainsKey(material)) durationMonths = materialDurations[material];

            DateTime expirationDate = firstDate.AddMonths(durationMonths);

            // Apple Red / Apple Green
            if (requestDate < expirationDate)
                return ("Not Good", Color.FromArgb(255, 59, 48));
            else
                return ("Good", Color.FromArgb(40, 205, 65));
        }

        // --- 4. EVENTS ---
        private void textQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            // Ignore placeholder
            if (txt.Text == SEARCH_PLACEHOLDER) return;

            string term = txt.Text.ToLower();

            foreach (DataGridViewRow row in gridDados.Rows)
            {
                string name = row.Cells[0].Value?.ToString().ToLower() ?? "";
                string material = row.Cells[3].Value?.ToString().ToLower() ?? "";
                try { row.Visible = name.Contains(term) || material.Contains(term); } catch { }
            }
        }

        // --- 5. BUTTONS ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textName.Text) || string.IsNullOrWhiteSpace(textQuantity.Text))
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = CheckStatus(cmbMaterial.Text, dtpLastDate.Value, dtpFirstDate.Value);

            int index = gridDados.Rows.Add(
                textName.Text,
                dtpFirstDate.Value,
                dtpLastDate.Value,
                cmbMaterial.Text,
                textQuantity.Text,
                result.status
            );

            ApplyStatusColor(gridDados.Rows[index], result.color);
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (gridDados.SelectedRows.Count > 0)
            {
                DataGridViewRow row = gridDados.SelectedRows[0];

                // Update Columns Manually
                row.Cells[0].Value = textName.Text;
                row.Cells[1].Value = dtpFirstDate.Value.ToShortDateString();
                row.Cells[2].Value = dtpLastDate.Value.ToShortDateString();
                row.Cells[3].Value = cmbMaterial.Text;
                row.Cells[4].Value = textQuantity.Text;

                // Recalculate
                var result = CheckStatus(cmbMaterial.Text, dtpLastDate.Value, dtpFirstDate.Value);
                row.Cells[5].Value = result.status;

                ApplyStatusColor(row, result.color);

                MessageBox.Show("Updated!", "Success");
                ClearFields();
            }
            else
            {
                MessageBox.Show("Please select a row first", "Warning");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridDados.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Delete item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in gridDados.SelectedRows) gridDados.Rows.Remove(row);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("safety_data.csv"))
                {
                    sw.WriteLine("Name,First,Last,Mat,Qty,Status");
                    foreach (DataGridViewRow r in gridDados.Rows)
                    {
                        if (r.Visible && !r.IsNewRow)
                        {
                            string d1 = DateTime.Parse(r.Cells[1].Value.ToString()).ToShortDateString();
                            string d2 = DateTime.Parse(r.Cells[2].Value.ToString()).ToShortDateString();
                            sw.WriteLine($"{r.Cells[0].Value},{d1},{d2},{r.Cells[3].Value},{r.Cells[4].Value},{r.Cells[5].Value}");
                        }
                    }
                }
                MessageBox.Show("Saved!", "Success");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV|*.csv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        gridDados.Rows.Clear();
                        foreach (string line in File.ReadAllLines(ofd.FileName).Skip(1))
                        {
                            string[] d = line.Split(',');
                            if (d.Length >= 6)
                            {
                                DateTime dt1 = DateTime.Parse(d[1]);
                                DateTime dt2 = DateTime.Parse(d[2]);
                                var res = CheckStatus(d[3], dt2, dt1);

                                int i = gridDados.Rows.Add(d[0], dt1, dt2, d[3], d[4], res.status);
                                ApplyStatusColor(gridDados.Rows[i], res.color);
                            }
                        }
                        MessageBox.Show("Loaded!", "Success");
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                }
            }
        }

        private void gridDados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var c = gridDados.Rows[e.RowIndex].Cells;
                textName.Text = c[0].Value.ToString();
                dtpFirstDate.Value = DateTime.Parse(c[1].Value.ToString());
                dtpLastDate.Value = DateTime.Parse(c[2].Value.ToString());
                cmbMaterial.Text = c[3].Value.ToString();
                textQuantity.Text = c[4].Value.ToString();
            }
        }

        private void ClearFields() { textName.Clear(); textQuantity.Clear(); textName.Focus(); }

        // --- HELPER: APPLY STATUS COLOR ---
        private void ApplyStatusColor(DataGridViewRow row, Color color)
        {
            row.Cells[5].Style.ForeColor = color;
            row.Cells[5].Style.SelectionForeColor = color;
        }

        // ==================================================================================
        // --- 6. APPLE DESIGN (VISUALS) ---
        // ==================================================================================
        private void ApplyAppleDesign()
        {
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Grid Style
            gridDados.BackgroundColor = Color.White;
            gridDados.BorderStyle = BorderStyle.None;
            gridDados.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridDados.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            gridDados.EnableHeadersVisualStyles = false;
            gridDados.RowHeadersVisible = false;

            // Headers
            gridDados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 247);
            gridDados.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            gridDados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            gridDados.ColumnHeadersHeight = 40;

            // Rows
            gridDados.DefaultCellStyle.BackColor = Color.White;
            gridDados.DefaultCellStyle.ForeColor = Color.Black;
            gridDados.RowTemplate.Height = 35;
            gridDados.GridColor = Color.FromArgb(230, 230, 230);

            // Soft Selection
            gridDados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 235, 240);
            gridDados.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Anchors (Fixes Maximizing)
            gridDados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Fix Search Bar
            Control[] s = this.Controls.Find("textSearch", true);
            if (s.Length > 0)
            {
                s[0].Width = 300;
                s[0].Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            // --- BUTTONS WITH EMOJIS ---
            btnAdd.Text = "➕  Add";
            StyleAppleButton(btnAdd, Color.FromArgb(0, 122, 255)); // Blue

            btnUpdate.Text = "✏️  Update";
            StyleAppleButton(btnUpdate, Color.FromArgb(88, 86, 214)); // Purple

            btnDelete.Text = "🗑️  Delete";
            StyleAppleButton(btnDelete, Color.FromArgb(255, 59, 48)); // Red

            btnSave.Text = "💾  Save";
            StyleAppleButton(btnSave, Color.FromArgb(52, 199, 89)); // Green

            btnLoad.Text = "📂  Load";
            StyleAppleButton(btnLoad, Color.FromArgb(142, 142, 147)); // Gray

            // Button Anchors (Right Aligned)
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // INPUTS
            foreach (Control c in this.Controls)
            {
                if (c is Label l) { l.ForeColor = Color.Black; l.Font = new Font("Segoe UI", 9, FontStyle.Bold); }
                else if (c is TextBox t) { t.BackColor = Color.FromArgb(242, 242, 247); t.BorderStyle = BorderStyle.FixedSingle; }
                else if (c is ComboBox cb) { cb.FlatStyle = FlatStyle.Flat; cb.BackColor = Color.FromArgb(242, 242, 247); }
                else if (c is DateTimePicker dtp) { dtp.Format = DateTimePickerFormat.Short; }
            }
        }

        private void StyleAppleButton(Button btn, Color color)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }
    }
}
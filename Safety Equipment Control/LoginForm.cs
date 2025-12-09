using System;
using System.Drawing;
using System.Windows.Forms;

namespace Safety_Equipment_Control
{
    public partial class LoginForm : Form
    {
        // Define controls globally so we can access them
        private CheckBox chkShowPass;
        private Label lblLogo;
        private Button btnExit;

        // Placeholder Texts
        private const string PLACEHOLDER_USER = "Username";
        private const string PLACEHOLDER_PASS = "Password";

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
            ApplyAppleLoginDesign();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Focus on title initially so the placeholders stay visible 
            // instead of clearing the username immediately
            lblLogo.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Check if the user typed something real, or if it's just the placeholder
            string user = txtUser.Text == PLACEHOLDER_USER ? "" : txtUser.Text;
            string pass = txtPass.Text == PLACEHOLDER_PASS ? "" : txtPass.Text;

            // --- CREDENTIALS ---
            string correctUsername = "admin";
            string correctPassword = "1234";

            if (user == correctUsername && pass == correctPassword)
            {
                this.Hide();
                Form1 mainSystem = new Form1();
                mainSystem.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Access Denied.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Reset Password Field
                txtPass.Text = PLACEHOLDER_PASS;
                txtPass.ForeColor = Color.Gray;
                txtPass.PasswordChar = '\0'; // Show text "Password"
            }
        }

        // Toggle Password Visibility
        private void ChkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            // Only toggle if it's NOT the placeholder text
            if (txtPass.Text != PLACEHOLDER_PASS)
            {
                txtPass.PasswordChar = chkShowPass.Checked ? '\0' : '●';
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ==================================================================================
        // --- APPLE PREMIUM DESIGN + PLACEHOLDER LOGIC ---
        // ==================================================================================
        private void ApplyAppleLoginDesign()
        {
            // 1. Window Settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(350, 480);
            this.BackColor = Color.White;

            // 2. Logo (Emoji)
            lblLogo = new Label();
            lblLogo.Text = "🛡️";
            lblLogo.Font = new Font("Segoe UI Emoji", 40, FontStyle.Regular);
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point((this.Width - 80) / 2, 40);
            this.Controls.Add(lblLogo);

            // 3. Title
            Control[] titles = this.Controls.Find("lblTitle", true);
            if (titles.Length > 0)
            {
                Label title = (Label)titles[0];
                title.Text = "Safety Equipament Control";
                title.ForeColor = Color.Black;
                title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                title.AutoSize = false;
                title.TextAlign = ContentAlignment.MiddleCenter;
                title.Size = new Size(this.Width, 30);
                title.Location = new Point(0, 110);
            }

            // Hide old labels
            foreach (Control c in this.Controls)
            {
                if (c is Label && c.Name != "lblTitle" && c != lblLogo) c.Visible = false;
            }

            int centerX = (this.Width - 250) / 2;

            // 4. SETUP TEXTBOXES WITH PLACEHOLDERS
            StyleInput(txtUser, centerX, 160);
            SetupPlaceholder(txtUser, PLACEHOLDER_USER, false); // False = Not Password

            StyleInput(txtPass, centerX, 210);
            SetupPlaceholder(txtPass, PLACEHOLDER_PASS, true);  // True = Is Password

            // 5. Show Password Checkbox
            chkShowPass = new CheckBox();
            chkShowPass.Text = "Show Password";
            chkShowPass.Font = new Font("Segoe UI", 8);
            chkShowPass.ForeColor = Color.Gray;
            chkShowPass.Location = new Point(centerX, 245);
            chkShowPass.AutoSize = true;
            chkShowPass.Cursor = Cursors.Hand;
            chkShowPass.CheckedChanged += ChkShowPass_CheckedChanged;
            this.Controls.Add(chkShowPass);

            // 6. Login Button
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.BackColor = Color.FromArgb(0, 122, 255);
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Text = "Sign In";
            btnLogin.Size = new Size(250, 40);
            btnLogin.Location = new Point(centerX, 280);

            // 7. Exit Button
            btnExit = new Button();
            btnExit.Text = "Exit Application";
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.BackColor = Color.White;
            btnExit.ForeColor = Color.Gray;
            btnExit.Font = new Font("Segoe UI", 9, FontStyle.Underline);
            btnExit.Cursor = Cursors.Hand;
            btnExit.Size = new Size(150, 30);
            btnExit.Location = new Point((this.Width - 150) / 2, 380);
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);
        }

        private void StyleInput(TextBox txt, int x, int y)
        {
            if (txt == null) return;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 11);
            txt.BackColor = Color.FromArgb(245, 245, 247);
            txt.Size = new Size(250, 30);
            txt.Location = new Point(x, y);
        }

        // --- PLACEHOLDER LOGIC ---
        private void SetupPlaceholder(TextBox txt, string placeholderText, bool isPassword)
        {
            // Initial State
            txt.Text = placeholderText;
            txt.ForeColor = Color.Gray;
            if (isPassword) txt.PasswordChar = '\0'; // Show text initially

            // Event: User Clicks/Enters
            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholderText)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                    if (isPassword) txt.PasswordChar = '●'; // Hide chars
                }
            };

            // Event: User Leaves
            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholderText;
                    txt.ForeColor = Color.Gray;
                    if (isPassword) txt.PasswordChar = '\0'; // Show "Password" text
                }
            };
        }
    }
}
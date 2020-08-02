namespace DirectoryInfoCollector
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.checkBox6 = new System.Windows.Forms.CheckBox();
      this.checkBox5 = new System.Windows.Forms.CheckBox();
      this.checkBox4 = new System.Windows.Forms.CheckBox();
      this.checkBox3 = new System.Windows.Forms.CheckBox();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.checkBox2 = new System.Windows.Forms.CheckBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.textBox4 = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.groupBox2);
      this.groupBox1.Controls.Add(this.textBox2);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Controls.Add(this.checkBox1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.textBox1);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(520, 207);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Collect folder info:";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.checkBox6);
      this.groupBox2.Controls.Add(this.checkBox5);
      this.groupBox2.Controls.Add(this.checkBox4);
      this.groupBox2.Controls.Add(this.checkBox3);
      this.groupBox2.Controls.Add(this.comboBox1);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.textBox3);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Controls.Add(this.checkBox2);
      this.groupBox2.Location = new System.Drawing.Point(9, 99);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(505, 103);
      this.groupBox2.TabIndex = 7;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Database options:";
      // 
      // checkBox6
      // 
      this.checkBox6.AutoSize = true;
      this.checkBox6.Location = new System.Drawing.Point(280, 19);
      this.checkBox6.Name = "checkBox6";
      this.checkBox6.Size = new System.Drawing.Size(210, 17);
      this.checkBox6.TabIndex = 9;
      this.checkBox6.Text = "Fix files with corrupted Digital Signature";
      this.checkBox6.UseVisualStyleBackColor = true;
      // 
      // checkBox5
      // 
      this.checkBox5.AutoSize = true;
      this.checkBox5.Location = new System.Drawing.Point(280, 48);
      this.checkBox5.Name = "checkBox5";
      this.checkBox5.Size = new System.Drawing.Size(218, 17);
      this.checkBox5.TabIndex = 8;
      this.checkBox5.Text = "Only files with corrupted Digital Signature";
      this.checkBox5.UseVisualStyleBackColor = true;
      // 
      // checkBox4
      // 
      this.checkBox4.AutoSize = true;
      this.checkBox4.Location = new System.Drawing.Point(141, 48);
      this.checkBox4.Name = "checkBox4";
      this.checkBox4.Size = new System.Drawing.Size(135, 17);
      this.checkBox4.TabIndex = 7;
      this.checkBox4.Text = "Only non-Microsoft files";
      this.checkBox4.UseVisualStyleBackColor = true;
      // 
      // checkBox3
      // 
      this.checkBox3.AutoSize = true;
      this.checkBox3.Checked = true;
      this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBox3.Location = new System.Drawing.Point(6, 19);
      this.checkBox3.Name = "checkBox3";
      this.checkBox3.Size = new System.Drawing.Size(129, 17);
      this.checkBox3.TabIndex = 6;
      this.checkBox3.Text = "Exclude empty folders";
      this.checkBox3.UseVisualStyleBackColor = true;
      // 
      // comboBox1
      // 
      this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "Ticks",
            "String"});
      this.comboBox1.Location = new System.Drawing.Point(74, 45);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(55, 21);
      this.comboBox1.TabIndex = 5;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 48);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(65, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Time format:";
      // 
      // textBox3
      // 
      this.textBox3.Location = new System.Drawing.Point(63, 77);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new System.Drawing.Size(436, 20);
      this.textBox3.TabIndex = 3;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 80);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(54, 13);
      this.label4.TabIndex = 2;
      this.label4.Text = "Comment:";
      // 
      // checkBox2
      // 
      this.checkBox2.AutoSize = true;
      this.checkBox2.Location = new System.Drawing.Point(141, 19);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new System.Drawing.Size(133, 17);
      this.checkBox2.TabIndex = 0;
      this.checkBox2.Text = "Use lower-case names";
      this.checkBox2.UseVisualStyleBackColor = true;
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(64, 48);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(450, 20);
      this.textBox2.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 51);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(54, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "File mask:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(205, 75);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(64, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "levels deep.";
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(130, 73);
      this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(69, 20);
      this.numericUpDown1.TabIndex = 3;
      this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Checked = true;
      this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBox1.Location = new System.Drawing.Point(9, 74);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(115, 17);
      this.checkBox1.TabIndex = 2;
      this.checkBox1.Text = "Include subfolders:";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(39, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Folder:";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(64, 22);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(450, 20);
      this.textBox1.TabIndex = 0;
      this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.textBox4);
      this.groupBox3.Controls.Add(this.label6);
      this.groupBox3.Location = new System.Drawing.Point(12, 225);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(520, 50);
      this.groupBox3.TabIndex = 1;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Database:";
      // 
      // textBox4
      // 
      this.textBox4.Location = new System.Drawing.Point(64, 19);
      this.textBox4.Name = "textBox4";
      this.textBox4.Size = new System.Drawing.Size(450, 20);
      this.textBox4.TabIndex = 1;
      this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 22);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(32, 13);
      this.label6.TabIndex = 0;
      this.label6.Text = "Path:";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.textBox5);
      this.groupBox4.Controls.Add(this.label7);
      this.groupBox4.Location = new System.Drawing.Point(12, 281);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(520, 50);
      this.groupBox4.TabIndex = 2;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Compare to another database:";
      // 
      // textBox5
      // 
      this.textBox5.Location = new System.Drawing.Point(64, 19);
      this.textBox5.Name = "textBox5";
      this.textBox5.Size = new System.Drawing.Size(450, 20);
      this.textBox5.TabIndex = 1;
      this.textBox5.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 22);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(32, 13);
      this.label7.TabIndex = 0;
      this.label7.Text = "Path:";
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(457, 337);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Start";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label8
      // 
      this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(9, 342);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(38, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "Ready";
      // 
      // backgroundWorker1
      // 
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(544, 370);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.Text = "Directory Info Collector";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.CheckBox checkBox3;
    private System.Windows.Forms.CheckBox checkBox5;
    private System.Windows.Forms.CheckBox checkBox4;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.CheckBox checkBox6;
  }
}


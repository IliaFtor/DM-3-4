using System.Drawing;
using System;
using System.Windows.Forms;

namespace DM___5
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private TextBox textBox2;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            // DataGridView
            dataGridView1 = new DataGridView();
            dataGridView1.Dock = DockStyle.Fill;

            // TextBox1
            textBox1 = new TextBox();
            textBox1.Location = new Point(10, 320);
            textBox1.Size = new Size(50, 20);
            textBox1.TextChanged += textBox1_TextChanged;

            // TextBox2
            textBox2 = new TextBox();
            textBox2.Location = new Point(70, 320);
            textBox2.Size = new Size(50, 20);

            // Button
            button1 = new Button();
            button1.Location = new Point(170, 320);
            button1.Size = new Size(75, 23);
            button1.Text = "Вычислить";
            button1.Click += new EventHandler(button1_Click);

            // Label
            label1 = new Label();
            label1.Location = new Point(10, 350);
            label1.Size = new Size(300, 550);

            // Добавление элементов управления на форму
            Controls.Add(dataGridView1);
            Controls.Add(textBox1);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(label1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}



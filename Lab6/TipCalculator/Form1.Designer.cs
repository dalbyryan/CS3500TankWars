namespace TipCalculator
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
            this.enterTotalBillText = new System.Windows.Forms.Label();
            this.computeTipButton = new System.Windows.Forms.Button();
            this.enterTotalBillInput = new System.Windows.Forms.TextBox();
            this.computeTipOutput = new System.Windows.Forms.TextBox();
            this.enterTip = new System.Windows.Forms.Label();
            this.tipPercentageInput = new System.Windows.Forms.TextBox();
            this.totalToPayOutput = new System.Windows.Forms.TextBox();
            this.totalToPayText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // enterTotalBillText
            // 
            this.enterTotalBillText.AutoSize = true;
            this.enterTotalBillText.Location = new System.Drawing.Point(84, 76);
            this.enterTotalBillText.Name = "enterTotalBillText";
            this.enterTotalBillText.Size = new System.Drawing.Size(141, 25);
            this.enterTotalBillText.TabIndex = 0;
            this.enterTotalBillText.Text = "enter total bill";
            // 
            // computeTipButton
            // 
            this.computeTipButton.Location = new System.Drawing.Point(77, 140);
            this.computeTipButton.Name = "computeTipButton";
            this.computeTipButton.Size = new System.Drawing.Size(171, 89);
            this.computeTipButton.TabIndex = 1;
            this.computeTipButton.Text = "compute tip";
            this.computeTipButton.UseVisualStyleBackColor = true;
            this.computeTipButton.Click += new System.EventHandler(this.ComputeTipButton_Click);
            // 
            // enterTotalBillInput
            // 
            this.enterTotalBillInput.Location = new System.Drawing.Point(358, 70);
            this.enterTotalBillInput.Name = "enterTotalBillInput";
            this.enterTotalBillInput.Size = new System.Drawing.Size(296, 31);
            this.enterTotalBillInput.TabIndex = 2;
            this.enterTotalBillInput.Text = "enter text here";
            this.enterTotalBillInput.TextChanged += new System.EventHandler(this.EnterTotalBillInput_TextChanged);
            // 
            // computeTipOutput
            // 
            this.computeTipOutput.Location = new System.Drawing.Point(358, 187);
            this.computeTipOutput.Name = "computeTipOutput";
            this.computeTipOutput.Size = new System.Drawing.Size(296, 31);
            this.computeTipOutput.TabIndex = 3;
            // 
            // enterTip
            // 
            this.enterTip.AutoSize = true;
            this.enterTip.Location = new System.Drawing.Point(72, 278);
            this.enterTip.Name = "enterTip";
            this.enterTip.Size = new System.Drawing.Size(204, 25);
            this.enterTip.TabIndex = 4;
            this.enterTip.Text = "enter tip percentage";
            // 
            // tipPercentageInput
            // 
            this.tipPercentageInput.Location = new System.Drawing.Point(464, 278);
            this.tipPercentageInput.Name = "tipPercentageInput";
            this.tipPercentageInput.Size = new System.Drawing.Size(100, 31);
            this.tipPercentageInput.TabIndex = 5;
            // 
            // totalToPayOutput
            // 
            this.totalToPayOutput.Location = new System.Drawing.Point(382, 380);
            this.totalToPayOutput.Name = "totalToPayOutput";
            this.totalToPayOutput.Size = new System.Drawing.Size(100, 31);
            this.totalToPayOutput.TabIndex = 6;
            // 
            // totalToPayText
            // 
            this.totalToPayText.AutoSize = true;
            this.totalToPayText.Location = new System.Drawing.Point(118, 366);
            this.totalToPayText.Name = "totalToPayText";
            this.totalToPayText.Size = new System.Drawing.Size(118, 25);
            this.totalToPayText.TabIndex = 7;
            this.totalToPayText.Text = "total to pay";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.totalToPayText);
            this.Controls.Add(this.totalToPayOutput);
            this.Controls.Add(this.tipPercentageInput);
            this.Controls.Add(this.enterTip);
            this.Controls.Add(this.computeTipOutput);
            this.Controls.Add(this.enterTotalBillInput);
            this.Controls.Add(this.computeTipButton);
            this.Controls.Add(this.enterTotalBillText);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label enterTotalBillText;
        private System.Windows.Forms.Button computeTipButton;
        private System.Windows.Forms.TextBox enterTotalBillInput;
        private System.Windows.Forms.TextBox computeTipOutput;
        private System.Windows.Forms.Label enterTip;
        private System.Windows.Forms.TextBox tipPercentageInput;
        private System.Windows.Forms.TextBox totalToPayOutput;
        private System.Windows.Forms.Label totalToPayText;
    }
}


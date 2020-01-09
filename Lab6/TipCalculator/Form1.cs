using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string totalBillTextBox;

        private void ComputeTipButton_Click(object sender, EventArgs e)
        {
            string totalBillString = enterTotalBillInput.Text;
            Double.TryParse(totalBillString, out double totalBill);

            string tipPercentageString = tipPercentageInput.Text;
            Double.TryParse(tipPercentageString, out double tipPercentage);

            double tip = totalBill * tipPercentage;

            string tipText = tip.ToString();
            computeTipOutput.Text = tipText;

            double totalToPayDouble = totalBill + tip;
            totalToPayOutput.Text = totalToPayDouble.ToString();

        }

        private void EnterTotalBillInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

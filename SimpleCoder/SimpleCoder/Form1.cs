using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void кодироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            //int mod = 0;
            string temp = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] >= 32 && input[i] <= 64) { temp += (char)(input[i] - 31); }
                if (input[i] >= 97 && input[i] <= 122) { temp += (char)(input[i] - 63); }
                if (input[i] >= 65 && input[i] <= 90) { temp += (char)(60); temp += (char)(input[i] - 31); }
            }
            while (temp.Length % 4 != 0)
                temp += (char)(1);

            string output = "";
            for (int i = 0; i < temp.Length; i += 4)
            {
                output += (char)(temp[i] << 2 | temp[i + 1] >> 4);
                output += (char)(temp[i + 1] << 4 | temp[i + 2] >> 2);
                output += (char)(temp[i + 2] << 6 | temp[i + 3]);
            }
            textBox2.Text = output;
        }




        private void декодироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string temp = "";
            for(int i=0; i < input.Length; i += 3)
            {
                temp += (char)(input[i] >> 2);
                temp += (char)(input[i] << 4 & 63 | input[i + 1] >> 4);
                temp += (char)(input[i + 1] << 2 & 63 | input[i + 2] >> 6);
                temp += (char)(input[i + 2] & 63);
            }
            string output="";
            for (int i=0; i<temp.Length; i++)
            {
                if (temp[i] == 60)
                {
                    if (temp[i + 1] >= 34 && temp[i + 1] <= 59) { output += (char)(temp[i+1] + 31); i++; }
                }
                else
                {
                    if (temp[i] >= 1 && temp[i] <= 33) { output += (char)(temp[i] + 31); }
                    if (temp[i] >= 34 && temp[i] <= 59) { output += (char)(temp[i] + 63); }
                }
            }
            textBox2.Text = output;

        }
    }
}

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
            // Получаем текст
            string input = textBox1.Text;
            // Кодируем его, чтобы освободить два старших бита
            string temp = coding(input);
            // Сжимаем текст
            string output = compress(temp);
            // Выводим результат
            textBox2.Text = output;
        }

        string coding(string input)
        {
            string temp = "";
            string missed = "";
            int mod = 0;
            // Проверяем каждый символ
            for (int i = 0; i < input.Length; i++)
            {
                // Если поступивший символ является разрешённым:
                if ((input[i]>=' '&&input[i]<='Z')|| (input[i] >= 'a' && input[i] <= 'z')|| (input[i] >= 'А' && input[i] <= 'п')|| (input[i] >= 'р' && input[i] <= 'я')) {
                    // Проверка на раскладку:
                    if (input[i] >= '8')
                    {
                        if (mod == 0 && input[i] >= 'А') { mod = 1; temp += (char)(63); }
                        if (mod == 1 && input[i] < 'А') { mod = 0; temp += (char)(63); }
                    }
                    // Непосредственно кодирование
                    if (input[i] >= ' ' && input[i] <= '@') { temp += (char)(input[i] - 31); }
                    else if (input[i] >= 'a' && input[i] <= 'z') { temp += (char)(input[i] - 63); }
                    else if (input[i] >= 'а' && input[i] <= 'я') { temp += (char)(input[i] - 1044); }
                    // Большие == (62)+маленькие
                    else if (input[i] >= 'A' && input[i] <= 'Z') { temp += (char)(62); temp += (char)(input[i] - 31); }
                    else if (input[i] >= 'А' && input[i] <= 'Я') { temp += (char)(62); temp += (char)(input[i] - 1012); }
                }
                else { missed += input[i]; }
            }
            // Добавляем пробелы, чтобы число символов стало кратно четырём
            while (temp.Length % 4 != 0)
                temp += (char)(1);
            // Возвращаем результат
            // Проверка на наличие потеряных символов
            if (missed.Length > 0)
            {
                label1.Visible = true;
                textBox3.Text = missed;
                textBox3.Visible = true;
            }
            else
            {
                label1.Visible = false;
                textBox3.Visible = false;
                textBox3.Text = "";
            }
            return temp;
        }

        string compress(string temp)
        {
            string output = "";
            // Выбираем символы пачками по 4 штуки
            for (int i = 0; i < temp.Length; i += 4)
            {
                // Кодируем каждые 4 символа в 3
                output += (char)(temp[i] << 2 | temp[i + 1] >> 4);
                output += (char)(temp[i + 1] << 4 | temp[i + 2] >> 2);
                output += (char)(temp[i + 2] << 6 | temp[i + 3]);
            }
            // Возвращаем результат
            return output;
        }



        private void декодироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Получаем закодированный текст
            string input = textBox1.Text;
            // Разворачиваем каждые 3 символа в 4
            string temp = decompress(input);     
            // Декодируем текст       
            string output=decoding(temp);            
            // Возвращаем результат
            textBox2.Text = output;
        }

        string decompress(string input)
        {
            // Проверка на корректность входных данных
            if (input.Length % 3 != 0) { return "<*/165\u0001<&3303\u0001\u0001\u0001"; }
            // Это закодированное "input error"
            string temp = "";
            // Превращаем каждые 3 символа обратно в 4
            for (int i = 0; i < input.Length; i += 3)
            {
                temp += (char)(input[i] >> 2);
                temp += (char)(input[i] << 4 & 63 | input[i + 1] >> 4);
                temp += (char)(input[i + 1] << 2 & 63 | input[i + 2] >> 6);
                temp += (char)(input[i + 2] & 63);
            }
            // Возвращаем то, что получилось
            return temp;
        }

        string decoding(string temp)
        {
            string output = "";
            int mod = 0;
            //Проверяем каждый символ
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == 63) { mod = (mod + 1) % 2; }
                // Если встретили символ, означающий заглавную букву
                if (temp[i] == 62)
                {
                    // Заглавные латиница
                    if (temp[i + 1] >= 34 && temp[i + 1] <= 59 && mod==0) { output += (char)(temp[i + 1] + 31); i++; }
                    // Заглавные Кириллица
                    if (temp[i + 1] >= 28 && temp[i + 1] <= 59 && mod==1) { output += (char)(temp[i + 1] + 1012); i++; }
                }
                // Если не встретили символ, означающий заглавную букву
                else
                {
                    // Символы до 8
                    if (temp[i] >= 1 && temp[i] <= 27) { output += (char)(temp[i] + 31); }
                    // Символы начиная с 8
                    if (temp[i] >= 28 && temp[i] <= 33 && mod==0) { output += (char)(temp[i] + 31); }
                    // Буквы на латинице
                    if (temp[i] >= 34 && temp[i] <= 59 && mod==0) { output += (char)(temp[i] + 63); }
                    // Буквы на Кириллице
                    if (temp[i] >= 28 && temp[i] <= 59 && mod == 1) { output += (char)(temp[i] + 1044); }
                    // P.S. Даже не спрашивай, откуда такие дикие числа
                }
            }
            // Возвращаем полученный текст
            return output;
        }
    }
}

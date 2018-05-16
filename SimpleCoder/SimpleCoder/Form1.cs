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
using System.Runtime.InteropServices;

namespace SimpleCoder
{
    public partial class Form1 : Form
    {
        UnicodeEncoding encoder;

        public Form1()
        {
            InitializeComponent();
            encoder = new UnicodeEncoding(false, true, true); //Для разбития строки на байты
        }

        private void кодироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
            catch
            {
                textBox2.Text = "Произошла какая-то ошибка. Попробуйте закодировать другой текст.";
            }
        }

        string coding(string input)
        {
            string temp = "";
            string missed = "";
            int mod = 0;
            //Вывод байтов оригинального слова
            byte[] original_bytes = encoder.GetBytes(input);
            Console.Write("Original bytes: ");
            foreach (byte b in original_bytes)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
            // Проверяем каждый символ
            for (int i = 0; i < input.Length; i++)
            {
                // Если поступивший символ является разрешённым:
                if ((input[i] >= ' ' && input[i] <= 'Z') || (input[i] >= 'a' && input[i] <= 'z') || (input[i] >= 'А' && input[i] <= 'п') || (input[i] >= 'р' && input[i] <= 'я'))
                {
                    // Проверка на раскладку:
                    if (input[i] >= '8')
                    {
                        if (mod == 0 && input[i] >= 'А' && input[i] <= 'я') { mod = 1; temp += (char)(63); }
                        if (mod == 1 && input[i] >= 'A' && input[i] <= 'Z') { mod = 0; temp += (char)(63); }
                        if (mod == 1 && input[i] >= 'a' && input[i] <= 'z') { mod = 0; temp += (char)(63); }
                    }

                    // Непосредственно кодирование
                    if (input[i] >= ' ' && input[i] <= '@') { temp += (char)(input[i] - 31); }
                    else if (input[i] >= 'a' && input[i] <= 'z') { temp += (char)(input[i] - 63); }
                    else if (input[i] >= 'а' && input[i] <= 'я') { temp += (char)(input[i] - 1044); }
                    // Большие == (62)+маленькие
                    else if (input[i] >= 'A' && input[i] <= 'Z') { temp += (char)(62); temp += (char)(input[i] - 31); }
                    else if (input[i] >= 'А' && input[i] <= 'Я') { temp += (char)(62); temp += (char)(input[i] - 1012); }
                }
                else //Добавляем в список пропущенных
                {
                    missed += input[i];
                    temp += (char)(61);
                    temp += input[i];
                }
            }

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
            //Вывод байтов закодированного слова
            byte[] encoded_bytes = encoder.GetBytes(temp);
            Console.Write("Encoded bytes: ");
            foreach (byte b in encoded_bytes)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
            // Возвращаем результат
            return temp;
        }

        string compress(string temp)
        {
            string output = "";
            // Алгоритм пошагового сжатия
            // Сдвиг
            int move = 0;
            // Создаём нулевой символ   
            char tchar = '\0';
            // Преобразуем каждый символ
            for (int i = 0; i < temp.Length; i++)
            {
                // Увеличиваем сдвиг на длину новой буквы
                move += 6;
                // Если не вышли за пределы
                if (move < 16)
                {
                    // то просто записываем
                    tchar = (char)((temp[i]) << (16 - move) | tchar);
                }
                else
                {
                    // Иначе дробим и записываем в два символа
                    move -= 16;
                    tchar = (char)((temp[i]) >> (move) | tchar);
                    output += tchar;
                    tchar = (char)((temp[i]) << (16 - move));
                }
                // Вписываем необрабатываемые символы:
                if (temp[i] == 61)
                {
                    // Переходим на следующий символ
                    i++;
                    // Записываем символ
                    tchar = (char)((temp[i]) >> (move) | tchar);
                    output += tchar;
                    tchar = (char)((temp[i]) << (16 - move));
                }
            }
            // Если остался неполный символ, записываем и его
            if (tchar != '\0') output += tchar;
            //Вывод байтов сжатого закодированного слова
            byte[] compressed_bytes = encoder.GetBytes(output);
            Console.Write("Compressed bytes: ");
            foreach (byte b in compressed_bytes)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
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
            string output = decoding(temp);
            // Возвращаем результат
            textBox2.Text = output;
        }

        string decompress(string input)
        {
            // Алгоритм пошаговой распаковки
            string temp = "";
            // Добавляем в конец пустой символ, чтобы не уйти за пределы массива
            input += '\0';
            // Сдвиг
            int move = 0;
            // Обрабаываем каждый символ
            int i = 0;
            while (i < input.Length - 1)
            {
                // Увеличиваем сдвиг
                move += 6;
                // Если не вышли за пределы символа
                if (move < 16)
                {
                    // Вырезаем нужную нам букву
                    temp += (char)((input[i] >> (16 - move)) & 63);
                }
                else
                {
                    // Если вышли, уменьшаем сдвиг
                    move -= 16;
                    // Вырезаем букву из двух других и скрепляем
                    temp += (char)((input[i] << (move) & 63 | input[i + 1] >> (16 - move)));
                    // Переходим к следующей букве
                    i++;
                }
                // Проверяем на незашифрованные
                if (temp[temp.Length - 1] == 61)
                {
                    temp += (char)((input[i] << (move) | input[i + 1] >> (16 - move)));
                    i++;
                }
            }
            //Вывод байтов расжатого закодированного слова
            byte[] decompressed_bytes = encoder.GetBytes(temp);
            Console.Write("Decompressed bytes: ");
            foreach (byte b in decompressed_bytes)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
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
                if (temp[i] == 61) { output += temp[i + 1]; i++; continue; }
                if (temp[i] == 63) { mod = (mod + 1) % 2; }
                // Если встретили символ, означающий заглавную букву
                if (temp[i] == 62)
                {
                    // Заглавные латиница
                    if (temp[i + 1] >= 34 && temp[i + 1] <= 59 && mod == 0) { output += (char)(temp[i + 1] + 31); i++; }
                    // Заглавные Кириллица
                    if (temp[i + 1] >= 28 && temp[i + 1] <= 59 && mod == 1) { output += (char)(temp[i + 1] + 1012); i++; }
                }
                // Если не встретили символ, означающий заглавную букву
                else
                {
                    // Символы до 8
                    if (temp[i] >= 1 && temp[i] <= 27) { output += (char)(temp[i] + 31); }
                    // Символы начиная с 8
                    else if (temp[i] >= 28 && temp[i] <= 33 && mod == 0) { output += (char)(temp[i] + 31); }
                    // Буквы на латинице
                    else if (temp[i] >= 34 && temp[i] <= 59 && mod == 0) { output += (char)(temp[i] + 63); }
                    // Буквы на Кириллице
                    else if (temp[i] >= 28 && temp[i] <= 59 && mod == 1) { output += (char)(temp[i] + 1044); }
                    // P.S. Даже не спрашивай, откуда такие дикие числа
                }
            }
            //Вывод байтов раскодированного слова
            byte[] decoded_bytes = encoder.GetBytes(output);
            Console.Write("Decoded bytes: ");
            foreach (byte b in decoded_bytes)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
            // Возвращаем полученный текст
            return output;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

#if DEBUG
            //Создание консоли для отладки
            AllocConsole();
#endif
        }
        //Если компилим в релиз конфигурации не привязывать консоль
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif
    }
}

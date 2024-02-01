using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace products
{
    public partial class Form1 : Form
    {
        Database database = new Database();
        string[] prod_types = { "Все типы", "Оружие", "Украшение", "Музыкальный инструмент" };
        string[] prod_cost = { "По возрастанию", "По убыванию" };
        public Form1()
        {
            InitializeComponent();

            foreach (string types in prod_types)
            {
                comboBoxFilter.Items.Add(types);
            }

            foreach (string costs in prod_cost)
            {
                comboBoxSort.Items.Add(costs);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns(dataGridView1);
            RefreshDataGrid(dataGridView1);
        }

        private void CreateColumns(DataGridView dataGridView1)
        {
            //dataGridView1.Columns.Add("picture", "Изображение");
            dataGridView1.Columns.Add("article", "Артикул");
            dataGridView1.Columns.Add("prod_name", "Наименование продукта");
            dataGridView1.Columns.Add("cost", "Стоимость");
            dataGridView1.Columns.Add("type_prod", "Категория");
        }

        public void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt64(0), record.GetString(1), record.GetInt32(2), record.GetString(3));
        }

        public void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from items";

            NpgsqlCommand command = new NpgsqlCommand(queryString, database.GetConnection());

            database.OpenConnection();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
            database.CloseConnection();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string query = $"select * from items where concat (prod_name, cost) like '%" + textBoxSearch.Text + "%'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());

            database.OpenConnection();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
            database.CloseConnection();
        }

        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSort.SelectedItem == "По возрастанию")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }

            if (comboBoxSort.SelectedItem == "По убыванию")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Descending);
            }
        }

        public void AllTypes(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string queryString = $"select * from items";
            NpgsqlCommand comm = new NpgsqlCommand(queryString, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }

        public void Wearon(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string queryString = $"select * from items where type_prod LIKE 'Оружие'";
            NpgsqlCommand comm = new NpgsqlCommand(queryString, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }
        private void Decor(DataGridView dgw)
        {
            dgw.Rows.Clear();
            database.OpenConnection();

            string queryString = $"select * from items where type_prod LIKE 'Украшение'";
            NpgsqlCommand comm = new NpgsqlCommand(queryString, database.GetConnection());

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
            database.CloseConnection();
        }

        private void MusInst(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from items where type_prod LIKE 'Музыкальный инструмент'";
            NpgsqlCommand comm = new NpgsqlCommand(queryString, database.GetConnection());

            database.OpenConnection();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
            database.CloseConnection();
        }
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSort.SelectedItem == "Все типы")
            {
                AllTypes(dataGridView1);
            }

            if (comboBoxSort.SelectedItem == "Оружие")
            {
                Wearon(dataGridView1);
            }

            if (comboBoxSort.SelectedItem == "Украшение")
            {
                Decor(dataGridView1);
            }

            if (comboBoxSort.SelectedItem == "Музыкальный инструмент")
            {
                MusInst(dataGridView1);
            }
        }
        private void DrawBarcode(string code, int resolution = 20) // resolution - пикселей на миллиметр
        {
            int numberCount = 15; // количество цифр
            float height = 25.93f * resolution; // высота штрих кода
            float lineHeight = 22.85f * resolution; // высота штриха
            float leftOffset = 3.63f * resolution; // свободная зона слева
            float rightOffset = 2.31f * resolution; // свободная зона справа
                                                    //штрихи, которые образуют правый и левый ограничивающие знаки,
                                                    //а также центральный ограничивающий знак должны быть удлинены вниз на 1,65мм
            float longLineHeight = lineHeight + 1.65f * resolution;
            float fontHeight = 2.75f * resolution; // высота цифр
            float lineToFontOffset = 0.165f * resolution; // минимальный размер от верхнего края цифр до нижнего края штрихов
            float lineWidthDelta = 0.15f * resolution; // ширина 0.15*{цифра}
            float lineWidthFull = 1.35f * resolution; // ширина белой полоски при 0 или 0.15*9
            float lineOffset = 0.2f * resolution; // между штрихами должно быть расстояние в 0.2мм

            float width = leftOffset + rightOffset + 6 * (lineWidthDelta + lineOffset) + numberCount * (lineWidthFull + lineOffset); // ширина штрих-кода

            Bitmap bitmap = new Bitmap((int)width, (int)height); // создание картинки нужных размеров
            Graphics g = Graphics.FromImage(bitmap); // создание графики

            Font font = new Font("Arial", fontHeight, FontStyle.Regular, GraphicsUnit.Pixel); // создание шрифта

            StringFormat fontFormat = new StringFormat(); // Центрирование текста
            fontFormat.Alignment = StringAlignment.Center;
            fontFormat.LineAlignment = StringAlignment.Center;

            float x = leftOffset; // позиция рисования по x
            for (int i = 0; i < numberCount; i++)
            {
                int number = Convert.ToInt32(code[i].ToString()); // число из кода
                if (number != 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, number * lineWidthDelta, lineHeight); // рисуем штрих
                }
                RectangleF fontRect = new RectangleF(x, lineHeight + lineToFontOffset, lineWidthFull, fontHeight); // рамки для буквы
                g.DrawString(code[i].ToString(), font, Brushes.Black, fontRect, fontFormat); // рисуем букву
                x += lineWidthFull + lineOffset; // смещаем позицию рисования по x
                if (i == 0 && i == numberCount / 2 && i == numberCount - 1) // если это начало, середина или конец кода рисуем разделители
                {
                    for (int j = 0; j < 2; j++) // рисуем 2 линии разделителя
                    {
                        g.FillRectangle(Brushes.Black, x, 0, lineWidthDelta, longLineHeight); // рисуем длинный штрих
                        x += lineWidthDelta + lineOffset; // смещаем позицию рисования по x
                    }
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // делаем чтобы картинка помещалась в pictureBox
            pictureBox1.Image = bitmap; // устанавливаем картинку
        }

        int selectedRow;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxArticle.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
                textBoxCost.Text = row.Cells[2].Value.ToString();
                textBoxType.Text = row.Cells[3].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawBarcode(textBoxArticle.Text);
        }


        //стрaницы
        private int currentPage = 1;
        private int itemsPerPage = 5;

        private void FillDataGridView()
        {
            database.OpenConnection();

            int offset = (currentPage - 1) * itemsPerPage;

            string query = $"SELECT * FROM items ORDER BY article OFFSET {offset} ROWS FETCH NEXT {itemsPerPage} ROWS ONLY";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, database.GetConnection()))
            {
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
            database.CloseConnection();
        }
    
        private void button3_Click(object sender, EventArgs e)
        {
            database.OpenConnection();
            string countQuery = "SELECT COUNT(*) FROM items";
            using (NpgsqlCommand countCmd = new NpgsqlCommand(countQuery, database.GetConnection()))
            {
                int totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
                int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                if (currentPage < totalPages)
                {
                    currentPage++;
                    FillDataGridView();
                }
            }
            database.CloseConnection();
         }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                FillDataGridView();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ExcelDataReader;
using System.Net;
using System.Data;
using Microsoft.Win32;
//using Syncfusion.Data;
//using ClosedXML.Excel;
namespace ThreatsList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // целая таблица
        private static ObservableCollection<ThreatsTable> threatsTable = new ObservableCollection<ThreatsTable>();
        // отображаемый на экране кусок таблицы
        private ObservableCollection<ThreatsTable> threatsSmallTable = new ObservableCollection<ThreatsTable>();
        private const string fileName = "thrlist.xlsx";
        // количество отображаемых строк
        private const int itemsInTable = 15;
        private static int pageCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            CreateDataGrid();
        }

        private void CreateData(string filePath, ref ObservableCollection<ThreatsTable> threatsTable)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // заполняем таблицу построчно
                        DataRowCollection data = result.Tables[0].Rows;
                        for (int i = 1; i < data.Count; i++)
                        {
                            threatsTable.Add(new ThreatsTable("УБИ." + data[i].ItemArray[0].ToString(),
                                                              data[i].ItemArray[1].ToString(),
                                                              data[i].ItemArray[2].ToString(),
                                                              data[i].ItemArray[3].ToString(),
                                                              data[i].ItemArray[4].ToString(),
                                                              data[i].ItemArray[5].ToString() == "1" ? "Да" : "Нет",
                                                              data[i].ItemArray[6].ToString() == "1" ? "Да" : "Нет",
                                                              data[i].ItemArray[7].ToString() == "1" ? "Да" : "Нет"));
                        }
                        // считаем количество страниц
                        pageCount = (threatsTable.Count % itemsInTable == 0) ? (threatsTable.Count / itemsInTable) : (threatsTable.Count / itemsInTable + 1);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Упс! Невозможно продолжить работу, пока файл открыт!");
            }
            
            
        }

        private void CreateDataGrid()
        {
            try
            {
                CreateData(fileName, ref threatsTable);
                // заполняем видимую часть таблицы
                for (int i = 0; i < itemsInTable; i++)
                {
                    threatsSmallTable.Add(threatsTable[i]);
                }
                // отображаем часть таблицы
                Threats_Grid.ItemsSource = threatsSmallTable;
                Number_of_Page.Content = $"Страница {currentPage} из {pageCount}";
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Упс! Исходный файл не найден. Идет загрузка...");
                DownloadTable();
            }
        }
        private void DownloadTable()
        {
            try
            {
                new WebClient().DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", fileName);
                CreateDataGrid();
            }
            catch (Exception)
            {
                MessageBox.Show("Упс! Ошибка загрузки!");
            }
        }

        private void Threats_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Worksheets|*.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(fileName, saveFileDialog.FileName);
                    MessageBox.Show("Файл загружен на диск.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Упс! Ошибка скачивания!");
                }
            }
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            // обновлённая таблица
            ObservableCollection<ThreatsTable> newThreatsTable = new ObservableCollection<ThreatsTable>();
            List<string> updates = new List<string>();
            int updatesCount = 0;
            try
            {
                CreateData(fileName, ref newThreatsTable);
                if (newThreatsTable.Count != threatsTable.Count)
                {
                    if (newThreatsTable.Count > threatsTable.Count)
                    {
                        List<ThreatsTable> newItems = newThreatsTable.Where(item => !threatsTable.Contains(item)).ToList();
                        MessageBox.Show($"Добавлено {newItems.Count} новых строк!");
                        UpdateInfoWindow updateInfoWindow = new UpdateInfoWindow();
                        updateInfoWindow.Show();
                        updateInfoWindow.Title = "Обновления";
                        for (int i = 0; i < newItems.Count; i++)
                        {
                            updates.Add($"{i + 1}.\n" +
                                        $"  {newItems[i].ToString()}\n\n");
                        }

                        for (int i = 0; i < updates.Count; i++)
                        {
                            updateInfoWindow.Update_Info_TextBlock.Text += updates[i].ToString();
                        }
                    }
                    else
                    {
                        List<ThreatsTable> newItems = threatsTable.Where(item => !newThreatsTable.Contains(item)).ToList();
                        MessageBox.Show($"Удалено {newItems.Count} строк!");
                        UpdateInfoWindow updateInfoWindow = new UpdateInfoWindow();
                        updateInfoWindow.Show();
                        updateInfoWindow.Title = "Обновления";
                        for (int i = 0; i < newItems.Count; i++)
                        {
                            updates.Add($"{i + 1}.\n" + 
                                        $"  {newItems[i].ToString()}\n\n");
                        }

                        for (int i = 0; i < updates.Count; i++)
                        {
                            updateInfoWindow.Update_Info_TextBlock.Text += updates[i].ToString();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newThreatsTable.Count; i++)
                    {
                        try
                        {
                            if (!threatsTable[i].Equals(newThreatsTable[i]))
                            {
                                updatesCount++;
                                updates.Add($"{updatesCount}.\n" +
                                            $"БЫЛО:\n{threatsTable[i].ToString()}\n" +
                                            $"СТАЛО:\n{newThreatsTable[i].ToString()}\n\n");
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Скройте дополнительную информацию!");
                            return;
                        }
                    }
                    if (updatesCount == 0)
                    {
                        MessageBox.Show("Обновления не найдены!");
                    }
                    else
                    {
                        MessageBox.Show($"Найдено {updatesCount} новых обновлений! ");
                        UpdateInfoWindow updateInfoWindow = new UpdateInfoWindow();
                        updateInfoWindow.Show();
                        updateInfoWindow.Title = "Обновления";
                        for (int i = 0; i < updatesCount; i++)
                        {
                            updateInfoWindow.Update_Info_TextBlock.Text += updates[i].ToString();
                        }

                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Упс! Исходный файл не найден, возможно он был удалён. Идет загрузка...");
                DownloadTable();
                return;
            }

        }

        // текущая страница
        public int currentPage = 1;
        
        private void Previous_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (threatsSmallTable.Count == 0 || threatsTable.Count == 0) return;

            int currentFirstItem = int.Parse((threatsSmallTable[0].Number.Substring(4))); // индекс первого отображаемого элемента
            if (currentFirstItem == 1) return;

            threatsSmallTable.Clear();

            // отображение первой страницы
            if (currentFirstItem - itemsInTable < 0)
            {
                currentFirstItem = itemsInTable + 1;
            }
            currentPage--;
            Number_of_Page.Content = $"Страница {currentPage} из {pageCount}";
            // добавляем предыдущие элементы в отображаемую таблицу
            for (int i = currentFirstItem - itemsInTable - 1; i < currentFirstItem - 1; i++)
            {
                threatsSmallTable.Add(threatsTable[i]);
            }
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (threatsSmallTable.Count == 0 || threatsTable.Count == 0) return;

            int currentLastItem = int.Parse((threatsSmallTable.Last().Number.Substring(4))); // индекс последнего отображаемого элемента
            if (currentLastItem == threatsTable.Count) return;

            threatsSmallTable.Clear();

            // отображение последней страницы
            if (threatsTable.Count - currentLastItem < itemsInTable)
            {
                currentLastItem = threatsTable.Count - itemsInTable;
            }
            currentPage++;
            Number_of_Page.Content = $"Страница {currentPage} из {pageCount}";
            // добавляем следующие элементы в отображаемую таблицу
            for (int i = currentLastItem; i < currentLastItem + itemsInTable; i++)
            {
                threatsSmallTable.Add(threatsTable[i]);
            }
        }
    }
}

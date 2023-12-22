using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace DM___5
{
    public class Graph
    {
        private Dictionary<string, List<Edge>> adjacencyList;

        public Graph()
        {
            adjacencyList = new Dictionary<string, List<Edge>>();
        }

        public void AddVertex(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = new List<Edge>();
            }
        }

        public void AddEdge(string from, string to, double weight)
        {
            adjacencyList[from].Add(new Edge(to, weight));
        }

        public List<string> GetVertices()
        {
            return adjacencyList.Keys.ToList();
        }

        public List<Edge> GetNeighbors(string vertex)
        {
            return adjacencyList[vertex];
        }
    }

    public class Edge
    {
        public string To { get; set; }
        public double Weight { get; set; }

        public Edge(string to, double weight)
        {
            To = to;
            Weight = weight;
        }
    }

    public static class BellmanFordAlgorithm
    {
        public static bool BellmanFord(Graph graph, string sourceVertex, out Dictionary<string, double> distances, out Dictionary<string, string> predecessors)
        {
            distances = new Dictionary<string, double>();
            predecessors = new Dictionary<string, string>();

            // Инициализация расстояний
            foreach (var vertex in graph.GetVertices())
            {
                distances[vertex] = double.PositiveInfinity;
                predecessors[vertex] = null;
            }

            distances[sourceVertex] = 0;

            // Релаксация рёбер
            for (int i = 0; i < graph.GetVertices().Count - 1; i++)
            {
                foreach (var vertex in graph.GetVertices())
                {
                    foreach (var neighbor in graph.GetNeighbors(vertex))
                    {
                        if (distances[vertex] + neighbor.Weight < distances[neighbor.To])
                        {
                            distances[neighbor.To] = distances[vertex] + neighbor.Weight;
                            predecessors[neighbor.To] = vertex;
                        }
                    }
                }
            }

            // Вывод для диагностики
            Console.WriteLine("Distances after Bellman-Ford algorithm:");
            foreach (var vertex in graph.GetVertices())
            {
                Console.WriteLine($"{vertex}: {distances[vertex]}");
            }

            // Проверка наличия контуров отрицательной длины
            foreach (var vertex in graph.GetVertices())
            {
                foreach (var neighbor in graph.GetNeighbors(vertex))
                {
                    if (distances[vertex] + neighbor.Weight < distances[neighbor.To])
                    {
                        return false; // Граф содержит контур отрицательной длины
                    }
                }
            }

            return true;
        }
    }
    public partial class Form1 : Form
    {
        private DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private Label label1;
        Graph graph;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получение данных из DataGridView
            graph = new Graph();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DataGridViewCellCollection cells = dataGridView1.Rows[i].Cells;

                // Проверка наличия данных в ячейках
                if (cells.Count >= 3 && cells[0].Value != null && cells[1].Value != null && cells[2].Value != null)
                {
                    string from = cells[0].Value.ToString();
                    string to = cells[1].Value.ToString();
                    double weight;

                    // Проверка успешного преобразования веса
                    if (double.TryParse(cells[2].Value.ToString(), out weight))
                    {
                        graph.AddVertex(from);
                        graph.AddVertex(to);
                        graph.AddEdge(from, to, weight);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка в весе в строке {i + 1}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка в данных в строке {i + 1}");
                    return;
                }
            }

            // Получение вершины-источника из TextBox
            string sourceVertex = textBox2.Text;

            bool success = BellmanFord(graph, sourceVertex, out Dictionary<string, double> distances, out Dictionary<string, string> predecessors);

            // Вывод результатов в Label
            if (success)
            {
                Dictionary<string, List<string>> shortestPaths = GetShortestPaths(sourceVertex, predecessors);

                StringBuilder resultBuilder = new StringBuilder();
                foreach (var vertex in graph.GetVertices())
                {
                    resultBuilder.AppendLine($"Расстояние до вершины {vertex}: {distances[vertex]}");
                    resultBuilder.AppendLine($"Кратчайшие пути из вершины {sourceVertex} в вершину {vertex}: [{string.Join(", ", shortestPaths[vertex])}]\n");
                }

                label1.Text = resultBuilder.ToString();
            }
            else
            {
                label1.Text = "Граф содержит контур отрицательной длины или вершины недостижимы из источника.";
            }
        }

        private Dictionary<string, List<string>> GetShortestPaths(string sourceVertex, Dictionary<string, string> predecessors)
        {
            Dictionary<string, List<string>> shortestPaths = new Dictionary<string, List<string>>();

            foreach (var vertex in graph.GetVertices())
            {
                shortestPaths[vertex] = GetShortestPath(sourceVertex, vertex, predecessors);
            }

            return shortestPaths;
        }

        private List<string> GetShortestPath(string source, string destination, Dictionary<string, string> predecessors)
        {
            List<string> path = new List<string>();
            string current = destination;

            while (current != null)
            {
                path.Insert(0, current);
                current = predecessors[current];
            }

            return path;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int size;

            if (int.TryParse(textBox1.Text, out size) && size > 0)
            {
                // Очищаем существующие столбцы и строки в DataGridView
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                // Добавляем новые столбцы
                for (int colIndex = 0; colIndex < size; colIndex++)
                {
                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        Name = "Column" + (colIndex + 1),
                        HeaderText = "Column" + (colIndex + 1),
                        ValueType = typeof(double) // Замените double на тип данных вашего столбца
                    });
                }

                // Добавляем новые строки
                for (int rowIndex = 0; rowIndex < size; rowIndex++)
                {
                    // Создаем массив объектов для каждой строки
                    object[] rowItems = new object[size];
                    for (int colIndex = 0; colIndex < size; colIndex++)
                    {
                        rowItems[colIndex] = 0.0; // Используйте значение по умолчанию вашего типа данных
                    }

                    // Добавляем строку в DataGridView
                    dataGridView1.Rows.Add(rowItems);
                }
            }
            else
            {
                // В случае некорректного ввода, можно вывести сообщение или выполнить другие действия
                MessageBox.Show("Введите корректное число (больше 0) в textBox1");
            }
        }
            private bool BellmanFord(Graph graph, string sourceVertex, out Dictionary<string, double> distances, out Dictionary<string, string> predecessors)
        {
            return BellmanFordAlgorithm.BellmanFord(graph, sourceVertex, out distances, out predecessors);
        }
    }
}


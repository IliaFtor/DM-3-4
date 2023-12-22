using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuickGraph;
using QuickGraph.Serialization.DirectedGraphML;

namespace DM3_4_возможно_5
{
    public partial class Form1 : Form
    {
        Font font = new Font("Arial", 12, FontStyle.Bold);
        Brush brush = new SolidBrush(Color.Black);

        UndirectedGraph<int, UndirectedEdge<int>> graph;
        BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph;
        private bool isDirectedGraph = false;
        private bool FirstStart = false;
        private string connectivityCategory;

        public Form1()
        {
            InitializeComponent();
            // Добавляем RadioButton для выбора типа графа
            RadioButton radioButtonDirected = new RadioButton();
            radioButtonDirected.Text = "Ориентированный";
            radioButtonDirected.Location = new Point(10, 10);
            radioButtonDirected.CheckedChanged += RadioButton_CheckedChanged;
            Controls.Add(radioButtonDirected);

            RadioButton radioButtonUndirected = new RadioButton();
            radioButtonUndirected.Text = "Неориентированный";
            radioButtonUndirected.Location = new Point(150, 10);
            radioButtonUndirected.CheckedChanged += RadioButton_CheckedChanged;
            Controls.Add(radioButtonUndirected);

            // По умолчанию выбран неориентированный граф
            radioButtonUndirected.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int columns) && int.TryParse(textBox1.Text, out int rows))
            {
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Clear existing columns and rows
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                // Add columns with letter headers
                for (int i = 0; i < columns; i++)
                {
                    char columnHeader = (char)('A' + i);
                    dataGridView1.Columns.Add(columnHeader.ToString(), columnHeader.ToString());
                }

                // Add rows with numeric headers
                for (int i = 1; i <= rows; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i - 1].HeaderCell.Value = i.ToString();
                }

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Width = 10;
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 10;
                }
            }
        }


        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FirstStart)
            {
                isDirectedGraph = ((RadioButton)sender).Text == "Ориентированный";
                button2_Click(sender, e);
            }
            FirstStart = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Refresh();

            int rows = dataGridView1.Rows.Count - 1;
            int cols = dataGridView1.Columns.Count;

            if (isDirectedGraph)
            {
                // Создаем ориентированный граф
                directedGraph =
                    new BidirectionalGraph<int, TaggedEdge<int, int>>();

                // Добавляем вершины в зависимости от количества строк
                for (int i = 0; i < rows; i++)
                {
                    directedGraph.AddVertex(i);
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null &&
                            int.TryParse(dataGridView1.Rows[i].Cells[j].Value.ToString(), out int weight) && weight == 1)
                        {
                            directedGraph.AddEdge(new TaggedEdge<int, int>(i, j, weight));

                            // Если есть петля (i == j), добавим соответствующее ребро
                            if (i == j)
                            {
                                directedGraph.AddEdge(new TaggedEdge<int, int>(i, j, weight));
                            }
                        }
                    }
                }
                connectivityCategory = IsStronglyConnected(directedGraph) ? "Сильно связный" : "Односторонне связный";
                DrawDirectedGraph(directedGraph, panel1.CreateGraphics());
            }
            else
            {
                // Создаем неориентированный граф
                graph = new UndirectedGraph<int, UndirectedEdge<int>>();

                // Добавляем вершины в зависимости от количества строк
                for (int i = 0; i < rows; i++)
                {
                    graph.AddVertex(i);
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i + 1; j < cols; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null &&
                            int.TryParse(dataGridView1.Rows[i].Cells[j].Value.ToString(), out int weight) && weight == 1)
                        {
                            // Добавляем ребро в граф
                            graph.AddEdge(new UndirectedEdge<int>(i, j));
                            graph.AddEdge(new UndirectedEdge<int>(j, i));

                            // Если есть петля (i == j), добавим соответствующее ребро
                            if (i == j)
                            {
                                graph.AddEdge(new UndirectedEdge<int>(i, j));
                            }
                        }
                    }
                }

                connectivityCategory = IsConnected(graph) ? "Связный" : "Несвязный";

                // Отображаем граф
                DrawUndirectedGraph(graph, panel1.CreateGraphics());
            }

            // Дополнительно: выводим характеристики графа
            DrawCharacteristics(panel1.CreateGraphics());
        }
        private bool IsConnected(UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            HashSet<int> visitedVertices = new HashSet<int>();

            // Выбираем произвольную вершину в графе
            int startVertex = graph.Vertices.First();

            // Выполняем обход в глубину (DFS)
            DepthFirstSearch(graph, startVertex, visitedVertices);

            // Проверяем, были ли посещены все вершины
            return visitedVertices.Count == graph.VertexCount;
        }

        private void DepthFirstSearch(UndirectedGraph<int, UndirectedEdge<int>> graph, int currentVertex, HashSet<int> visitedVertices)
        {
            if (!visitedVertices.Contains(currentVertex))
            {
                visitedVertices.Add(currentVertex);

                // Рекурсивно обходим соседние ребра текущей вершины
                foreach (var edge in graph.AdjacentEdges(currentVertex))
                {
                    int neighbor = edge.Source == currentVertex ? edge.Target : edge.Source;
                    DepthFirstSearch(graph, neighbor, visitedVertices);
                }
            }
        }


        private bool IsStronglyConnected(BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph)
        {
            foreach (var vertex in directedGraph.Vertices)
            {
                HashSet<int> visitedVertices = new HashSet<int>();
                DepthFirstSearch(directedGraph, vertex, visitedVertices);

                if (visitedVertices.Count != directedGraph.VertexCount)
                {
                    return false; // Найдена недостижимая вершина, граф не сильно связный
                }
            }

            return true;
        }

        private void DepthFirstSearch(BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph, int currentVertex, HashSet<int> visitedVertices)
        {
            if (!visitedVertices.Contains(currentVertex))
            {
                visitedVertices.Add(currentVertex);

                // Рекурсивно обходим соседей текущей вершины
                foreach (var edge in directedGraph.OutEdges(currentVertex))
                {
                    int neighbor = edge.Target;
                    DepthFirstSearch(directedGraph, neighbor, visitedVertices);
                }
            }
        }

        private void DrawUndirectedGraph(UndirectedGraph<int, UndirectedEdge<int>> graph, Graphics graphics)
        {
            Pen pen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 10);
            int nodeRadius = 25;

            foreach (var vertex in graph.Vertices)
            {
                char vertexName = (char)('A' + vertex);
                double angle = 2 * Math.PI * vertex / graph.VertexCount;
                int x = (int)(panel1.Width / 2 + Math.Cos(angle) * 100);
                int y = (int)(panel1.Height / 2 + Math.Sin(angle) * 100);

                // Отрисовываем вершину
                graphics.DrawEllipse(pen, x - nodeRadius, y - nodeRadius, 2 * nodeRadius, 2 * nodeRadius);
                graphics.DrawString(vertexName.ToString(), font, Brushes.Black, x - nodeRadius / 2, y - nodeRadius / 2);

                // Проверяем наличие петли
                if (graph.ContainsEdge(vertex, vertex))
                {
                    // Если есть петля, рисуем ребро
                    double loopX = x + 1.5 * nodeRadius * Math.Cos(angle);
                    double loopY = y + 1.5 * nodeRadius * Math.Sin(angle);

                    // Отрисовываем петлю
                    graphics.DrawEllipse(pen, (float)(x - 0.5 * nodeRadius), (float)(y - 0.5 * nodeRadius), (float)nodeRadius, (float)nodeRadius);
                }
            }

            // Отрисовываем рёбра
            foreach (var edge in graph.Edges)
            {
                var source = edge.Source;
                var target = edge.Target;

                double sourceAngle = 2 * Math.PI * source / graph.VertexCount;
                double targetAngle = 2 * Math.PI * target / graph.VertexCount;

                int sourceX = (int)(panel1.Width / 2 + Math.Cos(sourceAngle) * 100);
                int sourceY = (int)(panel1.Height / 2 + Math.Sin(sourceAngle) * 100);

                int targetX = (int)(panel1.Width / 2 + Math.Cos(targetAngle) * 100);
                int targetY = (int)(panel1.Height / 2 + Math.Sin(targetAngle) * 100);

                // Отобразим ребро
                graphics.DrawLine(pen, sourceX, sourceY, targetX, targetY);
            }
        }

        private void DrawDirectedGraph(BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph, Graphics graphics)
        {
            Pen pen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 10);
            int nodeRadius = 25;

            foreach (var vertex in directedGraph.Vertices)
            {
                char vertexName = (char)('A' + vertex);
                double angle = 2 * Math.PI * vertex / directedGraph.VertexCount;
                int x = (int)(panel1.Width / 2 + Math.Cos(angle) * 100);
                int y = (int)(panel1.Height / 2 + Math.Sin(angle) * 100);

                // Отрисовываем вершину
                graphics.DrawEllipse(pen, x - nodeRadius, y - nodeRadius, 2 * nodeRadius, 2 * nodeRadius);
                graphics.DrawString(vertexName.ToString(), font, Brushes.Black, x - nodeRadius / 2, y - nodeRadius / 2);

                if (directedGraph.ContainsEdge(vertex, vertex))
                {
                    // Если есть петля, рисуем ребро
                    double loopX = x + 1.5 * nodeRadius * Math.Cos(angle);
                    double loopY = y + 1.5 * nodeRadius * Math.Sin(angle);

                    // Отрисовываем петлю
                    graphics.DrawEllipse(pen, (float)(x - 0.5 * nodeRadius), (float)(y - 0.5 * nodeRadius - 10), (float)nodeRadius, (float)nodeRadius);
                }


                foreach (var edge in directedGraph.Edges)
                {
                    var source = edge.Source;
                    var target = edge.Target;

                    double sourceAngle = 2 * Math.PI * source / directedGraph.VertexCount;
                    double targetAngle = 2 * Math.PI * target / directedGraph.VertexCount;

                    int sourceX = (int)(panel1.Width / 2 + Math.Cos(sourceAngle) * 100);
                    int sourceY = (int)(panel1.Height / 2 + Math.Sin(sourceAngle) * 100);

                    int targetX = (int)(panel1.Width / 2 + Math.Cos(targetAngle) * 100);
                    int targetY = (int)(panel1.Height / 2 + Math.Sin(targetAngle) * 100);

                    // Отобразим направление ребра
                    graphics.DrawLine(pen, sourceX, sourceY, targetX, targetY);

                    // Добавим стрелку
                    DrawArrow(graphics, pen, new Point(sourceX, sourceY), new Point(targetX, targetY));
                }
            }
        }


        private void DrawArrow(Graphics g, Pen pen, Point source, Point target)
        {
            const int arrowSize = 10;

            float dy = target.Y - source.Y;
            float dx = target.X - source.X;
            double angle = Math.Atan2(dy, dx);

            g.FillPolygon(Brushes.Black, new PointF[]
            {
                target,
                new PointF(target.X - arrowSize * (float)Math.Cos(angle - Math.PI / 6), target.Y - arrowSize * (float)Math.Sin(angle - Math.PI / 6)),
                new PointF(target.X - arrowSize * (float)Math.Cos(angle + Math.PI / 6), target.Y - arrowSize * (float)Math.Sin(angle + Math.PI / 6))
            });

            g.DrawLine(pen, source, target);
        }

        private void DrawCharacteristics(Graphics graphics)
        {
            try
            {
                int numVertices;
                int numEdges;
                int numSelfLoops;
                int maxDegree;

                string characteristicsText;

                if (isDirectedGraph)
                {
                    numVertices = directedGraph.VertexCount;
                    numEdges = directedGraph.Edges.Count();
                    numSelfLoops = CountOnMainDiagonalDirected();
                    maxDegree = directedGraph.Vertices.Max(v => directedGraph.OutDegree(v));

                    characteristicsText = $"Характеристики графа:\n" +
                                         $"Количество вершин: {numVertices}\n" +
                                         $"Количество рёбер: {numEdges}\n" +
                                         $"Количество петель: {numSelfLoops}\n" +
                                         $"Максимальная степень исходящих: {maxDegree}" +
                                         $"\nКатегория связности: {connectivityCategory}";
                }
                else
                {
                    numVertices = graph.VertexCount;
                    numEdges = graph.Edges.Count();
                    numSelfLoops = CountOnMainDiagonal();
                    maxDegree = graph.Vertices.Max(v => graph.AdjacentDegree(v));

                    characteristicsText = $"Характеристики графа:\n" +
                                         $"Количество вершин: {numVertices}\n" +
                                         $"Количество рёбер: {numEdges}\n" +
                                         $"Количество петель: {numSelfLoops}\n" +
                                         $"Максимальная степень: {maxDegree}" +
                                         $"\nКатегория связности: {connectivityCategory}";
                }

                // Вывод матрицы смежности
                int[,] adjacencyMatrix;
                if (isDirectedGraph)
                {
                    adjacencyMatrix = GetAdjacencyMatrixDirected(directedGraph);
                }
                else
                {
                    adjacencyMatrix = GetAdjacencyMatrix(graph);
                }

                characteristicsText += "\nМатрица смежности:\n";
                for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                    {
                        characteristicsText += $"{adjacencyMatrix[i, j]} ";
                    }
                    characteristicsText += "\n";
                }

                // Вывод списка инцидентности
                List<List<int>> incidenceList;
                if (isDirectedGraph)
                {
                    incidenceList = GetIncidenceListDirected(directedGraph);
                }
                else
                {
                    incidenceList = GetIncidenceList(graph);
                }

                characteristicsText += "\nСписок инцидентности:\n";
                for (int i = 0; i < incidenceList.Count; i++)
                {
                    characteristicsText += $"{i + 1}: ";
                    for (int j = 0; j < incidenceList[i].Count; j++)
                    {
                        characteristicsText += $"{incidenceList[i][j]} ";
                    }
                    characteristicsText += "\n";
                }

                graphics.DrawString(characteristicsText, font, brush, 10, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении характеристик графа: {ex.Message}");
            }
        }
        

        private int[,] GetAdjacencyMatrixDirected(BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph)
        {
            int numVertices = directedGraph.VertexCount;
            int[,] adjacencyMatrix = new int[numVertices, numVertices];

            foreach (var edge in directedGraph.Edges)
            {
                adjacencyMatrix[edge.Source, edge.Target] = 1;
            }

            return adjacencyMatrix;
        }

        private List<List<int>> GetIncidenceListDirected(BidirectionalGraph<int, TaggedEdge<int, int>> directedGraph)
        {
            int numVertices = directedGraph.VertexCount;
            int numEdges = directedGraph.Edges.Count();
            List<List<int>> incidenceList = new List<List<int>>();

            for (int i = 0; i < numEdges; i++)
            {
                incidenceList.Add(new List<int>());
            }

            int edgeIndex = 0;
            foreach (var edge in directedGraph.Edges)
            {
                incidenceList[edgeIndex].Add(edge.Source);
                incidenceList[edgeIndex].Add(edge.Target);
                edgeIndex++;
            }

            return incidenceList;
        }

        private int CountOnMainDiagonalDirected()
        {
            int count = 0;

            for (int i = 0; i < dataGridView1.Rows.Count && i < dataGridView1.Columns.Count; i++)
            {
                object cellValue = dataGridView1.Rows[i].Cells[i].Value;

                if (cellValue != null && cellValue.ToString() == "1")
                {
                    count++;
                }
            }

            return count;
        }

        private int[,] GetAdjacencyMatrix(UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            int numVertices = graph.VertexCount;
            int[,] adjacencyMatrix = new int[numVertices, numVertices];

            foreach (var edge in graph.Edges)
            {
                adjacencyMatrix[edge.Source, edge.Target] = 1;
                adjacencyMatrix[edge.Target, edge.Source] = 1;
            }

            return adjacencyMatrix;
        }

        private List<List<int>> GetIncidenceList(UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            int numEdges = graph.Edges.Count();
            List<List<int>> incidenceList = new List<List<int>>();

            for (int i = 0; i < numEdges; i++)
            {
                incidenceList.Add(new List<int>());
            }

            int edgeIndex = 0;
            foreach (var edge in graph.Edges)
            {
                incidenceList[edgeIndex].Add(edge.Source);
                incidenceList[edgeIndex].Add(edge.Target);
                edgeIndex++;
            }

            return incidenceList;
        }

        private int CountOnMainDiagonal()
        {
            int count = 0;

            for (int i = 0; i < dataGridView1.Rows.Count && i < dataGridView1.Columns.Count; i++)
            {
                object cellValue = dataGridView1.Rows[i].Cells[i].Value;

                if (cellValue != null && cellValue.ToString() == "1")
                {
                    count++;
                }
            }

            return count;
        }

        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.Value != null && int.TryParse(e.Value.ToString(), out int value))
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            }
            else
            {
                // Если введено некорректное значение, можно обработать ошибку или предпринять другие действия
            }

            // Если граф неориентированный, автоматически дополняем матрицу единицами на диагонали
            if (!isDirectedGraph)
            {
                int rowIndex = e.RowIndex;
                int columnIndex = e.ColumnIndex;

                // Дополняем верхний треугольник матрицы (зеркально относительно главной диагонали)
                if (rowIndex != columnIndex)
                {
                    dataGridView1.Rows[columnIndex].Cells[rowIndex].Value = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value;
                }
                // Если редактируется элемент на главной диагонали, обновляем все соответствующие ячейки
                else
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (i != rowIndex)
                        {
                            dataGridView1.Rows[i].Cells[i].Value = 1;
                        }
                    }
                }
            }
        }
        //4 задания        №№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№№


  
        private void button3_Click(object sender, EventArgs e)
        {
            DrawCharacteristicsOnPanel2(panel2.CreateGraphics());
        }
        private void DrawCharacteristicsOnPanel2(Graphics graphics)
        {
            panel2.Refresh();
            try
            {               
                string graphType = isDirectedGraph ? "орграф" : "граф";

                // Проверка на гамильтоновость
                bool isHamiltonian;
                List<List<int>> hamiltonianCycles;

                if (isDirectedGraph)
                {
                    isHamiltonian = CheckHamiltonianDirected();
                    hamiltonianCycles = FindHamiltonianCyclesDirected();
                }
                else
                {
                    isHamiltonian = CheckHamiltonianUndirected();
                    hamiltonianCycles = FindHamiltonianCyclesUndirected();
                }

                string hamiltonianText = $"Гамильтонов {graphType}: {isHamiltonian}\n";

                // Вывод информации о гамильтоновых циклах
                if (isHamiltonian)
                {
                    hamiltonianText += "\nГамильтоновы циклы:\n";
                    foreach (var cycle in hamiltonianCycles)
                    {
                        hamiltonianText += $"{string.Join(" -> ", cycle)}\n";
                    }
                }
                else
                {
                    hamiltonianText += $"\n{graphType} не является гамильтоновым.";
                }

                graphics.DrawString(hamiltonianText, font, brush, 10, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении информации о гамильтоновых циклах: {ex.Message}");
            }

        }
        private bool CheckHamiltonianDirected()
        {
            int numVertices = directedGraph.VertexCount;
            List<int>[] graph = new List<int>[numVertices];

            // Инициализация списков смежности
            for (int i = 0; i < numVertices; i++)
            {
                graph[i] = new List<int>();
            }

            // Заполнение списков смежности из ориентированного графа
            foreach (var edge in directedGraph.Edges)
            {
                graph[edge.Source].Add(edge.Target);
            }

            // Создайте массив для отслеживания посещенных вершин
            bool[] visited = new bool[numVertices];

            // Начнем проверку с каждой вершины
            for (int i = 0; i < numVertices; i++)
            {
                // Создаем список для хранения текущего пути
                List<int> path = new List<int>();
                path.Add(i);

                // Начинаем backtracking из текущей вершины
                if (CheckHamiltonianRecursive(graph, visited, path))
                    return true;
            }

            return false;
        }

        private List<List<int>> FindHamiltonianCyclesDirected()
        {
            int numVertices = directedGraph.VertexCount;
            List<int>[] graph = new List<int>[numVertices];

            // Инициализация списков смежности
            for (int i = 0; i < numVertices; i++)
            {
                graph[i] = new List<int>();
            }

            // Заполнение списков смежности из ориентированного графа
            foreach (var edge in directedGraph.Edges)
            {
                graph[edge.Source].Add(edge.Target);
            }

            List<List<int>> hamiltonianCycles = new List<List<int>>();

            // Начнем поиск с каждой вершины
            for (int i = 0; i < numVertices; i++)
            {
                // Создаем список для хранения текущего пути
                List<int> path = new List<int>();
                path.Add(i);

                // Начинаем backtracking из текущей вершины
                FindHamiltonianCyclesRecursive(graph, path, hamiltonianCycles);
            }

            return hamiltonianCycles;
        }

        private bool CheckHamiltonianUndirected()
        {
            int numVertices = graph.VertexCount;
            List<int>[] graphs = new List<int>[numVertices];

            // Инициализация списков смежности
            for (int i = 0; i < numVertices; i++)
            {
                graphs[i] = new List<int>();
            }

            // Заполнение списков смежности из неориентированного графа
            foreach (var edge in graph.Edges)
            {
                graphs[edge.Source].Add(edge.Target);
                graphs[edge.Target].Add(edge.Source);
            }

            // Создайте массив для отслеживания посещенных вершин
            bool[] visited = new bool[numVertices];

            // Начнем проверку с каждой вершины
            for (int i = 0; i < numVertices; i++)
            {
                // Создаем список для хранения текущего пути
                List<int> path = new List<int>();
                path.Add(i);

                // Начинаем backtracking из текущей вершины
                if (CheckHamiltonianRecursive(graphs, visited, path))
                    return true;
            }

            return false;
        }

        private List<List<int>> FindHamiltonianCyclesUndirected()
        {
            int numVertices = graph.VertexCount;
            List<int>[] graphs = new List<int>[numVertices];

            // Инициализация списков смежности
            for (int i = 0; i < numVertices; i++)
            {
                graphs[i] = new List<int>();
            }

            // Заполнение списков смежности из неориентированного графа
            foreach (var edge in graph.Edges)
            {
                graphs[edge.Source].Add(edge.Target);
                graphs[edge.Target].Add(edge.Source);
            }

            List<List<int>> hamiltonianCycles = new List<List<int>>();

            // Начнем поиск с каждой вершины
            for (int i = 0; i < numVertices; i++)
            {
                // Создаем список для хранения текущего пути
                List<int> path = new List<int>();
                path.Add(i);

                // Начинаем backtracking из текущей вершины
                FindHamiltonianCyclesRecursive(graphs, path, hamiltonianCycles);
            }

            return hamiltonianCycles;
        }

        private bool CheckHamiltonianRecursive(List<int>[] graph, bool[] visited, List<int> path)
        {
            int currentVertex = path[path.Count - 1];

            // Проверяем, можно ли добавить ребро к текущему пути
            foreach (var neighbor in graph[currentVertex])
            {
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    path.Add(neighbor);

                    // Рекурсивно проверяем оставшийся путь
                    if (CheckHamiltonianRecursive(graph, visited, path))
                        return true;

                    // Если не получилось найти цикл, отменяем текущее добавление
                    visited[neighbor] = false;
                    path.RemoveAt(path.Count - 1);
                }
            }

            // Если все вершины посещены, проверяем, образует ли путь цикл
            if (path.Count == graph.Length && graph[currentVertex].Contains(path[0]))
                return true;

            return false;
        }

        private void FindHamiltonianCyclesRecursive(List<int>[] graph, List<int> path, List<List<int>> hamiltonianCycles)
        {
            int currentVertex = path[path.Count - 1];

            foreach (var neighbor in graph[currentVertex])
            {
                if (!path.Contains(neighbor))
                {
                    path.Add(neighbor);

                    // Рекурсивно ищем циклы
                    FindHamiltonianCyclesRecursive(graph, path, hamiltonianCycles);

                    // Если не получилось найти цикл, отменяем текущее добавление
                    path.RemoveAt(path.Count - 1);
                }
            }

            // Если все вершины посещены и последняя вершина соединена с начальной, образуется цикл
            if (path.Count == graph.Length && graph[currentVertex].Contains(path[0]))
            {
                hamiltonianCycles.Add(new List<int>(path));
            }
        }

    }
}

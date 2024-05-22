using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Percolation_simulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public decimal p; // probability that an edge is open
        public Node[,] nodeLattice;
        private void Form1_Shown(object sender, EventArgs e)
        {
            p = 0.2m; // probability that an edge is open
            nodeLattice = UniformCouplingSeed(pictureBox1.Width, pictureBox1.Height); // generates lattice of nodes with random clusters and weights
            UpdateEdges(ref nodeLattice, p); // closes/opens edges based off of the probability an edge is open i.e. open if p > weight
            UpdateClusters(ref nodeLattice);
            UpdateImage(ref pictureBox1, nodeLattice);
            UpdateText(p);

        }

        public static Node[,] UniformCouplingSeed(int width, int height)
        {
            var rnd = new Random();
            var nodeLattice = new Node[width, height]; // create lattice (grid) of nodes
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    nodeLattice[x, y] = new Node(rnd.Next());
                }
            }

            for (int y = 0; y < height; y++) // assign edges and weights to nodes
            {
                for (int x = 0; x < width; x++)
                {
                    if (x + 1 < width)
                    {
                        var newEdge = new Edge(Convert.ToDecimal(rnd.NextDouble()), nodeLattice[x, y], nodeLattice[x + 1, y]);
                        nodeLattice[x, y].edges.Add(newEdge);
                        nodeLattice[x + 1, y].edges.Add(newEdge);
                    }
                    if (y + 1 < height)
                    {
                        var newEdge = new Edge(Convert.ToDecimal(rnd.NextDouble()), nodeLattice[x, y], nodeLattice[x, y + 1]);
                        nodeLattice[x, y].edges.Add(newEdge);
                        nodeLattice[x, y + 1].edges.Add(newEdge);
                    }
                }
            }

            return nodeLattice;
        }

        public static void UpdateEdges(ref Node[,] nodeLattice, decimal p)
        {
            for (int y = 0; y < nodeLattice.GetLength(1); y++) // set edges as open/closed
            {
                for (int x = 0; x < nodeLattice.GetLength(0); x++)
                {
                    nodeLattice[x, y].edges[0].open = nodeLattice[x, y].edges[0].weight < p ? true : false;
                    nodeLattice[x, y].edges[1].open = nodeLattice[x, y].edges[1].weight < p ? true : false;
                }
            }
        }

        public static void UpdateClusters(ref Node[,] nodeLattice)
        {
            int currentCluster = nodeLattice[1, 0].cluster;
            for (int y = 0; y < nodeLattice.GetLength(1); y++)
            {
                for (int x = 0; x < nodeLattice.GetLength(0); x++)
                {
                    if (nodeLattice[x, y].cluster != currentCluster)
                    {
                        currentCluster = nodeLattice[x, y].cluster;
                        ExpandCluster(ref nodeLattice[x, y], currentCluster, null, 1000);
                    }
                }
            }
        }

        public static void ExpandCluster(ref Node node, int cluster, Edge visitedFrom, int availableRecursions)
        {
            node.cluster = cluster;
            if (availableRecursions == 0)
            {
                return;
            }
            foreach (var edge in node.edges)
            {
                if (edge != visitedFrom && edge.open)
                {
                    Node nextNode = edge.connects[0] == node ? edge.connects[1] : edge.connects[0];
                    if (nextNode.cluster != cluster)
                    {
                        ExpandCluster(ref nextNode, cluster, edge, availableRecursions - 1);
                    }
                }
            }
        }

        public static void UpdateImage(ref PictureBox pictureBox, Node[,] nodeLattice)
        {
            var bm = new Bitmap(pictureBox.Width, pictureBox.Height); // each pixel represents colour of node, representing the cluster it is in
            for (int y = 0; y < pictureBox.Height; y++)
            {
                for (int x = 0; x < pictureBox.Width; x++)
                {
                    bm.SetPixel(x, y, Color.FromArgb(nodeLattice[x, y].cluster));
                }
            }

            pictureBox.Image = bm;
        }

        public static void RandomizeClusters(ref Node[,] nodeLattice)
        {
            for (int y = 0; y < nodeLattice.GetLength(1); y++)
            {
                for (int x = 0; x < nodeLattice.GetLength(0); x++)
                {
                    var rnd = new Random();
                    nodeLattice[x, y].cluster = rnd.Next();
                }
            }
        }

        public void UpdateText(decimal p)
        {
            label1.Text = "p = " + p.ToString();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                if (p + 0.01m <= 1)
                {
                    p += 0.01m;
                    UpdateEdges(ref nodeLattice, p); // closes/opens edges based off of the probability an edge is open i.e. open if p > weight
                    UpdateClusters(ref nodeLattice);
                    UpdateImage(ref pictureBox1, nodeLattice);
                    UpdateText(p);
                }
            }
            if (e.KeyChar == 's')
            {
                if (p - 0.01m >= 0)
                {
                    p -= 0.01m;
                    UpdateEdges(ref nodeLattice, p); // closes/opens edges based off of the probability an edge is open i.e. open if p > weight
                    RandomizeClusters(ref nodeLattice);
                    UpdateClusters(ref nodeLattice);
                    UpdateImage(ref pictureBox1, nodeLattice);
                    UpdateText(p);
                }
            }
        }
    }


    public class Node
    {
        public Node(int c) 
        {
            cluster = c;
        }
        public int cluster;
        public List<Edge> edges = new List<Edge>();

    }

    public class Edge
    {
        public Edge(decimal w, Node n1, Node n2)
        {
            weight = w;
            connects = new Node[2];
            connects[0] = n1;
            connects[1] = n2;
        }

        public decimal weight;
        public Node[] connects;
        public bool open;
    }
}

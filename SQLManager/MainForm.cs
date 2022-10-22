using SQLManager.Dal;
using SQLManager.Models;
using SQLManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLManager
{
    public partial class MainForm : Form
    {
        private TabControl tcQuery = new TabControl();
        private string selectedDatabase;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitTabControl();
            LoadDatabases();
        }

        private void LoadDatabases()
        {
            IList<Database> databases = new List<Database>(RepositoryFactory.GetRepository().GetDatabases());
            InitializeMenuTreeView(databases);  
        }

        private void InitializeMenuTreeView(IList<Database> databases)
        {
            TreeNode rootNode = new TreeNode("Server");
            ContextMenuStrip contextMenu = CreateNodeContextMenu();

            foreach (var item in databases)
            {
                TreeNode dbNode = new TreeNode(item.ToString());
                dbNode.ContextMenuStrip = contextMenu;
                rootNode.Nodes.Add(dbNode);
            }

            tvDatabases.Nodes.Add(rootNode);
            tvDatabases.NodeMouseClick += (sender, args) => tvDatabases.SelectedNode = args.Node;
        }

        private void InitTabControl()
        {
            tcQuery.SizeMode = TabSizeMode.Normal;
            tcQuery.DrawMode = TabDrawMode.OwnerDrawFixed;
            tcQuery.Size = new Size(895, 440);
            tcQuery.DrawItem += TcQuery_DrawItem;
            tcQuery.MouseDown += TcQuery_MouseDown;
            queryPanel.Controls.Add(tcQuery);
        }

        private ContextMenuStrip CreateNodeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem queryLabel = new ToolStripMenuItem();
            queryLabel.Text = "New Query";
            contextMenu.Items.AddRange(new ToolStripMenuItem[] { queryLabel });
            queryLabel.Click += QueryLabel_Click;

            return contextMenu;
        }

        private void QueryLabel_Click(object sender, EventArgs e)
        {
            TreeNode SelectedNode = tvDatabases.SelectedNode;

            selectedDatabase = SelectedNode.Text;

            string title = "New Query | " + selectedDatabase + "      ";
            TabPage myTabPage = new TabPage(title);

            TextBox textBox = new TextBox
            {
                Name = "tbQuery",
                Size = new Size(890, 420),
                Font = new Font("Verdana", 14),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                AcceptsTab = true,
                WordWrap = true
            };

            myTabPage.Controls.Add(textBox);
            tcQuery.TabPages.Add(myTabPage);
            tcQuery.SelectedTab = myTabPage;

        }

        private void TcQuery_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics graphics = e.Graphics;
            var tabPage = tcQuery.TabPages[e.Index];
            var tabRect = tcQuery.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);

            Bitmap closeImage = new Bitmap(Resources.close);
            SolidBrush sb = new SolidBrush(Color.FromArgb(233, 233, 233));

            if (tcQuery.SelectedIndex == e.Index)
                sb.Color = Color.FromArgb(255, 242, 157);

            graphics.FillRectangle(sb, e.Bounds);

            graphics.DrawImage(
                closeImage,
                (tabRect.Right - closeImage.Width),
                tabRect.Top + (tabRect.Height - closeImage.Height) / 2);

            TextRenderer.DrawText(
                graphics,
                tabPage.Text,
                tabPage.Font,
                tabRect,
                tabPage.ForeColor,
                TextFormatFlags.Left);
        }

        private void TcQuery_MouseDown(object sender, MouseEventArgs e)
        {
            for (var i = 0; i < tcQuery.TabPages.Count; i++)
            {
                var tabRect = tcQuery.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeImage = new Bitmap(Resources.close);
                var imageRect = new Rectangle(
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                    closeImage.Width,
                    closeImage.Height);
                if (imageRect.Contains(e.Location))
                {
                    tcQuery.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) => Application.Exit();

        private void BtnExecute_Click(object sender, EventArgs e) => ExecuteQuery();

        private void ExecuteQuery()
        {
            ClearResultsTabControl();

            if (selectedDatabase == null)
            {
                return;
                //var test = ds.Tables[0];
            }
            try
            {
                TextBox textBox = tcQuery.SelectedTab.Controls.OfType<TextBox>().First();
                Database db = new Database(selectedDatabase);
                RepositoryFactory.GetRepository().GetDataSet(db, textBox.Text);
            }
            catch (Exception e)
            {
                tcResults.SelectedTab = tpMessage;
                tbMessage.Text = e.Message;
            }
        }

        private void ClearResultsTabControl()
        {
            tcResults.SelectedTab = tpResults;
            tbMessage.Text = "";
        }
    }
}

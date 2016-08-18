using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FGame;
using System.Threading;
using System.Drawing.Imaging;

namespace FGame.LocationEditor
{
    public partial class EditorForm : Form
    {
        //TODO: It's rly slow. Optimize it! (slowpoke.jpg)

        private Location location;
        private Point viewPoint = new Point(0,0);

        private Mutex renderMutex = new Mutex();
        private ManualResetEvent closeEvnt = new ManualResetEvent(true);
        private Image tilesList = Properties.Resources.tiles;
        private Image chestList = Properties.Resources.chest;
        private Image[] tiles;
        private Image[,] chest;

        private bool isViewPointMoving = false;
        private Keys viewPointMoveKey;
        private int viewPointMovingSpeed;

        private bool isSelectedObjectMoving = false;
        private Keys selectedObjectMoveKey;
        private int selectedObjectMovingSpeed;

        private GamePoleObject selected;

        public EditorForm()
        {
            InitializeComponent();
            location = new Location(null);
            tiles = new Image[128];
            chest = new Image[4,4];
            for (int i = 0; i < 128; i++)
            {
                tiles[i] = new Bitmap(32, 32);
                Graphics g = Graphics.FromImage(tiles[i]);

                Rectangle src = GetSourceRectTile(i);
                g.DrawImage(tilesList, new Point[] { new Point(0, 0), new Point(tiles[i].Width, 0), new Point(0, tiles[i].Height) }, src, GraphicsUnit.Pixel);
                g.Dispose();
            }
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    chest[x, y] = new Bitmap(32, 48);
                    Graphics g = Graphics.FromImage(chest[x, y]);

                    Rectangle src = new Rectangle(x * 32, y * 48, 32, 48);
                    g.DrawImage(chestList, new Point[] { new Point(0, 0), new Point(chest[x, y].Width, 0), new Point(0, chest[x, y].Height) }, src, GraphicsUnit.Pixel);
                    g.Dispose();
                }
            }
        }

        private void EditorForm_Paint(object sender, PaintEventArgs e)
        {
            ReDraw();
        }

        private void ReDraw()
        {
            cameraInfoLabel.Text = viewPoint.ToString();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                if (location != null)
                {
                    Image r = RenderGamePole();
                    try
                    {
                        Invoke(new Action(() => gamePoleView.BackgroundImage = r));
                    }
                    catch (Exception) { }

                }
            };
            bw.RunWorkerAsync();
        }

        private Image RenderGamePole()
        {
            Bitmap result = new Bitmap(gamePoleView.Width, gamePoleView.Height);
            Graphics g = Graphics.FromImage(result);

            g.Clear(Color.Black);

            renderMutex.WaitOne();
            foreach (var obj in location.GetObjectsIntersectsRect(new Rectangle(viewPoint, new Size(gamePoleView.Width, gamePoleView.Height))).OrderBy((GamePoleObject o) => o.Layer))
            {
                Point pos = (obj.Position - viewPoint.ToVector2()).ToDrawingPoint();
                Size s = new Size(obj.Size.ToDrawingPoint());
                Image texture = GetObjectTexture(obj);
                if (selected == obj)
                {
                    Bitmap bm = new Bitmap(texture.Width, texture.Height);
                    ImageAttributes ia = new ImageAttributes();
                    ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(new float[][]
                    {
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {.00f, .00f, 1.00f, .0f, 1}
                    });
                    ia.SetColorMatrix(cm);
                    Graphics gr = Graphics.FromImage(bm);
                    gr.DrawImage(texture, new Rectangle(0, 0, texture.Width, texture.Height), 0, 0, texture.Width, texture.Height, GraphicsUnit.Pixel, ia);
                    texture = bm;
                }
                g.DrawImage(texture, new Rectangle(pos, s));
            }
            renderMutex.ReleaseMutex();

            g.Dispose();

            return result;
        }


        private Rectangle GetSourceRectTile(int type)
        {
            int width = 8;
            int x = type % width;
            int y = type / width;
            return new Rectangle(x * 32, y * 32, 32, 32);
        }

        private Image GetObjectTexture(GamePoleObject obj)
        {
            if (obj is GamePoleObjectTile)
            {
                var t = (GamePoleObjectTile)obj;
                return tiles[t.Type];
            }
            if (obj is GamePoleObjectChest)
            {
                var t = (GamePoleObjectChest)obj;
                return chest[t.Type, t.AnimationFrame];
            }
            return new Bitmap((int)obj.Size.X, (int)obj.Size.Y);
        }

        private void moveLeftButton_Click(object sender, EventArgs e)
        {
            viewPoint.X -= 32;
            ReDraw();
        }

        private void moveRightButton_Click(object sender, EventArgs e)
        {
            viewPoint.X += 32;
            ReDraw();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            viewPoint.Y -= 32;
            ReDraw();
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            viewPoint.Y += 32;
            ReDraw();
        }

        private void gamePoleView_MouseClick(object sender, MouseEventArgs e)
        {
            Point glob = (viewPoint.ToVector2() + e.Location.ToVector2()).ToDrawingPoint();
            GamePoleObject[] objs = location.GetObjectsIntersectsPoint(glob.ToVector2()).OrderByDescending((GamePoleObject o) => o.Layer).ToArray();
            if (objs.Length < 2)
                Select(objs.ElementAtOrDefault(0));
            else
            {
                SelectorForm sf = new SelectorForm();
                sf.Text = "Select one objet";
                sf.values = objs.Select((GamePoleObject o) => o.ToString()).ToArray();
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    Select(objs[sf.selected]);
                }
            }
        }

        private void Select(GamePoleObject obj)
        {
            location.UpdateCache();
            selected = obj;
            if (selected != null)
            {
                objectInfoLabel.Text = obj.GetType().ToString() + "\n" + obj.Position.ToString() + "\n" + obj.UUID + "\nLayer: " + obj.Layer;
                objectMovePanel.Show();
                if (selected is GamePoleObjectTile)
                {
                    var o = (GamePoleObjectTile)obj;
                    tilePanel.Show();
                    tileLabel.Text = "Type: " + o.Type;
                }
                else
                {
                    tilePanel.Hide();
                    tileLabel.Text = "";
                }
            }
            else
            {
                objectInfoLabel.Text = "";
                objectMovePanel.Hide();
                tilePanel.Hide();
                tileLabel.Text = "";
            }
            Application.DoEvents();
            Invalidate();
        }

        private void EditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S || e.KeyCode == Keys.A || e.KeyCode == Keys.D)
                if (e.Shift)
                {
                    keyInputTimer.Interval = 100;
                    isViewPointMoving = true;
                    viewPointMoveKey = e.KeyCode;
                    viewPointMovingSpeed = (e.Control) ? 64 : 32;
                    if (!keyInputTimer.Enabled)
                        keyInputTimer_Tick(null, null);
                    keyInputTimer.Enabled = true;
                }
                else if (selected != null)
                {
                    keyInputTimer.Interval = 150;
                    isSelectedObjectMoving = true;
                    selectedObjectMoveKey = e.KeyCode;
                    selectedObjectMovingSpeed = (e.Control) ? 64 : 32;
                    if (!keyInputTimer.Enabled)
                        keyInputTimer_Tick(null, null);
                    keyInputTimer.Enabled = true;
                }
            if (e.KeyCode == Keys.Delete)
                deleteToolStripMenuItem_Click(null, null);
        }

        private void EditorForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (isViewPointMoving)
            {
                if (e.KeyCode == viewPointMoveKey)
                {
                    isViewPointMoving = false;
                    keyInputTimer.Enabled = false;
                }
            }
            if (isSelectedObjectMoving)
            {
                if (e.KeyCode == selectedObjectMoveKey)
                {
                    isSelectedObjectMoving = false;
                    keyInputTimer.Enabled = false;
                }
            }
        }

        private void keyInputTimer_Tick(object sender, EventArgs e)
        {
            if (isViewPointMoving)
            {
                switch (viewPointMoveKey)
                {
                    case Keys.W:
                        viewPoint.Y -= viewPointMovingSpeed;
                        ReDraw();
                        break;
                    case Keys.S:
                        viewPoint.Y += viewPointMovingSpeed;
                        ReDraw();
                        break;
                    case Keys.A:
                        viewPoint.X -= viewPointMovingSpeed;
                        ReDraw();
                        break;
                    case Keys.D:
                        viewPoint.X += viewPointMovingSpeed;
                        ReDraw();
                        break;
                }
            }
            if (isSelectedObjectMoving)
            {
                switch (selectedObjectMoveKey)
                {
                    case Keys.W:
                        selected.Position -= new Microsoft.Xna.Framework.Vector2(0, selectedObjectMovingSpeed);
                        Select(selected);
                        ReDraw();
                        break;
                    case Keys.S:
                        selected.Position += new Microsoft.Xna.Framework.Vector2(0, selectedObjectMovingSpeed);
                        Select(selected);
                        ReDraw();
                        break;
                    case Keys.A:
                        selected.Position -= new Microsoft.Xna.Framework.Vector2(selectedObjectMovingSpeed, 0);
                        Select(selected);
                        ReDraw();
                        break;
                    case Keys.D:
                        selected.Position += new Microsoft.Xna.Framework.Vector2(selectedObjectMovingSpeed, 0);
                        Select(selected);
                        ReDraw();
                        break;
                }
            }
        }

        private void moveToButton_Click(object sender, EventArgs e)
        {
            if (selected != null)
            {
                MoveToForm mf = new MoveToForm();
                mf.X = selected.Position.X;
                mf.Y = selected.Position.Y;
                if (mf.ShowDialog() == DialogResult.OK)
                {
                    selected.Position = new Microsoft.Xna.Framework.Vector2(mf.X, mf.Y);
                }
            }
        }

        private void layerPlusButton_Click(object sender, EventArgs e)
        {
            if (selected != null)
            {
                selected.__hackSetLayer(selected.Layer + 1);
                Select(selected);
                ReDraw();
            }
        }

        private void layerMinusButton_Click(object sender, EventArgs e)
        {
            if (selected != null)
            {
                selected.__hackSetLayer(selected.Layer - 1);
                Select(selected);
                ReDraw();
            }
        }

        private void changeTypeButton_Click(object sender, EventArgs e)
        {
            if (selected != null && selected is GamePoleObjectTile)
            {
                var o = (selected as GamePoleObjectTile);
                IntegerDialogForm id = new IntegerDialogForm();
                id.Text = "Change Type";
                id.value = o.Type;
                if (id.ShowDialog() == DialogResult.OK)
                {
                    o.Type = id.value;
                    Select(selected);
                    ReDraw();
                }
            }
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var o = new GamePoleObjectTile(Microsoft.Xna.Framework.Vector2.Zero, 0, false, 0);
            location.AddObject(o);
            Select(o);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected != null)
            {
                location.RemoveObject(selected);
                Select(null);
            }
        }
    }
}

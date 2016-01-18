using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using OpenTKLib;

using ImageTool;

namespace rangeImageGenerator
{
	public partial class Form1 : Form
	{
		//Global
		private OpenGLRenderer GLrender;
		private Model3D model;

		private int objImgWidth, objImgHeight;
		private int pcImgWidth, pcImgHeight;
		private double gridSize;

		private float[] pixels;
		private byte[,,] objRangeImg;


		public static DialogResult InputBox(string title, string promptText, out double value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = String.Empty;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			try
			{
				value = Convert.ToDouble(textBox.Text);
			}
			catch (Exception)
			{
				value = -1.0;
			}

			return dialogResult;
		}


		public Form1()
		{
			InitializeComponent();
		}


		private void loadobjToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "obj files (*.obj)|*.obj";
			try
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					glControl1.Visible = true;

					//GL initialization
					GL.ClearColor(Color.SkyBlue);
					GL.Enable(EnableCap.DepthTest);
					GLrender = new OpenGLRenderer(glControl1);

					//glControl1 initialization
					GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
					GL.LoadIdentity();


					model = new Model3D(ofd.FileName);
					model.ModelRenderStyle = CLEnum.CLRenderStyle.Solid;
					model.ResetModelToOrigin();
					GLrender.Models3D.Add(model);


					//ask for grid size
					DialogResult dr = Form1.InputBox("Grid Size?", "Please enter grid size(m):", out gridSize);
					if (dr == DialogResult.OK && gridSize == -1.0)
					{
						MessageBox.Show("Invalid value!\nPlease try again.", "Error", MessageBoxButtons.OK);
						return;
					}
					else if (dr == DialogResult.Cancel)
					{
						return;
					}


					//glControl1 initialization
					GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
					GL.LoadIdentity();


					//calculate range image size of '.obj'
					model.CalculateBoundingBox(true);
					Vector3d vMax = model.MaxPoint.Vector;
					Vector3d vMin = model.MinPoint.Vector;
					objImgWidth = (int)Math.Ceiling((Math.Ceiling(vMax[0]) - Math.Floor(vMin[0])) / gridSize);
					objImgHeight = (int)Math.Ceiling((Math.Ceiling(vMax[1]) - Math.Floor(vMin[1])) / gridSize);


					//setupViewport
					Vector3d eye = new Vector3d(0, 0, vMax[2]);
					Vector3d center = new Vector3d(0, 0, 0);
					Vector3d up = new Vector3d(0, 1, 0);

					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadIdentity();
					Matrix4d mat = Matrix4d.LookAt(eye.X, eye.Y, eye.Z, center.X, center.Y, center.Z, up.X, up.Y, up.Z);
					GL.LoadMatrix(ref mat);

					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					GL.Ortho(Math.Floor(vMin[0]), Math.Ceiling(vMax[0]), Math.Floor(vMin[1]), Math.Ceiling(vMax[1]), 0, vMax[2] - vMin[2]);

					GL.Viewport(new Rectangle(0, 0, objImgWidth, objImgHeight));


					//resize glControl1
					glControl1.Width = objImgWidth;
					glControl1.Height = objImgHeight;

					glControl1.Visible = false;

					//draw model
					model.Render(false);

					generateRangeImageToolStripMenuItem.PerformClick();

					MessageBox.Show("Successful!");
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Error!");
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			if (pictureBox1.Image == null)
				return;

			SaveFileDialog sfd = new SaveFileDialog();
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					pictureBox1.Image.Save(sfd.FileName);
					MessageBox.Show("Successful!");
				}
				catch (Exception)
				{
					MessageBox.Show("Error!");
				}
			}
		}

		private void loadxyzToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "xyz files (*.xyz)|*.xyz";
			try
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{

				}
			}
			catch (Exception)
			{
				MessageBox.Show("Error!");
			}
		}


		private void generateRangeImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (model == null)
				return;



			//draw depth
			GL.Enable(EnableCap.DepthTest);
			GL.ClearDepth(1.0);
			pixels = new float[objImgWidth * objImgHeight];
			objRangeImg = new byte[objImgWidth, objImgHeight, 3];
			GL.ReadPixels(0, 0, objImgWidth, objImgHeight, PixelFormat.DepthComponent, PixelType.Float, pixels);

			for (int i = 0; i < pixels.Length; i++)
				pixels[i] *= 255.0f;

			for (int i = 0; i < objImgWidth; i++)
			{
				for (int j = 0; j < objImgHeight; j++)
				{
					objRangeImg[i, j, 0] = Convert.ToByte(pixels[(objImgHeight - 1 - j) * objImgWidth + i]);
                }
			}


			//prepare display range image of 'obj'
			pictureBox1.Image = BitmapTool.Array2Bitmap(objRangeImg, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

		}

	}
}

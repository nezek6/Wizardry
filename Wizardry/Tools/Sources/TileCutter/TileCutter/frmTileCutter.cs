using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TileCutter
{
	public partial class frmTileCutter : Form
	{
		#region Data Members

		GraphicsDevice gd;

		Texture2D originalImg;

		#endregion

		#region Constructors / Initializers

		public frmTileCutter()
		{
			InitializeComponent();
			InitGraphicsDevice();
			ResetPanel();
		}

		public void InitGraphicsDevice()
		{
			PresentationParameters parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(this.ClientSize.Width, 1);
            parameters.BackBufferHeight = Math.Max(this.ClientSize.Height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DepthStencilFormat = DepthFormat.Depth24;
            parameters.DeviceWindowHandle = this.Handle;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.IsFullScreen = false;

            gd = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                    GraphicsProfile.HiDef,
                                    parameters);
		}

		#endregion

		#region Event Handlers

		private void mnuFile_Exit_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void mnuFile_Close_Click( object sender, EventArgs e )
		{
			originalImg = null;
			ResetPanel();
		}

		private void mnuFile_Open_Click( object sender, EventArgs e )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
			{
				lblFileName.Text = dlg.SafeFileName;
				LoadImage( dlg.FileName );
				EnablePanel();
			}
		}

		private void txtOutFileBaseName_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Space )
			{
				e.SuppressKeyPress = true;
			}
		}

		private void btnEngage_Click( object sender, EventArgs e )
		{
			if ( String.IsNullOrEmpty( txtOutFileBaseName.Text ) )
			{
				MessageBox.Show( "Please select a base output file name and directory.", "Halt!" );
				return;
			}
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select Output Directory:";
			if( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
			{
				ProcessImage( dlg.SelectedPath );
			}
		}

		#endregion

		#region Methods

		private void LoadImage( string fileName )
		{
			FileStream s = new FileStream( fileName, FileMode.Open );
			originalImg = Texture2D.FromStream( gd, s );
			s.Close();
		}

		private void EnablePanel()
		{
			if ( originalImg != null )
			{
				nudCutHeight.Maximum = originalImg.Height;
				nudCutWidth.Maximum = originalImg.Width;
				panControls.Enabled = true;
			}
		}

		private void ResetPanel()
		{
			panControls.Enabled = false;
			lblFileName.Text = "";
			nudCutHeight.Maximum = 10000;
			nudCutWidth.Maximum = 10000;
			nudCutHeight.Value = 64;
			nudCutWidth.Value = 64;
			cmbOutFileType.SelectedIndex = 0;
		}

		private void ProcessImage( string outputDir )
		{
			string outfileBase = outputDir + @"\" + txtOutFileBaseName.Text;
			Rectangle frame = new Rectangle( 0, 0, (int)nudCutWidth.Value, (int)nudCutHeight.Value );
			Texture2D workingImg = new Texture2D( gd, frame.Width, frame.Height );
			int count = 0;

			int numRows = originalImg.Height / frame.Height;
			int numCols = originalImg.Width / frame.Width;
			int numPixels = frame.Width * frame.Height;

			for ( int i = 0; i < numRows; i++ )
			{
				frame.Y = i * frame.Height;
				for ( int j = 0; j < numCols; j++ )
				{
					frame.X = j * frame.Width;

					Color[] cpy = new Color[numPixels];
					originalImg.GetData<Color>( 0, frame, cpy, 0, numPixels );
					workingImg.SetData<Color>( cpy );

					// Save the image
					Stream s;
					switch ( cmbOutFileType.SelectedIndex )
					{
						case 0:
							s = File.OpenWrite( outfileBase + count.ToString() + ".png" );
							workingImg.SaveAsPng( s, workingImg.Width, workingImg.Height );
							s.Close();
							break;
						case 1:
							s = File.OpenWrite( outfileBase + count.ToString() + ".jpg" );
							workingImg.SaveAsJpeg( s, workingImg.Width, workingImg.Height );
							s.Close();
							break;
						default:
							break;
					}

					count++;
				}
			}

			MessageBox.Show( "Created " + count.ToString() + " tiles!", "Processing Done!" );
		}

		#endregion
	}
}
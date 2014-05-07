using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIManager.Controls
{
	public class Selector : Control
    {
        #region Data Members

        //The index of the Currently selected string
        private int curSelect = 0;

        //The list of strings representing the possible choices
        private List<string> items = new List<string>();

        //The up arrow button to scroll left
        private Button left;

        //The down arrow button to scroll right
        private Button right;

        //The texture of the arrow buttons
        private Texture2D arrowTexture;

        // The text's font
        private SpriteFont textFont = null;

		// The text's colour
		private Color textColour = Color.Black;

        #endregion


        #region Initialization

        /// <summary>
        /// ListView class constructor.
        /// </summary>
        /// <param name="position">The top-left corner of the Selector.</param>
        /// <param name="width">The Selector's width.</param>
        /// <param name="height">The Selector's height.</param>
        /// <param name="screen">Reference to the screen on which the Selector exists.</param>
        public Selector(Point position, int width, int height, GameScreen screen)
            : base(position, width, height, screen)
        {
            // left button initialization
            left = new Button(new Point(base.Position.X - 50, position.Y + (height/2) - 25), 50, 50, base.screen);

            //right button initialization
            right = new Button(new Point(base.Position.X + width, position.Y + (height / 2) - 25), 50, 50, base.screen);

            //arrow button callbacks
            left.RegisterCallback(Left_OnClick);
            right.RegisterCallback(Right_OnClick);
        }


        //override abstract method of Control class
        protected override void InitInputs()
        {
        }

        #endregion


        #region Properties

        //get and set Selector text font
        public SpriteFont Font
        {
            get
            {
                return textFont;
            }
            set
            {
                textFont = value;
            }
        }

        //Read only current selected string getter
        public string SelectedItem
        {
            get
            {
                if (items.Count > 0)
                {
                    return items[curSelect];
                }
                else
                {
                    return "";
                }
            }
        }

		// Gets or sets the Selected index
		public int SelectedIndex
		{
			get { return curSelect; }
			set
			{
				curSelect = (int)MathHelper.Clamp( value, 0, items.Count );
			}
		}

        //Set the texture of the arrow buttons
        public Texture2D ArrowTexture
        {
            set
            {
                left.Texture = value;
                right.Texture = value;
            }
        }

        //Set the font of the arrow buttons (not needed when changed with image)
        public SpriteFont ArrowFont
        {
            set
            {
                left.SetFont(value, Color.Black, Color.Black, Color.Black, Color.Black);
                left.Text = "Left";
                right.SetFont(value, Color.Black, Color.Black, Color.Black, Color.Black);
                right.Text = "Right";
            }
        }

		// Gets or sets the colour of the text
		public Color TextColour
		{
			get { return textColour; }
			set { textColour = value; }
		}

        #endregion

        #region Methods

        //Clears the Selector deleting all items and clearing text.
        public void Clear()
        {
            curSelect = 0;
            items.Clear();
        }

        //Adds an element to the end of the items list, if the items list was empty, set the added
        //text as the Selector curSelect text.
        public void AddElement(string added)
        {
            items.Add(added);
        }

        //Removes the first instance of the removed string from the items list
        //if the removed element is the current displayed text of the Selector 
        //with the updated list.
        public void RemoveElement(string removed)
        {
            int index = items.IndexOf(removed);
            if (index >= 0)
            {
                items.RemoveAt(index);

                if (items.Count == 0)
                {
                    curSelect = 0;
                }
                else if ((index == curSelect) || (index < curSelect))
                {
                    curSelect--;
                }
            }
        }

        #endregion

        #region Update/Draw Overrides

        // Called automatically every frame if the ListView is active.
        protected override void Update(GameTime gameTime, InputState input)
        {
            //lets the arrow buttons handle their own inputs
            left.HandleInputs(gameTime, input);
            right.HandleInputs(gameTime, input);
        }

        // Called automatically every frame so that the ListView can draw itself.
        public override void Draw(GameTime gameTime, float alpha, SpriteEffects effect)
        {
            //lets the arrow buttons draw themselves
            left.Draw( gameTime, alpha );
            right.Draw( gameTime, alpha, SpriteEffects.FlipHorizontally );

            // Open the drawing buffer
            SpriteBatch sb = screen.ScreenManager.SpriteBatch;
            sb.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend );

            // Draw Selector text (if set)
			string text = ( items.Count > 0 ) ? items[curSelect] : "";
            if (text != "" && Font != null)
            {
                Vector2 textSize = Font.MeasureString(text);

                int textXOffset = (int)Math.Ceiling((Width - textSize.X) / 2);
                int textYOffset = (int)Math.Ceiling((Height - textSize.Y) / 2);

                sb.DrawString(Font, text, new Vector2(Position.X + textXOffset, Position.Y + textYOffset), textColour * alpha);
            }

            // Close the drawing buffer
            sb.End();
        }

        #endregion

        #region Callbacks

        //Called whenever the left arrow button is clicked
        public void Right_OnClick(Control sender)
        {
            //as long as the currently selected element isnt at the bottem of the list, and the list is not empty
            if ((curSelect != (items.Count() - 1)) && (items.Count() > 0))
            {
                //currently selected index updated
                curSelect++;
				Trigger();
            }
        }

        //Called whenever the right arrow button is clicked
        public void Left_OnClick(Control sender)
        {
            //as long as the currently selected element isnt at the top of the list, and the list is not empty
            if ((curSelect != 0) && (items.Count() > 0))
            {
                //currently selected index updated
                curSelect--;
				Trigger();
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIManager.Controls
{
    public class ListView : Control
    {
        #region Data Members

        //The index of the Currently selected string (Button Label)
        private int curSelect = 0;

        //The Index in items of the top visible string in the list view
        private int top = 0;

        //The list of strings representing the possible choices
        private List<string> items = new List<string>();

        //The list of Buttons comprising the ListView
        private List<Button> Buttons = new List<Button>();

        //The texture of the underline buttons of the ListView
        private Texture2D texture = null;

        //The Font of the underline buttons of the ListView
        private SpriteFont font = null;

        //The number of Buttons/visible choices in the ListView
        private int elements;

        //The up arrow button to scroll the list
        private Button up;

        //The down arrow button to scroll the list
        private Button down;

        //The texture of the arrow buttons
        private Texture2D arrowTexture;


        #endregion

        #region Initialization

        /// <summary>
        /// ListView class constructor.
        /// </summary>
        /// <param name="elements">The number of visible elements in ListView.</param>
        /// <param name="position">The top-left corner of the ListView.</param>
        /// <param name="width">The ListView's width.</param>
        /// <param name="height">The ListView's height.</param>
        /// <param name="screen">Reference to the screen on which the ListView exists.</param>
        public ListView(int elements, Point position, int width, int height, GameScreen screen)
            : base(position, width, height, screen)
        {
            // up button initialization
            up = new Button(new Point(base.Position.X + 10 + base.width, position.Y), 50, 50, base.screen);

            this.elements = elements;

            //listview button height
            int buttonHeight = base.height / elements;

            //down button initialization
            down = new Button(new Point(base.Position.X + 10 + base.width, position.Y + buttonHeight * (elements - 1)), 50, 50, base.screen);

            //arrow button callbacks
            up.RegisterCallback(Up_OnClick);
            down.RegisterCallback(Down_OnClick);

            //minimum threshold check for listview buttons height
            if (buttonHeight < 21)
            {
                buttonHeight = 21;
            }

            //initialize listview buttons
            for (int i = 0; i < elements; i++)
            {
                Buttons.Add(new Button(new Point(base.position.X, base.position.Y + (i * buttonHeight)), base.width, buttonHeight, base.screen));
            }
        }


        //override abstract method of Control class
        protected override void InitInputs()
        {
        }

        #endregion

        #region Properties

        //Read only current selected string getter
        public string CurrentSelection
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

		public int SelectedIndex
		{
			get
			{
				return curSelect;
			}
		}

        //get and set the ListView button's texture.
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                //set the texture for all the buttons in the ListView
                foreach (Button butt in Buttons)
                {
                    texture = value;
                    butt.Texture = texture;
                }
            }
        }

        //Set font of Listview buttons
        public SpriteFont Font
        {
            set
            {
                //sets the font for all the buttons in the ListView
				font = value;
                foreach (Button butt in Buttons)
                {
                    butt.SetFont(font, Color.Black, Color.Black, Color.Black, Color.Black);
                }
            }
			get
			{
				return font;
			}
        }

        //Set the texture of the arrow buttons
        public Texture2D ArrowTexture
        {
            set
            {
                up.Texture = value;
                down.Texture = value;
            }
        }

        //Set the font of the arrow buttons (not needed when changed with image)
        public SpriteFont ArrowFont
        {
            set
            {
                up.SetFont(value, Color.Black, Color.Black, Color.Black, Color.Black);
                up.Text = "Up";
                down.SetFont(value, Color.Black, Color.Black, Color.Black, Color.Black);
                down.Text = "Down";
            }
        }

		public Color ItemTextColour
		{
			set
			{
				foreach ( Button b in Buttons )
				{
					b.SetFont( font, value, value, value, value );
				}
			}
		}


        #endregion

        #region Methods

        //Clears the ListView deleting all elements and clearing all ListView button's text.
        public void Clear()
        {
            curSelect = 0;
            top = 0;
            items.Clear();
            foreach (Button butt in Buttons)
            {
                butt.Text = "";
				butt.Active = true;
            }
        }

        //Adds an element to the end of the items list, if there are still unlabeled buttons
        //in the ListView it labels them with the added string.
        public void AddElement( string added )
        {
            if (Buttons.Count > items.Count)
            {
                Buttons[items.Count].Text = added;
            }
            items.Add(added);
        }

        //Removes the first instance of the removed string from the items list
        //if the removed element is currently a label of a ListView button, relabe the buttons
        //with the updated list.
        public void RemoveElement( string removed )
        {
            int index = items.IndexOf(removed);

            //ensure removed string was in the items list, if not - do nothing
            if (index >= 0)
            {
                //remove the wanted string
                items.RemoveAt(index);
                //if the string was currently a label in one of the listviews buttons, adjust the labels
                if ((index < (top + elements)) && (index >= top))
                {
                    //ensure that from the current top index you can still label all the buttons, if not 
                    //adjust the top index to label all the buttons, if the items list is smaller than
                    //the number of elements/buttons in the list view, pad the bottom buttons with empty
                    //labels.
                    while ((elements > (items.Count - top)) && (top > 0))
                    {
                        top--;
                        if (curSelect != index)
                        {
                            curSelect--;
                        }
                    }
                    //adjust curselected index 
                    if (curSelect == index)
                    {
                        curSelect = top;
                    }
                    else if (index < curSelect)
                    {
                        curSelect--;
                    }
                    int curtop = top;
                    foreach (Button butt in Buttons)
                    {
                        if (curtop <= (items.Count - 1))
                        {
                            butt.Text = items[curtop];
                            curtop++;
                        }
                        else
                        {
                            butt.Text = "";
                        }
                    }

                }
                //if the removed element was above the viewed elements in the list, adjust the top and curSelect indexes
                else if (index < top)
                {
                    curSelect--;
                    top--;
                }
            }

        }
        #endregion

        #region Update/Draw Overrides

        // Called automatically every frame if the ListView is active.
        protected override void Update(GameTime gameTime, InputState input)
        {
            //lets the arrow buttons handle their own inputs
            up.HandleInputs(gameTime, input);
            down.HandleInputs(gameTime, input);

			if ( !(items.Count == 0) )
			{
				//sets the currently selected ListView button to inactive, to mark it visually
				Buttons[curSelect - top].Active = false;
			}
        }

        // Called automatically every frame so that the ListView can draw itself.
        public override void Draw( GameTime gameTime, float alpha, SpriteEffects effect )
        {
            //lets the arrow buttons draw themselves
            up.Draw(gameTime, alpha);
            down.Draw(gameTime, alpha, SpriteEffects.FlipVertically);
            //let each ListView button draw itself
            foreach (Button butt in Buttons)
            {
                butt.Draw(gameTime, alpha);
            }
        }

        #endregion

        #region Callbacks

        //Called whenever the down arrow button is clicked
        public void Down_OnClick(Control sender)
        {
            //as long as the currently selected element isnt at the bottem of the list, and the list is not empty
            if ((curSelect != (items.Count() - 1)) && (items.Count() > 0))
            {
                int curtop = top;
                //set the old selected button in the view to active (the visual mark of selection is inactive)
                Buttons[curSelect - top].Active = true;
                //currently selected index updated
                curSelect++;
                //if the view needs to be scrolled down, update the index of the top viewable element and update the button labels
                if ((curSelect - top) >= elements)
                {
                    top++;
                    curtop++;
                    foreach (Button butt in Buttons)
                    {
                        butt.Text = items[curtop];
                        curtop++;
                    }
                }
            }
        }

        //Called whenever the up arrow button is clicked
        public void Up_OnClick(Control sender)
        {
            //as long as the currently selected element isnt at the top of the list, and the list is not empty
            if ((curSelect != 0) && (items.Count() > 0))
            {
                int curtop = top;
                //set the old selected button in the view to active (the visual mark of selection is inactive)
                Buttons[curSelect - top].Active = true;
                //currently selected index updated
                curSelect--;
                //if the view needs to be scrolled up, update the index of the top viewable element and update the button labels
                if ((curSelect - top) < 0)
                {
                    top--;
                    curtop--;
                    foreach (Button butt in Buttons)
                    {
                        butt.Text = items[curtop];
                        curtop++;
                    }
                }
            }
        }

        #endregion
    }
}

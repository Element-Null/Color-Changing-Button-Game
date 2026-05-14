using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace ColorButtons
{
    public partial class Form1 : Form
    {

        public class ButtonScript
        {
            public Button self;
            public List<ButtonScript> Neighbors = new List<ButtonScript>();
            public Color onColor = Color.AntiqueWhite;
            public Color offColor = Color.Gray;
            public Color currentColor = Color.White;

            public void Initialize(Button you)
            {
                //do more than one color as a rotating list!!
                self = you;
                self.Click += OnClick;
                self.Text = "";
                Random rnd = new Random();
                if (rnd.NextDouble() < 0.5f)
                {
                    currentColor = onColor;
                }
                else
                {
                    currentColor = offColor;
                }
                self.BackColor = currentColor;
            }

            private void OnClick(object sender, EventArgs e)
            {
                GetClicked();
            }

            public void ChangeColor()
            {
                if (currentColor == onColor)
                {
                    currentColor = offColor;
                }
                else
                {
                    currentColor = onColor;
                }
                self.BackColor = currentColor;
            }

            public void GetClicked()
            {
                ChangeColor();
                if (Neighbors.Count > 0)
                {
                    foreach (ButtonScript neighbor in Neighbors)
                    {
                        neighbor.ChangeColor();
                    }
                }
            }
        }


        ButtonScript[] line1 = new ButtonScript[4];
        ButtonScript[] line2 = new ButtonScript[4];
        ButtonScript[] line3 = new ButtonScript[4];
        List<ButtonScript> allButtonScripts = new List<ButtonScript>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupButtons();
        }

        public void SetupButtons()
        {
            List<Button> allButtons = GetAllButtons(this);

            for (int i = 0; i < line1.Length; i++)
            {
                line1[i] = new ButtonScript();
                line2[i] = new ButtonScript();
                line3[i] = new ButtonScript();
            }

            List<Button> highestButtons = GetHighestButtons(allButtons);
            for (int i = 0; i < highestButtons.Count; i++)
            {
                line1[i].Initialize(highestButtons[i]);
                allButtons.Remove(highestButtons[i]);
            }
            List<Button> middleButtons = GetHighestButtons(allButtons);
            for (int i = 0; i < middleButtons.Count; i++)
            {
                line2[i].Initialize(middleButtons[i]);
                allButtons.Remove(middleButtons[i]);
            }
            List<Button> lowestButtons = GetHighestButtons(allButtons);
            for (int i = 0; i < lowestButtons.Count; i++)
            {
                line3[i].Initialize(lowestButtons[i]);
            }
            RerandomizeButtonColors();

            LinkNeighbors();
        }

        public void LinkNeighbors()
        {
            for(int i = 0;i < line1.Length;i++)
            {
                line1[i].Neighbors.Add(line2[i]);
                line2[i].Neighbors.Add(line3[i]);


                line2[i].Neighbors.Add(line1[i]);
                line3[i].Neighbors.Add(line2[i]);
                if(i > 0)
                {
                    line1[i].Neighbors.Add(line1[i - 1]);
                    line2[i].Neighbors.Add(line2[i - 1]);
                    line3[i].Neighbors.Add(line3[i - 1]);
                }
                if(i < line1.Length - 1)
                {
                    line1[i].Neighbors.Add(line1[i + 1]);
                    line2[i].Neighbors.Add(line2[i + 1]);
                    line3[i].Neighbors.Add(line3[i + 1]);
                }
            }
        }

        public void RerandomizeButtonColors()
        {
            for (int i = 0; i < line1.Length; i++)
            {
                allButtonScripts.Add(line1[i]);
                allButtonScripts.Add(line2[i]);
                allButtonScripts.Add(line3[i]);
            }
            foreach (ButtonScript bs in allButtonScripts)
            {
                Random ran = new Random();
                if (ran.NextDouble() < 0.5f)
                {
                    bs.ChangeColor();
                }
            }
            Color button1Color = allButtonScripts[0].currentColor;
            bool allSame = true;
            foreach (ButtonScript bs in allButtonScripts)
            {
                if(bs.currentColor != button1Color)
                {
                    allSame = false;
                }
            }
            if(allSame)
            {
                RerandomizeButtonColors();
            }
        }

        public List<Button> GetHighestButtons(List<Button> allButtons)
        {
            float highestY = float.PositiveInfinity;
            List<int> highestButtonsIndices = new List<int>();

            for (int i = 0; i < allButtons.Count; i++)
            {
                if (allButtons[i].Location.Y < highestY)
                {
                    highestButtonsIndices.Clear();
                    highestButtonsIndices.Add(i);
                    highestY = allButtons[i].Location.Y;
                }
                else if (allButtons[i].Location.Y == highestY)
                {
                    highestButtonsIndices.Add(i);
                }
            }
            List<Button> unsortedHighestButtons = new List<Button>();
            foreach (int i in highestButtonsIndices)
            {
                unsortedHighestButtons.Add(allButtons[i]);
            }
            return SortButtonListByX(unsortedHighestButtons, new List<Button>());
        }

        public List<Button> SortButtonListByX(List<Button> unsortedButtons, List<Button> sortedButtons)
        {
            float highestX = float.PositiveInfinity;
            int highestXIndex = -1;
            for(int i = 0; i < unsortedButtons.Count; i++)
            {
                if (unsortedButtons[i].Location.X < highestX)
                {
                    highestX = unsortedButtons[i].Location.X;
                    highestXIndex = i;
                }
            }
            sortedButtons.Add(unsortedButtons[highestXIndex]);
            unsortedButtons.RemoveAt(highestXIndex);
            if(unsortedButtons.Count > 0)
            {
                return SortButtonListByX(unsortedButtons, sortedButtons);
            }
            else
            {
                return sortedButtons;
            }
        }

        private List<Button> GetAllButtons(Control parent)
        {
            List<Button> buttons = new List<Button>();

            foreach (Control c in parent.Controls)
            {
                if (c is Button)
                    buttons.Add((Button)c);

                if (c.HasChildren)
                    buttons.AddRange(GetAllButtons(c));
            }

            return buttons;
        }
    }
}

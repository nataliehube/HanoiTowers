using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.ComponentModel;
using System.Speech.Recognition;

namespace MBO_HanoiTowers
{

    /// Logic for MainWindow.xaml


    public partial class MainWindow : Window
    {
        // number of movable panel
        int maxCount = 6;
        // min width of first panel
        const double minWidth = 100;
        // height of all panels
        const double height = 100;
        // height of all panels
        const double canvasWidth = 350;
        // width of the grid
        double gridWidth = 0;
        // Animation Speed
        int moveSpeed = 1;
        // Canvas List
        List<Canvas>[] canvasList = new List<Canvas>[3];
        // click sender and click target
        int? senderPole = null;
        int? targetPole = null;

        private SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine();


        //main method
        public MainWindow()
        {
            InitializeComponent();
            init();

            int? numberOfPlates = null;

            // Create the Grammar instance and load it into the speech recognition engine.
            speechRecognizer.UnloadAllGrammars();
            Grammar g = new Grammar(@"C:\Users\Natalie\Dropbox\TU Dresden\VMI-10 Multimodale Benutzungsoberflächen\Uebungen\HanoiTowers\MBO_HanoiTowers\gb.xml", "hanoiTowers");
            speechRecognizer.LoadGrammar(g);

            speechRecognizer.SetInputToDefaultAudioDevice();

            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

            speechRecognizer.SpeechRecognized += (s, e1) =>
            {

                string from = e1.Result.Text;
                printSpeechRecognized(from);
                switch (from)
                {

                    //TODO: further commands by speech to be implemented (number of discs and solving and refreshing)

                    case "von":
                        senderPole = null;
                        break;
                    case "zu":
                    case "Turm Eins":
                        Console.WriteLine("Click 1");
                        handleInput(0);
                        break;
                    case "Turm Zwei":
                        Console.WriteLine("Click 2");
                        handleInput(1);
                        break;
                    case "Turm Drei":
                        Console.WriteLine("Click 3");
                        handleInput(2);
                        break;
                    case "Lösen":
                        solve_Click(null, null);
                        break;
                    case "Zurücksetzen":
                        reset_Click(null, null);
                        break;
                    case "Eins":
                        numberOfPlates = 1;
                        break;
                    case "Zwei":
                        numberOfPlates = 2;
                        break;
                    case "Drei":
                        numberOfPlates = 3;
                        break;
                    case "Vier":
                        numberOfPlates = 4;
                        break;
                    case "Fünf":
                        numberOfPlates = 5;
                        break;
                    case "Sechs":
                        numberOfPlates = 6;
                        break;
                    case "Sieben":
                        numberOfPlates = 7;
                        break;
                    case "Acht":
                        numberOfPlates = 8;
                        break;
                    case "Neun":
                        numberOfPlates = 9;
                        break;
                    case "Zehn":
                        numberOfPlates = 10;
                        break;
                    case "Anzahl":
                        numberOfPlates = null;
                        break;
                    case "Scheiben":
                        if (numberOfPlates != null)
                        {
                            Plate.Text = numberOfPlates.ToString();
                            changePlateCount(null, null);
                        }
                        break;
                    case "Scheibe":
                        if (numberOfPlates != null)
                        {
                            Plate.Text = numberOfPlates.ToString();
                            changePlateCount(null, null);
                        }
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            };


        }

        private void printSpeechRecognized(string speech)
        {
            Console.WriteLine(speech);
            message.Content = "Nummer erkannt " + speech;
        }

        // click command on grid rectangle
        // position 1,2 or 3
        private void userCommand(object sender, MouseButtonEventArgs e)
        {
            int? clicked = null;
            // specify origin
            switch (((FrameworkElement)e.Source).Name)
            {
                case "one":
                    Console.WriteLine("Click 1");
                    clicked = 0;
                    break;
                case "two":
                    Console.WriteLine("Click 2");
                    clicked = 1;
                    break;
                case "three":
                    Console.WriteLine("Click 3");
                    clicked = 2;
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            handleInput(clicked);
        }

        //function to handle Input
        private void handleInput(int? pole)
        {
            // define sender/from
            if (!senderPole.HasValue)
            {
                senderPole = pole;
            }
            // define target/to
            else
            {
                targetPole = pole;
                // check if turn is allowed (sender.width < target.width)!
                if ((getLastElementWidth(senderPole.Value) < getLastElementWidth(targetPole.Value) || getLastElementWidth(targetPole.Value) == 0) && getLastElementWidth(senderPole.Value) != 0)
                {
                    // start animation
                    moveTo(senderPole.Value, targetPole.Value);
                    System.Media.SystemSounds.Hand.Play();
                    message.Content = "";
                }
                else
                {
                    // wrong turn
                    System.Media.SystemSounds.Beep.Play();
                    message.Content = "Wrong turn.";
                }
                // reset parameters
                senderPole = null;
                targetPole = null;
            }


        }


        // get last element in canvas list to return the width of the specific rectangle
        // -> to check if turn is allowed
        private int getLastElementWidth(int i)
        {
            int w = 0;
            if (canvasList[i].Count() > 0)
            {
                Canvas cn = canvasList[i][canvasList[i].Count() - 1];
                foreach (UIElement element in cn.Children)
                {
                    if (element is Rectangle)
                    {
                        Rectangle rec = element as Rectangle;
                        w = (int) rec.ActualWidth;
                    }
                }
            }
            return w;
        }

        // click handler for reset button
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Beep.Play();
            senderPole = null;
            targetPole = null;
            init();
        }

        // click handler for solving button
        private void solve_Click(object sender, RoutedEventArgs e)
        {
            init();
            // disable buttons
            solve.IsEnabled = false;
            reset.IsEnabled = false;
            message.Content = "Rätsel wird gelöst. Bitte warten...";
            System.Media.SystemSounds.Hand.Play();
            // start thread queue
            ThreadPool.QueueUserWorkItem(new WaitCallback(startSolver));
        }

        // start automatic solving
        private void startSolver(Object stateInfo)
        {
            move(maxCount, 0, 2, 1);
        }

        // initiate canvas and all discs
        private void init() {
            // clear canvas
            Canvas1.Children.Clear();
            message.Content = "";
            Plate.Text = maxCount.ToString();
            canvasList[0] = new List<Canvas>();
            canvasList[1] = new List<Canvas>();
            canvasList[2] = new List<Canvas>();

            double d = Canvas1.Width / 3;
            gridWidth = d;
            // create discs
            for (int i = 0; i < maxCount; i++)
            {
                // new canvas for every disc
                Canvas cn = new Canvas();
                cn.Width = canvasWidth;
                cn.Height = height + 20;
                // new rectangle as "disc"
                Rectangle rc = new System.Windows.Shapes.Rectangle();
                rc.Fill = Brushes.Pink;
                rc.Width = canvasWidth - (i) * (250/(maxCount-1));
                rc.Height = height;
                rc.Stroke = Brushes.Indigo;
                // position of rectangle in canvas
                Canvas.SetTop(rc, 0);
                Canvas.SetLeft(rc, (i) * (250 / (maxCount - 1)) / 2);
                cn.Children.Add(rc);
                // position of canvas
                Canvas.SetLeft(cn, (d - cn.Width) / 2);
                Canvas.SetTop(cn, 1000 - (i+1) * height);
                Canvas1.Children.Add(cn);
                canvasList[0].Add(cn);
            }
        }

        // method to animate the move of the discs from grids
        private void moveTo(int from, int to)
        {
            if (to <= 2 && to >= 0)
            {
                System.Console.WriteLine("Bewege Scheibe von Turm " + from + " zu Turm " + to + ".");
                if (canvasList[from][0] != null)
                {
                    // handle selected canvas
                    Canvas cn = canvasList[from][canvasList[from].Count()-1];
                    canvasList[from].Remove(cn);
                    canvasList[to].Add(cn);
                    int count = canvasList[to].Count;
                    System.Console.WriteLine("FromCount: " + canvasList[from].Count());
                    System.Console.WriteLine("ToCount: " + count);
                    // animation process
                    Storyboard sb = new Storyboard();
                    DoubleAnimation da = new DoubleAnimation(Canvas.GetLeft(cn), (Canvas1.Width / 3 - cn.Width) / 2 + to * 400, new Duration(new TimeSpan(0, 0, moveSpeed)));
                    DoubleAnimation da1 = new DoubleAnimation(Canvas.GetTop(cn), 1000 - (count) * height, new Duration(new TimeSpan(0, 0, moveSpeed)));
                    System.Console.WriteLine("Height: " + (1000 - (count) * height));
                    Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Left)"));
                    Storyboard.SetTargetProperty(da1, new PropertyPath("(Canvas.Top)"));
                    sb.Children.Add(da);
                    sb.Children.Add(da1);

                    cn.BeginStoryboard(sb);
                    // finish check
                    if (isDone())
                    {
                        solve.IsEnabled = true;
                        reset.IsEnabled = true;
                        message.Content = "Fertig.";
                    }
                }
            }
            
        }

        delegate void moveAnimation(int from, int to);
        
        // recursiv method to solve the game
        // towers of hanoi algorithm
        public void move(int n, int from, int to, int via)
        {
            if (n == 1)
            {
                System.Console.WriteLine("Bewege Scheibe von Turm " + from + " zu Turm " + to + ".");
                // call gui thread for animation handling
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new moveAnimation(this.moveTo),from, to);
                System.Threading.Thread.Sleep(800);
            }
            else
            {
                move(n - 1, from, via, to);
                move(1, from, to, via);
                move(n - 1, via, to, from);
            }
        }

        // check if game is finished
        private Boolean isDone()
        {
            if (canvasList[2].Count() == maxCount || canvasList[1].Count() == maxCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        // handling key events for game interaction TODO!
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /*Key key = (Key)e.Key;
            String number = (String)key.ToString();
            int? clicked = null;

            // number keys
            if (number.Equals("D1")) { clicked = 0; System.Console.WriteLine("Press 1"); }
            else if (number.Equals("D2")) { clicked = 1; System.Console.WriteLine("Press 2"); }
            else if (number.Equals("D3")) { clicked = 2; System.Console.WriteLine("Press 3"); }
            
            if (!senderGrid.HasValue)
            {
                senderGrid = clicked;
            }
            else
            {
                targetGrid = clicked;
                if ((getLastElementWidth(senderGrid.Value) < getLastElementWidth(targetGrid.Value) || getLastElementWidth(targetGrid.Value) == 0) && getLastElementWidth(senderGrid.Value) != 0)
                {
                    moveTo(senderGrid.Value, targetGrid.Value);
                    System.Media.SystemSounds.Hand.Play();
                }
                senderGrid = null;
                targetGrid = null;
            }*/
        }

        // change plate count and reset game
        private void changePlateCount(object sender, KeyEventArgs e)
        {
            if (Plate.Text != null && Plate.Text != "")
            {
                int num = Convert.ToInt32(Plate.Text);
                if (num > 1 && num < 11)
                {
                    System.Console.WriteLine("Anzahl der Scheiben: " + Plate.Text);
                    maxCount = num;
                    init();
                }
                else
                {
                    message.Content = "Bitte eine Zahl zwischen 1 und 10 auswählen.";
                    Plate.Text = maxCount.ToString();
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }
    }
}

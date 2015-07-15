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
//using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Threading;
using System.ComponentModel;
using System.Speech.Recognition;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Timers;

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
        int? said_first = null;
        int? said_second = null;
        int? said = null;
        bool saidMove = false;
        bool nextMove = false;
        int senderValue;

        private SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine(new CultureInfo("de-DE"));

        // Variables for Mouse Gesture Processing
        Boolean button_pressed;
        Boolean Templating = false;
        MouseGesture current_gesture;
        MouseGesture one_gesture;
        MouseGesture two_gesture;
        MouseGesture three_gesture;
        Frame putthatthere;
        Frame close;

       
        //main method
        public MainWindow()
        {
            InitializeComponent();
            init();

            int? numberOfPlates = null;

            // Create the Grammar instance and load it into the speech recognition engine.
            speechRecognizer.UnloadAllGrammars();
            Grammar g = new Grammar(@"C:\Users\andyy_000\Documents\Visual Studio 2013\Projects\HanoiTowers\MBO_HanoiTowers\gb.xml", "hanoiTowers");
            speechRecognizer.LoadGrammar(g);

            speechRecognizer.SetInputToDefaultAudioDevice();

            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

            speechRecognizer.SpeechRecognized += (s, e1) =>
            {
                said_first = null;
                said_second = null;
                said = null;
                string from = e1.Result.Text;
                printSpeechRecognized(from);
                switch (from)
                {

                    //TODO: further commands by speech to be implemented (number of discs and solving and refreshing)

                    case "Turm Eins":
                        Console.WriteLine("Click 1");
                        said = 0;
                        break;
                    case "Turm Zwei":
                        Console.WriteLine("Click 2");
                        said = 1;
                        break;
                    case "Turm Drei":
                        Console.WriteLine("Click 3");
                        said = 2;
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
                    case "Bewege":
                        senderPole = null;
                        targetPole = null;
                        saidMove = true;
                        break;
                    case "Das":
                        if (this.one.IsMouseOver && saidMove == true) {
                            said_first = 0;
                            Console.WriteLine("Bla 1");
                            saidMove = false;
                            nextMove = true;
                        }
                        else if (this.two.IsMouseOver && saidMove == true) {
                            said_first = 1;
                            Console.WriteLine("Bla 2");
                            saidMove = false;
                            nextMove = true;
                        }
                        else if (this.three.IsMouseOver && saidMove == true) {
                            said_first = 2;
                            Console.WriteLine("Bla 3");
                            saidMove = false;
                            nextMove = true;
                        }
                        break;
                    case "Nach Da":
                        if (this.one.IsMouseOver) {
                            said_second = 0;
                            Console.WriteLine("Möp 1");
                        }
                        else if (this.two.IsMouseOver) {
                            said_second = 1;
                            Console.WriteLine("Möp 2");
                        }
                        else if (this.three.IsMouseOver) {
                            said_second = 2;
                            Console.WriteLine("Möp 3");
                        }
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                //if (said_first == null || said_second == null)
                //{
                    setPoles(said);
                    Console.WriteLine("oder hier?");
                    Console.WriteLine("boolaean: " + saidMove);
                    saidOrder();
                //}
                //else if (said_first.HasValue || said_second.HasValue)
                //{
                //    Console.WriteLine("Komme ich hie rein?");
                //    saidOrder();
                //}
            };
            if (!Templating)
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(MouseGesture));
                FileStream filestream_1 = new FileStream("templates/Template_1.xml", FileMode.Open);
                this.one_gesture = (MouseGesture)mySerializer.Deserialize(filestream_1);
                FileStream filestream_2 = new FileStream("templates/Template_2.xml", FileMode.Open);
                this.two_gesture = (MouseGesture)mySerializer.Deserialize(filestream_2);
                FileStream filestream_3 = new FileStream("templates/Template_3.xml", FileMode.Open);
                this.three_gesture = (MouseGesture)mySerializer.Deserialize(filestream_3);
            }

            putthatthere = new Frame("putthatthere");
            close = new Frame("close");
        }


            

        private void saidOrder()
        {
            if (!senderPole.HasValue)
            {
                setPoles(said_first);
                Console.WriteLine("setze first");
            }
            else if (senderPole.HasValue)
            {
                setPoles(said_second);
                Console.WriteLine("setze second");
            } 
            else if (senderPole == null && targetPole ==null)
            {
                Console.WriteLine("Do Nothing");
            }
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
            String modality = "mouse";
            int? clicked = null;
            // specify origin
            switch (((FrameworkElement)e.Source).Name)
            {
                case "one":
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


            putthatthere.fillSlot((int)clicked, modality);

            //setPoles(clicked);
        }

        //check if move is valid
        private void isValid()
        {
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
                //Console.WriteLine("canvasList" + canvasList);
            }
            // reset parameters
            senderPole = null;
            targetPole = null;

        }

        //set values of poles
        private void setPoles(int? pole)
        {
            if (!senderPole.HasValue)
            {
                senderPole = pole;
                switch (senderPole)
                {
                    case 0:
                        senderValue = 0;
                        break;
                    case 1:
                        senderValue = 1;
                        break;
                    case 2:
                        senderValue = 2;
                        break;
                    default:
                        break;
                }

                Console.WriteLine("canvasList: " + canvasList[senderValue].Count());
                //if (getLastElementWidth(senderPole.Value) < 0)
                if (canvasList[senderValue].Count() <= 0)
                {
                    message.Content = "Hier ist doch gar nichts!";
                    senderPole = null;
                    Console.WriteLine("Value: " + senderValue);
                    senderValue = 3;
                }
                Console.WriteLine("Neuer Wert: " + senderPole);
            }
            else
            {
                targetPole = pole;
            }
            if (senderPole.HasValue && targetPole.HasValue)
            {
                isValid();
            }
        }


        ////function to handle Input
        //private void handleInput(int? pole)
        //{
        //    // define sender/from
        //    if (!senderPole.HasValue)
        //    {
        //        senderPole = pole;
                
        //    }
        //    // define target/to
        //    else
        //    {
        //        targetPole = pole;
        //        // check if turn is allowed (sender.width < target.width)!
        //        if ((getLastElementWidth(senderPole.Value) < getLastElementWidth(targetPole.Value) || getLastElementWidth(targetPole.Value) == 0) && getLastElementWidth(senderPole.Value) != 0)
        //        {
        //            // start animation
        //            moveTo(senderPole.Value, targetPole.Value);
        //            System.Media.SystemSounds.Hand.Play();
        //            message.Content = "";
        //        }
        //        else
        //        {
        //            // wrong turn
        //            System.Media.SystemSounds.Beep.Play();
        //            message.Content = "Wrong turn.";
        //        }
        //        // reset parameters
        //        senderPole = null;
        //        targetPole = null;
        //    }


        //}

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


        // GESTURE DETECTION
        private void Start_Gesture(object sender, MouseEventArgs e)
        {
            this.button_pressed = true;
            double X = e.GetPosition(this).X;
            double Y = e.GetPosition(this).Y;
            this.current_gesture = new MouseGesture();
            this.current_gesture.init(X, Y);
        }

        private void Track_Position(object sender, MouseEventArgs e)
        {
            if (button_pressed)
            {
                double X = e.GetPosition(this).X;
                double Y = e.GetPosition(this).Y;
                this.current_gesture.AddValue(X, Y);
            }
        }

        private void Finish_Gesture(object sender, MouseEventArgs e)
        {
            int? drawed = null;
            button_pressed = false;
            if (Templating)
            {
                XmlSerializer ser = new XmlSerializer(typeof(MouseGesture));
                TextWriter writer = new StreamWriter("Template.xml");
                ser.Serialize(writer, this.current_gesture);
                writer.Close();


            }
            else
            {
                /*
                 * Record Gesture 
                XmlSerializer serializer = new XmlSerializer(typeof(MouseGesture));
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter writerXML = XmlWriter.Create("templates/example.xml", settings);
                serializer.Serialize(writerXML, this.current_gesture, null);

                writerXML.Close();
                */
                
                double one = this.dynamicTimeWarping(this.current_gesture, this.one_gesture);
                double two = this.dynamicTimeWarping(this.current_gesture, this.two_gesture);
                double three = this.dynamicTimeWarping(this.current_gesture, this.three_gesture);
                Debug.WriteLine("One: {0}", one);
                Debug.WriteLine("Two: {0}", two);
                Debug.WriteLine("Three: {0}", three);
                if (one < Math.Min(two, three))
                {
                    drawed = 0;
                    Debug.WriteLine("One: {0}", one);
                }
                if (two < Math.Min(one, three))
                {
                    drawed = 1;
                    Debug.WriteLine("Two: {0}", two);
                }
                if (three < Math.Min(two, one))
                {
                    drawed = 2;
                    Debug.WriteLine("Three: {0}", three);
                }

                setPoles(drawed);

            }
        }

        private double dynamicTimeWarping(MouseGesture gesture, MouseGesture template)
        {
            double[,] dwt_matrix = new double[gesture.movement.Count, template.movement.Count];

            for (int i = 1; i < gesture.movement.Count; i++)
            {
                dwt_matrix[i, 0] = Double.MaxValue;
            }
            for (int j = 1; j < template.movement.Count; j++)
            {
                dwt_matrix[0, j] = Double.MaxValue;
            }
            dwt_matrix[0, 0] = 0;

            for (int i = 1; i < gesture.movement.Count; i++)
            {
                for (int j = 1; j < template.movement.Count; j++)
                {
                    double cost = Math.Abs(gesture.movement.ElementAt(i) - template.movement.ElementAt(j));
                    dwt_matrix[i, j] = cost + Math.Min(Math.Min(dwt_matrix[i - 1, j], dwt_matrix[i, j - 1]), dwt_matrix[i - 1, j - 1]);
                }
            }
            return dwt_matrix[gesture.movement.Count - 1, template.movement.Count - 1];
        }
    }


    public partial class MouseGesture
    {
        Point start_point;
        Point last_point;
        public List<Double> movement = new List<Double>();

        public MouseGesture()
        {
        }

        public void init(double x, double y)
        {
            start_point.X = x;
            start_point.Y = y;
            last_point.X = x;
            last_point.Y = y;
            movement = new List<Double>();
        }

        public void AddValue(double x, double y)
        {
            Vector vector_start = new Vector(Math.Abs(start_point.X - x), Math.Abs(start_point.Y - y));
            Vector vector_last = new Vector(Math.Abs(last_point.X - x), Math.Abs(last_point.Y - y));

            Double angle_between = Vector.AngleBetween(vector_start, vector_last);

            movement.Add(angle_between * Math.PI / 180);

            last_point.X = x;
            last_point.Y = y;
        }
    }

    public partial class Frame {

        String command;
        int[,] slots;
        private Object thisLock = new Object();
        private System.Timers.Timer timer;
        int? sender;
        int? target;


        public void initTimer() {
            timer = new System.Timers.Timer(5000);
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public Frame(String command) {
            this.command = command;
            this.slots = initSlots(command);
            this.sender = null;
            this.target = null;
            initTimer();
        }

        public int[,] initSlots(String command){
            int[,] _slots;
            switch (command) {
                case "putthatthere":
                    _slots = new int[4,3];
                    break;
                case "close":
                    _slots = new int[2,2];
                    break;
                default:
                    _slots = new int[0,0];
                    break;
            }
            return _slots;
        }

        //TODO LOGIK ÜBERPRÜFEN
        public void fillSlot(int value, String modality) { 
            lock (thisLock) {
                switch (modality) { 
                    case "mouse":
                        if (!this.sender.HasValue)
                        {

                        }
                        if (this.slots[3, 1] != 0)
                        {
                            this.slots[3, 2] = value;
                        }
                        else {
                            this.slots[3, 1] = value;
                        }
                        break;
                    case "speech":
                        if (this.slots[0, 1] != 0) {
                            this.slots[0, 2] = value;
                        }
                        else
                        {
                            this.slots[0, 1] = value;
                        }
                        break;
                    case "gesture":
                        if (this.slots[1, 1] != 0)
                        {
                            this.slots[1, 2] = value;
                        }
                        else
                        {
                            this.slots[1, 1] = value;
                        }
                        break;
                    case "smouse":
                        if (this.slots[2, 0] != 0)
                        {

                            this.slots[2, 2] = value;
                        }
                        
                        else
                        {
                            this.slots[2, 1] = value;
                        }
                        break;
                    default:
                        break;
                }
                //check which slot gets filled
                //this.slots[slotX, slotY] = value;
            }     
        }

        public void clearSlots() {
            Array.Clear(this.slots, 0, this.slots.Length);
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            clearSlots();
        }
    }


}

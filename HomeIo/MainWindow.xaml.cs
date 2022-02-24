using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EngineIO;

namespace HomeIo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ft1 = false;
        private bool ft2 = false;
        private bool X1 = true;
        private bool X2 = false;
        private bool front = false;
        private bool bpPrec = false;
        private bool bp;

        private MemoryBit lampe;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
            dispatcherTimer.Start();

            //conditions initiales
            this.X1 = true;
            this.X2 = false;
            this.bpPrec = false;

            x0v = true;
            x1v = false;
            x2v = false;

            this.lampe = MemoryMap.Instance.GetBit(0, MemoryType.Output);

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tick");
            MemoryMap.Instance.Update();
            this.runCycleApi();
            this.runCycleVolet();
            MemoryMap.Instance.Update();
        }


        private void runCycleApi()
        {
            //mise à jour HomeIO
           

            bpPrec = this.bp;
            //lecture des entrées
            this.bp = MemoryMap.Instance.GetBit(2, MemoryType.Input).Value;

            //calculs des FTs
            this.front = !this.bpPrec && this.bp;

            this.ft1 = this.X1 && this.front;
            this.ft2 = this.X2 && this.front;
            //calculs des étapes
            this.X1 = this.ft2 || this.X1 && !this.ft1;
            this.X2 = this.ft1 || this.X2 && !this.ft2;

            //écriture des sorties
            lampe.Value = X2;

            //mise à jour HomeIO
            
        }

        bool ft1v;
        bool ft2v;
        bool ft3v;
        bool ft4v;
        bool ft5v;
        bool ft6v;

        bool x0v = true;
        bool x1v = false;
        bool x2v = false;

        bool btnMonterPrec;
        bool btnMonter;
        bool btnDescendrePrec;
        bool btnDescendre;
        bool frontMonter;
        bool frontDescendre;
        bool voletHaut;
        bool voletBas;
        MemoryBit monter;
        MemoryBit descendre;
        float positionVolet;

        private void runCycleVolet()
        {
            btnMonterPrec = btnMonter;
            btnDescendrePrec = btnDescendre;

            btnMonter = MemoryMap.Instance.GetBit(3, MemoryType.Input).Value;
            btnDescendre = MemoryMap.Instance.GetBit(4, MemoryType.Input).Value;

            positionVolet = MemoryMap.Instance.GetFloat(3, MemoryType.Input).Value;

            voletHaut = positionVolet == 10 ? true : false;
            voletBas = positionVolet == 0 ? true : false;
            
            frontMonter = !btnMonterPrec && btnMonter;
            frontDescendre = !btnDescendrePrec && btnDescendre;

            ft1v = x0v && (!voletBas && frontDescendre);
            ft2v = x1v && voletBas;
            ft3v = x1v && frontMonter;
            ft4v = x0v && (!voletHaut && frontMonter);
            ft5v = x2v && voletHaut;
            ft6v = x2v && frontDescendre;

            x0v = ft2v || ft5v || x0v && !(ft1v || ft4v);
            x1v = ft6v || ft1v || x1v && !(ft2v || ft3v);
            x2v = ft3v || ft4v || x2v && !(ft5v || ft6v);

            monter = MemoryMap.Instance.GetBit(1, MemoryType.Output);
            descendre = MemoryMap.Instance.GetBit(2, MemoryType.Output);


            monter.Value = x2v;
            descendre.Value = x1v;

        }
    }
}

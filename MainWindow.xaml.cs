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
        System.Windows.Threading.DispatcherTimer dispatcherTimer5s;

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

            dispatcherTimer5s = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer5s.Tick += new EventHandler(dispatcherTimer5s_Tick);
            dispatcherTimer5s.Interval = TimeSpan.FromSeconds(5);

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
           // System.Diagnostics.Debug.WriteLine("tick");
            MemoryMap.Instance.Update();
            this.runCycleApi();
            this.runCycleVolet();
            this.runCycleGarage();
            MemoryMap.Instance.Update();
        }

        private void dispatcherTimer5s_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tick 5 secondes");
            timerTermine = true;
            this.dispatcherTimer5s.Stop();
            timerRun = false;
        }


        bool ft1g;
        bool ft2g;
        bool ft3g;
        bool ft4g;
        bool ft5g;
        bool ft6g;

        bool x1g = true;
        bool x2g = false;
        bool x3g = false;
        bool x4g = false;
        bool x5g = false;

        bool bp1;
        bool bp1Prec;
        bool frontBp1;
        bool porteOuverte;
        bool timerTermine = false;
        bool timerRun = false;
        bool porteFermee;
        bool infraRouge;

        MemoryBit fermerPorte;
        MemoryBit ouvrirPorte;
        private void runCycleGarage()
        {

            bp1Prec = bp1;
            
            bp1 = MemoryMap.Instance.GetBit(274, MemoryType.Input).Value;
            porteOuverte = MemoryMap.Instance.GetBit(100, MemoryType.Input).Value;
            porteFermee = MemoryMap.Instance.GetBit(101, MemoryType.Input).Value;
            infraRouge = MemoryMap.Instance.GetBit(102, MemoryType.Input).Value;

            frontBp1 = !bp1Prec && bp1;

            ft1g = x1g && frontBp1;
            ft2g = x2g && porteOuverte;
            ft3g = x3g && timerTermine;
            ft4g = x4g && !porteOuverte;
            ft5g = x5g && porteFermee;
            ft6g = x5g && (frontBp1 || infraRouge) && !porteFermee;

            x1g = ft5g || x1g && !ft1g;
            x2g = (ft1g || ft6g) || x2g && !ft2g;
            x3g = ft2g || x3g && !ft3g;
            x4g = ft3g || x4g && !ft4g;
            x5g = ft4g || x5g && !(ft5g || ft6g);

            System.Diagnostics.Debug.WriteLine("****");
            System.Diagnostics.Debug.WriteLine("ft5g " + ft5g);
            System.Diagnostics.Debug.WriteLine("ft6g " + ft6g);
            System.Diagnostics.Debug.WriteLine(x1g);
            System.Diagnostics.Debug.WriteLine(x2g);
            System.Diagnostics.Debug.WriteLine(x3g);
            System.Diagnostics.Debug.WriteLine(x4g);
            System.Diagnostics.Debug.WriteLine("x5g " + x5g);

            ouvrirPorte = MemoryMap.Instance.GetBit(72, MemoryType.Output);
            fermerPorte = MemoryMap.Instance.GetBit(73, MemoryType.Output);

            ouvrirPorte.Value = x2g;
            fermerPorte.Value = x4g || x5g;

            if (x3g == true && timerRun == false)
            {
                timerRun = true;
                this.dispatcherTimer5s.Start();
            }

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

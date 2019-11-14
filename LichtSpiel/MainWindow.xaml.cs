using System;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LichtSpiel
{
    public partial class MainWindow : Window
    {
        private Liste<Farbe> computerliste;
        private Liste<Farbe> _benutzerListe;

        public MainWindow()
        {
            InitializeComponent();

            LichtSpielAufstarten();
        }

        private void LichtSpielAufstarten()
        {
            //Knöpfe apschaltten
            BenutzerKnopfRot.IsEnabled = false;
            BenutzerKnopfBlau.IsEnabled = false;
            BenutzerKnopfGruen.IsEnabled = false;
            BenutzerKnopfGold.IsEnabled = false;

            KnopfRot.Background = Brushes.DarkRed;
            BenutzerKnopfRot.Background = Brushes.DarkRed;

            KnopfBlau.Background = Brushes.SteelBlue;
            BenutzerKnopfBlau.Background = Brushes.SteelBlue;

            KnopfGruen.Background = Brushes.Green;
            BenutzerKnopfGruen.Background = Brushes.Green;
        }

        private async void Start_Knopf_Geklickt(object sender, RoutedEventArgs e)
        {
            Timer.Text = "00:00:00";
            var watch = new Stopwatch();
            watch.Start();

            KnoepfeAbschalten();
            await Task.Delay(400);

            computerliste = ZufaelligeFarbliste();
            _benutzerListe = new Liste<Farbe>();

            await FarbenAbspielen(computerliste);
            KnoepfeEinschalten();
            Timer.Text = watch.Elapsed.ToString();
        }

        private void KnoepfeEinschalten()
        {
            KnopfStart.IsEnabled = true;
            BenutzerKnopfRot.IsEnabled = true;
            BenutzerKnopfBlau.IsEnabled = true;
            BenutzerKnopfGruen.IsEnabled = true;
            BenutzerKnopfGold.IsEnabled = true;
        }

        private void KnoepfeAbschalten()
        {
            KnopfStart.IsEnabled = false;
            BenutzerKnoepfeAbschalten();
        }

        private void BenutzerKnoepfeAbschalten()
        {
            BenutzerKnopfRot.IsEnabled = false;
            BenutzerKnopfBlau.IsEnabled = false;
            BenutzerKnopfGruen.IsEnabled = false;
            BenutzerKnopfGold.IsEnabled = false;
        }

        private async Task FarbenAbspielen(Liste<Farbe> farbliste)
        {
            foreach(Farbe farbe in farbliste)
            {
                await EineFarbeAbspielen(farbe);
            }
        }

        private async Task EineFarbeAbspielen(Farbe farbe)
        {
            Storyboard animation = FindResource("KnopfAnimation") as Storyboard;
            Button aktuellerKnopf = null;

            switch (farbe)
            {
                case Farbe.Rot:
                    {
                        aktuellerKnopf = KnopfRot;
                        break;
                    }
                case Farbe.Blau:
                    {
                        aktuellerKnopf = KnopfBlau;
                        break;
                    }
                case Farbe.Gruen:
                    {
                        aktuellerKnopf = KnopfGruen;
                        break;
                    }
                        case Farbe.Gold:
                    {
                        aktuellerKnopf = KnopfGold;
                        break;
                    }
            }

            Storyboard.SetTarget(animation, aktuellerKnopf);
            await AnimationAbspielen(animation);
        }

        private async Task AnimationAbspielen(Storyboard animation)
        {
            var animationKomplett = new TaskCompletionSource<bool>();
            EventHandler kompletierer = (s, e) => animationKomplett.SetResult(true);

            animation.Completed += kompletierer;

            animation.Begin();
            await animationKomplett.Task;

            animation.Completed -= kompletierer;
        }

        private Liste<Farbe> ZufaelligeFarbliste()
        {
            var neueFarbenListe = new Liste<Farbe>();
            var zufallsGenerator = new FarbenZufallsGenerator();

            for(int i=0; i < 2; i++)
            {
                Farbe farbe = zufallsGenerator.GibFarbe();

                neueFarbenListe.Hinzufuegen(farbe);
                neueFarbenListe.Hinzufuegen(Farbe.Rot);
                neueFarbenListe.Hinzufuegen(Farbe.Gold);
                neueFarbenListe.Hinzufuegen(Farbe.Blau);
                neueFarbenListe.Hinzufuegen(Farbe.Gruen);
            }

            return neueFarbenListe;
        }

        private void BenutzerKnopfRot_Geklickt(object sender, RoutedEventArgs e)
        {
            _benutzerListe.Hinzufuegen(Farbe.Rot);
            BenutzerEingabeUeberpruefen();
        }

        private void BenutzerKnopfBlau_Geklickt(object sender, RoutedEventArgs e)
        {
            _benutzerListe.Hinzufuegen(Farbe.Blau);
            BenutzerEingabeUeberpruefen();
        }

        private void BenutzerKnopfGruen_Geklickt(object sender, RoutedEventArgs e)
        {
            _benutzerListe.Hinzufuegen(Farbe.Gruen);
            BenutzerEingabeUeberpruefen();
        }
        private void BenutzerKnopfGold_Geklickt(object sender, RoutedEventArgs e)
        {
            _benutzerListe.Hinzufuegen(Farbe.Gold);
            BenutzerEingabeUeberpruefen();
        }

        private void BenutzerEingabeUeberpruefen()
        {
            if(_benutzerListe.Laenge == computerliste.Laenge)
            {
                for(int i=0; i < _benutzerListe.Laenge; i++)
                {
                    if(_benutzerListe[i] != computerliste[i])
                    {
                        UngueltigeEingabe();
                        BenutzerKnoepfeAbschalten();
                        return;
                    }
                }

                RichtigeEingabe();
                BenutzerKnoepfeAbschalten();
                return;
            }
        }

        private void UngueltigeEingabe()
        {
            TonAbspielen("LichtSpiel.sounds.boo.wav");
        }

        private void RichtigeEingabe()
        {
            TonAbspielen("LichtSpiel.sounds.applause.wav");
        }

        private void TonAbspielen(string tonName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(tonName);

            SoundPlayer player = new SoundPlayer(stream);
            player.Load();
            player.Play();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

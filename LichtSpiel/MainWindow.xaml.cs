using System;
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
        private Liste<Farbe> _generierteListe;
        private Liste<Farbe> _benutzerListe;

        public MainWindow()
        {
            InitializeComponent();

            LichtSpielAufstarten();
        }

        private void LichtSpielAufstarten()
        {
            BenutzerKnopfRot.IsEnabled = false;
            BenutzerKnopfBlau.IsEnabled = false;
            BenutzerKnopfGruen.IsEnabled = false;
            BenutzerKnopfOrange.IsEnabled = false;

            KnopfRot.Background = Brushes.DarkRed;
            BenutzerKnopfRot.Background = Brushes.DarkRed;

            KnopfBlau.Background = Brushes.SteelBlue;
            BenutzerKnopfBlau.Background = Brushes.SteelBlue;

            KnopfGruen.Background = Brushes.Green;
            BenutzerKnopfGruen.Background = Brushes.Green;
        }

        private async void Start_Knopf_Geklickt(object sender, RoutedEventArgs e)
        {
            KnoepfeAbschalten();
            await Task.Delay(400);

            _generierteListe = ZufaelligeFarbliste();
            _benutzerListe = new Liste<Farbe>();

            await FarbenAbspielen(_generierteListe);
            KnoepfeEinschalten();
        }

        private void KnoepfeEinschalten()
        {
            KnopfStart.IsEnabled = true;
            BenutzerKnopfRot.IsEnabled = true;
            BenutzerKnopfBlau.IsEnabled = true;
            BenutzerKnopfGruen.IsEnabled = true;

            BenutzerKnopfOrange.IsEnabled = true;
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
            BenutzerKnopfOrange.IsEnabled = false;
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
                case Farbe.Orange:
                    {
                        aktuellerKnopf = KnopfOrange;
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

            for(int i=0; i < 5; i++)
            {
                Farbe farbe = zufallsGenerator.GibFarbe();

                neueFarbenListe.Hinzufuegen(farbe);
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
        private void BenutzerKnopfOrange_Geklickt(object sender, RoutedEventArgs e)
        {
            _benutzerListe.Hinzufuegen(Farbe.Orange);
            BenutzerEingabeUeberpruefen();
        }

        private void BenutzerEingabeUeberpruefen()
        {
            if(_benutzerListe.Laenge == _generierteListe.Laenge)
            {
                for(int i=0; i < _benutzerListe.Laenge; i++)
                {
                    if(_benutzerListe[i] != _generierteListe[i])
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
    }
}

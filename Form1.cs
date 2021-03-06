using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis ;

namespace REcoSample
{
    public partial class Form1 : Form
    {
     private System.Speech.Recognition.SpeechRecognitionEngine _recognizer = 
        new SpeechRecognitionEngine();
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        private Boolean victory = false;

        private String color="";
        private int contador=0;
        private String[] tablero = new String[9];
        private int jugador1 = 0, jugador2 = 0;
        private Boolean partidaTerminada = false;
        private Boolean colores = false;

        private String colorSel1 = "";
        private String colorSel2 = "";

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Bienvenidos, Inicializando el juego");

           Grammar grammar= CreateGrammarJugadasBN();
            Grammar grammar2 = CreateGrammarJugadasAR();
            Grammar grammar3 = CreateGrammarJugadasVA();
            Grammar grammar4 = CreateGrammarJugadasNA();
            Grammar grammarAction = CreateGrammarActions();
            Grammar grammarReiniciarAction = CreateGrammarReiniciarActions();
            Grammar eleccionColor = CreateGrammarEleccionColor();
            Grammar cambiar = CreateGrammarCambiar();
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.UnloadAllGrammars();
            // Nivel de confianza del reconocimiento 70%
            _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);
            grammar.Enabled = true;
            grammar2.Enabled = true;
            grammar3.Enabled = true;
            grammar4.Enabled = true;
            cambiar.Enabled = true;
            eleccionColor.Enabled = true;
            grammarAction.Enabled = true;
            _recognizer.LoadGrammar(grammar);
            _recognizer.LoadGrammar(grammar2);
            _recognizer.LoadGrammar(grammar3);
            _recognizer.LoadGrammar(grammar4);
            _recognizer.LoadGrammar(grammarAction);
            _recognizer.LoadGrammar(grammarReiniciarAction);
            _recognizer.LoadGrammar(eleccionColor);
            _recognizer.LoadGrammar(cambiar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            //reconocimiento as�ncrono y m�ltiples veces
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            
            synth.Speak("Para iniciar la partida, indique qu� colores quiere utilizar para jugar");
        }

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //obtenemos un diccionario con los elementos sem�nticos
            SemanticValue semantics = e.Result.Semantics;
          
            string rawText = e.Result.Text;

            if (!colores)
            {
                if (rawText.Contains("Azul") || rawText.Contains("Rojo"))
                {
                    this.labelJug1.Text = "Azul:";
                    this.labelJug2.Text = "Rojo:";
                    this.labelJug1.ForeColor = SeleccionarColor("Az");
                    this.labelJug2.ForeColor = SeleccionarColor("Ro");
                    colorSel1 = "Az";
                    colorSel2 = "Ro";
                }
                else if (rawText.Contains("Verde") || rawText.Contains("Amarillo"))
                {
                    this.labelJug1.Text = "Verde:";
                    this.labelJug2.Text = "Amarillo:";
                    this.labelJug1.ForeColor = SeleccionarColor("Ve");
                    this.labelJug2.ForeColor = SeleccionarColor("Am");
                    colorSel1 = "Ve";
                    colorSel2 = "Am";
                }
                else if (rawText.Contains("Naranja") || rawText.Contains("Morado"))
                {
                    this.labelJug1.Text = "Naranja:";
                    this.labelJug2.Text = "Morado:";
                    this.labelJug1.ForeColor = SeleccionarColor("Na");
                    this.labelJug2.ForeColor = SeleccionarColor("Mo");
                    colorSel1 = "Na";
                    colorSel2 = "Mo";
                }
                else if (rawText.Contains("Blanco") || rawText.Contains("Negro"))
                {
                    this.labelJug1.Text = "Blanco:";
                    this.labelJug2.Text = "Negro:";
                    this.labelJug1.ForeColor = SeleccionarColor("Bl");
                    this.labelJug2.ForeColor = SeleccionarColor("Ne");
                    colorSel1 = "Bl";
                    colorSel2 = "Ne";
                }
                colores = true;
                this.panel1.Visible = false;
                this.label2.Visible = true;
                this.label3.Visible = true;
                Update();
                synth.Speak("Ya estamos listos para jugar");
                return;
            }
            else 
            {
                if (partidaTerminada && rawText.Contains("Cambiar"))
                {
                    this.panel1.Visible = true;
                    this.label2.Visible = false;
                    this.label3.Visible = false;
                    colores = false;
                    Reiniciar();
                    Update();
                    synth.Speak("Indique qu� colores quiere utilizar para jugar");
                    return;
                }
                else if (!partidaTerminada && color.Equals(rawText.Substring(0, 2)))
                {
                    synth.Speak("Ya has jugado, le toca al otro jugador");
                }
                else if (rawText.Contains("Salir"))
                {
                    synth.Speak("Saliendo del juego.");
                    Application.Exit();
                }
                else if (rawText.Contains("Reiniciar"))
                {
                    Reiniciar();
                }
                else if(colorSel1 == rawText.Substring(0, 2) ||  colorSel2 == rawText.Substring(0, 2))
                {
                    if (!partidaTerminada)
                    {
                        int jugada = (int)semantics["posicion"].Value;

                        if (tablero[jugada] == null)
                        {
                            RealizarJugada(jugada, rawText.Substring(0, 2));

                            if (contador == 9)
                            {
                                partidaTerminada = true;
                                synth.Speak("Fin de la partida, no ha habido ganador");
                                synth.Speak("Si deseas salir di salir, si deseas jugar otra partida di reiniciar");
                            }
                            else
                            {
                                color = rawText.Substring(0, 2);
                            }
                        }
                        else
                        {
                            synth.Speak("La jugada que has indicado no es v�lida");
                        }
                    }
                }

            }

        }
        
        private Color SeleccionarColor(String jugador)
        {
            Color color = Color.Transparent;

            switch (jugador)
            {
                case "Az":
                    color = botonAzul.BackColor;
                    break;
                case "Ro":
                    color = botonRojo.BackColor;
                    break;
                case "Ve":
                    color = botonVerde.BackColor;
                    break;
                case "Am":
                    color = botonAmarillo.BackColor;
                    break;
                case "Na":
                    color = botonNaranja.BackColor;
                    break;
                case "Bl":
                    color = botonBlanco.BackColor;
                    break;
                case "Ne":
                    color = botonNegro.BackColor;
                    break;
                case "Mo":
                    color = botonMorado.BackColor;
                    break;
            }
            return color;
        }
        private void RealizarJugada(int jugada, string jugador)
        {

            tablero[jugada] = jugador;
            contador++;
            Color color = SeleccionarColor(jugador);
            switch (jugada)
            {
                case 0:
                    this.button0.BackColor = color;
                    this.button0.Text = "";
                    victory = (this.button4.BackColor == color && this.button8.BackColor == color) || (this.button2.BackColor == color && this.button1.BackColor == color);
                    break;
                case 1:
                    this.button1.BackColor = color;
                    this.button1.Text = "";
                    victory = (this.button4.BackColor == color && this.button7.BackColor == color) || (this.button2.BackColor == color && this.button0.BackColor == color);
                    break;
                case 2:
                    this.button2.BackColor = color;
                    this.button2.Text = "";
                    victory = (this.button4.BackColor == color && this.button6.BackColor == color) || (this.button1.BackColor == color && this.button0.BackColor == color) || (this.button4.BackColor == color && this.button6.BackColor == color);
                    break;
                case 3:
                    this.button3.BackColor = color;
                    this.button3.Text = "";
                    victory = (this.button0.BackColor == color && this.button6.BackColor == color) || (this.button4.BackColor == color && this.button5.BackColor == color);
                    break;
                case 4:
                    this.button4.BackColor = color;
                    this.button4.Text = "";
                    victory = (this.button1.BackColor == color && this.button7.BackColor == color) || (this.button3.BackColor == color && this.button5.BackColor == color) || (this.button6.BackColor == color && this.button2.BackColor == color) || (this.button0.BackColor == color && this.button8.BackColor == color);
                    break;
                case 5:
                    this.button5.BackColor = color;
                    this.button5.Text = "";
                    victory = (this.button3.BackColor == color && this.button4.BackColor == color) || (this.button2.BackColor == color && this.button8.BackColor == color);
                    break;
                case 6:
                    this.button6.BackColor = color;
                    this.button6.Text = "";
                    victory = (this.button4.BackColor == color && this.button2.BackColor == color) || (this.button0.BackColor == color && this.button3.BackColor == color) || (this.button7.BackColor == color && this.button8.BackColor == color);
                    break;
                case 7:
                    this.button7.BackColor = color;
                    this.button7.Text = "";
                    victory = (this.button4.BackColor == color && this.button1.BackColor == color) || (this.button6.BackColor == color && this.button8.BackColor == color);
                    break;
                case 8:
                    this.button8.BackColor = color;
                    this.button8.Text = "";
                    victory = (this.button6.BackColor == color && this.button7.BackColor == color) || (this.button0.BackColor == color && this.button4.BackColor == color) || (this.button2.BackColor == color && this.button5.BackColor == color);
                    break;
            }
            Update();

            if (victory) {
                if(jugador == "Az" || jugador == "Ve" || jugador == "Na" || jugador == "Bl") { 
                    synth.Speak("El jugador uno ha ganado");
                    jugador1++;
                    this.labelBlanco.Text = jugador1.ToString();
                }
                else
                {
                    synth.Speak("El jugador dos ha ganado");
                    jugador2++;
                    this.labelNegro.Text = jugador2.ToString();
                }
                Update();
                synth.Speak("Si deseas salir di salir, si deseas jugar otra partida di reiniciar");
                partidaTerminada = true;
            }
        }
        
        private void Reiniciar()
        {
            synth.Speak("Reiniciando la partida");
            partidaTerminada = false;
            tablero = new String[9];
            contador = 0;
            color = "";
            victory = false;
            this.button0.BackColor = Color.Empty;
            this.button0.Text = "Uno";
            this.button1.BackColor = Color.Empty;
            this.button1.Text = "Dos";
            this.button2.BackColor = Color.Empty;
            this.button2.Text = "Tres";
            this.button3.BackColor = Color.Empty;
            this.button3.Text = "Cuatro";
            this.button4.BackColor = Color.Empty;
            this.button4.Text = "Cinco";
            this.button5.BackColor = Color.Empty;
            this.button5.Text = "Seis";
            this.button6.BackColor = Color.Empty;
            this.button6.Text = "Siete";
            this.button7.BackColor = Color.Empty;
            this.button7.Text = "Ocho";
            this.button8.BackColor = Color.Empty;
            this.button8.Text = "Nueve";
        }
      
        private GrammarBuilder CreatePositions()
        {
            Choices choice = new Choices();

            SemanticResultValue choiceResultValue =
                    new SemanticResultValue("uno", 0);
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("dos", 1);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("tres", 2);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("cuatro", 3);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("cinco", 4);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("seis", 5);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("siete", 6);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("ocho", 7);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("nueve", 8);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            choice.Add(resultValueBuilder);

            SemanticResultKey choiceResultKey = new SemanticResultKey("posicion", choice);
            GrammarBuilder posicion = new GrammarBuilder(choiceResultKey);

            return posicion;
        }
        private Grammar CreateGrammarEleccionColor()
        {
            GrammarBuilder azul = "Azul";
            GrammarBuilder rojo = "Rojo";
            GrammarBuilder verde = "Verde";
            GrammarBuilder amarillo = "Amarillo";
            GrammarBuilder naranja = "Naranja";
            GrammarBuilder morado = "Morado";
            GrammarBuilder negro = "Negro";
            GrammarBuilder blanco = "Blanco";

            Choices eleccion = new Choices(azul, rojo, verde, amarillo, naranja, morado, negro, blanco);
            return new Grammar(eleccion);
        }

        private Grammar CreateGrammarJugadasBN()
        {
            GrammarBuilder blanco = "Blanco";
            GrammarBuilder negro = "Negro";
            Choices dos_inicios = new Choices(blanco, negro);
            GrammarBuilder frase = new GrammarBuilder(dos_inicios);
            
            frase.Append(CreatePositions());

            Grammar grammar = new Grammar(frase);            

            return grammar;
        }

        private Grammar CreateGrammarJugadasAR()
        {
            GrammarBuilder azul = "Azul";
            GrammarBuilder rojo = "Rojo";
            Choices dos_inicios = new Choices(azul, rojo);
            GrammarBuilder frase = new GrammarBuilder(dos_inicios);

            frase.Append(CreatePositions());

            Grammar grammar = new Grammar(frase);

            return grammar;
        }

        private Grammar CreateGrammarJugadasVA()
        {
            GrammarBuilder verde = "Verde";
            GrammarBuilder amarillo = "Amarillo";
            Choices dos_inicios = new Choices(verde, amarillo);
            GrammarBuilder frase = new GrammarBuilder(dos_inicios);

            frase.Append(CreatePositions());

            Grammar grammar = new Grammar(frase);

            return grammar;
        }

        private Grammar CreateGrammarJugadasNA()
        {
            GrammarBuilder naranja = "Naranja";
            GrammarBuilder morado = "Morado";
            Choices dos_inicios = new Choices(naranja, morado);
            GrammarBuilder frase = new GrammarBuilder(dos_inicios);

            frase.Append(CreatePositions());

            Grammar grammar = new Grammar(frase);

            return grammar;
        }

        private Grammar CreateGrammarActions() {
            GrammarBuilder salir = new GrammarBuilder("Salir");
            Grammar grammar = new Grammar(salir); 

            return grammar;
        }

        private Grammar CreateGrammarCambiar()
        {
            GrammarBuilder cambiar = new GrammarBuilder("Cambiar");
            Grammar grammar = new Grammar(cambiar);

            return grammar;
        }
        private Grammar CreateGrammarReiniciarActions() {
            GrammarBuilder reiniciar = new GrammarBuilder("Reiniciar");
            Grammar grammar = new Grammar(reiniciar); 

            return grammar;
        }
    }
}
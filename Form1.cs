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
        private int blanco = 0, negro = 0;
        private Boolean partidaTerminada = false;
        
          
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Bienvenidos, Inicializando el juego");

           Grammar grammar= CreateGrammarJugadas();
            Grammar grammarAction = CreateGrammarActions();
            Grammar grammarReiniciarAction = CreateGrammarReiniciarActions();
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.UnloadAllGrammars();
            // Nivel de confianza del reconocimiento 70%
            _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);
            grammar.Enabled = true;
            grammarAction.Enabled = true;
            _recognizer.LoadGrammar(grammar);
            _recognizer.LoadGrammar(grammarAction);
            _recognizer.LoadGrammar(grammarReiniciarAction);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            //reconocimiento asíncrono y múltiples veces
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            synth.Speak("Ya estamos listos para jugar");
         }

     

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //obtenemos un diccionario con los elementos semánticos
            SemanticValue semantics = e.Result.Semantics;
          
            string rawText = e.Result.Text;
                      
            if(!partidaTerminada && color.Equals(rawText.Substring(0,1))){
                synth.Speak("Ya has jugado, le toca al otro jugador");
            }
            else {
                if (rawText.Contains("Salir")) {
                    synth.Speak("Saliendo del juego.");
                    Application.Exit();
                } else if (rawText.Contains("Reiniciar")) {
                    Reiniciar();
                } 
                else if(!partidaTerminada){
                    int jugada = (int)semantics["posicion"].Value;

                    if (tablero[jugada] == null)
                    {
                        RealizarJugada(jugada, rawText.Substring(0, 1));

                        if (contador == 9)
                        {
                            synth.Speak("Fin de la partida, no ha habido ganador");
                            synth.Speak("Si deseas salir di salir, si deseas jugar otra partida di reiniciar");
                        }

                        color = rawText.Substring(0, 1);
                    }
                    else
                    {
                        synth.Speak("La jugada que has indicado no es válida");
                    }
                }
            }

        }
        
        private void RealizarJugada(int jugada, string jugador)
        {

            tablero[jugada] = jugador;
            contador++;
            Color color = jugador == "B" ? Color.White : Color.Black;
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
                if(jugador == "B") { 
                    synth.Speak("El jugador blanco ha ganado");
                    blanco++;
                    this.labelBlanco.Text = blanco.ToString();
                }
                else
                {
                    synth.Speak("El jugador negro ha ganado");
                    negro++;
                    this.labelNegro.Text = negro.ToString();
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
      
        private Grammar CreateGrammarJugadas()
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

            
            GrammarBuilder blanco = "Blanco";
            GrammarBuilder negro = "Negro";
            
            
            Choices dos_inicios = new Choices(blanco, negro);
            GrammarBuilder frase = new GrammarBuilder(dos_inicios);
            
            frase.Append(posicion);

            Grammar grammar = new Grammar(frase);            

            return grammar;
        }

        private Grammar CreateGrammarActions() {
            GrammarBuilder salir = new GrammarBuilder("Salir");
            Grammar grammar = new Grammar(salir); 

            return grammar;
        }

        private Grammar CreateGrammarReiniciarActions() {
            GrammarBuilder reiniciar = new GrammarBuilder("Reiniciar");
            Grammar grammar = new Grammar(reiniciar); 

            return grammar;
        }
    }
}
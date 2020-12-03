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
        Boolean victory = false;

        private String color="";
        private int contador=0;
        private String[] tablero = new String[9];

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Bienvenido, Inicializando el juego");

           Grammar grammar= CreateGrammarBuilderRGBSemantics2(null);
            Grammar grammarAction = CreateGrammarActions();
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.UnloadAllGrammars();
            // Nivel de confianza del reconocimiento 70%
            _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);
            grammar.Enabled = true;
            grammarAction.Enabled = true;
            _recognizer.LoadGrammar(grammar);
            _recognizer.LoadGrammar(grammarAction);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            //reconocimiento asíncrono y múltiples veces
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            synth.Speak("Ya estamos listos para jugar.");
         }

     

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //obtenemos un diccionario con los elementos semánticos
            SemanticValue semantics = e.Result.Semantics;
          
            string rawText = e.Result.Text;
                      
            if(color.Equals(rawText.Substring(0,1))){
                synth.Speak("Ya has jugado, le toca al otro jugador");
            }
            else {
                if (rawText.Contains("Salir") && victory) {
                    synth.Speak("Saliendo de la aplicacion");
                    Application.Exit();
                } else {
                    int jugada = (int)semantics["posicion"].Value;

                    if (tablero[jugada] == null)
                    {
                        realizarJugada(jugada, rawText.Substring(0, 1));

                        /*if (contador == 9)
                        {
                            synth.Speak("Fin de la partida");
                        
                            if (!ganador)
                            {
                                synth.Speak("Ha ganado la vieja");
                            }
                            //pendiente añadir comandos para que reinicie o cierre
                        }*/

                        color = rawText.Substring(0, 1);
                    }
                    else
                    {
                        synth.Speak("La jugada que has indicado no es válida");
                    }
                }
            }

        }
        
        private void realizarJugada(int jugada, string jugador)
        {

            tablero[jugada] = jugador;
            contador++;
            Color color = jugador == "B" ? Color.White : Color.Black;
            switch (jugada)
            {
                case 0:
                    this.button0.BackColor = color;
                    this.button0.ForeColor = color;
                    this.button0.Text = "";
                    if ((this.button4.BackColor == color && this.button8.BackColor == color) || (this.button2.BackColor == color && this.button1.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 1:
                    this.button1.BackColor = color;
                    this.button1.ForeColor = color;
                    this.button1.Text = "";
                    if ((this.button4.BackColor == color && this.button7.BackColor == color) || (this.button2.BackColor == color && this.button0.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 2:
                    this.button2.BackColor = color;
                    this.button2.ForeColor = color;
                    this.button2.Text = "";
                    if ((this.button4.BackColor == color && this.button6.BackColor == color) || (this.button1.BackColor == color && this.button0.BackColor == color) || (this.button4.BackColor == color && this.button6.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 3:
                    this.button3.BackColor = color;
                    this.button3.ForeColor = color;
                    this.button3.Text = "";
                    if ((this.button0.BackColor == color && this.button6.BackColor == color) || (this.button4.BackColor == color && this.button5.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 4:
                    this.button4.BackColor = color;
                    this.button4.ForeColor = color;
                    this.button4.Text = "";
                    if ((this.button1.BackColor == color && this.button7.BackColor == color) || (this.button3.BackColor == color && this.button5.BackColor == color) || (this.button6.BackColor == color && this.button2.BackColor == color) || (this.button0.BackColor == color && this.button8.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 5:
                    this.button5.BackColor = color;
                    this.button5.ForeColor = color;
                    this.button5.Text = "";
                    if ((this.button3.BackColor == color && this.button4.BackColor == color) || (this.button2.BackColor == color && this.button8.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 6:
                    this.button6.BackColor = color;
                    this.button6.ForeColor = color;
                    this.button6.Text = "";
                    if ((this.button4.BackColor == color && this.button2.BackColor == color) || (this.button0.BackColor == color && this.button3.BackColor == color) || (this.button7.BackColor == color && this.button8.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 7:
                    this.button7.BackColor = color;
                    this.button7.ForeColor = color;
                    this.button7.Text = "";
                    if ((this.button4.BackColor == color && this.button1.BackColor == color) || (this.button6.BackColor == color && this.button8.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
                case 8:
                    this.button8.BackColor = color;
                    this.button8.ForeColor = color;
                    this.button8.Text = "";
                    if ((this.button6.BackColor == color && this.button7.BackColor == color) || (this.button0.BackColor == color && this.button4.BackColor == color) || (this.button2.BackColor == color && this.button5.BackColor == color)) {
                        synth.Speak("Has ganado");
                        victory = true;
                    }
                    break;
            }
            Update();

            if (victory) {
                synth.Speak("Si deseas salir di salir, si deseas jugar otra partida di reiniciar");
            }
        }
        
        private void comprobarGanador()
        {
            //pendiente
        }
      
        private Grammar CreateGrammarBuilderRGBSemantics2(params int[] info)
        {
            //synth.Speak("Creando ahora la gramática");
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
    }
}
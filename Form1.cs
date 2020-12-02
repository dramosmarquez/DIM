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
        
        private String color="";
        private int contador=9;
        private String[] tablero = new String[9];
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Bienvenido al diseño de interfaces avanzadas. Inicializando la Aplicación");

           Grammar grammar= CreateGrammarBuilderRGBSemantics2(null);
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.UnloadAllGrammars();
            // Nivel de confianza del reconocimiento 70%
            _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);
            grammar.Enabled = true;
            _recognizer.LoadGrammar(grammar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            //reconocimiento asíncrono y múltiples veces
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            synth.Speak("Aplicación preparada para reconocer su voz");
         }

     

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
                      //obtenemos un diccionario con los elementos semánticos
                      SemanticValue semantics = e.Result.Semantics;
          
                      string rawText = e.Result.Text;
                      RecognitionResult result = e.Result;
                        
                      
                     if(color.Equals(rawText.Substring(0,1))){
                         synth.Speak("Ya has jugado, le toca al otro jugador");
                      }
                    int jugada = semantics["posicion"].Value;
                    
                     if(tablero[jugada] == ""){
                        tablero[jugada] = rawText.Substring(0,1);
                        contador--;
                     }else{
                        synth.Speak("La jugada que has indicado no es válida");
                        this.label1.Text=rawText;
                     }
                     color = rawText.Substring(0,1);
                      
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
    }
}
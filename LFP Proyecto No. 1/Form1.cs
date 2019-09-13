﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LFP_Proyecto_No._1.Controlador;
using System.Text.RegularExpressions;
namespace LFP_Proyecto_No._1
{
    public partial class Form1 : Form
    {
        //Variables Globales

        //pestañas
        int tabContador = 2;
        //Analizado Lexico
        string auxiliar = "";
        public string charInicial = "";
        string fila = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void NuevaPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tabContador++;
            var tabPage = new TabPage("Pestaña " + tabContador);
            tabControl1.Controls.Add(tabPage);
            var richTextBox = new RichTextBox();
            richTextBox.Width = 622;
            richTextBox.Height = 741;
            tabPage.Controls.Add(richTextBox);
        }

        private void AbrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "org files (*.org)|*.org";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                tabControl1.SelectedTab.Text = Path.GetFileName(filePath);


            }

            if (File.Exists(filePath))
            {
                StreamReader streamReader = new StreamReader(filePath);
                string line;
                foreach (Control c in tabControl1.SelectedTab.Controls)
                {
                    RichTextBox richTextBox = c as RichTextBox;
                    try
                    {
                        line = streamReader.ReadLine();
                        while (line != null)
                        {
                            richTextBox.AppendText(line + "\n");
                            line = streamReader.ReadLine();
                        }
                    }
                    catch (Exception)
                    {
                        alertMessage("Ha ocurrido un error.");
                    }
                }
            }
        }

        public void alertMessage(String mensaje)
        {
            MessageBox.Show(mensaje, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        //Boton analizar
        private void BotonAnalizar_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                RichTextBox richTextBox = c as RichTextBox;
                //pintarLexemas(richTextBox, richTextBox.Text, 0);
                TokenControlador.Instancia.clearListaTokens();
                TokenControlador.Instancia.clearListaTokensError();
                analizador_Lexico(richTextBox.Text); //Manda a llamar al metodo analizar cadena que se encarga de separar las instrucadenaFechasiones del textArea
            }
        }

        //Metodo que sirve para pintar las palabras reservadas 
        public void pintarLexemas(string lexema, string tipoLexema, int startIndex)
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                RichTextBox box = c as RichTextBox;
                int index = -1;
                int selectStart = box.SelectionStart;
                string word = lexema;
                if (box.Text.Contains(word))
                {
                    while ((index = box.Text.IndexOf(word, (index + 1))) != -1)
                    {
                        box.Select((index + startIndex), word.Length);
                        if (tipoLexema.Equals("reservada"))
                        {
                            box.SelectionColor = Color.FromArgb(28, 87, 157);
                        }
                        else if (tipoLexema.Equals("numero"))
                        {
                            box.SelectionColor = Color.Green;
                        }
                        else if (tipoLexema.Equals("llave"))
                        {
                            box.SelectionColor = Color.FromArgb(206,69,40);
                        }
                        else if (tipoLexema.Equals("punto"))
                        {
                            box.SelectionColor = Color.Orange;
                        }
                        else if (tipoLexema.Equals("cadena"))
                        {
                            box.SelectionColor = Color.FromArgb(131, 207, 26);
                        }
                    }
                }                
                box.Select(selectStart, 0);
                box.SelectionColor = Color.Black;

            }
        }


        #region ANALIZADOR_LEXICO
        //analizador_Lexico lexico
        public async void analizador_Lexico(String totalTexto)
        {
            ////
            int opcion = 0;
            int columna = 0;
            int fila = 1;
            totalTexto = totalTexto + " ";

            char[] charsRead = new char[totalTexto.Length];
            using (StringReader reader = new StringReader(totalTexto))
            {
                await reader.ReadAsync(charsRead, 0, totalTexto.Length);
            }

            StringBuilder reformattedText = new StringBuilder();
            using (StringWriter writer = new StringWriter(reformattedText))
            {
                for (int i = 0; i < charsRead.Length; i++)
                {
                    columna++;
                    Char c = totalTexto[i];
                    switch (opcion)
                    {
                        case 0:
                            //VERIFICA SI LO QUE VIENE ES LETRA
                            if (char.IsLetter(c))
                            {
                                charInicial = "";
                                opcion = 1;
                                auxiliar += c;
                                charInicial += c;
                            }

                            //VERIFICA SI ES ESPACIO EN BLANCO O SALTO DE LINEA
                            else if (c.Equals('\n'))
                            {

                                opcion = 0;
                                columna = 0;//COLUMNA 0
                                fila++; //FILA INCREMENTA

                            }
                            //VERIFICA SI ES ESPACIO EN BLANCO O SALTO DE LINEA
                            else if (char.IsWhiteSpace(c))
                            {
                                columna++;
                                opcion = 0;
                            }
                            //VERIFICA SI LO QUE VIENE ES DIGITO
                            else if (char.IsDigit(c))
                            {
                                opcion = 2;
                                auxiliar += c;
                            }
                            //VERIFICA SI LO QUE VIENE ES SIGNO DE PUNTUACION
                            else if (char.IsPunctuation(c))
                            {
                                //Console.WriteLine("esta entrando a puntuacion");

                                if (c.Equals('"'))
                                {
                                    columna++;
                                    opcion = 3;
                                    i--;
                                }
                                else if (c.Equals(','))
                                {
                                    opcion = 5;
                                    i--;
                                }
                                else if (c.Equals('{'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Llave_Izquierda");
                                    pintarLexemas(c.ToString(), "llave", 0);

                                }
                                else if (c.Equals('}'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Llave_Derecha");
                                    pintarLexemas(c.ToString(), "llave", 0);
                                }
                                else if (c.Equals(';'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Punto_y_Coma");
                                    pintarLexemas(c.ToString(), "punto", 0);
                                }
                                else if (c.Equals(':'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Dos_puntos");
                                }

                                /*else if (c.Equals('('))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Parentesis_Derecho");
                                }
                                else if (c.Equals(')'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Parentesis_Izquierdo");
                                }
                                else if (c.Equals('['))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Corchete_Derecho");
                                }
                                else if (c.Equals(']'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Corchete_Izquierdo");
                                }*/
                                else if (c.Equals('%'))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, c.ToString(), "Simb_Punt_Porcentaje");
                                }
                                else
                                {
                                    //Console.WriteLine("ULTIMO ELSE PUNTUACION");
                                    columna++;
                                    TokenControlador.Instancia.agregarError(fila, columna, c.ToString(), "Simb_Desconocido");
                                    opcion = 10;
                                    i--;
                                }

                            }
                            //LO MANDA A SIGNOS DESCONOCIDOS
                            else
                            {
                                columna++;
                                //Console.WriteLine("esta entrando al ultimo else");
                                TokenControlador.Instancia.agregarError(fila, columna, c.ToString(), "Simb_Desconocido");
                                opcion = 10;
                                i--;
                            }
                            break;
                        case 1:
                            if (Char.IsLetterOrDigit(c) || c == '_')
                            {
                                auxiliar += c;
                                opcion = 1;
                            }
                            else
                            {
                                if (auxiliar.Equals("Grafica") || auxiliar.Equals("Nombre") || auxiliar.Equals("Continente")
                                    || auxiliar.Equals("Pais") || auxiliar.Equals("Poblacion") || auxiliar.Equals("Saturacion") 
                                    || auxiliar.Equals("Bandera"))
                                {
                                    TokenControlador.Instancia.agregarToken(fila, columna, auxiliar, "Palabra_Reservada_"+auxiliar);
                                    pintarLexemas(auxiliar, "reservada", 0);
                                }
                                else
                                {
                                    TokenControlador.Instancia.agregarError(fila, columna, auxiliar, "Patron_Desconocido_"+auxiliar);
                                }

                                auxiliar = "";
                                i--;
                                opcion = 0;
                            }
                            break;
                        case 2:
                            if (Char.IsDigit(c))
                            {
                                auxiliar += c;
                                opcion = 2;
                            }
                            else if (c == '.')
                            {
                                opcion = 8;
                                auxiliar += c;
                            }
                            else
                            {
                                TokenControlador.Instancia.agregarToken(fila, columna, auxiliar, "Digito");
                                pintarLexemas(auxiliar, "numero", 0);
                                auxiliar = "";
                                i--;
                                opcion = 0;
                            }
                            break;
                        case 3:
                            if (c == '"')
                            {
                                auxiliar += c;
                                opcion = 4;
                            }
                            break;
                        case 4:
                            if (c != '"')
                            {
                                if (c.Equals('\n')) { fila++; columna = 0; }
                                auxiliar += c;
                                opcion = 4;
                            }
                            else
                            {
                                opcion = 5;
                                i--;
                            }
                            break;
                        case 5:
                            if (c == '"')
                            {
                                auxiliar += c;
                                TokenControlador.Instancia.agregarToken(fila, columna, auxiliar, "Cadena");
                                pintarLexemas(auxiliar, "cadena", 0);
                                opcion = 0;
                                auxiliar = "";
                            }
                            break;
                        case 8:
                            if (char.IsDigit(c))
                            {
                                opcion = 9;
                                auxiliar += c;
                            }
                            else
                            {
                                opcion = 0;
                                auxiliar = "";
                            }
                            break;
                        case 9:
                            if (Char.IsDigit(c))
                            {
                                opcion = 9;
                                auxiliar += c;
                            }
                            else
                            {
                                TokenControlador.Instancia.agregarToken(fila, columna, auxiliar, "Digito");
                                auxiliar = "";
                                i--;

                                opcion = 0;
                            }

                            break;
                        case 10:
                            auxiliar += c;
                            //TokenControlador.Instancia.error(auxiliar, "Desconocido");
                            opcion = 0;
                            auxiliar = "";
                            break;
                    }
                }
            }

        }

        #endregion


        private void ImprimirTokensToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TokenControlador.Instancia.ImprimirTokens(tabControl1.SelectedTab.Text);
        }

    }
}
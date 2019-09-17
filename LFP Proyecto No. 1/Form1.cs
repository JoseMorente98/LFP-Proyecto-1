using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using LFP_Proyecto_No._1.Controlador;
using System.Text.RegularExpressions;
using LFP_Proyecto_No._1.Modelo;
using System.Collections;

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
        string appPath = Application.StartupPath;


        public Form1()
        {
            InitializeComponent();
            richDescripcion.Visible = false;

        }



        #region MenuBar

        #region Menu_Archivo

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

        #endregion


        #region Menu_Reportes
        //Imprimir tokens
        private void ImprimirTokensToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TokenControlador.Instancia.ImprimirTokens(tabControl1.SelectedTab.Text);
        }
        private void ImprimirErroresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TokenControlador.Instancia.ImprimirErrores(tabControl1.SelectedTab.Text);
        }
        #endregion

        #endregion




        //Boton analizar
        private void BotonAnalizar_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                RichTextBox rich = c as RichTextBox;
                //pintarLexemas(richTextBox, richTextBox.Text, 0);
                if (rich.Text != "")
                {
                    limpieza();
                    analizador_Lexico(rich.Text); //Manda a llamar al metodo analizar cadena que se encarga de separar las instrucadenaFechasiones del textArea
                            //OBTENER CONTINENTE
                    obtenerContinentes();
                    //GrafoControlador.Instancia.generarPaises();
                    GrafoControlador.Instancia.generarTexto();
                    //string a = "";
                    generarImagen("diag", this.appPath);
                }
                else
                {
                    alertMessage("No se ha detectado texto para analizar");
                }
            }




        }

        //Sirve para limpiar los componentes principales
        public void limpieza()
        {
            //Limpia la imagen
            this.pictureGrafico.InitialImage = null;
            this.pictureGrafico.Image = null;
            
            //Limpia los tokens
            TokenControlador.Instancia.clearListaTokens();
            TokenControlador.Instancia.clearListaTokensError();
            ContinenteControlador.Instancia.limpiarArrayListContinentes();

        }


        #region Pintar_Lexema
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
                            box.SelectionColor = Color.FromArgb(206, 69, 40);
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
#endregion


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
                                    TokenControlador.Instancia.agregarToken(fila, columna, auxiliar, "Palabra_Reservada_" + auxiliar);
                                    pintarLexemas(auxiliar, "reservada", 0);
                                }
                                else
                                {
                                    TokenControlador.Instancia.agregarError(fila, columna, auxiliar, "Patron_Desconocido_" + auxiliar);
                                    alertMessage("Se detecto un error, Linea" + fila + " , columna " + columna);
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



        public void generarImagen(string nombre, string path)
        {

            System.IO.File.WriteAllText(path + "\\" + nombre + ".dot", GrafoControlador.Instancia.getGrafoDot());
            var command = "dot -Tpng \"" + path + "\\" + nombre + ".dot\"  -o \"" + path + "\\" + nombre + ".png\"   ";
            var procStarInfo = new ProcessStartInfo("cmd", "/C" + command);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStarInfo;
            proc.Start();
            proc.WaitForExit();

            


            string var = path + "\\" + "diag.png";
            if (File.Exists(var))
            {
                Image img = Image.FromFile(var.Replace("\"", ""));
                this.pictureGrafico.Image = img;
            }
            else
            {
                richDescripcion.Text = richDescripcion.Text + " " + "\n\n--IMAGEN NO DISPONIBLE---";
            }



        }

        public void alertMessage(String mensaje)
        {
            MessageBox.Show(mensaje, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void ImprimirContinentesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Continente c in ContinenteControlador.Instancia.getArrayListContinentes())
            {
                Console.WriteLine("________________________________________");
                Console.WriteLine(c.Nombre);
                Console.WriteLine("________________________________________");
                foreach (Pais p in c.Paises)
                {
                    Console.WriteLine(p.Nombre);
                }
            }
        }

        public void obtenerContinentes()
        {
            for (int i = 0; i < TokenControlador.Instancia.getArrayListTokens().Count; i++)
            {
                Token tok = (Token)TokenControlador.Instancia.getArrayListTokens()[i];
                if (tok.Lexema.Equals("Continente"))
                {
                    ArrayList arrayListPais = new ArrayList();
                    Token tok2 = (Token)TokenControlador.Instancia.getArrayListTokens()[i + 5];
                    //GUARDAR NOMBRE DE CONTINENTE
                    string continente = "";
                    string continente2 = "";
                    continente = tok2.Lexema;
                    Console.WriteLine("______________________________________");
                    Console.WriteLine(continente);
                    Console.WriteLine("______________________________________");
                    for (int j = 0; j < TokenControlador.Instancia.getArrayListTokens().Count; j++)
                    {
                        //Comparación del Continente
                        Token tok3 = (Token)TokenControlador.Instancia.getArrayListTokens()[j];
                        if (tok3.Lexema.Equals("Continente"))
                        {
                            Token tok4 = (Token)TokenControlador.Instancia.getArrayListTokens()[j + 5];
                            continente2 = tok4.Lexema;
                        }
                        if (tok3.Lexema.Equals("Pais"))
                        {
                            if (continente.Equals(continente2))
                            {
                                string nombre = "";
                                string bandera = "";
                                int poblacion = 0;
                                int saturacion = 0;
                                for (int k = j; k < TokenControlador.Instancia.getArrayListTokens().Count; k++)
                                {
                                    Token tok4 = (Token)TokenControlador.Instancia.getArrayListTokens()[k];
                                    if (tok4.Lexema.Equals("Nombre"))
                                    {
                                        Token tok5 = (Token)TokenControlador.Instancia.getArrayListTokens()[k + 2];
                                        nombre = tok5.Lexema;
                                        break;
                                    }
                                }
                                for (int k = j; k < TokenControlador.Instancia.getArrayListTokens().Count; k++)
                                {
                                    Token tok4 = (Token)TokenControlador.Instancia.getArrayListTokens()[k];
                                    if (tok4.Lexema.Equals("Poblacion"))
                                    {
                                        Token tok5 = (Token)TokenControlador.Instancia.getArrayListTokens()[k + 2];
                                        poblacion = Int32.Parse(tok5.Lexema);
                                        break;
                                    }
                                }
                                for (int k = j; k < TokenControlador.Instancia.getArrayListTokens().Count; k++)
                                {
                                    Token tok4 = (Token)TokenControlador.Instancia.getArrayListTokens()[k];
                                    if (tok4.Lexema.Equals("Saturacion"))
                                    {
                                        Token tok5 = (Token)TokenControlador.Instancia.getArrayListTokens()[k + 2];
                                        saturacion = Int32.Parse(tok5.Lexema);
                                        break;
                                    }
                                }
                                for (int k = j; k < TokenControlador.Instancia.getArrayListTokens().Count; k++)
                                {
                                    Token tok4 = (Token)TokenControlador.Instancia.getArrayListTokens()[k];
                                    if (tok4.Lexema.Equals("Bandera"))
                                    {
                                        Token tok5 = (Token)TokenControlador.Instancia.getArrayListTokens()[k + 2];
                                        bandera = tok5.Lexema;
                                        break;
                                    }
                                }
                                Pais p = new Pais(nombre.Replace("\"", ""), poblacion, saturacion, bandera.Replace("\"", ""));
                                arrayListPais.Add(p);
                                Console.WriteLine("PAIS: " + nombre);
                                Console.WriteLine("SATURACION: " + saturacion);
                                Console.WriteLine("POBLACION: " + poblacion);
                                Console.WriteLine("BANDERA: " + bandera);
                            }
                        }
                    }
                    ContinenteControlador.Instancia.agregarContinente(continente.Replace("\"", ""), arrayListPais);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
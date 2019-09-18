using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using LFP_Proyecto_No._1.Controlador;
using LFP_Proyecto_No._1.Modelo;

namespace LFP_Proyecto_No._1.Controlador
{
    class GrafoControlador
    {
        private readonly static GrafoControlador instancia = new GrafoControlador();
        public string grafoDot = "";
        ArrayList listaPaises = new ArrayList();
        ArrayList arayAuxiliar = new ArrayList();
        ArrayList paises = new ArrayList();
        ArrayList continentes = ContinenteControlador.Instancia.getArrayListContinentes();

        public static GrafoControlador Instancia
        {
            get
            {
                return instancia;
            }
        }


        //Arma el texto para graficar
        public void generarTexto()
        {
            ArrayList continentes = ContinenteControlador.Instancia.getArrayListContinentes();

            int satContinente = 0;
            int satPais = 0;
            string cabeza = "";
            string cuerpo = "";
            string aux = "";
            //Encabezado



            grafoDot = "digraph G {" + "start[shape = Mdiamond label = \"" + TokenControlador.Instancia.getNombreGrafica() + "\"];";

            for (int i = 0; i < continentes.Count; i++)
            {
                Continente c = (Continente)continentes[i];
                string nContinente = c.Nombre.Replace(" ", "");
                cabeza = aux + "start->" + c.Nombre + ";";
                for (int j = 0; j < c.Paises.Count; j++)
                {

                    Pais p = (Pais)c.Paises[j];
                    string nPais = p.Nombre.Replace(" ", "");


                    //Suma las saturaciones de los paises
                    satPais = satPais + p.Satuacion;
                    //Arma el cuerpo
                    cuerpo = cuerpo +
                        nContinente + "->" + nPais + ";" +
                        nPais + "[shape = record label = \"{" + p.Nombre + "|" + p.Satuacion + "}\"style = filled fillcolor = " + getColor(p.Satuacion) + "];";

                }
                //Saturacion del continente
                double auxd = satPais / c.Paises.Count;
                satContinente = (int)Math.Round(auxd, 0, MidpointRounding.AwayFromZero);

                aux = cabeza + cuerpo + nContinente + "[shape=record label=\"{" + nContinente + "| " + satContinente + "} \" style=filled fillcolor=" + getColor(satContinente) + "];";
                grafoDot = grafoDot + aux;
                cabeza = ""; cuerpo = ""; aux = ""; satPais = 0;
            }

            grafoDot = grafoDot + " } ";



            ordenarPais();
        }

        //Devuelve el texto para graficar
        public string getGrafoDot()
        {
            return this.grafoDot;
        }

        public string getColor(int saturacion)
        {
            string colorPais = "";

            if (0 <= saturacion && saturacion <= 15)
            {
                colorPais = "White";
            }
            else if (16 <= saturacion && saturacion <= 30)
            {
                colorPais = "Blue";
            }
            else if (31 <= saturacion && saturacion <= 45)
            {
                colorPais = "Green";
            }
            else if (46 <= saturacion && saturacion <= 60)
            {
                colorPais = "Yellow";
            }
            else if (61 <= saturacion && saturacion <= 75)
            {
                colorPais = "Orange";
            }
            else if (76 <= saturacion && saturacion <= 100)
            {
                colorPais = "Red";
            }
            return colorPais;

        }


        public void ordenarPais()
        {

            //Agrega los paises de los continentes a un array nuevo que va a ser ordenado
            if (continentes.Count != 0)
            {
                for (int i = 0; i < continentes.Count; i++)
                {
                    Continente c = (Continente)continentes[i];
                    for (int j = 0; j < c.Paises.Count; j++)
                    {
                        Pais p = (Pais)c.Paises[j];
                        paises.Add(p);
                    }
                }


                //Metodo burbuja que ordena los paises en forma ascendente segun saturacion
                for (int i = 0; i <= paises.Count - 1; i++)
                {
                    for (int j = 0; j < paises.Count - i - 1; j++)
                    {
                        if (((Pais)paises[j]).Satuacion > ((Pais)paises[j + 1]).Satuacion)
                        {
                            object tem = paises[j];
                            paises[j] = paises[j + 1];
                            paises[j + 1] = tem;
                        }
                    }
                }
            }

        }

        public Pais getPaisMejorOpcion()
        {
            this.arayAuxiliar.Clear();
            ////////////////////////////////////////////////////////
            ///          parte que verifica si la saturacion más pequeña viene mas de una vez
            ///                       
            if (this.paises.Count !=0)
            {
                int saturacion = ((Pais)paises[0]).Satuacion;

                int contador = 0;
                listaPaises.Add(((Pais)paises[0]).Nombre);
                for (int i = 1; i < paises.Count; i++)
                {
                    Pais p = (Pais)paises[i];
                    //verifica si la saturacion mas pequeña se repite
                    if (saturacion == p.Satuacion)
                    {
                        //Agrega el nombre del pais que se repitió
                        listaPaises.Add(p.Nombre);
                        contador++;
                    }
                }

            


                if (contador >= 1)
                {
                    
                    for (int i = 0; i < continentes.Count; i++)
                    {
                        Continente c = (Continente)continentes[i];

                        for (int j = 0; j < c.Paises.Count; j++)
                        {
                            Pais p = (Pais)c.Paises[j];
                            for (int k = 0; k < listaPaises.Count; k++)
                            {
                                if (listaPaises[k].ToString() != "")
                                {
                                    if (listaPaises[k].ToString().Equals(p.Nombre))
                                    {
                                        int satCont = 0;
                                        for (int m = 0; m < c.Paises.Count; m++)
                                        {
                                            satCont = satCont + ((Pais)c.Paises[m]).Satuacion;
                                        }
                                        string var = c.Nombre + "," + (satCont/c.Paises.Count);
                                        arayAuxiliar.Add(var);
                                    }
                                }
                                
                            }
                        }

                    }

                    //METODO BURBUJA QUE ORDENA SEGUN CONTINENTE
                    
                    for (int i = 0; i <= arayAuxiliar.Count-1; i++)
                    {
                        for (int j = 0; j < arayAuxiliar.Count-i-1; j++)
                        {
                            string[] a = ((String)arayAuxiliar[j]).Split(',');
                            string[] b = ((String)arayAuxiliar[j+1]).Split(',');

                            int a1 = int.Parse(a[1]);
                            int b1 = int.Parse(b[1]);
                            if (a1 > b1)
                            {
                                object tem = arayAuxiliar[j];
                                arayAuxiliar[j] = arayAuxiliar[j + 1];
                                arayAuxiliar[j + 1] = tem;
                            }
                        }
                    }


                    for (int i = 0; i < continentes.Count; i++)
                    {
                        Continente c = (Continente)continentes[i];
                        String[] detCont = ((String)arayAuxiliar[0]).Split(',');
                        String var = detCont[0];
                        if (c.Nombre.Equals(var))
                        {
                            for (int j = 0; j < c.Paises.Count; j++)
                            {
                                int sat = ((Pais)c.Paises[j]).Satuacion;
                                if (sat == saturacion)
                                {
                                    return (Pais)c.Paises[j];

                                }
                            }

                            break;
                        }
                        

                    }
                   
                    Console.WriteLine("la saturacion es " + saturacion + " y se repite " + contador + " veces");
                }
                else
                {
                    return ((Pais)paises[0]);
                    Console.WriteLine("la saturacion es " + saturacion + " y se repite " + contador + " veces");
                }

            }
            return null;
        }
        public void limpiarAnalisisDePais()
        {
            continentes.Clear();
            paises.Clear();
            listaPaises.Clear();
            arayAuxiliar.Clear();
        }
    }
}
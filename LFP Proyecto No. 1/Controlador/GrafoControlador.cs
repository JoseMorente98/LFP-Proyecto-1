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
        String[] listaPaises = new String[20];
        String[] listaContinentes = new String[20];
        public static GrafoControlador Instancia
        {
            get
            {
                return instancia;
            }
        }



        public void generarPaises()
        {
            Pais p = new Pais("Japon", 123456, 80, "path");
            Pais p1 = new Pais("China", 852147, 95, "path");
            Pais p2 = new Pais("Korea", 123456, 40, "path");
            ArrayList a = new ArrayList();
            a.Add(p);
            a.Add(p1);
            a.Add(p2);
            ContinenteControlador.Instancia.agregarContinente("Asia", a);


            Pais p3 = new Pais("USA", 852348, 30, "path");
            Pais p4 = new Pais("Colombia", 852147, 20, "path");
            Pais p5 = new Pais("Argentina", 123456, 10, "path");

            ArrayList a1 = new ArrayList();
            a1.Add(p3);
            a1.Add(p4);
            a1.Add(p5);
            ContinenteControlador.Instancia.agregarContinente("America", a1);

            Pais p6 = new Pais("España", 852348, 60, "path");
            Pais p7 = new Pais("Italia", 852147, 70, "path");
            Pais p8 = new Pais("Inglaterra", 123456, 80, "path");
            ArrayList a2 = new ArrayList();
            a2.Add(p6);
            a2.Add(p7);
            a2.Add(p8);
            ContinenteControlador.Instancia.agregarContinente("Europa", a2);

            Pais p9 = new Pais("Egipto", 852348, 77, "path");
            Pais p10 = new Pais("Berlin", 852147, 85, "path");
            Pais p11 = new Pais("Dubai", 123456, 100, "path");
            ArrayList a3 = new ArrayList();
            a3.Add(p9);
            a3.Add(p10);
            a3.Add(p11);
            ContinenteControlador.Instancia.agregarContinente("Africa", a3);



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
            ArrayList continentes = ContinenteControlador.Instancia.getArrayListContinentes();
            ArrayList paises = new ArrayList();

            //Agrega los paises de los continentes a un array nuevo que va a ser ordenado
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


            ////////////////////////////////////////////////////////
            ///          parte que verifica si la saturacion más pequeña viene mas de una vez
            ///                       

            int saturacion = ((Pais)paises[0]).Satuacion;
            int contador = 0;

            for (int i = 1; i < paises.Count; i++)
            {
                Pais p = (Pais)paises[i];
                //verifica si la saturacion mas pequeña se repite
                if (saturacion == p.Satuacion)
                {
                    //Agrega el nombre del pais que se repitió
                    listaPaises[contador] = p.Nombre;
                    contador++;
                }
            }

            if (contador > 1)
            {

                for (int i = 0; i < continentes.Count; i++)
                {
                    Continente c = (Continente)continentes[i];

                    for (int j = 0; j < c.Paises.Count; j++)
                    {
                        Pais p = (Pais)c.Paises[j];
                        for (int k = 0; k < listaPaises.Length; k++)
                        {

                            if (p.Nombre.Equals(listaPaises[k]))
                            {
                                int satContinente = 0;
                            }
                        }
                    }
                }

                Console.WriteLine("la saturacion es " + saturacion + " y se repite " + contador + " veces");
            }
            else
            {
                //return ((Pais)paises[0])
                Console.WriteLine("la saturacion es " + saturacion + " y se repite " + contador + " veces");
            }



        }

    }
}
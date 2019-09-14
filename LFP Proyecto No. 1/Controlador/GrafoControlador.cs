﻿using System;
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
        public static GrafoControlador Instancia
        {
            get
            {
                return instancia;
            }
        }



        public void generarPaises()
        {
            Pais p = new Pais("Japon", 123456, "80", "path");
            Pais p1 = new Pais("China", 852147, "95", "path");
            Pais p2 = new Pais("Korea", 123456, "40", "path");
            ArrayList a = new ArrayList();
            a.Add(p);
            a.Add(p1);
            a.Add(p2);
            ContinenteControlador.Instancia.agregarContinente("Asia", a);


            Pais p3 = new Pais("USA", 852348, "30", "path");
            Pais p4 = new Pais("Colombia", 852147, "20", "path");
            Pais p5 = new Pais("Argentina", 123456, "10", "path");
            ArrayList a1 = new ArrayList();
            a1.Add(p3);
            a1.Add(p4);
            a1.Add(p5);
            ContinenteControlador.Instancia.agregarContinente("America", a1);

            Pais p6 = new Pais("España", 852348, "60", "path");
            Pais p7 = new Pais("Italia", 852147, "70", "path");
            Pais p8 = new Pais("Inglaterra", 123456, "80", "path");
            ArrayList a2 = new ArrayList();
            a2.Add(p6);
            a2.Add(p7);
            a2.Add(p8);
            ContinenteControlador.Instancia.agregarContinente("Europa", a2);

            Pais p9 = new Pais("Egipto", 852348, "77", "path");
            Pais p10 = new Pais("Berlin", 852147, "85", "path");
            Pais p11 = new Pais("Dubai", 123456, "100", "path");
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
            grafoDot = "digraph G {" + "start[shape = Mdiamond label = \"Paises\"];";

            for (int i = 0; i < continentes.Count; i++)
            {
                Continente c = (Continente)continentes[i];
                cabeza = aux + "start->" + c.Nombre  + ";";
                for (int j = 0; j < c.Paises.Count; j++)
                {
                    Pais p = (Pais)c.Paises[j];
                    //Suma las saturaciones de los pai
                    satPais = satPais + int.Parse(p.Satuacion);
                    //Arma el cuerpo
                    cuerpo = cuerpo +
                        c.Nombre + "->" + p.Nombre + ";" +
                        p.Nombre + "[shape = record label = \"{" + p.Nombre + "|" + p.Satuacion + "}\"style = filled fillcolor = " + getColor(int.Parse(p.Satuacion)) +"];";
                }
                //Saturacion del continente
                double auxd = satPais / c.Paises.Count;
                satContinente = (int)Math.Round(auxd, 0, MidpointRounding.AwayFromZero);

                aux = cabeza + cuerpo + c.Nombre+"[shape=record label=\"{"+c.Nombre+"| " + satContinente +"} \" style=filled fillcolor="+ getColor(satContinente) +"];";
                grafoDot = grafoDot + aux;
                cabeza = ""; cuerpo = ""; aux = ""; satPais = 0; 
            }

            grafoDot = grafoDot + " } ";

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

    }
}

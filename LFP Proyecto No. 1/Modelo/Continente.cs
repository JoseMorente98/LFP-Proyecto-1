using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFP_Proyecto_No._1.Modelo
{
    class Continente
    {
        private string nombre;
        private ArrayList arrayListPaises;
        
        public Continente(string nombre, ArrayList arrayListPaises)
        {
            this.Nombre = nombre;
            this.Paises = arrayListPaises;
        }

        public Continente(string nombre)
        {
            this.Nombre = nombre;
        }

        public Continente()
        {
            arrayListPaises = new ArrayList();
        }

        //Setters y Getters
        public string Nombre { get => nombre; set => nombre = value; }
        public ArrayList Paises { get => arrayListPaises; set => arrayListPaises = value; }

        public void agregarPais(string nombre, int poblacion, string satuacion, string path)
        {
            Pais pais = new Pais(nombre, poblacion, satuacion, path);
            arrayListPaises.Add(pais);
        }
    }
}

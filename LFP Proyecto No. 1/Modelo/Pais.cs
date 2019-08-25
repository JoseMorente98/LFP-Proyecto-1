using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFP_Proyecto_No._1.Modelo
{
    class Pais
    {
        private string nombre;
        private int poblacion;
        private string satuacion;
        private string path;
        
        public Pais(string nombre, int poblacion, string satuacion, string path)
        {
            this.Nombre = nombre;
            this.Poblacion = poblacion;
            this.Satuacion = satuacion;
            this.Path = path;
        }

        public Pais()
        {
        }

        //Getters y Setters
        public string Nombre { get => nombre; set => nombre = value; }
        public int Poblacion { get => poblacion; set => poblacion = value; }
        public string Satuacion { get => satuacion; set => satuacion = value; }
        public string Path { get => path; set => path = value; }
    }
}

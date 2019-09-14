using LFP_Proyecto_No._1.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFP_Proyecto_No._1.Controlador
{
    class ContinenteControlador
    {
        private readonly static ContinenteControlador instancia = new ContinenteControlador();
        private ArrayList arrayListContinentes = new ArrayList();
        
        private ContinenteControlador()
        {
        }

        public static ContinenteControlador Instancia
        {
            get
            {
                return instancia;
            }
        }

        public void agregarContinente(string nombre, ArrayList paises)
        {
            Continente continente = new Continente(nombre, paises);
            arrayListContinentes.Add(continente);
        }


        

        public ArrayList getArrayListContinentes()
        {
            return arrayListContinentes;
        }

        

    }
}

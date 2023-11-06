using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
     class Files
    {

        public List<Candidates> CargarAplicantes(string filePath)
        {
            List<Candidates> aplicantes = new List<Candidates>();
            

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] values = line.Split(';', 2);

                    string operacion = values[0];
                    string jsonContent = values[1];

                    if ("INSERT".Equals(operacion))
                    {
                        Candidates aplicante = JsonConvert.DeserializeObject<Candidates>(jsonContent);
                        aplicantes.Add(aplicante);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return aplicantes;
        }

        public List<Candidates> LeerLlave(string filePath)
        {
            List<Candidates> aplicantes = new List<Candidates>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    byte[] decodedBytes = Convert.FromBase64String(line);
                    string decodedString = Encoding.UTF8.GetString(decodedBytes);
                    Console.WriteLine(decodedString);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return aplicantes;
        }

        public static void EscribirArchivo(string path, string fileName, string content)
        {
            try
            {
                string filePath = Path.Combine(path, fileName);
                File.WriteAllText(filePath, content, Encoding.UTF8);

                Console.WriteLine("Archivo " + fileName + " ha sido creado con éxito en " + path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al escribir en el archivo: " + e.Message);
            }
        }
    }
}

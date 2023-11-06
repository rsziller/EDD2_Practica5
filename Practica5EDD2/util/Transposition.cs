using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
    public static class Transposition
    {
        // Key for Columnar Transposition
        const string key = "HACK";
        static Dictionary<char, int> keyMap = new Dictionary<char, int>();

        public static void SetPermutationOrder()
        {
            // Add the permutation order into map
            for (int i = 0; i < key.Length; i++)
            {
                keyMap[key[i]] = i;
            }
        }

        // Encryption
        public static string EncryptMessage(string msg)
        {
            int row, col, j;
            string cipher = "";

            /* calculate column of the matrix*/
            col = key.Length;

            /* calculate Maximum row of the matrix*/
            row = msg.Length / col;

            if (msg.Length % col != 0)
                row += 1;

            char[,] matrix = new char[row, col];

            for (int i = 0, k = 0; i < row; i++)
            {
                for (j = 0; j < col;)
                {
                    if (k >= msg.Length)
                    {
                        /* Adding the padding character '_' */
                        matrix[i, j] = '_';
                        j++;
                    }
                    else
                    {
                        /* Adding any character into matrix*/
                        matrix[i, j] = msg[k];
                        j++;
                    }
                    k++;
                }
            }

            // Rearrange the matrix column wise based on the key
            string sortedKey = new string(key.OrderBy(c => c).ToArray());
            for (int i = 0; i < col; i++)
            {
                char currentKeyChar = sortedKey[i];
                int originalIndex = keyMap[currentKeyChar];
                for (int r = 0; r < row; r++)
                {
                    cipher += matrix[r, originalIndex];
                }
            }

            return cipher;
        }

        // Decryption
        public static string DecryptMessage(string cipher)
        {
            /* calculate row and column for cipher Matrix */
            int col = key.Length;

            int row = cipher.Length / col;
            char[,] cipherMat = new char[row, col];

            // Rearrange the cipher into the matrix column wise based on the key
            string sortedKey = new string(key.OrderBy(c => c).ToArray());
            int k = 0;
            for (int i = 0; i < col; i++)
            {
                char currentKeyChar = sortedKey[i];
                int originalIndex = keyMap[currentKeyChar];
                for (int r = 0; r < row; r++)
                {
                    cipherMat[r, originalIndex] = cipher[k++];
                }
            }

            /* Arrange the matrix column wise according
            to permutation order by adding into new matrix */
            char[,] decCipher = new char[row, col];
            int l = 0;
            foreach (char ch in key)
            {
                int j = keyMap[ch];
                for (int i = 0; i < row; i++)
                {
                    decCipher[i, j] = cipherMat[i, l];
                }
                l++;
            }

            /* getting Message using matrix */
            string msg = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (decCipher[i, j] != '_')
                        msg += decCipher[i, j];
                }
            }
            return msg;
        }

        public static Dictionary<string, List<string>> ReadFilesInFolder(string folderPath)
        {
            Dictionary<string, List<string>> dpiContentMap = new Dictionary<string, List<string>>();

            try
            {
                string[] files = Directory.GetFiles(folderPath, "REC-*.txt");
                foreach (var filePath in files)
                {
                    // Obtener DPI del nombre del archivo
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string[] parts = fileName.Split('-');
                    if (parts.Length >= 2)
                    {
                        string dpi = parts[1];

                        // Leer el contenido del archivo y guardarlo en la lista
                        string fileContent = File.ReadAllText(filePath);

                        // Agregar el contenido a la lista asociada al DPI
                        if (!dpiContentMap.ContainsKey(dpi))
                        {
                            dpiContentMap[dpi] = new List<string>();
                        }
                        dpiContentMap[dpi].Add(fileContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer los archivos: " + ex.Message);
            }

            return dpiContentMap;
        }

        public static Dictionary<string, List<string>> ReadFilesInFolderConv(string folderPath)
        {
            Dictionary<string, List<string>> dpiContentMapConv = new Dictionary<string, List<string>>();

            try
            {
                string[] files = Directory.GetFiles(folderPath, "CONV-*.txt");
                foreach (var filePath in files)
                {
                    // Obtener DPI del nombre del archivo
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string[] parts = fileName.Split('-');
                    if (parts.Length >= 2)
                    {
                        string dpi = parts[1];

                        // Leer el contenido del archivo y guardarlo en la lista
                        string fileContent = File.ReadAllText(filePath);

                        // Agregar el contenido a la lista asociada al DPI
                        if (!dpiContentMapConv.ContainsKey(dpi))
                        {
                            dpiContentMapConv[dpi] = new List<string>();
                        }
                        dpiContentMapConv[dpi].Add(fileContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer los archivos: " + ex.Message);
            }

            return dpiContentMapConv;
        }

        public static void EncriptarValores(Dictionary<string, List<string>> dpiContentMap)
        {
            foreach (var kvp in dpiContentMap)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i] = EncryptMessage(kvp.Value[i]);
                }
            }
        }

        public static void DesencriptarValores(Dictionary<string, List<string>> dpiContentMap)
        {
            foreach (var kvp in dpiContentMap)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i] = DecryptMessage(kvp.Value[i]);
                }
            }
        }

        public static void MostrarContenidoEncriptado(Dictionary<string, List<string>> dpiContentMap)
        {
            //Console.WriteLine("Contenido encriptado:");
            foreach (var kvp in dpiContentMap)
            {
                //Console.WriteLine("DPI: " + kvp.Key);
                //Console.WriteLine("Contenido encriptado:");

                int numeroContador = 0;
                foreach (var content in kvp.Value)
                {
                    //Console.WriteLine();
                    //Console.Write(content);
                    //Console.WriteLine();

                    //GuardarRecomendacionEnc(kvp.Key, kvp.Value[numeroContador], numeroContador + 1);
                    numeroContador++;
                }
                //Console.WriteLine();
            }
        }

        public static void MostrarContenidoDesencriptado(Dictionary<string, List<string>> dpiContentMap)
        {
            //Console.WriteLine("Contenido desencriptado:");
            foreach (var kvp in dpiContentMap)
            {
                //Console.WriteLine("DPI: " + kvp.Key);
                //Console.WriteLine("Contenido desencriptado:");

                int numeroContador = 0;

                foreach (var content in kvp.Value)
                {
                    //Console.WriteLine();
                    //Console.WriteLine(DecryptMessage(content));
                    //Console.Write(content);
                    //Console.WriteLine();

                    //GuardarRecomendacionDec(kvp.Key, kvp.Value[numeroContador], numeroContador + 1);
                    numeroContador++;
                }
                //Console.WriteLine();
            }
        }


        public static void GuardarRecomendacionDec(string key, string value, int numeroCarta)
        {
            string nombreArchivo = $"REC-{key}-{numeroCarta}.txt";
            string carpeta = @"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2\datospractica3\dec_inputs";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

            File.WriteAllText(rutaArchivo, value);
        }

        public static void GuardarRecomendacionEnc(string key, string value, int numeroCarta)
        {
            string nombreArchivo = $"REC-{key}-{numeroCarta}.txt";
            string carpeta = @"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2\datospractica3\enc_inputs";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

            File.WriteAllText(rutaArchivo, value);
        }

        public static List<String> MostrarContenidoDesencriptadoDPI(Dictionary<string, List<string>> dpiContentMap, string dpi)
        {

            List<String> contenidoDesencriptado = new List<String>();

            foreach (var kvp in dpiContentMap)
            {
                if (kvp.Key == dpi)
                {
                    foreach (var content in kvp.Value)
                    {
                        contenidoDesencriptado.Add(DecryptMessage(content));
                    }
                }
            }

            return contenidoDesencriptado;
        }

        

        public static void MostrarContenidoDesencriptadoDPI1(Dictionary<string, List<string>> dpiContentMap, string dpi)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Cartas:");
            foreach (var kvp in dpiContentMap)
            {
                if (kvp.Key == dpi)
                {
                    //Console.WriteLine("DPI: " + kvp.Key);
                    //Console.WriteLine("Contenido desencriptado:");
                    foreach (var content in kvp.Value)
                    {
                        Console.WriteLine();
                        Console.WriteLine(DecryptMessage(content));
                        //Console.Write(content);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }

            }
        }

        public static void MostrarContenidoEncriptadoDPI(Dictionary<string, List<string>> dpiContentMap, string dpi)
        {
            //Console.WriteLine("Contenido encriptado:");
            foreach (var kvp in dpiContentMap)
            {
                if (kvp.Key == dpi)
                {
                    //Console.WriteLine("DPI: " + kvp.Key);
                    //Console.WriteLine("Contenido encriptado:");
                    foreach (var content in kvp.Value)
                    {
                        Console.WriteLine();
                        Console.Write(content);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}

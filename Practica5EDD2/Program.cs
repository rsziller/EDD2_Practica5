using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Practica5EDD2.model;
using Practica5EDD2.util;
using RSA = Practica5EDD2.util.RSA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Practica5EDD2
{
    class Program
    {

        static bool AreBitArraysEqual(BitArray bitArray1, BitArray bitArray2)
        {
            if (bitArray1.Length != bitArray2.Length)
            {
                return false;
            }

            for (int i = 0; i < bitArray1.Length; i++)
            {
                if (bitArray1[i] != bitArray2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public class CsvReader
        {
            public List<List<string>> ReadCsv(string filePath)
            {
                List<List<string>> csvData = new List<List<string>>();

                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        if (fields != null)
                        {
                            List<string> rowData = new List<string>(fields);
                            csvData.Add(rowData);
                        }
                    }
                }

                return csvData;
            }
        }



        public static Keys keyPair = RSA.GenerateKeys(2048);


        public static byte[] GenerarFirmaUnica(string message)
        {
            // Generar claves RSA



            // Firmar el mensaje
            byte[] signature = RSA.SignMessage(message, keyPair.PrivateKey, keyPair.N);


            // Devolver la firma
            return signature;
        }

        static Dictionary<string, List<string>> dpiContentMap = new Dictionary<string, List<string>>();

        public static void EncriptarReferencias()
        {

            Transposition.SetPermutationOrder();
            // Llamar a métodos de la clase TranspositionUtil
            dpiContentMap = Transposition.ReadFilesInFolder(@"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2\ingresolab5\inputs");

            // Encriptar los valores
            Transposition.EncriptarValores(dpiContentMap);

            // Mostrar el contenido encriptado
            Transposition.MostrarContenidoEncriptado(dpiContentMap);

            // Desencriptar los valores
            Transposition.DesencriptarValores(dpiContentMap);

            // Mostrar el contenido desencriptado
            Transposition.MostrarContenidoDesencriptado(dpiContentMap);

            Transposition.EncriptarValores(dpiContentMap);

        }

        static Dictionary<string, List<string>> dpiContentMapConversationConv = new Dictionary<string, List<string>>();
        static Dictionary<string, List<byte[]>> signatureMap = new Dictionary<string, List<byte[]>>();
        public static void EncriptarConversaciones()
        {

            Transposition.SetPermutationOrder();
            // Llamar a métodos de la clase TranspositionUtil
            dpiContentMapConversationConv = Transposition.ReadFilesInFolderConv(@"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2\ingresolab5\inputs");





            foreach (var conversation in dpiContentMapConversationConv)
            {
                string conversationId = conversation.Key;
                List<string> textsToSign = conversation.Value;
                List<byte[]> signatures = new List<byte[]>();

                foreach (string text in textsToSign)
                {
                    byte[] signature = GenerarFirmaUnica(text);
                    signatures.Add(signature);
                }

                signatureMap[conversationId] = signatures;
            }


            // Encriptar los valores
            Transposition.EncriptarValores(dpiContentMapConversationConv);

            // Mostrar el contenido encriptado
            Transposition.MostrarContenidoEncriptado(dpiContentMapConversationConv);

            // Desencriptar los valores
            Transposition.DesencriptarValores(dpiContentMapConversationConv);

            // Mostrar el contenido desencriptado
            Transposition.MostrarContenidoDesencriptado(dpiContentMapConversationConv);

            Transposition.EncriptarValores(dpiContentMapConversationConv);

        }

        public static KeyValuePair<string, List<byte[]>> BuscarStringEnDiccionario(Dictionary<string, List<byte[]>> dictionary, string valorBuscado)
        {
            foreach (var kvp in dictionary)
            {
                string clave = kvp.Key;

                if (clave == valorBuscado)
                {
                    return kvp; // Devuelve la clave y la lista de valores si se encuentra el valor buscado.
                }
            }

            // Si no se encuentra el valor, puedes devolver un valor predeterminado o lanzar una excepción, según tus necesidades.
            return new KeyValuePair<string, List<byte[]>>(null, null);
        }


        static void Main(string[] args)
        {


            EncriptarReferencias();
            EncriptarConversaciones();



            //Console.Write(signatureMap);


            Dictionary<BitArray, HuffmanTree> encripted = new Dictionary<BitArray, HuffmanTree>();

            List<string> listC = new List<string>();

            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            List<Person> personas = new List<Person>();
            BTree<Person> b = new BTree<Person>(250);

            string filePath = @"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2\ingresolab5\input.csv";

            CsvReader csvReader = new CsvReader();
            List<List<string>> csvData = csvReader.ReadCsv(filePath);

            // Ahora puedes trabajar con los datos del archivo CSV.
            foreach (List<string> row in csvData)
            {
                listA.Add(row[0]);
                listB.Add(row[1]);


                /*foreach (string cell in row)
                {
                    var line = cell;
                    var values = line.Split(';');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }*/



            }
            foreach (var item in listB)
            {
                Person input = JsonConvert.DeserializeObject<Person>(item)!;
                //Console.WriteLine($"Nombre: {input.Name}" + " " + $"Id: {input.Id}" + " " + $"Fecha: {input.BirthDate}" + " " + $"Direccion: {input.Address}" + " ");
                //Console.WriteLine("----------------------");
                //Console.WriteLine("");
                personas.Add(input);
            }



            for (int i = 0; i < listA.Count; i++)
            {
                //Console.WriteLine("Accion: " + listA[i] + " " + $"Nombre: {personas[i].Name}" + " " + $"Id: {personas[i].Id}" + " " + $"Fecha: {personas[i].BirthDate}" + " " + $"Direccion: {personas[i].Address}" + " ");
                if (listA[i] == "INSERT")
                {
                    b.Insert(personas[i]);
                    //b.PrintTreeGraph(@"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2");
                }
                else if (listA[i] == "PATCH")
                {

                    b.UpdatePersonInfo(personas[i].Id, personas[i].Address, personas[i].BirthDate, personas[i].Company, personas[i].Reclutador);

                }
                else if (listA[i] == "DELETE")
                {
                    b.RemovePerson(personas[i].Id);

                    //b.Remove(personas[i]);
                    //b.PrintTreeGraph(@"C:\Users\Rolando Ziller\Documents\Universidad\2023\segundo ciclo\edd2");
                }


            }

            foreach (Person person in personas)
            {
                if (person.Company != null)
                {
                    foreach (var item in person.Company)
                    {
                        if (!listC.Contains(item))
                        {
                            listC.Add(item);
                        }

                    }
                }

            }



            //Console.WriteLine(listC);
            //Console.WriteLine("");



            b.code();

            string company = "";

            bool showMenu;
            bool showMenu2;
            bool showMenu3;
            bool showMenu4;
            bool showMenu5;
            //bool showMenu6;
            bool showMenuPrincipal = true;
            Console.Clear();


            Dictionary<string, string> empresas = new Dictionary<string, string>
        {
            { "Shields, Ortiz and Schroeder", "123" },
            { "Empresa2", "456" },
            // Agrega más empresas y contraseñas aquí
        };

            bool loggedIn = false;
            string empresaElegida = "";
            string nombre = "";

            Files files = new Files();
            PasswordGiver contras = new PasswordGiver();
            String csvFile = @"C:\\Users\\Rolando Ziller\\Documents\\Universidad\\2023\\segundo ciclo\\edd2\\ingresolab5\\input.csv";
            List<Candidates> aplicantes = files.CargarAplicantes(csvFile);
            HashSet<Password> credenciales = contras.ProvideCredentials(aplicantes);

            while (true)
            {
                Console.Clear();
                if (!loggedIn)
                {
                    Console.WriteLine("Choose company:");
                    for (int i = 0; i < listC.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}) {listC[i]}");
                    }

                    Console.WriteLine($"Please enter an id between 1 and {listC.Count}.");
                    /*Console.WriteLine("Choose company:");
                    Console.WriteLine("");
                    for (int i = 0; i < listC.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}) {listC[i]}");
                    }
                    Console.WriteLine("");*/
                    if (int.TryParse(Console.ReadLine(), out int opcion) && opcion >= 1 && opcion <= listC.Count)
                    {
                        empresaElegida = listC.ElementAt(opcion - 1);

                        Console.Write("Ingrese su nombre: ");
                         nombre = Console.ReadLine();

                        Console.Write("Ingrese su contraseña: ");
                        string contraseña = Console.ReadLine();

                        bool validation = contras.ValidateCredentials(credenciales, nombre, empresaElegida, contraseña);

                        if (validation)
                        {
                            Console.Clear();
                            //Console.WriteLine("¡Bienvenido a " + empresaElegida + ", " + nombre + "!");
                            loggedIn = true;
                        }
                        else
                        {
                            Console.WriteLine("Credenciales incorrectas. Por favor, inténtelo de nuevo.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                      
                    }
                }
                else
                {


                    Console.Clear();
                    Console.WriteLine("Logueado en " + empresaElegida);
                    Console.WriteLine("");
                    Console.WriteLine("Choose an option: ");
                    Console.WriteLine("1) Search person");
                    Console.WriteLine("2) Show B Tree");
                    Console.WriteLine("3) Logout");
                    Console.Write("\r\nEnter an option: ");
                    Console.Write("");

                    switch (Console.ReadLine())
                    {

                        case "1":

                            showMenu5 = true;

                            while (showMenu5)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Enter id or type exit: ");

                                string userIdSearch = Console.ReadLine();

                                if (userIdSearch == "exit")
                                {
                                    break;
                                }
                                else
                                {


                                    Person personaResultante = new Person();
                                    long searchId = long.Parse(userIdSearch);

                                    Console.WriteLine("");
                                    //Console.WriteLine("Bitacora:");


                                    //Console.WriteLine("");
                                    //personaResultante = b.SearchById(searchId);
                                    //b.decoded(personaResultante);

                                    //if (personaResultante != null)
                                    //{
                                    //Console.Write($"Nombre: {b.SearchById(searchId).Name}, DPI: {b.SearchById(searchId).Id}, Fecha de Nacimiento: {b.SearchById(searchId).BirthDate}, Dirección: {b.SearchById(searchId).Address}");
                                    //Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMap, b.SearchById(searchId).Id.ToString());
                                    //}

                                    //Console.WriteLine("");


                                    //Console.WriteLine("");
                                    personaResultante = null;

                                    SearchResult searchResult = new SearchResult();

                                    //Console.WriteLine("");
                                    //Console.WriteLine("Bitacora:");

                                    //Console.WriteLine("");

                                    personaResultante = b.SearchById(searchId);
                                    //b.decoded(personaResultante);

                                    if (personaResultante != null)
                                    {
                                        searchResult.Nombre = b.SearchById(searchId).Name;
                                        searchResult.DPI = b.SearchById(searchId).Id;
                                        searchResult.FechaNacimiento = b.SearchById(searchId).BirthDate;
                                        searchResult.Direccion = b.SearchById(searchId).Address;

                                        List<string> companies = b.decoded(personaResultante);
                                        searchResult.Company = companies;

                                        List<string> dpiContent = Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMap, b.SearchById(searchId).Id.ToString());


                                        searchResult.Letter = dpiContent;

                                        List<string> dpiContentConv = Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMapConversationConv, b.SearchById(searchId).Id.ToString());


                                        searchResult.Conversation = dpiContentConv;


                                        string jsonResult = JsonConvert.SerializeObject(searchResult, Formatting.Indented);


                                        Console.WriteLine(jsonResult);

                                        bool validateMenu = true;

                                        while (validateMenu)
                                        {
                                            Console.WriteLine("");
                                            Console.WriteLine("Choose an option: ");
                                            Console.WriteLine("1. Validate sign");
                                            Console.WriteLine("2. Exit");
                                            Console.Write("\r\nEnter an option: ");
                                            Console.Write("");

                                            string choice = Console.ReadLine();

                                            switch (choice)
                                            {
                                                case "1":

                                                    var resultado = BuscarStringEnDiccionario(signatureMap, searchResult.DPI.ToString());

                                                    if (resultado.Key != null)
                                                    {




                                                        for (int i = 0; i < resultado.Value.Count; i++) // Bucle externo
                                                        {

                                                            byte[] firstValue = resultado.Value[i];
                                                            //bool isSignatureValid = RSA.VerifySignature(dpiContentConv[i]+"hacked", firstValue, keyPair.PublicKey, keyPair.N);
                                                            bool isSignatureValid = RSA.VerifySignature(dpiContentConv[i], firstValue, keyPair.PublicKey, keyPair.N);
                                                            string textValidate = "";
                                                            if (isSignatureValid)
                                                            {
                                                                textValidate = "valid";
                                                            }
                                                            else
                                                            {
                                                                textValidate = "invalid";
                                                            }
                                                            Console.Write("");
                                                            Console.WriteLine($"Signature for conversation {i + 1} is: {textValidate}");

                                                        }

                                                        validateMenu = false;
                                                        break;

                                                    }





                                                    break;

                                                case "2":
                                                    validateMenu = false; // Salir del menú de validación de firmas
                                                    break;

                                                default:
                                                    Console.WriteLine("Opción no válida. Por favor, elige 1 o 2.");
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No records found");
                                    }

                                    Console.WriteLine("");

                                }
                            }



                            break;

                        case "2":
                            Console.WriteLine("");
                            Console.WriteLine("B Tree: ");
                            Console.WriteLine("");
                            b.ShowRec(dpiContentMap, dpiContentMapConversationConv, nombre);
                            Console.WriteLine("");
                            Console.WriteLine("Type exit to return to menu");
                            Console.WriteLine("");

                            string userExit = Console.ReadLine();

                            if (userExit == "exit")
                            {
                                break;
                            }
                            break;
                        case "3":
                            loggedIn = false;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Por favor, elige 1 o 3.");
                            break;
                    }

                }
            }



            /*
            while (showMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("Choose an option: ");
                Console.WriteLine("1) Search person");
                Console.WriteLine("2) Show B Tree");
                Console.Write("\r\nEnter an option: ");
                Console.Write("");

                switch (Console.ReadLine())
                {
                    
                    case "1":

                        showMenu5 = true;

                        while (showMenu5)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Enter id or type exit: ");

                            string userIdSearch = Console.ReadLine();

                            if (userIdSearch == "exit")
                            {
                                break;
                            }
                            else
                            {


                                Person personaResultante = new Person();
                                long searchId = long.Parse(userIdSearch);

                                Console.WriteLine("");
                                //Console.WriteLine("Bitacora:");


                                //Console.WriteLine("");
                                //personaResultante = b.SearchById(searchId);
                                //b.decoded(personaResultante);

                                //if (personaResultante != null)
                                //{
                                //Console.Write($"Nombre: {b.SearchById(searchId).Name}, DPI: {b.SearchById(searchId).Id}, Fecha de Nacimiento: {b.SearchById(searchId).BirthDate}, Dirección: {b.SearchById(searchId).Address}");
                                //Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMap, b.SearchById(searchId).Id.ToString());
                                //}

                                //Console.WriteLine("");


                                //Console.WriteLine("");
                                personaResultante = null;

                                SearchResult searchResult = new SearchResult();

                                //Console.WriteLine("");
                                //Console.WriteLine("Bitacora:");

                                //Console.WriteLine("");

                                personaResultante = b.SearchById(searchId);
                                //b.decoded(personaResultante);

                                if (personaResultante != null)
                                {
                                    searchResult.Nombre = b.SearchById(searchId).Name;
                                    searchResult.DPI = b.SearchById(searchId).Id;
                                    searchResult.FechaNacimiento = b.SearchById(searchId).BirthDate;
                                    searchResult.Direccion = b.SearchById(searchId).Address;

                                    List<string> companies = b.decoded(personaResultante);
                                    searchResult.Company = companies;

                                    List<string> dpiContent = Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMap, b.SearchById(searchId).Id.ToString());


                                    searchResult.Letter = dpiContent;

                                    List<string> dpiContentConv = Transposition.MostrarContenidoDesencriptadoDPI(dpiContentMapConversationConv, b.SearchById(searchId).Id.ToString());


                                    searchResult.Conversation = dpiContentConv;


                                    string jsonResult = JsonConvert.SerializeObject(searchResult, Formatting.Indented);


                                    Console.WriteLine(jsonResult);

                                    bool validateMenu = true;

                                    while (validateMenu)
                                    {
                                        Console.WriteLine("");
                                        Console.WriteLine("Choose an option: ");
                                        Console.WriteLine("1. Validate sign");
                                        Console.WriteLine("2. Exit");
                                        Console.Write("\r\nEnter an option: ");
                                        Console.Write("");

                                        string choice = Console.ReadLine();

                                        switch (choice)
                                        {
                                            case "1":

                                                var resultado = BuscarStringEnDiccionario(signatureMap, searchResult.DPI.ToString());

                                                if (resultado.Key != null)
                                                {




                                                    for (int i = 0; i < resultado.Value.Count; i++) // Bucle externo
                                                    {

                                                        byte[] firstValue = resultado.Value[i];
                                                        //bool isSignatureValid = RSA.VerifySignature(dpiContentConv[i]+"hacked", firstValue, keyPair.PublicKey, keyPair.N);
                                                        bool isSignatureValid = RSA.VerifySignature(dpiContentConv[i], firstValue, keyPair.PublicKey, keyPair.N);
                                                        string textValidate = "";
                                                        if (isSignatureValid)
                                                        {
                                                            textValidate = "valid";
                                                        }
                                                        else
                                                        {
                                                            textValidate = "invalid";
                                                        }
                                                        Console.Write("");
                                                        Console.WriteLine($"Signature for conversation {i + 1} is: {textValidate}");

                                                    }

                                                    validateMenu = false;
                                                    break;

                                                }





                                                break;

                                            case "2":
                                                validateMenu = false; // Salir del menú de validación de firmas
                                                break;

                                            default:
                                                Console.WriteLine("Opción no válida. Por favor, elige 1 o 2.");
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No records found");
                                }

                                Console.WriteLine("");

                            }
                        }



                        break;

                    case "2":
                        Console.WriteLine("");
                        Console.WriteLine("B Tree: ");
                        Console.WriteLine("");
                        b.ShowRec(dpiContentMap, dpiContentMapConversationConv);
                        Console.WriteLine("");
                        Console.WriteLine("Type exit to return to menu");
                        Console.WriteLine("");

                        string userExit = Console.ReadLine();

                        if (userExit == "exit")
                        {
                            break;
                        }
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Por favor, elige 1 o 2.");
                        break;
                }
            }*/

        }
    }
}

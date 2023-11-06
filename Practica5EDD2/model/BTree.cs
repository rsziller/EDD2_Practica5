using Newtonsoft.Json;
using Practica5EDD2.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Practica5EDD2.model
{

    public class BTree<T> where T : IComparable<T>
    {
        private static int Z;
        Dictionary<BitArray, HuffmanTree> encripted = new Dictionary<BitArray, HuffmanTree>();

        public class Node
        {
            public int n;
            public Person[] key = new Person[2 * Z - 1];
            public Node[] child = new Node[2 * Z];
            public bool leaf = true;

            public int Find(long k)
            {
                for (int i = 0; i < this.n; i++)
                {
                    if (this.key[i].Id == k)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        private Node root;

        public BTree(int t)
        {
            Z = t;
            root = new Node();
            root.n = 0;
            root.leaf = true;
        }

        private Node Search(Node x, long key)
        {
            int i = 0;
            if (x == null)
                return x;
            for (i = 0; i < x.n; i++)
            {
                if (key.CompareTo(x.key[i].Id) < 0)
                {
                    break;
                }
                if (key.CompareTo(x.key[i].Id) == 0)
                {
                    return x;
                }
            }
            if (x.leaf)
            {
                return null;
            }
            else
            {
                
                return Search(x.child[i], key);
            }
        }

        private void Split(Node x, int pos, Node y)
        {
            Node z = new Node();
            z.leaf = y.leaf;
            z.n = Z - 1;
            for (int j = 0; j < Z - 1; j++)
            {
                z.key[j] = y.key[j + Z];
            }
            if (!y.leaf)
            {
                for (int j = 0; j < Z; j++)
                {
                    z.child[j] = y.child[j + Z];
                }
            }
            y.n = Z - 1;
            for (int j = x.n; j < pos; j--)
            {
                x.child[j + 1] = x.child[j];
            }
            x.child[pos + 1] = z;

            for (int j = x.n; j < pos; j--)
            {
                x.key[j + 1] = x.key[j];
            }
            x.key[pos] = y.key[Z - 1];
            x.n++;
        }

        public void Insert(Person key)
        {
            Node r = root;
            if (r.n == (2 * Z - 1))
            {
                Node s = new Node();
                root = s;
                s.leaf = false;
                s.n = 0;
                s.child[0] = r;
                Split(s, 0, r);
                InsertValue(s, key);
            }
            else
            {
                InsertValue(r, key);
            }
        }

        /*private void InsertValue(Node x, Person k)
        {
            if (x.leaf)
            {
                int i;
                for (i = x.n - 1; i >= 0 && k.CompareTo(x.key[i]) < 0; i--)
                {
                    x.key[i + 1] = x.key[i];
                }
                x.key[i + 1] = k;
                x.n = x.n + 1;
            }
            else
            {
                int i;
                for (i = x.n - 1; i >= 0 && k.CompareTo(x.key[i]) < 0; i--)
                {
                }
                i++;
                Node tmp = x.child[i];
                if (tmp.n == 2 * Z - 1)
                {
                    Split(x, i, tmp);
                    if (k.CompareTo(x.key[i]) > 0)
                    {
                        i++;
                    }
                }
                InsertValue(x.child[i], k);
            }
        }*/

        public void InsertValue(Node x, Person k)
        {
            if (x.leaf)
            {
                int i = x.n;
                while (i >= 1 && k.CompareTo(x.key[i - 1]) < 0)
                {
                    x.key[i] = x.key[i - 1];
                    i--;
                }
                x.key[i] = k;
                x.n++;
            }
            else
            {
                int j = 0;
                while (j < x.n && k.CompareTo(x.key[j]) > 0)
                {
                    j++;
                }
                if (x.child[j].n == (2 * Z - 1))
                {
                    // (U - máximo de claves)
                    Split(x, j, x.child[j]);
                    if (k.CompareTo(x.key[j]) > 0)
                    {
                        j++;
                    }
                }
                InsertValue(x.child[j], k);
            }
        }

        public void Show()
        {
            Show(root);
        }

        public void ShowRec(Dictionary<string, List<string>> dpiContentMap, Dictionary<string, List<string>> dpiContentMapConversationConv, string reclutador)
        {
            ShowRec(root, dpiContentMap, dpiContentMapConversationConv, reclutador);
        }

        public void code()
        {
            int i;
            Node x = root;
            for (i = 0; i < x.n; i++)
            {
 

                for (int j = 0; j < x.key[i].Company.Length; j++)
                {

                    string input = x.key[i].Name + " " + x.key[i].Company[j];
                    HuffmanTree huffmanTree = new HuffmanTree();

                    // Build the Huffman tree
                    huffmanTree.Build(input);

                    // Encode
                    BitArray encoded = huffmanTree.Encode(input);
                    encripted.Add(encoded, huffmanTree);
                    string result = BitArrayToString(encoded);
                    x.key[i].Company[j] = result; // Multiplicar cada elemento por 2
                }

            }

        }



        private void ShowRec(Node x, Dictionary<string, List<string>> dpiContentMap, Dictionary<string, List<string>> dpiContentMapConversationConv, string reclutador)
        {
            int i;
            for (i = 0; i < x.n; i++)
            {
                if (x.key[i].Reclutador.Equals(reclutador))
                {
                    var jsonData = new
                    {
                        Name = x.key[i].Name,
                        Id = x.key[i].Id,
                        BirthDate = x.key[i].BirthDate,
                        Address = x.key[i].Address,
                        Company = x.key[i].Company,
                        Letter = new List<string>(),
                        Conversation = new List<string>()
                    };

                    if (dpiContentMap.ContainsKey(x.key[i].Id.ToString()))
                    {
                        jsonData.Letter.AddRange(dpiContentMap[x.key[i].Id.ToString()]);
                    }

                    if (dpiContentMapConversationConv.ContainsKey(x.key[i].Id.ToString()))
                    {
                        jsonData.Conversation.AddRange(dpiContentMapConversationConv[x.key[i].Id.ToString()]);
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(jsonData, Formatting.Indented));



                    Console.WriteLine("");

                }
                
            }

            if (!x.leaf)
                ShowRec(x.child[i], dpiContentMap, dpiContentMapConversationConv, reclutador);
        }

        private void ShowRec1(Node x, Dictionary<string, List<string>> dpiContentMap)
        {



            int i;
            for (i = 0; i < x.n; i++)
            {
                Console.WriteLine("");
                if (!x.leaf)
                    ShowRec1(x.child[i], dpiContentMap);
                //Console.Write(" " + x.key[i].Name + " " + x.key[i].Id + " " + x.key[i].BirthDate + " " + x.key[i].Address + " ");
                Console.Write(" " + x.key[i].Name + " " + x.key[i].Id + " " + x.key[i].BirthDate + " " + x.key[i].Address + " ");
                //Console.Write(x.key[i].Id);

                Console.WriteLine("");

                Transposition.MostrarContenidoEncriptadoDPI(dpiContentMap, x.key[i].Id.ToString());

                /*for (int j = 0; j < x.key[i].Company.Length; j++)
                {

                    string input = x.key[i].Name + " " + x.key[i].Company[j];
                    HuffmanTree huffmanTree = new HuffmanTree();

                    // Build the Huffman tree
                    huffmanTree.Build(input);

                    // Encode
                    BitArray encoded = huffmanTree.Encode(input);
                    encripted.Add(encoded, huffmanTree);
                    string result = BitArrayToString(encoded);
                    x.key[i].Company[j] = result; // Multiplicar cada elemento por 2
                }*/

                foreach (var item in x.key[i].Company)
                {

                    Console.WriteLine(" " + item);
                }
                Console.WriteLine("");
            }

            if (!x.leaf)
                ShowRec1(x.child[i], dpiContentMap);
        }

        private void Show(Node x)
        {

            

            int i;
            for (i = 0; i < x.n; i++)
            {
                Console.WriteLine("");
                if (!x.leaf)
                    Show(x.child[i]);
                //Console.Write(" " + x.key[i].Name + " " + x.key[i].Id + " " + x.key[i].BirthDate + " " + x.key[i].Address + " ");
                Console.Write(" " + x.key[i].Name + " " + x.key[i].Id + " " + x.key[i].BirthDate + " " + x.key[i].Address + " ");
                //Console.Write(x.key[i].Id);

                Console.WriteLine("");

                

                /*for (int j = 0; j < x.key[i].Company.Length; j++)
                {

                    string input = x.key[i].Name + " " + x.key[i].Company[j];
                    HuffmanTree huffmanTree = new HuffmanTree();

                    // Build the Huffman tree
                    huffmanTree.Build(input);

                    // Encode
                    BitArray encoded = huffmanTree.Encode(input);
                    encripted.Add(encoded, huffmanTree);
                    string result = BitArrayToString(encoded);
                    x.key[i].Company[j] = result; // Multiplicar cada elemento por 2
                }*/

                foreach (var item in x.key[i].Company)
                {
                   
                    Console.WriteLine(" " + item);
                }
                Console.WriteLine("");
            }

            if (!x.leaf)
                Show(x.child[i]);
        }

        static string BitArrayToString(BitArray bits)
        {
            // Check for empty BitArray
            if (bits.Count == 0)
                return string.Empty;

            char[] chars = new char[bits.Count];

            for (int i = 0; i < bits.Count; i++)
            {
                // Use '1' for true and '0' for false
                chars[i] = bits[i] ? '1' : '0';
            }

            // Create a string from the characters
            return new string(chars);
        }

        public bool Contains(long k)
        {
            if (Search(root, k) != null)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

        public bool FindCompanyPerson(string company, long dpi)
        {
          
           

            Node x = Search(root, dpi);
            if (x == null)
            {
                Console.WriteLine("No records found");
                return false;
            }

            
            int personIndex = x.Find(dpi);

            foreach (var item in x.key[personIndex].Company)
            {
                if (item == company)
                {
                    return true;
                }
                /*else
                {
                    return false;
                }*/
            }

            return false;
        }

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

        public Person SearchById(long id)
        {


            return SearchById(root, id);
        }

        public List<string> decoded(Person persona)
        {
            List<string> decodedCompanies = new List<string>();

            if (persona != null)
            {
                for (int j = 0; j < persona.Company.Length; j++)
                {
                    string binaryString = persona.Company[j];
                    BitArray bitArrayFromString = new BitArray(binaryString.Select(c => c == '1').ToArray());

                    int indexDecode = -1;
                    for (int i = 0; i < encripted.Count; i++)
                    {
                        bool areEqual = AreBitArraysEqual(bitArrayFromString, encripted.ElementAt(i).Key);

                        if (areEqual)
                        {
                            indexDecode = i;
                        }
                    }

                    if (indexDecode >= 0)
                    {
                        string decoded = encripted.ElementAt(indexDecode).Value.Decode(encripted.ElementAt(indexDecode).Key);
                        decodedCompanies.Add(RemoveFirstWord(decoded));
                    }
                    else
                    {
                        decodedCompanies.Add("No records found");
                    }
                }
            }
            else
            {
                decodedCompanies.Add("No records found");
            }

            return decodedCompanies;
        }

        public void decoded1(Person persona)
        {
            if (persona!= null)
            {
                for (int j = 0; j < persona.Company.Length; j++)
                {


                    string binaryString = persona.Company[j];
                    BitArray bitArrayFromString = new BitArray(binaryString.Select(c => c == '1').ToArray());

                    int indexDecode = -1;
                    for (int i = 0; i < encripted.Count; i++)
                    {
                        bool areEqual = AreBitArraysEqual(bitArrayFromString, encripted.ElementAt(i).Key);

                        if (areEqual)
                        {
                            indexDecode = i;



                        }

                    }


                    if (indexDecode >= 0)
                    {
                        string decoded = encripted.ElementAt(indexDecode).Value.Decode(encripted.ElementAt(indexDecode).Key);

                        //persona.Company[j] = decoded;
                        Console.WriteLine(RemoveFirstWord(decoded));
                    }
                    else
                    {
                        Console.WriteLine("No records found");
                    }

                }
            }
            else
            {
                Console.WriteLine("No records found");
            }


        }

        static string RemoveFirstWord(string input)
        {
            // Encuentra la posición del primer espacio
            int firstSpaceIndex = input.IndexOf(' ');

            // Si hay un espacio, elimina la primera palabra y el espacio
            if (firstSpaceIndex >= 0)
            {
                return input.Substring(firstSpaceIndex + 1);
            }

            // Si no hay espacio, devuelve una cadena vacía o la cadena original
            return string.Empty;
        }

        private Person SearchById(Node x, long id)
        {
            if (x == null)
                return null;

            for (int i = 0; i < x.n; i++)
            {
                // Verifica si el nombre de la persona coincide


                // Si el nodo no es una hoja, busca en los hijos recursivamente
                if (!x.leaf)
                {
                    SearchById(x.child[i], id);
                }

                if (x.key[i].Id == id)
                {
                   
                    return x.key[i];
                }
            }

            // Busca en el último hijo si no es una hoja
            if (!x.leaf)
            {
                SearchById(x.child[x.n], id);
            }

            return null;
        }



        public bool UpdatePersonInfo(long dpi, string newAddress = null, string newBirthDate = null, string[] newCompany = null, string newReclutador = null)
        {
          

            // Buscar la persona en el árbol

            Node nodeContainingPerson = Search(root, dpi);

            if (nodeContainingPerson == null)
            {
                // La persona no se encontró en el árbol
                Console.WriteLine("No records found" + dpi);
                return false;
            }

            int personIndex = nodeContainingPerson.Find(dpi);

            if (personIndex != -1)
            {
                // Actualizar la dirección y la fecha de nacimiento
                //nodeContainingPerson.key[personIndex].Address = newAddress;
                //nodeContainingPerson.key[personIndex].BirthDate = newBirthDate.Value;

                // Aquí puedes también actualizar otros campos si es necesario
                if (newAddress != null)
                {
                    nodeContainingPerson.key[personIndex].Address = newAddress;
                }

                //if (newBirthDate.HasValue)
                if (newBirthDate != null)
                {
                    //nodeContainingPerson.key[personIndex].BirthDate = newBirthDate.Value;
                    nodeContainingPerson.key[personIndex].BirthDate = newBirthDate;
                }

                if (newCompany != null)
                {
                    //nodeContainingPerson.key[personIndex].BirthDate = newBirthDate.Value;
                    nodeContainingPerson.key[personIndex].Company = newCompany;
                }

                if (newReclutador != null)
                {
                    //nodeContainingPerson.key[personIndex].BirthDate = newBirthDate.Value;
                    nodeContainingPerson.key[personIndex].Reclutador = newReclutador;
                }

                return true; // Actualización exitosa
            }

            return false; // La persona no se encontró en el nodo, no se pudo actualizar
        }

        private void Remove(Node x, long key)
        {
            int pos = x.Find(key);
            if (pos != -1)
            {
                if (x.leaf)
                {
                    int i = 0;
                    for (i = 0; i < x.n && x.key[i].Id != key; i++)
                    {
                    }
                    for (; i < x.n; i++)
                    {
                        if (i != 2 * Z - 2)
                        {
                            x.key[i] = x.key[i + 1];
                        }
                    }
                    x.n--;
                    return;
                }
                if (!x.leaf)
                {
                    Node pred = x.child[pos];
                    long predKey = 0;
                    if (pred.n >= Z)
                    {
                        for (; ; )
                        {
                            if (pred.leaf)
                            {
                                Console.WriteLine(pred.n);
                                predKey = pred.key[pred.n - 1].Id;
                                break;
                            }
                            else
                            {
                                pred = pred.child[pred.n];
                            }
                        }
                        Remove(pred, predKey);
                        x.key[pos].Id = predKey;
                        return;
                    }

                    Node nextNode = x.child[pos + 1];
                    if (nextNode.n >= Z)
                    {
                        long nextKey = nextNode.key[0].Id;
                        if (!nextNode.leaf)
                        {
                            nextNode = nextNode.child[0];
                            for (; ; )
                            {
                                if (nextNode.leaf)
                                {
                                    nextKey = nextNode.key[nextNode.n - 1].Id;
                                    break;
                                }
                                else
                                {
                                    nextNode = nextNode.child[nextNode.n];
                                }
                            }
                        }
                        Remove(nextNode, nextKey);
                        x.key[pos].Id = nextKey;
                        return;
                    }

                    int temp = pred.n + 1;
                    pred.key[pred.n++] = x.key[pos];
                    for (int i = 0, j = pred.n; i < nextNode.n; i++, j++)
                    {
                        pred.key[j] = nextNode.key[i];
                        pred.n++;
                    }
                    for (int i = 0; i < nextNode.n + 1; i++)
                    {
                        pred.child[temp++] = nextNode.child[i];
                    }

                    x.child[pos] = pred;
                    for (int i = pos; i < x.n; i++)
                    {
                        if (i != 2 * Z - 2)
                        {
                            x.key[i] = x.key[i + 1];
                        }
                    }
                    for (int i = pos + 1; i < x.n + 1; i++)
                    {
                        if (i != 2 * Z - 1)
                        {
                            x.child[i] = x.child[i + 1];
                        }
                    }
                    x.n--;
                    if (x.n == 0)
                    {
                        if (x == root)
                        {
                            root = x.child[0];
                        }
                        x = x.child[0];
                    }
                    Remove(pred, key);
                    return;
                }
            }
            else
            {
                for (pos = 0; pos < x.n; pos++)
                {
                    if (x.key[pos].Id > key)
                    {
                        break;
                    }
                }
                Node tmp = x.child[pos];
                if (tmp.n >= Z)
                {
                    Remove(tmp, key);
                    return;
                }
                if (true)
                {
                    Node nb = null;
                    long devider = -1;

                    if (pos != x.n && x.child[pos + 1].n >= Z)
                    {
                        devider = x.key[pos].Id;
                        nb = x.child[pos + 1];
                        x.key[pos] = nb.key[0];
                        tmp.key[tmp.n++].Id = devider;
                        tmp.child[tmp.n] = nb.child[0];
                        for (int i = 1; i < nb.n; i++)
                        {
                            nb.key[i - 1] = nb.key[i];
                        }
                        for (int i = 1; i <= nb.n; i++)
                        {
                            nb.child[i - 1] = nb.child[i];
                        }
                        nb.n--;
                        Remove(tmp, key);
                        return;
                    }
                    else if (pos != 0 && x.child[pos - 1].n >= Z)
                    {
                        devider = x.key[pos - 1].Id;
                        nb = x.child[pos - 1];
                        x.key[pos - 1] = nb.key[nb.n - 1];
                        Node child = nb.child[nb.n];
                        nb.n--;

                        for (int i = tmp.n; i > 0; i--)
                        {
                            tmp.key[i] = tmp.key[i - 1];
                        }
                        tmp.key[0].Id = devider;
                        for (int i = tmp.n + 1; i > 0; i--)
                        {
                            tmp.child[i] = tmp.child[i - 1];
                        }
                        tmp.child[0] = child;
                        tmp.n++;
                        Remove(tmp, key);
                        return;
                    }
                    else
                    {
                        Node lt = null;
                        Node rt = null;
                        bool last = false;
                        if (pos != x.n)
                        {
                            devider = x.key[pos].Id;
                            lt = x.child[pos];
                            rt = x.child[pos + 1];
                        }
                        else
                        {
                            devider = x.key[pos - 1].Id;
                            rt = x.child[pos];
                            lt = x.child[pos - 1];
                            last = true;
                            pos--;
                        }
                        for (int i = pos; i < x.n - 1; i++)
                        {
                            x.key[i] = x.key[i + 1];
                        }
                        for (int i = pos + 1; i < x.n; i++)
                        {
                            x.child[i] = x.child[i + 1];
                        }
                        x.n--;
                        lt.key[lt.n++].Id = devider;

                        for (int i = 0, j = lt.n; i < rt.n + 1; i++, j++)
                        {
                            if (i < rt.n)
                            {
                                lt.key[j] = rt.key[i];
                            }
                            lt.child[j] = rt.child[i];
                        }
                        lt.n += rt.n;
                        if (x.n == 0)
                        {
                            if (x == root)
                            {
                                root = x.child[0];
                            }
                            x = x.child[0];
                        }
                        Remove(lt, key);
                        return;
                    }
                }
            }
        }

        public bool RemovePerson(long dpi)
        {
          

            Node x = Search(root, dpi);
            if (x == null)
            {
                Console.WriteLine("No records found");
                return false;
            }

            //Console.WriteLine("vamos a borrar a: " + removedPerson.Id + removedPerson.Name);
            int personIndex = x.Find(dpi);
            Remove(root, x.key[personIndex].Id);
            return true;
        }
    }
}

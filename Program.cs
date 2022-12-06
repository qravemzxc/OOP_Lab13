using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Lab13
{
   
    interface INobBall
    {
        void Ball();
    }
    [Serializable]
    public class Production
    {
        public string Product { get; set; }
        public Production(string production)
        {
            Product = production;
        }
        public Production() { }
    }

    [Serializable]
    abstract public class Nob
    {
        public abstract void Ball();
    }
    [Serializable]
    public class NameOfBall : Nob, INobBall
    {

        public string Name { get; set; }
        public int Size { get; set; }
        [NonSerialized]
        public string Material;
        public NameOfBall(string name, int size,string material)
        {
            Name = name;
            Size = size;
            Material = material;
        }
        public NameOfBall() { }
        public override void Ball()
        {
            Console.WriteLine("Баскетбольный мяч");
        }
        void INobBall.Ball()
        {
            Console.WriteLine("Тенисный мяч");
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            NameOfBall nameofball = new NameOfBall("playing", 10,"rubber");
            BinaryFormatter formatter = new BinaryFormatter();
            Console.WriteLine("Работа с Binary:");
                   using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.bin", FileMode.OpenOrCreate))
                   {
                     formatter.Serialize(fs, nameofball);
                     Console.WriteLine("Обьект сериализован");
                   };
                   using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.bin", FileMode.OpenOrCreate))
                   {
                     NameOfBall newNameOfball = (NameOfBall)formatter.Deserialize(fs);
                     Console.WriteLine("Обьект десериализован");
                   };
            Console.WriteLine();
            Console.WriteLine("Работа с Json:");
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.json", FileMode.OpenOrCreate))
            {
               
                await JsonSerializer.SerializeAsync<NameOfBall>(fs, nameofball);
                Console.WriteLine("Обьект сериализован");
            }
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.json", FileMode.OpenOrCreate))
            {
                NameOfBall? ball = await JsonSerializer.DeserializeAsync<NameOfBall>(fs);
                Console.WriteLine($"Name: {ball?.Name}  Size: {ball?.Size}");
            }
            Console.WriteLine();
            Console.WriteLine("Работа с XML:");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(NameOfBall));
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, nameofball);
                Console.WriteLine("Обьект сериализован");
            }
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\serialization.xml", FileMode.OpenOrCreate))
            {
                NameOfBall? ball = xmlSerializer.Deserialize(fs) as NameOfBall;
                Console.WriteLine($"Name: {ball?.Name}  Size: {ball?.Size}");
            }
            Console.WriteLine();
            Production[] Ball = new Production[]
            {
                new Production("adidas"),
                new Production("nike")
            };
            XmlSerializer formatter1 = new XmlSerializer(typeof(Production[]));
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\objects.xml", FileMode.OpenOrCreate))
            {
                formatter1.Serialize(fs, Ball);
            }
            using (FileStream fs = new FileStream("C:\\Пацей\\Lab13\\Lab13\\objects.xml", FileMode.OpenOrCreate))
            {
                Production[]? newball = formatter1.Deserialize(fs) as Production[];

                if (newball != null)
                {
                    foreach (Production ball in newball)
                    {
                        Console.WriteLine($"Production: {ball.Product}");
                    }
                }
            }
            Console.WriteLine();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("C:\\Пацей\\Lab13\\Lab13\\serialization.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            XmlNodeList? personNodes = xRoot?.SelectNodes("*");
            if (personNodes is not null)
            {
                foreach (XmlNode node in personNodes)
                    Console.WriteLine(node.OuterXml);
            }
            Console.WriteLine();
            Console.WriteLine("Первый запрос");
            XDocument xdoc = XDocument.Load("C:\\Пацей\\Lab13\\Lab13\\company.xml");
            var comp = xdoc.Element("companies") ?
                  .Elements("company")
                  .Where(p => p.Element("place")?.Value == "USA")
                  .Select(p => new       
                    {
                        name = p.Attribute("name")?.Value,
                        place = p.Element("place")?.Value,
                        found = p.Element("base")?.Value
                    });

            if (comp is not null)
            {
                foreach (var c in comp)
                {
                    Console.WriteLine($"Name: {c.name}  Base: {c.found}");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Второй запрос");
            var comp1 = xdoc.Element("companies")?   
            .Elements("company")
            .Where(p => p.Attribute("name")?.Value == "Google")
            .Select(p => new
            {
                name = p.Attribute("name")?.Value,
                place = p.Element("place")?.Value,
                found = p.Element("base")?.Value
            });

            if (comp1 is not null)
            {
                foreach (var c in comp1)
                {
                    Console.WriteLine($"Name: {c.name}  Base: {c.found}  Plae:{c.place}");
                }
            }
        }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class Serializer
{
    private const string FILE_NAME = "data.json";
    public void Serialize(List<Item> items)
    {
        using (StreamWriter sw = new StreamWriter(FILE_NAME))
        {
            string data = JsonConvert.SerializeObject(items);
            sw.Write(data);
        }
    }

    public List<Item> Deserialize()
    {
        if (!File.Exists(FILE_NAME)) return null;

        List<Item> items = null;
        using (StreamReader sr = new StreamReader(FILE_NAME))
        {
            string data = sr.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Item>>(data);
        }
        return items;
    }
}

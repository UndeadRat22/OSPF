using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace OSPF.Settings
{
    public class RouterSettings
    {
        public static RouterSettings Settings { get; set; }

        public List<string> Routers { get; set; }
        public List<Link> Links { get; set; }

        public static void LoadSettings(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string raw = reader.ReadToEnd();
                Settings = JsonConvert.DeserializeObject<RouterSettings>(raw);
            }
        }
    }
}

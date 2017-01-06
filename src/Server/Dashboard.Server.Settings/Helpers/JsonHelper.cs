using System;
using System.IO;
using Newtonsoft.Json;

namespace Dashboard.Server.Settings.Helpers
{
    public class JsonHelper
    {
        #region public methods

        public static void SaveJson<T>(T model, string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path))
                {
                    var json =  JsonConvert.SerializeObject(model);
                    file.Write(json);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public static T GetModel<T>(string path, T model)
        {
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    var json = file.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        #endregion public methods

    }
}
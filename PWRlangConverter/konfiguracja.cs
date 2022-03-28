using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

namespace PWRlangConverter
{
    

    public class konfiguracja
    {
        public string autoWprowadzanieNazwPlikowWejsciowych { get; set; }
        public string domyslnaNazwaPlikuTXTZTransifexCOM { get; set; }
        public string domyslnaNazwaPlikukeysTransifexCOMTXT { get; set; }
        public string domyslnaNazwaPlikustringsTransifexCOMTXT { get; set; }
        public string domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON { get; set; }

        const string skrypt = "konfiguracja.cs";

        public static void WygenerujDomyslnyPlikKonfiguracyjnyJesliNieIstnieje()
        {
            if (File.Exists("cfg.json") == false)
            {
                FileStream plikCFGdomyslny_fs = new FileStream("cfg.json", FileMode.Create, FileAccess.Write);
                StreamWriter plikCFGdomyslny_sw = new StreamWriter(plikCFGdomyslny_fs);

                //DOMYŚLNE WARTOŚCI KONFIGURACJI, GDY BRAK PLIKU CFG.JSON W FOLDERZE Z PROGRAMEM
                plikCFGdomyslny_sw.WriteLine("{");

                plikCFGdomyslny_sw.WriteLine("      \"autoWprowadzanieNazwPlikowWejsciowych\": \"0\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikuTXTZTransifexCOM\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikukeysTransifexCOMTXT\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikustringsTransifexCOMTXT\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON\": \"\",");

                plikCFGdomyslny_sw.WriteLine("      \"__domyslnyPlikKonfiguracyjny\": \"1\"");
                plikCFGdomyslny_sw.WriteLine("}");
                plikCFGdomyslny_sw.Close();
                plikCFGdomyslny_fs.Close();

            }
        }

        public static konfiguracja WczytajDaneKonfiguracyjne()
        {

            if (File.Exists("cfg.json") == true)
            {
                return JSON.WczytajStaleIIchWartosciZPlikuJSON_v2("cfg.json");
            }
            else
            {
                return null;
            }

        }

    }


}

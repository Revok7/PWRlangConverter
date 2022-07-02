﻿//UWAGA: NIE ZMIENIAĆ NUMERÓW OPERACJI, ZE WZGLĘDU NA ZAIMPLEMENTOWANE MAKRA OD WERSJI v.1.7

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

using System.Diagnostics;
using System.ComponentModel;

using Goblinfactory.ProgressBar.moddedbyRevok;

namespace PWRlangConverter
{
    class PWRlangConverter
    {
        readonly static bool wl_pasekpostepu = false; //!!!wlaczenie tej opcji znacznie wydłuża wykonywanie operacji!!!
        const string skrypt = "PWRlangConverter.cs";
        static public string folderglownyprogramu = Directory.GetCurrentDirectory();
        const string nazwafolderutmp = "tmp";

        public static konfiguracja cfg;

        static DateTime aktualny_czas = DateTime.Now;

        static List<string> makro_operacje_lista = new List<string>();
        static int makro_aktualny_indeks_listy;
        static bool makro_aktywowane;
        static bool makro_pomyslnezakonczenieoperacjinr2 = false;
        static bool makro_pomyslnezakonczenieoperacjinr100 = false;
        static List<string> makro_bledy_lista = new List<string>(); //element listy: "makro_numeroperacjiwkolejnosci;komunikat_obledzie"
        static List<string> makro_sukcesy_lista = new List<string>(); //element listy: "makro_numeroperacjiwkolejnosci;komunikat_osukcesie"

        static int tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor;
        static string tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy; //numer porządkowy bazy lub aktualizacji, np.: #1, #2 itd.
        static int tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor;
        static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP;
        static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii = 1;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP;

        static uint tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii = 1;
        static string tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow;
        static int tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP;
        static List<string> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP;
        static List<int> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD;
        static List<int> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO;

        static ProgressBar pasek_postepu;

        private static void InicjalizacjaPaskaPostepu(int ilosc_wszystkich_operacji)
        {
            pasek_postepu = new ProgressBar(PbStyle.DoubleLine, ilosc_wszystkich_operacji, 100, '■');
        }

        private static void Blad(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces2(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }

        private static string PoliczPostepWProcentach(double aktualna_linia, double wszystkich_linii)
        {
            double rezultat = (aktualna_linia / wszystkich_linii) * 100;

            return Math.Round(rezultat, 0).ToString();
        }

        private static string PobierzTimestamp(DateTime wartosc)
        {
            return wartosc.ToString("yyyyMMddHHmmss");
        }

        private static bool CzyParsowanieINTUdane(string wartosc)
        {
            bool rezultat_bool = false;
            int rezultat_int = -1;

            if (int.TryParse(wartosc, out rezultat_int))
            {
                rezultat_bool = true;
            }

            return rezultat_bool;
        }

        public static uint PoliczLiczbeLinii(string nazwa_pliku)
        {
            uint liczbalinii = 0;

            if (File.Exists(nazwa_pliku))
            {
                FileStream plik_fs = new FileStream(nazwa_pliku, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plik_sr = new StreamReader(plik_fs);

                    while (plik_sr.Peek() != -1)
                    {
                        plik_sr.ReadLine();
                        liczbalinii++;
                    }

                    plik_sr.Close();

                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do pliku (metoda: PoliczLiczbeLinii).");
                    Console.ResetColor();
                }

                plik_fs.Close();

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Nie istnieje wskazany plik (metoda: PoliczLiczbeLinii).");
                Console.ResetColor();
            }

            return liczbalinii;
        }

        public static bool CzyIstniejeDanyKluczWLiscieKluczy(List<dynamic> lista_kluczy, string szukany_dany_klucz)
        {
            bool rezultat = false;

            rezultat = lista_kluczy.Exists(x => x == szukany_dany_klucz);

            return rezultat;
        }

        public static int PobierzNumerIndeksuZListyKluczyIStringow(dynamic[] tablica_list_kluczy_i_stringow, string szukany_dany_klucz)
        {
            int znaleziony_index = -1;

            if (tablica_list_kluczy_i_stringow.Length == 2)
            {
                List<dynamic> lista_kluczy = tablica_list_kluczy_i_stringow[0];
                List<List<dynamic>> lista_stringow = tablica_list_kluczy_i_stringow[1];

                znaleziony_index = lista_kluczy.IndexOf(szukany_dany_klucz);

            }

            return znaleziony_index;
        }

        public static List<string> PobierzNazwyPlikowJSONzFolderu(string nazwa_folderu)
        {
            List<string> nazwy_plikow_JSON = new List<string>();

            string folderglowny = Directory.GetCurrentDirectory();

            if (Directory.Exists(folderglowny + "//" + nazwa_folderu) == true)
            {
                string[] plikiJSONwfolderze_nazwy = Directory.GetFiles(folderglowny + "//" + nazwa_folderu, "*.json");

                foreach (string s in plikiJSONwfolderze_nazwy)
                {
                    FileInfo plik_fileinfo = null;

                    try
                    {
                        plik_fileinfo = new FileInfo(s);

                        nazwy_plikow_JSON.Add(plik_fileinfo.Name);
                    }
                    catch (FileNotFoundException e)
                    {
                        Blad("Blad: " + e.Message);
                        continue;
                    }

                }

            }
            else
            {
                Blad("Blad: Nie istnieje folder o nazwie: " + nazwa_folderu);
            }

            return nazwy_plikow_JSON;


        }

        public static void Makro_UruchomienieKolejnejOperacji()
        {
            if (makro_bledy_lista.Count == 0)
            {
                //reset zmiennych po poprzednich operacjach
                makro_pomyslnezakonczenieoperacjinr2 = false;
                makro_pomyslnezakonczenieoperacjinr100 = false;

                int makro_sprawdzeniekolejnejoperacji = makro_aktualny_indeks_listy + 1;

                if (makro_sprawdzeniekolejnejoperacji < makro_operacje_lista.Count)
                {
                    makro_aktualny_indeks_listy++;

                    int makro_operacjadowykonania = int.Parse(makro_operacje_lista[makro_aktualny_indeks_listy]);

                    if (makro_operacjadowykonania == 1)
                    {
                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");
                    }
                    else if (makro_operacjadowykonania == 2)
                    {
                        WeryfikacjaPlikowMetadanych("TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON");
                    }
                    else if (makro_operacjadowykonania == 100)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych();
                    }

                }
                else
                {
                    Sukces2("Wszystkie operacje ze zdefiniowanego makra zostały wykonane.");

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();

                }

            }
            else
            {
                Makro_BladStopujacy();
            }


        }

        public static void Makro_BladStopujacy()
        {
            Blad("Wykryto błędy, które uniemożliwiły ukończenie wszystkich zaplanowanych operacji ze zdefiniowanego makra:");

            for (int wib = 0; wib < makro_bledy_lista.Count; wib++)
            {
                string[] dany_komunikat = makro_bledy_lista[wib].Split(';');
                if (dany_komunikat.Length == 2)
                {
                    Blad("[Błąd podczas wykonywania operacji makra: " + dany_komunikat[0] + "/" + makro_operacje_lista.Count + "] " + dany_komunikat[1]);
                }
            }

            Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
            Console.ReadKey();
        }

        public static void MenuMakr()
        {
            string numer_operacji2_string;
            Console.WriteLine("1. Uruchom zdefiniowane wcześniej makro: \"" + cfg.zdefiniowaneMakro + "\"");
            Console.WriteLine("2. Instrukcja zdefiniowania makra.");
            Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");

            numer_operacji2_string = Console.ReadLine();

            if (CzyParsowanieINTUdane(numer_operacji2_string))
            {
                int numer_operacji2_int = int.Parse(numer_operacji2_string);

                if (numer_operacji2_int == 1)
                {
                    string[] makro_operacje_string = cfg.zdefiniowaneMakro.Split(';');

                    for (int mis = 0; mis < makro_operacje_string.Length; mis++)
                    {
                        makro_operacje_lista.Add(makro_operacje_string[mis]);

                        //Console.WriteLine(makro_operacje_string[mis]);

                    }

                    /*
                    //DEBUGtest - START
                    Console.WriteLine("makro_operacje_lista.Count: " + makro_operacje_lista.Count);
                    for (int t1 = 0; t1 < makro_operacje_lista.Count; t1++)
                    {
                        Console.WriteLine("makro_operacje_lista[t1]:" + makro_operacje_lista[t1]);
                    }
                    //DEBUGtest - STOP
                    */

                    if (int.Parse(makro_operacje_lista[0]) == 1)
                    {
                        makro_aktywowane = true;
                        makro_aktualny_indeks_listy = 0;

                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");

                    }
                    else
                    {
                        Blad("BŁĄD MAKRA: Nieprawidłowa kolejność wykonywania operacji! Pierwsza operacja musi być zdefiniowana w makrze jako operacja nr.: 1 (weryfikacja).");

                        Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                        Console.ReadKey();

                    }





                }
                else if (numer_operacji2_int == 2)
                {
                    Console.WriteLine("Prawidłowo zdefiniowane makro umożliwia wykonanie wszystkich wymaganych operacji w zautomatyzowany sposób." +
                              "Aby zdefiniować makro lub zmodyfikować już istniejące, należy:\n" +
                              "1. Edytować plik \"cfg.json\" w edytorze tekstowym.\n" +
                              "2. Odnaleźć linię zawierającą wpis: '\"autoWprowadzanieNazwPlikowWejsciowych\": \"1\",' a jeśli wartość jest ustawiona na 0 to zmień ją na 1.\n" +
                              "3. Odnaleźć linię zawierającą wpis: '\"zdefiniowaneMakro\": \"<TUTAJ_ZDEFINIUJ_MAKRO>\",';\n" +
                              "4. Zamiast tekstu <TUTAJ_ZDEFINIUJ_MAKRO> wpisać makro, które chcemy zdefiniować (przykład poniżej).\n" +
                              "5. Zapisać plik i uruchomić ponownie PWRlangConverter.\n" +
                              "Zamiast <TUTAJ_ZDEFINIUJ_MAKRO> wpisz numery operacji, które mają zostać automatycznie wybrane, oddzielając je średnikami ','.\n" +
                              "Na przykład zdefiniowanie makra: 1;1;1;2;2;1;0;0;2;2;1;0;100;1 spowoduje po kolei wykonywanie przez narzędzie operacji:\n" +
                              "-Weryfikacja identyfikatorów numerów linii w pliku lokalizacyjnym dla wersji gry 1.0.1c\n" +
                              "-Weryfikacja identyfikatorów numerów linii w pliku lokalizacyjnym aktualizacji dla wersji gry x.x.x\n" +
                              "-Konwersja pliku lokalizacji TXT->JSON dla wersji gry: 1.0.1c (bez dołączenia numerów porządkowych i bez dołączenia numerów/id linii)\n" +
                              "-Konwersja pliku aktualizacji lokalizacji TXT->JSON dla wersji gry: x.x.x (z dołączeniem numerów porządkowych, ale bez dołączenia numerów/id linii)\n" +
                              "-Wdrażanie aktualizacji do pliku lokalizacji 1.0.1c->x.x.x\n" +
                              "UWAGA: Makra działają wyłącznie dla operacji narzędzia: 1, 2 i 100 tj. weryfikacji, konwersji TXT->JSON i wdrażania aktualizacji.\n" +
                              "UWAGA2: Pierwsza operacja makra musi zostać zdefiniowana jako: 1 (tj. weryfikacja).\n" +
                              "UWAGA3: Zaleca się definiowanie makra w taki sposób aby w pierwszej kolejności wykonywać operacje weryfikacji (1) dla wszystkich plików po kolei, następnie konwersję (2) dla wszystkich plików, a na końcu wdrażanie aktualizacji (100) po kolei dla wszystkich plików.");

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Podano błędny numer operacji.");
                    Console.ResetColor();

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Podano błedny numer operacji.");
                Console.ResetColor();

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();
            }



        }

        static void Main(string[] args)
        {
            
            Process[] procesy_nazwa = Process.GetProcessesByName("PWRlangConverter");

            /*
            Console.WriteLine("Lista uruchomionych procesów:");
            for (int x = 0; x < procesy_nazwa.Length; x++)
            {
                Console.WriteLine("(" + x + ") " + procesy_nazwa[x].ToString());
            }
            */

            if (procesy_nazwa.Length <= 1)
            {
                konfiguracja.WygenerujDomyslnyPlikKonfiguracyjnyJesliNieIstnieje();

                cfg = konfiguracja.WczytajDaneKonfiguracyjne();

                string numer_operacji_string;

                Console.WriteLine("PWRlangConverter v.1.74 by Revok (2022)");

                Console.WriteLine("WAŻNE: Pliki poddawane operacjom muszą zostać skopiowane wcześniej do folderu z tym programem.");
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("0. Uruchomienie makra lub jego zdefiniowanie.");
                Console.WriteLine("1. [1xstringsTransifexCOM] Weryfikacja identyfikatorów numerów linii na początku stringów w pliku TXT pochodzącego z Transifex.com.");
                Console.WriteLine("2. [2xTransifex.com.TXT->1xJSON] Konwersja plików TXT z platformy Transifex.com do pliku JSON.");
                Console.WriteLine("3. [JSON->JSON] Konwersja pliku JSON z polskimi znakami na plik bez polskich znakow.");
                Console.WriteLine("100. [JSON+Metadane->JSON] Wdrażanie aktualizacji do pliku JSON.");
                Console.WriteLine("---------------------------------------");
                Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");
                numer_operacji_string = Console.ReadLine();



                if (CzyParsowanieINTUdane(numer_operacji_string))
                {
                    int numer_operacji_int = int.Parse(numer_operacji_string);

                    if (numer_operacji_int == 0)
                    {
                        MenuMakr();
                    }
                    else if (numer_operacji_int == 1)
                    {
                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");
                    }
                    else if (numer_operacji_int == 2)
                    {
                        WeryfikacjaPlikowMetadanych("TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON");
                    }
                    else if (numer_operacji_int == 3)
                    {
                        UsuwanieZnakowPL();
                    }
                    else if (numer_operacji_int == 100)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych();
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("Podano błędny numer operacji.");
                        Console.ResetColor();

                        Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Podano błedny numer operacji.");
                    Console.ResetColor();

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Aplikacja PWRlangConverter jest aktualnie uruchomiona w innym oknie lub nazwa pliku wykonywalnego jest nieprawidłowa.");
                Console.ResetColor();

                Console.WriteLine("Kliknij ENTER aby zamknąć to okno.");
                Console.ReadKey();
            }

        }


        private static bool UtworzNaglowekJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            if (nazwafolderu == "")
            {
                plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }
            else
            {
                plikJSON_fs = new FileStream(nazwafolderu + "//" + nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }

            try
            {
                StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                plikJSON_sw.WriteLine("{");
                plikJSON_sw.WriteLine("  \"$id\": \"1\",");
                plikJSON_sw.WriteLine("  \"strings\": {");

                plikJSON_sw.Close();

                rezultat = true;

            }
            catch
            {
                rezultat = false;
            }

            plikJSON_fs.Close();


            return rezultat;
        }

        private static bool UtworzStopkeJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            string sprawdzenie_istnienia;

            if (nazwafolderu == "")
            {
                sprawdzenie_istnienia = nazwaplikuJSON;
            }
            else
            {
                sprawdzenie_istnienia = nazwafolderu + "//" + nazwaplikuJSON;
            }

            if (File.Exists(sprawdzenie_istnienia))
            {
                if (nazwafolderu == "")
                {
                    plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    plikJSON_fs = new FileStream(nazwafolderu + "//" + nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }

                try
                {
                    StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                    plikJSON_sw.WriteLine("  }");
                    plikJSON_sw.Write("}");

                    plikJSON_sw.Close();

                    rezultat = true;

                }
                catch
                {
                    rezultat = false;
                }

                plikJSON_fs.Close();

            }
            else
            {
                rezultat = false;
            }

            return rezultat;
        }

        private static void UtworzPlikTXT_TMP(string nazwa_pliku, List<string> lista_danych, int index_od, int index_do)
        {
            //Console.WriteLine("UtworzPlikTXT_TMP(" + nazwa_pliku + ", " + lista_danych + "," + index_od + ", " + index_do + ")");

            if (Directory.Exists(nazwafolderutmp) == false)
            {
                Directory.CreateDirectory(nazwafolderutmp);
            }

            if (File.Exists(nazwafolderutmp + "//" + nazwa_pliku) == true)
            {
                File.Delete(nazwafolderutmp + "//" + nazwa_pliku);
            }

            FileStream plikTMP_fs = new FileStream(nazwafolderutmp + "//" + nazwa_pliku, FileMode.Append, FileAccess.Write);

            try
            {
                StreamWriter plikTMP_sw = new StreamWriter(plikTMP_fs);

                for (int zd = index_od; zd <= index_do; zd++)
                {
                    plikTMP_sw.WriteLine(lista_danych[zd]);
                }



                plikTMP_sw.Close();
            }
            catch (Exception Error)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików w metodzie UtworzPlikTXT_TMP(" + nazwa_pliku + ", " + lista_danych + "," + index_od + ", " + index_do + ") - (Error: " + Error + ")!");
                Console.ResetColor();

            }

            plikTMP_fs.Close();

        }

        private static void UsunPlikiTMP(List<string> lista_nazw_plikow)
        {
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                for (int i = 0; i < lista_nazw_plikow.Count; i++)
                {
                    if (File.Exists(nazwafolderutmp + "//" + lista_nazw_plikow[i]) == true)
                    {
                        File.Delete(nazwafolderutmp + "//" + lista_nazw_plikow[i]);
                    }
                }

            }
        }

        public static void WeryfikacjaPlikowMetadanych(string nazwa_metody_ktora_ma_zostac_uruchomiona)
        {
            Console.WriteLine("1. #1_1.0.1c");

            string folderupdate = folderglownyprogramu + "//" + "update";
            string przyrostek_UpdateLocStruct = ".UpdateLocStruct.json";
            string przyrostek_UpdateLog = ".UpdateLog.json";
            string przyrostek_UpdateSchema = ".UpdateSchema.json";

            List<string> lista_oznaczen_aktualizacji = new List<string>();

            if (Directory.Exists(folderupdate) == true)
            {
                List<string> istniejacenazwyplikowmetadanych = PobierzNazwyPlikowJSONzFolderu("update");

                int np = 2;
                for (int i = 0; i < istniejacenazwyplikowmetadanych.Count; i++)
                {
                    //Console.WriteLine("Nazwa pliku metadanych: " + istniejacenazwyplikowmetadanych[i]);

                    if (istniejacenazwyplikowmetadanych[i].Contains(przyrostek_UpdateSchema) == true)
                    {
                        string oznaczenie_aktualizacji = istniejacenazwyplikowmetadanych[i].Split(new string[] { ".Update" }, StringSplitOptions.None)[0];

                        //Console.WriteLine("Oznaczenie aktualizacji: " + oznaczenie_aktualizacji);

                        if (File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateLocStruct) == true
                            && File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateLog) == true
                            && File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateSchema) == true)
                        {
                            lista_oznaczen_aktualizacji.Add(oznaczenie_aktualizacji);

                            Console.WriteLine(np + ". " + oznaczenie_aktualizacji.Replace("-", "->"));

                            np++;
                        }

                    }

                }

            }



            string numer_pozycji_string;
            Console.Write("Wpisz numer pozycji, której konwersja ma dotyczyć: ");

            if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
            {
                makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                numer_pozycji_string = makro_operacje_lista[makro_aktualny_indeks_listy];

                Console.WriteLine(makro_operacje_lista[makro_aktualny_indeks_listy]);
            }
            else
            {
                numer_pozycji_string = Console.ReadLine();
            }

            if (CzyParsowanieINTUdane(numer_pozycji_string))
            {
                string domyslna_nazwaplikukeysTransifexCOMTXT = "";
                string domyslna_nazwaplikustringsTransifexCOMTXT = "";


                int numer_pozycji_int = int.Parse(numer_pozycji_string);

                if (numer_pozycji_int == 1)
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy = "#1";

                    //Console.WriteLine("DBG: Wybrano domyślny plik (czyli pozycję nr.: " + numer_pozycji_string + ").");

                    if (nazwa_metody_ktora_ma_zostac_uruchomiona == "StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach")
                    {
                        if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
                        {
                            domyslna_nazwaplikustringsTransifexCOMTXT = cfg.domyslnaNazwaPlikuTXTZTransifexCOM;
                        }
                        else
                        {
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(domyslna_nazwaplikustringsTransifexCOMTXT);

                    }
                    else if (nazwa_metody_ktora_ma_zostac_uruchomiona == "TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON")
                    {
                        if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = cfg.domyslnaNazwaPlikukeysTransifexCOMTXT;
                            domyslna_nazwaplikustringsTransifexCOMTXT = cfg.domyslnaNazwaPlikuTXTZTransifexCOM;
                        }
                        else
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = "";
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(domyslna_nazwaplikukeysTransifexCOMTXT, domyslna_nazwaplikustringsTransifexCOMTXT);


                    }
                }
                else if (numer_pozycji_int > 1)
                {
                    int indeks_oznaczeniaaktualizacji = (numer_pozycji_int) - 2;

                    if ((indeks_oznaczeniaaktualizacji >= 0) && (lista_oznaczen_aktualizacji.Count - 1 >= indeks_oznaczeniaaktualizacji))
                    {
                        //Console.WriteLine("DBG: Wybrano plik aktualizacji (a konkretnie pozycję nr.: " + numer_pozycji_string + ").");

                        if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
                        {
                            string[] tmp1_loa = lista_oznaczen_aktualizacji[indeks_oznaczeniaaktualizacji].Split(new char[] { '_' });

                            if (tmp1_loa.Length >= 2)
                            {
                                string numerporzadkowy_aktualizacji = tmp1_loa[0];
                                tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy = tmp1_loa[0];
                                string oznaczenie_aktualizacji = tmp1_loa[1];

                                domyslna_nazwaplikukeysTransifexCOMTXT = cfg.domyslnaNazwaPlikuAktualizacjikeysTransifexCOMTXT
                                .Replace("%OZNACZENIE_AKTUALIZACJI%", oznaczenie_aktualizacji)
                                ;

                                domyslna_nazwaplikustringsTransifexCOMTXT = cfg.domyslnaNazwaPlikuTXTAktualizacjiZTransifexCOM
                                .Replace("%NUMER_PORZADKOWY_AKTUALIZACJI%", numerporzadkowy_aktualizacji.Replace("#", ""))
                                .Replace("%OZNACZENIE_AKTUALIZACJI%", oznaczenie_aktualizacji.Replace(".", ""))
                                ;
                            }
                            else
                            {
                                Blad("Wykryto przynajmniej jedną nieprawidłowość w nazwach plików metadanych aktualizacji.");

                                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = "";
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        if (nazwa_metody_ktora_ma_zostac_uruchomiona == "StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach")
                        {
                            StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(domyslna_nazwaplikustringsTransifexCOMTXT);
                        }
                        else if (nazwa_metody_ktora_ma_zostac_uruchomiona == "TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON")
                        {
                            TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(domyslna_nazwaplikukeysTransifexCOMTXT, domyslna_nazwaplikustringsTransifexCOMTXT);
                        }

                    }
                    else
                    {

                        Blad("Podano błędny numer pozycji. (#3)");

                        Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Blad("Podano błędny numer pozycji. (#2)");

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }

            }
            else
            {
                Blad("Podano błędny numer pozycji. (#1)");

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();
            }


        }

        private static void StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(string domyslna_nazwaplikustringsTransifexCOMTXT)
        {
            bool nie_wyswietlaj_komunikatu_o_sukcesie = false;

            List<int> bledy = new List<int>();

            string nazwaplikustringsTransifexCOMTXT;

            if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
            {
                nazwaplikustringsTransifexCOMTXT = domyslna_nazwaplikustringsTransifexCOMTXT;

            }
            else
            {
                Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
                nazwaplikustringsTransifexCOMTXT = Console.ReadLine();
            }

            Console.WriteLine("Podano nazwę pliku: " + nazwaplikustringsTransifexCOMTXT);
            if (File.Exists(nazwaplikustringsTransifexCOMTXT))
            {
                

                uint plik_stringsTransifexCOMTXT_liczbalinii = PoliczLiczbeLinii(nazwaplikustringsTransifexCOMTXT);

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plik_stringsTransifexCOMTXT_liczbalinii));
                }

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_stringsTransifexCOMTXT_fs = new FileStream(nazwaplikustringsTransifexCOMTXT, FileMode.Open, FileAccess.Read);

                try
                {
                    string plik_stringsTransifexCOMTXT_trescaktualnejlinii;

                    StreamReader plik_stringsTransifexCOMTXT_sr = new StreamReader(plik_stringsTransifexCOMTXT_fs);

                    int plik_stringsTransifexCOMTXT_aktualnalinia = 1;
                    while (plik_stringsTransifexCOMTXT_sr.Peek() != -1)
                    {
                        plik_stringsTransifexCOMTXT_trescaktualnejlinii = plik_stringsTransifexCOMTXT_sr.ReadLine();

                        int plik_stringsTransifexCOMTXT_aktualnyidlinii = plik_stringsTransifexCOMTXT_aktualnalinia + 3;

                        string[] podzial1 = plik_stringsTransifexCOMTXT_trescaktualnejlinii.Split(new char[] { '>' });
                        int id_pobrane_z_tresci_pliku;

                        if (wl_pasekpostepu == false)
                        {
                            string komunikat_aktualnypostep = "Trwa analizowanie linii nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "/" + plik_stringsTransifexCOMTXT_liczbalinii + " [" + PoliczPostepWProcentach(plik_stringsTransifexCOMTXT_aktualnalinia, plik_stringsTransifexCOMTXT_liczbalinii) + "%]";

                            if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                            {
                                int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                                komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                            }

                            Console.WriteLine(komunikat_aktualnypostep);
                        }
                        else if (wl_pasekpostepu == true)
                        {
                            pasek_postepu.Refresh(plik_stringsTransifexCOMTXT_aktualnalinia, "Trwa analizowanie linii...");
                        }

                        try
                        {

                            if (podzial1[0].Contains(' ') == false)
                            {
                                id_pobrane_z_tresci_pliku = int.Parse(podzial1[0].Replace("<", ""));

                                if (plik_stringsTransifexCOMTXT_aktualnyidlinii == id_pobrane_z_tresci_pliku)
                                {
                                    //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] ID-ok");
                                }
                                else
                                {
                                    bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);

                                    //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] Błędna wartość ID w stringu! ID w tej linii powinien mieć treść <" + plik_stringsTransifexCOMTXT_aktualnyidlinii + ">");
                                }

                            }
                            else
                            {
                                bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);
                            }


                        }
                        catch
                        {
                            bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);

                            //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] Błędna wartość id w stringu (zmienna nie może przyjąć wartości innej niż int).");
                        }


                        plik_stringsTransifexCOMTXT_aktualnalinia++;
                    }



                    plik_stringsTransifexCOMTXT_sr.Close();

                }
                catch
                {
                    nie_wyswietlaj_komunikatu_o_sukcesie = true;

                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Wystapil nieoczekiwany błąd w dostępie do pliku.";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);

                }


                plik_stringsTransifexCOMTXT_fs.Close();



            }
            else
            {
                nie_wyswietlaj_komunikatu_o_sukcesie = true;

                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: W folderze z programem nie istnieje plik o nazwie \"" + nazwaplikustringsTransifexCOMTXT + "\".";

                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);

                
            }


            int bledy_iloscwykrytych = bledy.Count();
            if (bledy_iloscwykrytych == 0)
            {
                if (nie_wyswietlaj_komunikatu_o_sukcesie == false)
                {

                    string komunikat_osukcesie;
                    komunikat_osukcesie = "Nie znaleziono błędów w identyfikatorach linii na początku stringów.";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_sukcesy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_osukcesie);

                    }


                    Sukces(komunikat_osukcesie);
                }

            }
            else
            {
                

                Blad("Znaleziono błędów w pliku: " + bledy_iloscwykrytych);
                Blad("UWAGA: Jeśli identyfikator w danej linii się zgadza to należy skontrolować również czy nie ma nieprawidłowo wstawionych spacji (spacje przed <id> oraz w treści <id> są niedozwolone).");

                for (int ib = 0; ib < bledy_iloscwykrytych; ib++)
                {
                    int numer_linii = bledy[ib];
                    int poprawny_identyfikator_linii = numer_linii + 3;

                    string komunikat_obledzie;
                    komunikat_obledzie = "Wykryto błąd w linii nr: " + numer_linii + " (poprawny id powinien mieć treść: \"<" + poprawny_identyfikator_linii.ToString() + ">\")";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }


                    Blad(komunikat_obledzie);
                }

            }


            if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
            {
                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {
                Console.ResetColor();

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();
            }




        }

        public static void TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(string domyslna_nazwaplikuaktualizacjikeysTransifexCOMTXT, string domyslna_nazwaplikuaktualizacjistringsTransifexCOMTXT)
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */


            string nazwaplikukeystxt;
            string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const int ilosc_watkow = 20;
            List<string> plikkeystxt_trescilinii = new List<string>();
            List<string> plikstringstxt_trescilinii = new List<string>();

            if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
            {
                nazwaplikukeystxt = domyslna_nazwaplikuaktualizacjikeysTransifexCOMTXT;

                nazwaplikustringstxt = domyslna_nazwaplikuaktualizacjistringsTransifexCOMTXT;
            }
            else
            {
                Console.Write("Podaj nazwę pliku .keysTransifexCOM.txt: ");
                nazwaplikukeystxt = Console.ReadLine();

                Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
                nazwaplikustringstxt = Console.ReadLine();
            }

            Console.WriteLine("Podano nazwę pliku .keysTransifexCOM.txt: " + nazwaplikukeystxt);
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt: " + nazwaplikustringstxt);

            if (File.Exists(nazwaplikukeystxt) && File.Exists(nazwaplikustringstxt))
            {
                Console.WriteLine("Czy dołączyć numer porządkowy bazy lub aktualizacji do każdej linii? (Wybierz numer poniższej opcji i zatwierdź ENTEREM.)");
                Console.WriteLine("0. Nie dołączaj. (format: string)");
                Console.WriteLine("1. Dołącz numer porządkowy. (format: #numer_porządkowy:string)");

                string czydolaczycnumerporzadkowy_wybor;

                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                {
                    makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                    czydolaczycnumerporzadkowy_wybor = makro_operacje_lista[makro_aktualny_indeks_listy];
                }
                else
                {
                    czydolaczycnumerporzadkowy_wybor = Console.ReadLine();
                }

                if (czydolaczycnumerporzadkowy_wybor == "1")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor = 1;
                    Console.WriteLine("Numery porządkowe zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }




                Console.WriteLine("Czy dołączyć numery linii do lokalizacji, aby wyświetlały się w grze? (Wybierz numer poniższej opcji i zatwierdź ENTEREM.)");
                Console.WriteLine("0. Nie dołączaj. (format: string)");
                Console.WriteLine("1. Dołącz numery linii. (format: [numer_linii]string)");
                Console.WriteLine("2. Dołącz numery i identyfikatory linii. (format: [numer_linii]<id_linii>string)");

                string czydolaczycnumerylinii_wybor;

                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                {
                    makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                    czydolaczycnumerylinii_wybor = makro_operacje_lista[makro_aktualny_indeks_listy];

                }
                else
                {
                    czydolaczycnumerylinii_wybor = Console.ReadLine();
                }


                if (czydolaczycnumerylinii_wybor == "1")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor = 1;
                    Console.WriteLine("Numery linii zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }
                else if (czydolaczycnumerylinii_wybor == "2")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor = 2;
                    Console.WriteLine("Numery wraz z identyfikatorami linii zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }


                nazwanowegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");

                Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);

                tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP = plikkeystxt_ilosclinii;

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plikkeystxt_ilosclinii));
                }

                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {


                    if (Directory.Exists(nazwafolderutmp) == false)
                    {
                        Directory.CreateDirectory(nazwafolderutmp);
                    }

                    decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(plikkeystxt_ilosclinii) / Convert.ToDecimal(ilosc_watkow));

                    //Console.WriteLine("plikkeystxt_ilosclinii: " + plikkeystxt_ilosclinii);
                    //Console.WriteLine("plikstringstxt_ilosclinii: " + plikstringstxt_ilosclinii);
                    //Console.WriteLine("ilosc_watkow: " + ilosc_watkow);
                    //Console.WriteLine("maksymalna_ilosc_linii_dla_1_watku: " + maksymalna_ilosc_linii_dla_1_watku);


                    FileStream plikkeystxt_fs = new FileStream(nazwaplikukeystxt, FileMode.Open, FileAccess.Read);
                    FileStream plikstringstxt_fs = new FileStream(nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                    StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);
                    StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);



                    while (plikkeystxt_sr.Peek() != -1)
                    {

                        string plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();

                        plikkeystxt_trescilinii.Add(plikkeystxt_trescaktualnejlinii);

                    }


                    while (plikstringstxt_sr.Peek() != -1)
                    {

                        string plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();

                        plikstringstxt_trescilinii.Add(plikstringstxt_trescaktualnejlinii);

                    }


                    List<string> listaplikowkeystxtTMP = new List<string>();
                    List<string> listaplikowstringstxtTMP = new List<string>();
                    List<string> listaplikowjsonTMP = new List<string>();


                    for (int lw = 0; lw < ilosc_watkow; lw++)
                    {
                        int numer_pliku = lw + 1;

                        int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                        int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                        if (index_do > Convert.ToInt32(plikkeystxt_ilosclinii) - 1)
                        {
                            index_do = Convert.ToInt32(plikkeystxt_ilosclinii) - 1;
                        }

                        UtworzPlikTXT_TMP(nazwaplikukeystxt + "_" + numer_pliku + ".tmp", plikkeystxt_trescilinii, index_od, index_do);
                        UtworzPlikTXT_TMP(nazwaplikustringstxt + "_" + numer_pliku + ".tmp", plikstringstxt_trescilinii, index_od, index_do);

                        listaplikowkeystxtTMP.Add(nazwaplikukeystxt + "_" + numer_pliku + ".tmp");
                        listaplikowstringstxtTMP.Add(nazwaplikustringstxt + "_" + numer_pliku + ".tmp");
                        listaplikowjsonTMP.Add(nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "") + "_" + numer_pliku + ".tmp");
                    }

                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP = listaplikowkeystxtTMP;
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP = listaplikowstringstxtTMP;
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP = listaplikowjsonTMP;


                    plikkeystxt_sr.Close();
                    plikstringstxt_sr.Close();

                    plikkeystxt_fs.Close();
                    plikstringstxt_fs.Close();




                    //TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje("test1.json.keysTransifexCOM.txt_1.tmp", "test1.json.stringsTransifexCOM.txt_1.tmp");

                    Thread watek1_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek1);
                    Thread watek2_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek2);
                    Thread watek3_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek3);
                    Thread watek4_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek4);
                    Thread watek5_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek5);
                    Thread watek6_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek6);
                    Thread watek7_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek7);
                    Thread watek8_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek8);
                    Thread watek9_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek9);
                    Thread watek10_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek10);
                    Thread watek11_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek11);
                    Thread watek12_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek12);
                    Thread watek13_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek13);
                    Thread watek14_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek14);
                    Thread watek15_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek15);
                    Thread watek16_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek16);
                    Thread watek17_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek17);
                    Thread watek18_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek18);
                    Thread watek19_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek19);
                    Thread watek20_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek20);

                    watek1_transifexCOMtxttoJSON.Start();
                    watek2_transifexCOMtxttoJSON.Start();
                    watek3_transifexCOMtxttoJSON.Start();
                    watek4_transifexCOMtxttoJSON.Start();
                    watek5_transifexCOMtxttoJSON.Start();
                    watek6_transifexCOMtxttoJSON.Start();
                    watek7_transifexCOMtxttoJSON.Start();
                    watek8_transifexCOMtxttoJSON.Start();
                    watek9_transifexCOMtxttoJSON.Start();
                    watek10_transifexCOMtxttoJSON.Start();
                    watek11_transifexCOMtxttoJSON.Start();
                    watek12_transifexCOMtxttoJSON.Start();
                    watek13_transifexCOMtxttoJSON.Start();
                    watek14_transifexCOMtxttoJSON.Start();
                    watek15_transifexCOMtxttoJSON.Start();
                    watek16_transifexCOMtxttoJSON.Start();
                    watek17_transifexCOMtxttoJSON.Start();
                    watek18_transifexCOMtxttoJSON.Start();
                    watek19_transifexCOMtxttoJSON.Start();
                    watek20_transifexCOMtxttoJSON.Start();


                    watek1_transifexCOMtxttoJSON.Join();
                    watek2_transifexCOMtxttoJSON.Join();
                    watek3_transifexCOMtxttoJSON.Join();
                    watek4_transifexCOMtxttoJSON.Join();
                    watek5_transifexCOMtxttoJSON.Join();
                    watek6_transifexCOMtxttoJSON.Join();
                    watek7_transifexCOMtxttoJSON.Join();
                    watek8_transifexCOMtxttoJSON.Join();
                    watek9_transifexCOMtxttoJSON.Join();
                    watek10_transifexCOMtxttoJSON.Join();
                    watek11_transifexCOMtxttoJSON.Join();
                    watek12_transifexCOMtxttoJSON.Join();
                    watek13_transifexCOMtxttoJSON.Join();
                    watek14_transifexCOMtxttoJSON.Join();
                    watek15_transifexCOMtxttoJSON.Join();
                    watek16_transifexCOMtxttoJSON.Join();
                    watek17_transifexCOMtxttoJSON.Join();
                    watek18_transifexCOMtxttoJSON.Join();
                    watek19_transifexCOMtxttoJSON.Join();
                    watek20_transifexCOMtxttoJSON.Join();

                    //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");



                    string nazwafinalnegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");

                    if (File.Exists(nazwafinalnegoplikuJSON) == true) { File.Delete(nazwafinalnegoplikuJSON); }


                    UtworzNaglowekJSON(nazwanowegoplikuJSON);


                    FileStream finalnyplikJSON_fs = new FileStream(nazwafinalnegoplikuJSON, FileMode.Append, FileAccess.Write);

                    try //#1
                    {
                        StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                        for (int lpj = 0; lpj < tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP.Count; lpj++)
                        {
                            FileStream plikjsonTMP_fs = new FileStream(nazwafolderutmp + "//" + tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP[lpj], FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                plikjsonTMP_sr.Close();
                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj: " + lpj + ", Error: " + Error + ")!";

                                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                                Blad(komunikat_obledzie);

                            }

                            plikjsonTMP_fs.Close();


                        }

                        finalnyplikJSON_sw.Close();



                    }
                    catch (Exception Error)
                    {

                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                        if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                        Blad(komunikat_obledzie);

                    }


                    finalnyplikJSON_fs.Close();

                    bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwanowegoplikuJSON);

                    if (stopkaJSON_rezultat == true)
                    {
                        if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
                        {
                            if (File.Exists(nazwaplikustringstxt) == true) { File.Delete(nazwaplikustringstxt); }
                            if (File.Exists(cfg.domyslnaNazwaPlikustringsTransifexCOMTXT) == true) { File.Delete(cfg.domyslnaNazwaPlikustringsTransifexCOMTXT); }
                        }

                        makro_pomyslnezakonczenieoperacjinr2 = true;

                        string komunikat_osukcesie;
                        komunikat_osukcesie = "Plik JSON o nazwie \"" + nazwanowegoplikuJSON + "\" zostal wygenerowany.";

                        if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_sukcesy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_osukcesie);

                        }


                        Sukces(komunikat_osukcesie);

                    }

                }
                else
                {
                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);

                }


            }
            else
            {
                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: W folderze z programem nie istnieje przynajmniej jeden plik, których nazwy wskazano.";

                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);
                
            }
            
            //czyszczenie pamięci
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP);
            


            if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1 && makro_pomyslnezakonczenieoperacjinr2 == true)
            {
                //czyszczenie pamięci dodatkowej jeśli makro jest aktywowane
                tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP = 0;
                tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii = 1;
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP.Clear();
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP.Clear();
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP.Clear();


                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {
                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();
            }

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(string nazwaplikukeystxt, string nazwaplikustringstxt, bool ostatni_watek = false)
        {

            //string nazwaplikukeystxt;
            //string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';


            if (File.Exists(nazwafolderutmp + "//" + nazwaplikukeystxt) && File.Exists(nazwafolderutmp + "//" + nazwaplikustringstxt))
            {
                nazwanowegoplikuJSON = nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");


                //Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(nazwafolderutmp + "//" + nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(nazwafolderutmp + "//" + nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {
                    bool bledywplikuJSON = false;
                    FileStream nowyplikJSON_fs = new FileStream(nazwafolderutmp + "//" + nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                    FileStream plikkeystxt_fs = new FileStream(nazwafolderutmp + "//" + nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                    try //#1
                    {
                        string plikkeystxt_trescaktualnejlinii;
                        string plikstringstxt_trescaktualnejlinii;

                        StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                        StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);

                        int plikkeystxt_sr_nraktualnejlinii = 1;
                        while (plikkeystxt_sr.Peek() != -1)
                        {
                            plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                            string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                            //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                            FileStream plikstringstxt_fs = new FileStream(nazwafolderutmp + "//" + nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                int plikstringstxt_sr_nraktualnejlinii = 1;
                                while (plikstringstxt_sr.Peek() != -1)
                                {
                                    string nraktualnejliniiwplikuJSON = "";

                                    plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();


                                    if (plikstringstxt_sr_nraktualnejlinii == plikkeystxt_sr_nraktualnejlinii)
                                    {
                                        string plikstringstxt_trescaktualnejliniipofiltracjitmp;

                                        string[] tmp1 = plikstringstxt_trescaktualnejlinii.Split(new char[] { '>' });

                                        if (tmp1.Length > 0)
                                        {
                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejlinii;

                                            //Console.WriteLine("[DEBUG] tmp1[0]==" + tmp1[0]);

                                            string tmp2 = tmp1[0].TrimStart().Remove(0, 1);

                                            nraktualnejliniiwplikuJSON = tmp2;

                                            //Console.WriteLine("[DEBUG] tmp2==" + tmp2);

                                            //plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejliniipofiltracjitmp.Replace("<" + tmp2 + ">", "");

                                            string tmp3 = "<" + tmp2 + ">";
                                            int tmp4 = tmp3.Length;

                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejliniipofiltracjitmp.Remove(0, tmp4);
                                        }
                                        else
                                        {
                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejlinii;
                                        }



                                        string plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescaktualnejliniipofiltracjitmp;

                                        //Console.WriteLine("!!!: Liczba key+vars w linii nr. " + plikkeystxt_sr_nraktualnejlinii + ": " + plikkeystxt_wartoscilinii.Length);

                                        List<string> lista_zmiennych_linii = new List<string>();



                                        plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                        .Replace("\"", "<bs_n1>")
                                        .Replace("<br>", "\\n")
                                        .Replace("<bs_n1>", "\\\"")
                                        .Replace("<bs_br>", "<br>");


                                        if (plikkeystxt_wartoscilinii.Length > 1)
                                        {

                                            for (int ivw = 1; ivw < plikkeystxt_wartoscilinii.Length; ivw++)
                                            {
                                                int ivwminus1 = ivw - 1;

                                                lista_zmiennych_linii.Add("<kl" + ivwminus1 + ">" + ";" + plikkeystxt_wartoscilinii[ivw]);

                                            }

                                        }

                                        //Console.WriteLine("lista_zmiennych_linii.Count: " + lista_zmiennych_linii.Count);

                                        for (int it1 = 0; it1 < lista_zmiennych_linii.Count; it1++)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii.Replace(lista_zmiennych_linii[it1].Split(new char[] { ';' })[0], lista_zmiennych_linii[it1].Split(new char[] { ';' })[1]);

                                            //Console.WriteLine("Sparsowano zmienna w linii nr. " + plikstringstxt_sr_nraktualnejlinii + ": " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[0] + "na " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[1]);

                                        }


                                        //Console.WriteLine("MOMENT PRZED ZAPISEM: " + plikstringstxt_trescuaktualnionalinii);

                                        //Console.WriteLine("plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii);
                                        //Console.WriteLine("plikkeystxt_ilosclinii: " + plikkeystxt_ilosclinii);

                                        if (plikstringstxt_trescuaktualnionalinii == " ") { plikstringstxt_trescuaktualnionalinii = ""; }

                                        int rzeczywistynumer_aktualnejlinii = Convert.ToInt32(nraktualnejliniiwplikuJSON) - 3;

                                        if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor == 1)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=65%>[" + rzeczywistynumer_aktualnejlinii + "]</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }
                                        else if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor == 2)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=65%>[" + rzeczywistynumer_aktualnejlinii + "]</size>" + "<size=50%>" + "<" + nraktualnejliniiwplikuJSON + ">" + "</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }

                                        if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor == 1)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=30%>" + tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy + ":" + "</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }

                                        if (plikstringstxt_sr_nraktualnejlinii != plikkeystxt_ilosclinii)
                                        {
                                            nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                        }
                                        else
                                        {

                                            if (ostatni_watek == false)
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                            }
                                            else
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\"");
                                            }
                                        }


                                    }




                                    plikstringstxt_sr_nraktualnejlinii++;
                                }


                                plikstringstxt_sr.Close();

                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2) (Error: " + Error + ")";

                                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                                Blad(komunikat_obledzie);

                            }

                            if (wl_pasekpostepu == false)
                            {

                                string komunikat_aktualnypostep = "Trwa przygotowywanie linii nr.: " + tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii + "/" + tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP + " [" + PoliczPostepWProcentach(tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii, tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP) + "%]";

                                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                                    komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                                }

                                Console.WriteLine(komunikat_aktualnypostep);

                            }
                            else if (wl_pasekpostepu == true)
                            {
                                pasek_postepu.Refresh(Convert.ToInt32(tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii), "Trwa przygotowywanie linii...");
                            }

                            tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii++;


                            plikkeystxt_sr_nraktualnejlinii++;

                            plikstringstxt_fs.Close();

                        }
                        plikkeystxt_sr.Close();
                        nowyplikJSON_sw.Close();




                    }
                    catch (Exception Error)
                    {
                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1) (Error: " + Error + ")";

                        if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                        Blad(komunikat_obledzie);

                    }

                    plikkeystxt_fs.Close();
                    nowyplikJSON_fs.Close();

                    if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                    {
                        File.Delete(nazwanowegoplikuJSON);

                        /*
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("bledywplikuJSON: true");
                        Console.ResetColor();
                        */
                    }
                    else
                    {
                        /*
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("bledywplikuJSON: false");
                        Console.ResetColor();
                        */
                    }





                }
                else
                {

                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);


                }


            }
            else
            {

                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: Brak wskazanych plików.";

                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);

            }



        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek1()
        {
            int index = 0;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek2()
        {
            int index = 1;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek3()
        {
            int index = 2;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek4()
        {
            int index = 3;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek5()
        {
            int index = 4;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek6()
        {
            int index = 5;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek7()
        {
            int index = 6;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek8()
        {
            int index = 7;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek9()
        {
            int index = 8;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek10()
        {
            int index = 9;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek11()
        {
            int index = 10;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek12()
        {
            int index = 11;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek13()
        {
            int index = 12;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek14()
        {
            int index = 13;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek15()
        {
            int index = 14;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek16()
        {
            int index = 15;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek17()
        {
            int index = 16;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek18()
        {
            int index = 17;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek19()
        {
            int index = 18;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek20()
        {
            int index = 19;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index], true);

        }

        public static void UsuwanieZnakowPL()
        {
            string nazwaplikuzrodlowego;

            if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
            {
                nazwaplikuzrodlowego = cfg.domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON;
            }
            else
            {
                Console.Write("Podaj nazwę pliku: ");
                nazwaplikuzrodlowego = Console.ReadLine();
            }

            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuzrodlowego);
            if (File.Exists(nazwaplikuzrodlowego))
            {
                uint plikzrodlowy_liczbalinii = PoliczLiczbeLinii(nazwaplikuzrodlowego);

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plikzrodlowy_liczbalinii));
                }

                if (plikzrodlowy_liczbalinii > 0)
                {
                    FileStream plikzrodlowy_fs = new FileStream(nazwaplikuzrodlowego, FileMode.Open, FileAccess.Read);
                    FileStream nowyplik_fs = new FileStream("BezZnakowPL_" + nazwaplikuzrodlowego, FileMode.Create, FileAccess.Write);

                    try
                    {
                        StreamReader plikzrodlowy_sr = new StreamReader(plikzrodlowy_fs);
                        StreamWriter nowyplik_sw = new StreamWriter(nowyplik_fs);

                        int plikzrodlowy_numerlinii = 1;
                        while (plikzrodlowy_sr.Peek() != -1)
                        {
                            string uaktualniona_linia = plikzrodlowy_sr.ReadLine()

                            .Replace('ę', 'e')
                            .Replace('Ę', 'E')
                            .Replace('ó', 'o')
                            .Replace('Ó', 'O')
                            .Replace('ą', 'a')
                            .Replace('Ą', 'A')
                            .Replace('ś', 's')
                            .Replace('Ś', 'S')
                            .Replace('ł', 'l')
                            .Replace('Ł', 'L')
                            .Replace('ż', 'z')
                            .Replace('Ż', 'Z')
                            .Replace('ź', 'z')
                            .Replace('Ź', 'Z')
                            .Replace('ć', 'c')
                            .Replace('Ć', 'C')
                            .Replace('ń', 'n')
                            .Replace('Ń', 'N');


                            if (plikzrodlowy_numerlinii != plikzrodlowy_liczbalinii)
                            {
                                nowyplik_sw.WriteLine(uaktualniona_linia);
                            }
                            else
                            {
                                nowyplik_sw.Write(uaktualniona_linia);
                            }

                            if (wl_pasekpostepu == false)
                            {
                                Console.WriteLine("Trwa zapisywanie linii nr.: " + plikzrodlowy_numerlinii + "/" + plikzrodlowy_liczbalinii + " [" + PoliczPostepWProcentach(plikzrodlowy_numerlinii, plikzrodlowy_liczbalinii) + "%]");
                            }
                            else if (wl_pasekpostepu == true)
                            {
                                pasek_postepu.Refresh(plikzrodlowy_numerlinii, "Trwa zapisywanie linii...");
                            }

                            plikzrodlowy_numerlinii++;

                        }

                        nowyplik_sw.Close();
                        plikzrodlowy_sr.Close();

                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("Utworzono nowy plik nie zawierajacy polskich znakow: " + "BezZnakowPL_" + nazwaplikuzrodlowego);
                        Console.ResetColor();

                    }
                    catch
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow.");
                        Console.ResetColor();
                    }

                    nowyplik_fs.Close();
                    plikzrodlowy_fs.Close();

                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil problem ze zliczeniem linii w podanym pliku!");
                    Console.ResetColor();
                }

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Nie istnieje plik o nazwie \"" + nazwaplikuzrodlowego + "\" w folderze z programem.");
                Console.ResetColor();
            }

            Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
            Console.ReadKey();

        }

        public static void WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych()
        {
            string folderupdate = folderglownyprogramu + "//" + "update";
            string przyrostek_UpdateLocStruct = ".UpdateLocStruct.json";
            string przyrostek_UpdateLog = ".UpdateLog.json";
            string przyrostek_UpdateSchema = ".UpdateSchema.json";

            if (Directory.Exists(folderupdate) == true)
            {
                List<string> istniejacenazwyplikowmetadanych = PobierzNazwyPlikowJSONzFolderu("update");

                List<string> lista_oznaczen_aktualizacji = new List<string>();

                int np = 1;
                for (int i = 0; i < istniejacenazwyplikowmetadanych.Count; i++)
                {
                    //Console.WriteLine("Nazwa pliku metadanych: " + istniejacenazwyplikowmetadanych[i]);

                    if (istniejacenazwyplikowmetadanych[i].Contains(przyrostek_UpdateSchema) == true)
                    {
                        string oznaczenie_aktualizacji = istniejacenazwyplikowmetadanych[i].Split(new string[] { ".Update" }, StringSplitOptions.None)[0];

                        //Console.WriteLine("Oznaczenie aktualizacji: " + oznaczenie_aktualizacji);

                        if (File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateLocStruct) == true
                            && File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateLog) == true
                            && File.Exists(folderupdate + "//" + oznaczenie_aktualizacji + przyrostek_UpdateSchema) == true)
                        {
                            lista_oznaczen_aktualizacji.Add(oznaczenie_aktualizacji);

                            Console.WriteLine(np + ". " + oznaczenie_aktualizacji.Replace("-", "->"));

                            np++;
                        }

                    }

                }

                if (istniejacenazwyplikowmetadanych.Count > 0 && lista_oznaczen_aktualizacji.Count > 0)
                {
                    string numer_pozycji_string;
                    Console.Write("Wpisz numer pozycji aktualizacji, którą chcesz wdrożyć: ");

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                        numer_pozycji_string = makro_operacje_lista[makro_aktualny_indeks_listy];
                        Console.WriteLine(makro_operacje_lista[makro_aktualny_indeks_listy]);
                    }
                    else
                    {
                        numer_pozycji_string = Console.ReadLine();
                    }



                    if (CzyParsowanieINTUdane(numer_pozycji_string))
                    {
                        int indeks_oznaczeniaaktualizacji = (int.Parse(numer_pozycji_string)) - 1;

                        if ((indeks_oznaczeniaaktualizacji >= 0) && (lista_oznaczen_aktualizacji.Count - 1 >= indeks_oznaczeniaaktualizacji))
                        {

                            string[] tmp2_loa = lista_oznaczen_aktualizacji[indeks_oznaczeniaaktualizacji].Split(new char[] { '_' });

                            if (tmp2_loa.Length >= 2)
                            {
                                string numerporzadkowy_aktualizacji = tmp2_loa[0];
                                string oznaczenie_aktualizacji = tmp2_loa[1];

                                WdrazanieAktualizacji_Wielowatkowe(numerporzadkowy_aktualizacji, oznaczenie_aktualizacji);

                            }
                            else
                            {
                                Blad("Wykryto przynajmniej jedną nieprawidłowość w nazwach plików metadanych aktualizacji.");

                                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                                Console.ReadKey();
                            }

                        }
                        else
                        {
                            Blad("Podano błędny numer pozycji. (#2)");

                            Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                            Console.ReadKey();
                        }

                    }
                    else
                    {
                        Blad("Podano błędny numer pozycji aktualizacji. (#1)");

                        Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                        Console.ReadKey();
                    }


                }
                else
                {
                    Blad("Folder \"" + folderupdate.Replace("//", "\\") + "\" nie zawiera kompletnych metadanych o aktualizacjach.");

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();

                }
            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate.Replace("//", "\\") + "\" zawierający kompletne metadane o aktualizacjach.");

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();

            }

        }

        public static void WdrazanieAktualizacji_Wielowatkowe(string numerporzadkowy_aktualizacji, string oznaczenie_aktualizacji) //numerporzadkowy_aktualizacji np: #2 oznaczenie_aktualizacji np: 1.0.1c-1.1.7c
        /* 
         * WYMAGA PLIKÓW METADANYCH AKTUALIZACJI WYGENEROWANYCH W PWRlangTools DO DZIAŁANIA. PLIKI TE MUSZĄ ZOSTAĆ UMIESZCZONE W FOLDERZE "PWRlangConverter\update\":
         * - <oznaczenie_wersji>.UpdateLocStruct
         * - <oznaczenie_wersji>.UpdateLog.json
         * - <oznaczenie_wersji>.UpdateSchema.json
         */
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */


            const int ilosc_watkow = 20;

            string[] oa = oznaczenie_aktualizacji.Split(new char[] { '-' });
            string numer_starej_wersji = oa[0];
            string numer_nowej_wersji = oa[1];

            string folderupdate = folderglownyprogramu + "//" + "update";

            if (Directory.Exists(folderupdate) == true)
            {

                string plikUpdateSchemaJSON_nazwa;
                string plikUpdateLocStructJSON_nazwa;
                string pliklokalizacjistarejwersji_nazwa;
                string pliklokalizacjizaktualizacjadonowejwersji_nazwa;

                string NOWYpliklokalizacjipoaktualizacji_nazwa = "NOWY_plPL-" + numer_nowej_wersji + ".json";

                if (File.Exists(NOWYpliklokalizacjipoaktualizacji_nazwa) == true) { File.Delete(NOWYpliklokalizacjipoaktualizacji_nazwa); }

                if (cfg.autoWprowadzanieNazwPlikowWejsciowych == "1")
                {
                    pliklokalizacjistarejwersji_nazwa = "NOWY_plPL-" + numer_starej_wersji + ".json";
                    pliklokalizacjizaktualizacjadonowejwersji_nazwa = "NOWY_plPL-update-" + oznaczenie_aktualizacji + ".json";
                }
                else
                {


                    Console.Write("Podaj nazwę pliku json lokalizacji w wersji " + numer_starej_wersji + ": ");
                    pliklokalizacjistarejwersji_nazwa = Console.ReadLine();
                    Console.Write("Podaj nazwę pliku json, który zawiera aktualizacje do wersji " + numer_nowej_wersji + ": ");
                    pliklokalizacjizaktualizacjadonowejwersji_nazwa = Console.ReadLine();

                }

                plikUpdateSchemaJSON_nazwa = numerporzadkowy_aktualizacji + "_" + oznaczenie_aktualizacji + ".UpdateSchema.json";
                plikUpdateLocStructJSON_nazwa = numerporzadkowy_aktualizacji + "_" + oznaczenie_aktualizacji + ".UpdateLocStruct.json";

                Console.WriteLine("Podano nazwę pliku .UpdateSchema.json dla aktualizacji " + oznaczenie_aktualizacji + ": " + plikUpdateSchemaJSON_nazwa);
                Console.WriteLine("Podano nazwę pliku .UpdateLocStruct.json dla aktualizacji " + oznaczenie_aktualizacji + ": " + plikUpdateLocStructJSON_nazwa);
                Console.WriteLine("Podano nazwę pliku json lokalizacji w wersji " + numer_starej_wersji + ": " + pliklokalizacjistarejwersji_nazwa);
                Console.WriteLine("Podano nazwę pliku json, który zawiera aktualizacje do wersji " + numer_nowej_wersji + ": " + pliklokalizacjizaktualizacjadonowejwersji_nazwa);

                if ((File.Exists(folderupdate + "//" + plikUpdateSchemaJSON_nazwa) == true)
                   && (File.Exists(folderupdate + "//" + plikUpdateLocStructJSON_nazwa) == true)
                   && (File.Exists(pliklokalizacjistarejwersji_nazwa) == true)
                   && (File.Exists(pliklokalizacjizaktualizacjadonowejwersji_nazwa) == true))
                {


                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji = oznaczenie_aktualizacji;

                    dynamic[] _Schemat_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(folderupdate + "//" + plikUpdateSchemaJSON_nazwa);
                    dynamic[] _StrukturaLokalizacji_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(folderupdate + "//" + plikUpdateLocStructJSON_nazwa);
                    dynamic[] _PlikLokalizacjiStarejWersji_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(pliklokalizacjistarejwersji_nazwa);
                    dynamic[] _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(pliklokalizacjizaktualizacjadonowejwersji_nazwa);

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych = _Schemat_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych = _StrukturaLokalizacji_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych = _PlikLokalizacjiStarejWersji_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych;

                    List<dynamic> _Schemat_listakluczy = _Schemat_tablicalistdanych[0];
                    List<dynamic> _StrukturaLokalizacji_listakluczy = _StrukturaLokalizacji_tablicalistdanych[0];
                    List<dynamic> _PlikLokalizacjiStarejWersji_listakluczy = _PlikLokalizacjiStarejWersji_tablicalistdanych[0];
                    List<dynamic> _PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych[0];

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy = _Schemat_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy = _StrukturaLokalizacji_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy = _PlikLokalizacjiStarejWersji_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy = _PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy;

                    //List<List<dynamic>> _Schemat_listastringow = _Schemat_tablicalistdanych[1]; //lista stringów w pliku UpdateSchema.json jest zawsze pusta
                    List<List<dynamic>> _StrukturaLokalizacji = _StrukturaLokalizacji_tablicalistdanych[1];
                    List<List<dynamic>> _PlikLokalizacjiStarejWersji_listastringow = _PlikLokalizacjiStarejWersji_tablicalistdanych[1];
                    List<List<dynamic>> _PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych[1];

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji = _StrukturaLokalizacji;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow = _PlikLokalizacjiStarejWersji_listastringow;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow = _PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow;

                    //Console.WriteLine("_Schemat_listakluczy[0]:" + _Schemat_listakluczy[0].ToString());
                    //Console.WriteLine("_Schemat_listastringow[0][0]:" + _Schemat_listastringow[0][0].ToString()); //out of index
                    //Console.WriteLine("_Schemat_listakluczy[1]:" + _Schemat_listakluczy[1].ToString());
                    //Console.WriteLine("_Schemat_listastringow[1][0]:" + _Schemat_listastringow[1][0].ToString()); //out of index

                    int plikUpdateSchemaJSON_iloscwszystkichkluczy = _Schemat_listakluczy.Count;

                    if (wl_pasekpostepu == true)
                    {
                        InicjalizacjaPaskaPostepu(plikUpdateSchemaJSON_iloscwszystkichkluczy);
                    }

                    decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(plikUpdateSchemaJSON_iloscwszystkichkluczy) / Convert.ToDecimal(ilosc_watkow));

                    //Console.WriteLine("plikUpdateSchemaJSON_iloscwszystkichkluczy: " + plikUpdateSchemaJSON_iloscwszystkichkluczy);
                    //Console.WriteLine("ilosc_watkow: " + ilosc_watkow);
                    //Console.WriteLine("maksymalna_ilosc_linii_dla_1_watku: " + maksymalna_ilosc_linii_dla_1_watku);

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP = plikUpdateSchemaJSON_iloscwszystkichkluczy;


                    //bool t = CzyIstniejeDanyKluczWLiscieKluczy(_Schemat_listakluczy, "f0e56983-7bef-4f82-b95a-1c2e871f1319");
                    //Console.WriteLine("t: " + t);


                    List<string> listaplikowjsonTMP = new List<string>();
                    List<int> listazakresuindeksow_od = new List<int>();
                    List<int> listazakresuindeksow_do = new List<int>();


                    for (int lw = 0; lw < ilosc_watkow; lw++)
                    {
                        int numer_pliku = lw + 1;

                        int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                        int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                        if (index_do > Convert.ToInt32(plikUpdateSchemaJSON_iloscwszystkichkluczy) - 1)
                        {
                            index_do = Convert.ToInt32(plikUpdateSchemaJSON_iloscwszystkichkluczy) - 1;
                        }

                        listaplikowjsonTMP.Add(NOWYpliklokalizacjipoaktualizacji_nazwa + "_" + numer_pliku + ".tmp");
                        listazakresuindeksow_od.Add(index_od);
                        listazakresuindeksow_do.Add(index_do);
                    }

                    /*
                    for (int test1 = 0; test1 < listazakresuindeksow_od.Count; test1++)
                    {
                        Console.WriteLine("test1[" + test1 + "] (zakres od): " + listazakresuindeksow_od[test1]);
                    }
                    for (int test2 = 0; test2 < listazakresuindeksow_do.Count; test2++)
                    {
                        Console.WriteLine("test2[" + test2 + "] (zakres do): " + listazakresuindeksow_do[test2]);
                    }
                    */

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP = listaplikowjsonTMP;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD = listazakresuindeksow_od;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO = listazakresuindeksow_do;



                    Thread watek1 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek1);
                    Thread watek2 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek2);
                    Thread watek3 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek3);
                    Thread watek4 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek4);
                    Thread watek5 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek5);
                    Thread watek6 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek6);
                    Thread watek7 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek7);
                    Thread watek8 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek8);
                    Thread watek9 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek9);
                    Thread watek10 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek10);
                    Thread watek11 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek11);
                    Thread watek12 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek12);
                    Thread watek13 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek13);
                    Thread watek14 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek14);
                    Thread watek15 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek15);
                    Thread watek16 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek16);
                    Thread watek17 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek17);
                    Thread watek18 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek18);
                    Thread watek19 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek19);
                    Thread watek20 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek20);

                    watek1.Start();
                    watek2.Start();
                    watek3.Start();
                    watek4.Start();
                    watek5.Start();
                    watek6.Start();
                    watek7.Start();
                    watek8.Start();
                    watek9.Start();
                    watek10.Start();
                    watek11.Start();
                    watek12.Start();
                    watek13.Start();
                    watek14.Start();
                    watek15.Start();
                    watek16.Start();
                    watek17.Start();
                    watek18.Start();
                    watek19.Start();
                    watek20.Start();


                    watek1.Join();
                    watek2.Join();
                    watek3.Join();
                    watek4.Join();
                    watek5.Join();
                    watek6.Join();
                    watek7.Join();
                    watek8.Join();
                    watek9.Join();
                    watek10.Join();
                    watek11.Join();
                    watek12.Join();
                    watek13.Join();
                    watek14.Join();
                    watek15.Join();
                    watek16.Join();
                    watek17.Join();
                    watek18.Join();
                    watek19.Join();
                    watek20.Join();

                    //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");



                    string nazwafinalnegoplikuJSON = NOWYpliklokalizacjipoaktualizacji_nazwa;

                    if (File.Exists(nazwafinalnegoplikuJSON) == true) { File.Delete(nazwafinalnegoplikuJSON); }


                    UtworzNaglowekJSON(nazwafinalnegoplikuJSON);


                    FileStream finalnyplikJSON_fs = new FileStream(nazwafinalnegoplikuJSON, FileMode.Append, FileAccess.Write);

                    try //#1
                    {
                        StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                        for (int lpj2 = 0; lpj2 < tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP.Count; lpj2++)
                        {
                            FileStream plikjsonTMP_fs = new FileStream(nazwafolderutmp + "//" + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[lpj2], FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                plikjsonTMP_sr.Close();
                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj2: " + lpj2 + ", Error: " + Error + ")!";

                                if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                            }

                            plikjsonTMP_fs.Close();


                        }

                        finalnyplikJSON_sw.Close();



                    }
                    catch (Exception Error)
                    {

                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                        if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                    }


                    finalnyplikJSON_fs.Close();

                    bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwafinalnegoplikuJSON);

                    if (stopkaJSON_rezultat == true)
                    {
                        makro_pomyslnezakonczenieoperacjinr100 = true;



                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("Plik JSON o nazwie \"" + nazwafinalnegoplikuJSON + "\" zostal wygenerowany.");
                        Console.ResetColor();

                    }




                }
                else
                {
                    Blad("Nie istnieje przynajmniej jeden z podanych plików.");
                }

            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate.Replace("//", "\\") + "\" zawierający metadane o aktualizacji.");
            }

            //czyszczenie pamięci
            UsunPlikiTMP(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP);



            if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1 && makro_pomyslnezakonczenieoperacjinr100 == true)
            {
                //czyszczenie pamięci dodatkowej jeśli makro jest aktywne
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii = 1;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji = "";
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP = 0;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO.Clear();


                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();

            }



        }

        public static void WdrazanieAktualizacji_Wielowatkowe_Operacje(string nazwaplikuJSONTMP, int index_klucza_OD, int index_klucza_DO, bool ostatni_watek = false)
        {

            if (Directory.Exists(nazwafolderutmp) == false)
            {
                Directory.CreateDirectory(nazwafolderutmp);
            }

            if (File.Exists(nazwafolderutmp + "//" + nazwaplikuJSONTMP))
            {
                File.Delete(nazwafolderutmp + "//" + nazwaplikuJSONTMP);
            }


            List<string> lista_zaktualizowanychlinii_tmp = new List<string>();

            for (int i1 = index_klucza_OD; i1 <= index_klucza_DO; i1++)
            {
                int nr_ostatniej_linii = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP - 5;

                if (wl_pasekpostepu == false)
                {

                    string komunikat_aktualnypostep = "Trwa wdrażanie aktualizacji " + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji.Replace("-", "->") + " do linii nr.: " + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii + "/" + nr_ostatniej_linii + " [" + PoliczPostepWProcentach(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii, nr_ostatniej_linii) + "%]";

                    if (makro_aktywowane == true && int.Parse(cfg.autoWprowadzanieNazwPlikowWejsciowych) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                        komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                    }

                    Console.WriteLine(komunikat_aktualnypostep);

                }
                else if (wl_pasekpostepu == true)
                {
                    pasek_postepu.Refresh(Convert.ToInt32(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii), "Trwa wdrażanie aktualizaji...");
                }

                    string _Schemat_aktualnyklucz = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy[i1].ToString();

                if (_Schemat_aktualnyklucz != "$id" && _Schemat_aktualnyklucz != "strings")
                {

                    //sprci1 - sprawdzenie czy dany klucz znajduje się w pliku lokalizacji z aktualizacją do nowej wersji
                    //sprci2 - sprawdzenie czy dany klicz znajduje się w pliku lokalizacji starej wersji

                    bool sprci1 = CzyIstniejeDanyKluczWLiscieKluczy(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy, _Schemat_aktualnyklucz);

                    if (sprci1 == false)
                    {
                        bool sprci2 = CzyIstniejeDanyKluczWLiscieKluczy(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy, _Schemat_aktualnyklucz);

                        if (sprci2 == true)
                        {
                            int index = PobierzNumerIndeksuZListyKluczyIStringow(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych, _Schemat_aktualnyklucz);

                            string tresc_stringa = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow[index][0];

                            tresc_stringa = tresc_stringa
                                            .Replace("\n", "\\n")
                                            .Replace("\"", "\\\"")
                                            .Replace("\t", "\\t");

                            lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + tresc_stringa);


                        }
                        else
                        {
                            lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + "");



                        }

                    }
                    else
                    {
                        int index = PobierzNumerIndeksuZListyKluczyIStringow(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych, _Schemat_aktualnyklucz);

                        string tresc_stringa = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow[index][0];

                        tresc_stringa = tresc_stringa
                                        .Replace("\n", "\\n")
                                        .Replace("\"", "\\\"")
                                        .Replace("\t", "\\t");

                        lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + tresc_stringa);



                    }

                }

                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii++;

            }

            FileStream NOWYplikJSONTMPpoaktualizacji_fs = new FileStream("tmp" + "//" + nazwaplikuJSONTMP, FileMode.Append, FileAccess.Write);
            StreamWriter NOWYplikJSONTMPpoaktualizacji_sw = new StreamWriter(NOWYplikJSONTMPpoaktualizacji_fs);


            for (int zzd1 = 0; zzd1 < lista_zaktualizowanychlinii_tmp.Count; zzd1++)
            {
                string[] dane = lista_zaktualizowanychlinii_tmp[zzd1].Split(new string[] { "[[[---]]]" }, StringSplitOptions.None);

                string _KLUCZ = dane[0];
                string _STRING = dane[1];

                NOWYplikJSONTMPpoaktualizacji_sw.Write("    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                if (zzd1 + 1 != lista_zaktualizowanychlinii_tmp.Count || ostatni_watek == false)
                {
                    NOWYplikJSONTMPpoaktualizacji_sw.Write(",");
                }

                NOWYplikJSONTMPpoaktualizacji_sw.Write("\n");

            }


            NOWYplikJSONTMPpoaktualizacji_sw.Close();
            NOWYplikJSONTMPpoaktualizacji_fs.Close();


            //Sukces("Plik JSON o nazwie \"" + nazwaplikuJSONTMP + "\" zostal wygenerowany.");



        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek1()
        {
            int index = 0;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek2()
        {
            int index = 1;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek3()
        {
            int index = 2;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek4()
        {
            int index = 3;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek5()
        {
            int index = 4;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek6()
        {
            int index = 5;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek7()
        {
            int index = 6;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek8()
        {
            int index = 7;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek9()
        {
            int index = 8;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek10()
        {
            int index = 9;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek11()
        {
            int index = 10;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek12()
        {
            int index = 11;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek13()
        {
            int index = 12;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek14()
        {
            int index = 13;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek15()
        {
            int index = 14;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek16()
        {
            int index = 15;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek17()
        {
            int index = 16;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek18()
        {
            int index = 17;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek19()
        {
            int index = 18;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek20()
        {
            int index = 19;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index], true);

        }



    }
}

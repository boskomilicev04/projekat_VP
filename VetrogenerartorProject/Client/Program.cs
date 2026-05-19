using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Common;
using System.IO;
using System.Globalization;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IWindTurbineService> factory = null;
            IWindTurbineService proxy = null;
            NetTcpBinding binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.Security.Mode = SecurityMode.None;

            string folderAplikacije = AppDomain.CurrentDomain.BaseDirectory;
            string imeFajla = "Turbine_Data_Kelmarsh_1_2018-01-01_-_2019-01-01_228.csv";
            string csvPath = Path.Combine(folderAplikacije, imeFajla);
            string logPath = "client_errors.log";

            if (!File.Exists(csvPath))
            {
                csvPath = Path.Combine(folderAplikacije, "Data", imeFajla);
            }

            try
            {
                factory = new ChannelFactory<IWindTurbineService>(binding, new EndpointAddress("net.tcp://localhost:4000/WindTurbine"));
                proxy = factory.CreateChannel();

                Console.WriteLine("--- KLIJENT ZAPOCINJE RAD ---");

                if (!File.Exists(csvPath))
                {
                    
                    Console.WriteLine($"Greska: Fajl {csvPath} nije pronadjen!");
                    Console.WriteLine(csvPath);
                    Console.ReadLine();
                    return;
                }

                using (StreamReader reader = new StreamReader(csvPath))
                {
                    int currentRow = 1;

                    for (int i = 0; i < 9; i++ )
                    {
                        reader.ReadLine();
                        currentRow++;
                    }

                    
                    string header = reader.ReadLine();
                    currentRow++;

                    proxy.StartSession("Kelmarsh_1");
                    Console.WriteLine("Sesija na serveru je uspesno otvorena.");

                    
                    int sentCount = 0;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string[] cols = line.Split(',');
                        int[] bitniIndeksi = { 1, 15, 16, 61, 62, 82, 86, 211, 269 };

                        bool redJeValidan = true;

                        try
                        {
                            foreach (int index in bitniIndeksi)
                            {
                                if (cols[index].Contains("NaN"))
                                {
                                    redJeValidan = false;
                                    break;
                                }
                            }

                            if (!redJeValidan)
                            {
                                LogError(logPath, currentRow, "Red je preskocen: Jedan od bitnih kanala sadrzi NaN vrednost!");
                                currentRow++;
                                continue;
                            }

                            WindTurbineSample sample = new WindTurbineSample
                            {
                                TurbineId = "Kelmarsh_1",
                                RowIndex = currentRow,
                                Timestamp = DateTime.Parse(cols[0]),
                                WindSpeed = double.Parse(cols[1], CultureInfo.InvariantCulture),
                                WindDirection = double.Parse(cols[15], CultureInfo.InvariantCulture),
                                NacellePosition = double.Parse(cols[16], CultureInfo.InvariantCulture),
                                PowerKW = double.Parse(cols[61], CultureInfo.InvariantCulture),
                                PotentialPowerDefaultKW = double.Parse(cols[62], CultureInfo.InvariantCulture),
                                PowerFactor = double.Parse(cols[82], CultureInfo.InvariantCulture),
                                ReactivePowerKvar = double.Parse(cols[86], CultureInfo.InvariantCulture),
                                GridFrequencyHz = double.Parse(cols[269], CultureInfo.InvariantCulture),
                                GeneratorRpm = double.Parse(cols[211], CultureInfo.InvariantCulture)
                            };

                            proxy.PushSample(sample);
                            sentCount++;

                            if (sentCount % 10 == 0)
                            {
                                Console.WriteLine($"Poslato: {sentCount} redova... (Trenutni red u CSV: {currentRow})");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(logPath, currentRow, $"Greska pri parsiranju: {ex.Message}");
                        }

                        currentRow++;
                    }

                    proxy.EndSession();
                    Console.WriteLine("\nSlanje uspesno zavrseno.");
                    Console.WriteLine($"Ukupno poslato uzoraka: {sentCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KRITICNA GRESKA: {ex.Message}");
            }
            finally
            {
                if (factory != null && factory.State != CommunicationState.Faulted)
                {
                    factory.Close();
                }
            }

            Console.WriteLine("\nPritisnite bilo koji taster za izlaz.");
            Console.ReadLine();
        }

        static void LogError(string path, int row, string message)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Red: {row} | Poruka: {message}{Environment.NewLine}";
            File.AppendAllText(path, entry);
        }
    }
}

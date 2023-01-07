using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPV4
{

    class Program

    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inserisci l'indirizzo IPV4:");
            var input = Console.ReadLine();

            // Verifica che l'input sia valido
            if (!IsValidIPV4(input))
            {
                Console.WriteLine("L'indirizzo IPV4 inserito non è valido.");
                return;
            }
            
            Console.WriteLine("Inserisci la Subnet Mask:");

            // Crea un oggetto IPV4 usando l'input dell'utente
            var ip = new IPV4(input);

            // Stampa la subnet mask
            Console.WriteLine($"\nSubnet mask: {FormatByteArray(ip.GetSubnetMask())}");


            // Stampa il network address
            Console.WriteLine($"Network address: {FormatByteArray(ip.GetNetworkAddress())}");


            // Stampa l'indirizzo di broadcast
            Console.WriteLine($"Indirizzo di broadcast: {FormatByteArray(ip.GetBroadcast())}");

            // Stampa la wildcard mask
            Console.WriteLine($"Wildcard mask: {FormatByteArray(ip.GetWildcardMask())}");

            // Stampa il range di indirizzi disponibili
            Console.WriteLine($"Range di indirizzi disponibili: {FormatByteArray(ip.GetFirstHostIP())} - {FormatByteArray(ip.GetLastHostIP())}");

            // Stampa il numero di host possibili
            Console.WriteLine($"Numero di host possibili: {ip.GetTotalNumberOfHosts()}");

            Console.ReadLine();
        }



        static bool IsValidIPV4(string input)
        {
            // Divide l'input in un array di stringhe
            var parts = input.Split('.');

            // Verifica che ci siano esattamente 4 parti
            if (parts.Length != 4)
            {
                return false;
            }

            // Verifica che ogni parte sia un numero intero compreso tra 0 e 255
            return parts.All(part => int.TryParse(part, out int value) && value >= 0 && value <= 255);
        }

        static string FormatByteArray(byte[] array)
        {
            return string.Join(".", array);
        }
    }


    class IPV4
    {
        public byte[] IP { get; set; }
        public byte[] SubnetMask { get; set; }

        public IPV4(string ip)
        {
            // Inizializza le proprietà
            IP = ip.Split('.').Select(part => byte.Parse(part)).ToArray();
            SubnetMask = Console.ReadLine().Split('.').Select(part => byte.Parse(part)).ToArray();
        }

        public byte[] GetIP() => IP;
        public byte[] GetSubnetMask() => SubnetMask;
        public void SetSubnetMask(byte[] sm) => SubnetMask = sm;
        public void SetIP(byte[] ipv4) => IP = ipv4;

        public bool CheckSingleByte(byte single)
        {
            byte[] ValidBytes = { 255, 254, 252, 248, 240, 224, 192, 128, 0 };
            return Array.Exists(ValidBytes, element => element == single);
        }

        public bool CheckSubnetMask(byte[] sm)
        {
            foreach (var i in Enumerable.Range(0, 3))
            {
                if (sm[i + 1] != 0 && sm[i] != 255)
                {
                    return true;
                }
            }

            return false;
        }
        public byte[] GetNetworkAddress()
        {
            var networkAddress = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                networkAddress[i] = (byte)(IP[i] & SubnetMask[i]);
            }

            return networkAddress;
        }
        public byte[] GetFirstHostIP()
        {
            var firstHostIP = GetNetworkAddress();
            firstHostIP[3] += 1;
            return firstHostIP;
        }
        public byte[] GetBroadcast()
        {
            var broadcast = new byte[4];
            var networkAddress = GetNetworkAddress();
            var wildcard = GetWildcardMask();
            for (var i = 0; i < 4; i++)
            {
                broadcast[i] = (byte)(networkAddress[i] | wildcard[i]);
            }
            return broadcast;
        }


        public byte[] GetLastHostIP()
        {
            var lastHostIP = GetBroadcast();
            lastHostIP[3] -= 1;
            return lastHostIP;
        }

        public byte[] GetWildcardMask()
        {
            var wildcard = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                wildcard[i] = (byte)(255 - (int)SubnetMask[i]);
            }
            return wildcard;
        }
        public int GetCIDR()
        {
            var cidr = 0;
            foreach (var b in SubnetMask)
            {
                // Incrementa cidr per ogni bit a 1 nella maschera di sottorete
                cidr += Convert.ToString(b, 2).Count(ch => ch == '1');
            }
            return cidr;
        }


        public double GetTotalNumberOfHosts()
        {
            return Math.Pow(2, (32 - GetCIDR())) - 2;
        }

        
    }

    
}


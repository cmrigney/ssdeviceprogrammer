using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Device_Programmer
{
    public class DeviceListener
    {
        public static Task WaitForDeviceOfflineAsync(string deviceIp)
        {
            return Task.Factory.StartNew(() => WaitForDeviceOffline(deviceIp));
        }

        static void WaitForDeviceOffline(string deviceIp)
        {
            Ping pingSender = new Ping();
            int timeout = 5000;

            while (pingSender.Send(deviceIp, timeout).Status == IPStatus.Success)
            {
                //Nothing
            }
        }

        public static async Task<bool> MACFoundAtIpAsync(string ipAddress, string macAddress)
        {
            ArpRequester arp = new ArpRequester(ipAddress);
            await arp.StartAsync();
            return (arp.MAC.Equals(macAddress, StringComparison.OrdinalIgnoreCase));
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        sealed class ArpRequester
        {
            string ipAddress;

            bool success = false;
            string resultMac = string.Empty;

            public bool Success { get { return success; } }
            public string MAC { get { return resultMac; } }
            public string IP { get { return ipAddress; } }

            public ArpRequester(string ip)
            {
                ipAddress = ip;
            }

            void Start()
            {
                StartInternal();
            }

            public Task StartAsync()
            {
                return Task.Factory.StartNew(() => StartInternal());
            }

            void StartInternal()
            {
                try
                {
                    IPAddress dst = IPAddress.Parse(ipAddress);

                    byte[] macAddr = new byte[6];
                    uint macLen = (uint)macAddr.Length;

                    if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macLen) != 0)
                    {
                        success = false;
                        return;
                    }

                    string[] str = new string[(int)macLen];
                    for (int i = 0; i < macLen; i++)
                        str[i] = macAddr[i].ToString("x2");

                    resultMac = string.Join(":", str);
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }
        }
    }
}
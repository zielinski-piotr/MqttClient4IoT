using System;
using System.Diagnostics;
using System.Device.Gpio;
using Device.Esp32;
using nanoFramework.Hardware.Esp32;
using System.Device.Wifi;
using System.Net.NetworkInformation;
using System.Threading;
using Shared.Mqtt.Client;

namespace Clients.Esp32
{
    public class Program
    {
        private static Led SuccessLed = null;
        private static Led FailureLed = null;
        private static Led WifiLed = null;

        public static void Main()
        {
            Debug.WriteLine("Application Starting");

            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            using GpioController gpioController = new GpioController();

            SuccessLed = new Led(Settings.SuccessLedSettings, gpioController);
            FailureLed = new Led(Settings.FailureLedSettings, gpioController);
            WifiLed = new Led(Settings.DebugLedSettings, gpioController);

            InitLeds();

            using var i2CBmp280 = new EspBmp280(Settings.I2CConnectionSettings);

            SetupAndConnectNetwork();

            using var mqttClient = new MqttClient(Settings.BrokerSettings);

            Debug.WriteLine($"Connection is open: {mqttClient.IsConnected}");

            var deviceService = new DeviceService(mqttClient, Settings.BrokerSettings, i2CBmp280, i2CBmp280, SuccessLed, FailureLed);

            deviceService.Execute();
        }

        private static void InitLeds()
        {
            SuccessLed.Off();
            FailureLed.On();
            WifiLed.Off();
        }

        private static void SetupAndConnectNetwork()
        {
            var connected = false;
            do
            {
                try
                {
                    var wifiAdapter = WifiAdapter.FindAllAdapters()[0];

                    wifiAdapter.ScanAsync();

                    var ipAddress = NetworkInterface.GetAllNetworkInterfaces()[0].IPv4Address;
                    var needToConnect = string.IsNullOrEmpty(ipAddress) || (ipAddress == "0.0.0.0");
                    while (needToConnect)
                    {
                        foreach (var network in wifiAdapter.NetworkReport.AvailableNetworks)
                        {
                            Debug.WriteLine($"Net SSID :{network.Ssid},  BSSID : {network.Bsid},  rssi : {network.NetworkRssiInDecibelMilliwatts},  signal : {network.SignalBars}");

                            if (network.Ssid == Settings.WifiSettings.SSID)
                            {

                                var result = wifiAdapter.Connect(network, WifiReconnectionKind.Automatic, Settings.WifiSettings.Password);

                                if (result.ConnectionStatus == WifiConnectionStatus.Success)
                                {
                                    Debug.WriteLine($"Connected to Wifi network {network.Ssid}.");
                                    needToConnect = false;
                                    WifiLed.On();
                                    break;
                                }

                                Debug.WriteLine($"Error {result.ConnectionStatus} connecting to Wifi network {network.Ssid}.");
                            }
                        }

                        Thread.Sleep(10000);
                    }

                    ipAddress = NetworkInterface.GetAllNetworkInterfaces()[0].IPv4Address;
                    Debug.WriteLine($"Connected to Wifi network with IP address {ipAddress}");
                    connected = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WIFI adapter scanning failed with exception :'{ex.Message}'");
                }

            }
            while (!connected);
        }
    }
}


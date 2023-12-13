# MqttClient4IoT

MQTTClient4IoT is a sample project that allows you to understand how the MQTT protocol works and how to implement the MQTT client in your application using the shelf client libraries.

This project includes two solutions:

1. .NET solution explained in [README.MD](.NET/README.md)
2. nanoFramework solution explained in [README.MD](nanoFramework/README.md)

## .NET Solution

.NET solution shows how to use the MQTTNet library to implement a MQTT client that can be deployed on any platform that supports the full .NET platform.
You will find details of how to set up the devices used in the example, including how to set up Raspberry PI Zero 2W to read measurements from the BMP280 sensor and how to use Raspberry PI 4B with the Sense Hat.

If you do not have the boards in your workshop, you can still use this solution with mocked sensors on your regular development machine.

There is also a Docker Compose file that can be used to launch the whole solution with minimal effort.

## nanoFramework Solution

With this solution you will learn how to use [nanoFramework](https://www.nanoframework.net) in order to deploy applications written in C#. 
.NET nanoFramework is a framework that you can use to write C# code for embedded systems.
It has some limitations compared to its big brother, but the basic principles are almost the same.

This is a guide to setting up the ESP32 device to read pressure and temperature from the BMP280 sensor and send them to any subscribing clients via the MQTT protocol.
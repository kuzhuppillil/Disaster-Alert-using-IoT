# Disaster-Alert-using-IoT
Disaster Alert - Using IoT

This project is based on IoT, and establishing it for a good cause, that is for Diaster Alert and Rescue Aid (DARA)


Now a bit of explaination�

It�s a communication system using ISM bands, can be used in situations like natural calamities where, Oh.. The internet stopped working! Wait no reception? And the landline is also dead! Yeah a bit too much but you get the picture.

Now the overall system consists of a base station,stationary devices and few wearable bands (only one for now). The base station will alert disaster information to all stationary devices using the DRF1278DM (LoRa) which is a low-cost sub-1 GHz transceiver module. The stationary devices receives information and gives vocal/visual alert to individuals and also send location information back (acting like a beacon) along with their heart rate (BPM) recorded from the wearable bands using a Pulse sensor (Photoplethysmograph). The same DRF1278DM transceiver module is used to send the data back to base station.
Thus providing necessary information to prioritize and manage rescue operation.


HARDWARE USED:

Raspberry Pi 2 Model B, Arduino Nano, LoRa (DRF1278DM), BLE (HM 10), Pulse Sensor (Plethysmograph)

SOFTWARE USED:

C#, Arduino (IDE)

Operating system : Windows 10 IoT core




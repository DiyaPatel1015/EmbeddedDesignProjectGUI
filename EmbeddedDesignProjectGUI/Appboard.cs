using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace EmbeddedDesignProjectGUI
{
    class Appboard
    {
        private SerialPort serialPort;

        private const byte START = 0x53;
        private const byte STOP = 0xAA;
        private const byte RESET_COMMAND = 0xFF;

        private const byte READ_PINA = 0x01;
        private const byte READ_POT1 = 0x02;
        private const byte READ_POT2 = 0x03;
        private const byte READ_TEMP = 0x04;
        private const byte READ_LIGHT = 0x05;

        private const byte SET_PORTC = 0x0A;
        private const byte SET_HEATER = 0x0B;
        private const byte SET_LIGHT = 0x0C;
        private const byte SET_MOTOR = 0x0D;

        public Appboard()
        {
            // Initialize the serial port
            serialPort = new SerialPort();
        }

        public void InitializeSerialPort(string portName, int baudRate)
        {
            // Set serial port settings based on user input
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = 8; // Set the number of data bits
            serialPort.Parity = Parity.None; // Set the parity
            serialPort.StopBits = StopBits.One; // Set the number of stop bits
        }

        public bool ConnectSerialPort()
        {
            try
            {
                // Open the serial port
                serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error opening serial port: {ex.Message}");
                return false;
            }
        }

        public void DisconnectSerialPort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error closing serial port: {ex.Message}");
            }
        }

        public int WriteSerial(byte instruction, ushort value = 0)
        {
            if (!serialPort.IsOpen)
            {
                Console.Error.WriteLine("Error: Serial port is not open.");
                return -1;
            }

            byte lsb = (byte)(value & 0xFF); // Extract the least significant byte
            byte msb = (byte)((value >> 8) & 0xFF); // Extract the most significant byte
            byte[] byteArray = { START, instruction, lsb, msb, STOP };

            try
            {
                serialPort.Write(byteArray, 0, byteArray.Length);
                Stopwatch sw = Stopwatch.StartNew();

                while (serialPort.BytesToRead < 1)
                {
                    if (sw.ElapsedMilliseconds > 500)
                    {
                        throw new TimeoutException("No reply received within timeout period.");
                    }
                }

                int reply = serialPort.ReadByte();

                if (instruction >= 0x0A) // set instruction
                {
                    if (reply != instruction)
                    {
                        Console.Error.WriteLine("Mismatch in set instruction reply. Retrying...");
                        return WriteSerial(instruction, value); // Retry
                    }
                    return -1;
                }
                else // read instruction
                {
                    return reply; // return the data
                }
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error: {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in WriteSerial: {ex.Message}");
                return -1;
            }
        }

        // ******** Read from MCU ********//
        public byte ReadPINA()
        {
            int PinA_byte = WriteSerial(READ_PINA);
            return (byte)PinA_byte;
        }

        public int ReadPot1Value()
        {
            return ReadPotV(READ_POT1);
        }

        public int ReadPot2Value()
        {
            return ReadPotV(READ_POT2);
        }

        public int ReadPotV(int potNumber)
        {
            byte potNumberByte = (byte)potNumber; // Explicitly cast potNumber to byte
            int potValue = WriteSerial(potNumberByte); // Pass the byte value to WriteSerial
            return potValue;
        }

        public byte ReadTemp()
        {
            int tempSensor = WriteSerial(READ_TEMP);
            return (byte)tempSensor;
        }

        public byte ReadLight()
        {
            int Light_byte = WriteSerial(READ_LIGHT);
            return (byte)Light_byte;
        }

        //******** Write to MCU ********//
        public void WritePortC(byte data)
        {
            WriteSerial(SET_PORTC, data);
        }

        public void WriteHeater(ushort heaterValue)
        {
            WriteSerial(SET_HEATER, heaterValue);
        }

        public void WriteLight(ushort lightValue)
        {
            WriteSerial(SET_LIGHT, lightValue);
        }

        public void WriteMotor(ushort motorValue)
        {
            WriteSerial(SET_MOTOR, motorValue);
        }

        //******** Reset MCU ********//
        public void ResetMCU()
        {
            if (serialPort.IsOpen)
            {
                WriteSerial(RESET_COMMAND);
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }
    }
}

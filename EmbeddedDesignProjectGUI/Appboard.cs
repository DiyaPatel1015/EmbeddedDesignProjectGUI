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

        public int writeSerial(byte instruction, byte lsb = 0, byte msb = 0)
        {
            if (!serialPort.IsOpen)
            {
                Console.Error.WriteLine("Error: Serial port is not open.");
                return -1;
            }
            byte[] byteArray = { START, instruction, lsb, msb, STOP };
            serialPort.Write(byteArray, 0, byteArray.Length);
            while (serialPort.BytesToRead < 1) ;
            int reply = serialPort.ReadByte();
            if (instruction >= 0x0A) // set instruction
            {
                if (reply != instruction)
                {
                    writeSerial(instruction, lsb, msb);
                }
                return 0;
            }
            else // read instruction
            {
                return reply; // return the data
            }
        }

            


        // ******** Read from MCU ********//
        public byte ReadPINA()
        {
            int PinA_byte = writeSerial(READ_PINA);
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
            int potValue = writeSerial(potNumberByte); // Pass the byte value to WriteSerial
            return potValue;
        }

        public byte ReadTemp()
        {
            int tempSensor = writeSerial(READ_TEMP);
            return (byte)tempSensor;
        }

        public byte ReadLight()
        {
            int Light_byte = writeSerial(READ_LIGHT);
            return (byte)Light_byte;
        }

        //******** Write to MCU ********//
        public void WritePortC(byte data)
        {
            writeSerial(SET_PORTC, data);
        }

        public void WriteHeater(int heaterValue)
        {
            serialPort.DiscardInBuffer();
            var heaterbytes = BitConverter.GetBytes(heaterValue); //converting the int into and array of bytes
            byte[] c = { START, SET_LIGHT, heaterbytes[0], heaterbytes[1], STOP };
            serialPort.Write(c, 0, c.Length);
        }

        public void WriteLight(int lightValue)
        {
            serialPort.DiscardInBuffer();
            var lightbytes = BitConverter.GetBytes(lightValue); //converting the int into and array of bytes
            byte[] c = { START, SET_LIGHT, lightbytes[0], lightbytes[1], STOP };
            serialPort.Write(c, 0, c.Length);
        }

        public void WriteMotor(int motorValue)
        {
            serialPort.DiscardInBuffer();
            var motorbytes = BitConverter.GetBytes(motorValue); //converting the int into and array of bytes
            byte[] c = { START, SET_LIGHT, motorbytes[0], motorbytes[1], STOP };
            serialPort.Write(c, 0, c.Length);
        }


        //******** Reset MCU ********//
        public void ResetMCU()
        {
            if (serialPort.IsOpen)
            {
                writeSerial(RESET_COMMAND);
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }
    }
}

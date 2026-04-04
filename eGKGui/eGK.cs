using PCSC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace eGKGui;


internal class HealthCardData
{
    public string InsuranceId { get; set; }
    public string Birthdate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Sex { get; set; }
    public string Postcode { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string Country { get; set; }
}


internal sealed class EgkReader
{
    private static readonly byte[] SELECT_MF = { 0x00, 0xA4, 0x04, 0x0C, 0x07, 0xD2, 0x76, 0x00, 0x01, 0x44, 0x80, 0x00 };
    private static readonly byte[] SELECT_HCA = { 0x00, 0xA4, 0x04, 0x0C, 0x06, 0xD2, 0x76, 0x00, 0x00, 0x01, 0x02 };
    private static readonly byte[] SELECT_FILE_PD = { 0x00, 0xB0, 0x81, 0x00, 0x02 };

    private readonly ICardReader _reader;

    public EgkReader(ICardReader reader)
    {
        _reader = reader;
    }

    private byte[] CreateReadCommand(int pos, int length)
    {
        return new byte[]
        {
          0x00, 0xB0,
          (byte)((pos >> 8) & 0xFF),
          (byte)(pos & 0xFF),
          (byte)length
        };
    }

    private byte[] RunCommand(byte[] apdu)
    {
        var receiveBuffer = new byte[256 + 2]; // data + SW1 + SW2                                                                                                                                                                                                                                                
        var sendPci = SCardPCI.T1;             // adjust to T0/T1 as needed                                                                                                                                                                                                                                       

        int received = _reader.Transmit(sendPci, apdu, receiveBuffer);

        byte sw1 = receiveBuffer[received - 2];
        byte sw2 = receiveBuffer[received - 1];

        if (sw1 != 0x90 || sw2 != 0x00)
            throw new Exception($"Bad Status: SW1={sw1:X2} SW2={sw2:X2}");

        return receiveBuffer.Take(received - 2).ToArray();
    }

    private byte[] ReadFile(int offset, int length)
    {
        const int maxRead = 0xFC;
        var data = new List<byte>();
        int pointer = offset;

        while (data.Count < length)
        {
            int bytesLeft = length - data.Count;
            int readLen = Math.Min(bytesLeft, maxRead);
            byte[] chunk = RunCommand(CreateReadCommand(pointer, readLen));
            data.AddRange(chunk);
            pointer += readLen;
        }

        return data.ToArray();
    }

    public HealthCardData GetData()
    {
        // Select MF
        RunCommand(SELECT_MF);

        // Select HCA and PD file                                                                                                                                                                                                                                                                                 
        RunCommand(SELECT_HCA);
        RunCommand(SELECT_FILE_PD);

        // Read first 2 bytes to get PD length                                                                                                                                                                                                                                                                    
        byte[] lengthBytes = RunCommand(CreateReadCommand(0x00, 0x02));
        int pdLength = (lengthBytes[0] << 8) + lengthBytes[1] - 0x02;
        Debug.WriteLine($"Length of personal data: {pdLength} bytes");

        // Re-select and read compressed data starting at offset 2                                                                                                                                                                                                                                                
        RunCommand(SELECT_MF);
        RunCommand(SELECT_HCA);
        RunCommand(SELECT_FILE_PD);

        byte[] compressed = ReadFile(0x02, pdLength);

        // Append 16 zero bytes (as in the Python code)                                                                                                                                                                                                                                                           
        compressed = compressed.Concat(new byte[16]).ToArray();

        // Decompress (gzip / zlib with wbits=15+16)                                                                                                                                                                                                                                                              
        string xml = DecompressGzip(compressed);
        Debug.WriteLine(xml);

        return ParsePersonalData(xml);
    }

    private static string DecompressGzip(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }
    private static string ExtractFromNode(XmlDocument doc, string nodename)
    {
        XmlNodeList baseData = doc.GetElementsByTagName(nodename);
        if (baseData.Count == 0)
        {
            return "N/A";
        }
        else if (baseData.Count > 1)
        {
            return "multiple";
        }
        else
        {
            XmlNode item = baseData.Item(0);
            return item.InnerText;

        }
    }
    private static HealthCardData ParsePersonalData(string xmlString)
    {
        var result = new HealthCardData();
        var doc = new XmlDocument();
        doc.LoadXml(xmlString);


        Debug.WriteLine($"Versicherten_ID: {ExtractFromNode(doc, "Versicherten_ID")}");
        Debug.WriteLine($"Geburtsdatum: {ExtractFromNode(doc, "Geburtsdatum")}");

        result.InsuranceId = ExtractFromNode(doc, "Versicherten_ID");

        result.Birthdate = ExtractFromNode(doc, "Geburtsdatum");
        result.FirstName = ExtractFromNode(doc, "Vorname");
        result.LastName = ExtractFromNode(doc, "Nachname");
        result.Sex = ExtractFromNode(doc, "Geschlecht");

        result.Postcode = ExtractFromNode(doc, "Postleitzahl");
        result.City = ExtractFromNode(doc, "Ort");
        result.Street = ExtractFromNode(doc, "Strasse");
        result.HouseNumber = ExtractFromNode(doc, "Hausnummer");
        result.Country = ExtractFromNode(doc, "Wohnsitzlaendercode");
 



        return result;
    }
}

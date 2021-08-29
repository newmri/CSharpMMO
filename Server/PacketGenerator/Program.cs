using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (XmlReader reader = XmlReader.Create("PDL.xml", settings))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                        ParsePacket(reader);
                }

                File.WriteAllText("GenPackets.cs", genPackets);
            }
        }

        static string genPackets;

        public static void ParsePacket(XmlReader reader)
        {
            if (XmlNodeType.EndElement == reader.NodeType)
                return;

            if ("packet" != reader.Name.ToLower())
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = reader["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> tuple = ParseMembers(reader);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, tuple.Item1, tuple.Item2, tuple.Item3);
        }

        public static Tuple<string, string, string> ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = reader.Depth + 1;
            while (reader.Read())
            {
                if (reader.Depth != depth)
                    break;

                string memberName = reader["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (!string.IsNullOrEmpty(memberCode))
                    memberCode += Environment.NewLine;

                if (!string.IsNullOrEmpty(readCode))
                    readCode += Environment.NewLine;

                if (!string.IsNullOrEmpty(writeCode))
                    writeCode += Environment.NewLine;

                string memberType = reader.Name.ToLower();
                switch (memberType)
                {
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> tuple = ParseList(reader);
                        memberCode += tuple.Item1;
                        readCode += tuple.Item2;
                        writeCode += tuple.Item3;
                        break;
                    default:
                        break;

                }
            }

            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader reader)
        {
            string listName = reader["name"];
            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> tuple = ParseMembers(reader);

            string memberCode = string.Format(PacketFormat.memberListFormat, FirstCharToUpper(listName), FirstCharToLower(listName), tuple.Item1, tuple.Item2, tuple.Item3);
            string readCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}

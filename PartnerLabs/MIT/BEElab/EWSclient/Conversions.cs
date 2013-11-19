using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using CommonTypes;                   // SOAPVersions enum
using System.ServiceModel.Channels;  // System.ServiceModel.Channels.MessageVersion

namespace CommonFunctions.Conversion
{
    /// <summary>
    /// Includes support methods for type conversation
    /// </summary>
    public class Conversions
    {
        /// <summary>
        /// Encodes a given byte[] to Base64. Three replacements are made to make the resulting string URL-safe:
        /// + => - ; / => _ ; = => .
        /// If the string will not be decoded and is only used to create an unique identifier, 
        /// the padding can be omitted by setting the second paramter to TRUE.
        /// 
        /// Taken (but modified) from http://arcanecode.com/2007/03/21/encoding-strings-to-base64-in-c
        /// </summary>
        /// <param name="toEncode">byte[] to be encoded</param>
        /// <param name="nopadding">if TRUE, Base64-padding will be omitted</param>
        /// <returns>Base64-representation of the byte[]</returns>
        public static string Base64Encode(byte[] toEncode, bool nopadding)
        {
            string output = System.Convert.ToBase64String(toEncode).Replace("+", "-").Replace("/", "_").Replace("=", ".");
            if (nopadding)
            {
                return output.Replace(".", "");
            }
            else
            {
                return output;
            }
        }

        /// <summary>
        /// Decodes a given Base64-string back to byte[]. This function will replace characters used in URL-safe 
        /// Base64-strings back to the normal ones but can also handle "normal" Base64-strings.
        /// 
        /// Taken (but modified) from http://arcanecode.com/2007/03/21/encoding-strings-to-base64-in-c
        /// </summary>
        /// <param name="encodedData">Base64-encoded string to be decoded</param>
        /// <returns>Decoded representation of the Base64-encoded string</returns>
        public static byte[] Base64Decode(string encodedData)
        {
            return System.Convert.FromBase64String(encodedData.Replace("-", "+").Replace("_", "/").Replace(".", "="));
        }

        /// <summary>
        /// Converts different boolean strings to an integer value 
        /// </summary>
        /// <param name="booleanString">String with the boolean value</param>
        /// <returns>Returns boolean value as integer</returns>
        public static int BooleanToInt(string booleanString)
        {
            int result = 0;
            if (booleanString.Contains("0") || booleanString.Contains("false") || booleanString.Contains("FALSE"))
            {
                result = 0;
            }
            else if (booleanString.Contains("1") || booleanString.Contains("true") || booleanString.Contains("TRUE"))
            {
                result = 1;
            }
            return result;
        }

        /// <summary>
        /// Converts different boolean strings to an integer value 
        /// </summary>
        /// <param name="booleanString">String with the boolean value</param>
        /// <param name="doStrictContentProcessing">Check tolerance level</param>
        /// <param name="id"> of the item ==> ONLY FOR EXECPTION AND LOGGING PURPOSE</param>
        /// <returns>Returns boolean value as integer</returns>
        public static int BooleanToInt(string booleanString, Boolean doStrictContentProcessing, string id)
        {
            int result = 0;

            if (booleanString == null)
            {
                if (doStrictContentProcessing)
                {
                    throw new Exception(String.Format("Parameter is not Specification conform\nElement: {0}\nContent processing error: NULL value (boolean converting)\n\nUse Paramter NO STRICT CONTENT PROCESSING", id));  
                }
                else
                {
                    result = 0;
                }
            }
            else if (booleanString.Contains("0") || booleanString.Contains("false") || booleanString.Contains("FALSE"))
            {
                result = 0;
            }
            else if (booleanString.Contains("1") || booleanString.Contains("true") || booleanString.Contains("TRUE"))
            {
                result = 1;
            }
            return result;
        }

        /// <summary>
        /// Compress a byte[] using GZipStream
        /// Taken from "http://atakala.com/browser/Item.aspx?user_id=amos&amp;dict_id=1980"
        /// </summary>
        /// <param name="input">byte[] to be compressed</param>
        /// <returns>Compressed representation of input</returns>
        public static byte[] Compress(byte[] input)
        {
            byte[] output;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.Compression.GZipStream gs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
                {
                    gs.Write(input, 0, input.Length);
                    gs.Close();
                    output = ms.ToArray();
                }
                ms.Close();
            }
            return output;
        }

        /// <summary>
        /// UNCompress a byte[] using GZipStream
        /// 
        /// Taken from: http://atakala.com/browser/Item.aspx?user_id=amos&amp;dict_id=1980
        /// </summary>
        /// <param name="input">byte[] to be uncompressed</param>
        /// <returns>Uncompressed representation of input</returns>
        public static byte[] Decompress(byte[] input)
        {
            System.Collections.Generic.List<byte> output = new List<byte>();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(input))
            {
                using (System.IO.Compression.GZipStream gs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    int readByte = gs.ReadByte();
                    while (readByte != -1)
                    {
                        output.Add((byte)readByte);
                        readByte = gs.ReadByte();
                    }
                    gs.Close();
                }
                ms.Close();
            }
            return output.ToArray();
        }

        /// <summary>
        /// Converts our CommonTypes.SOAPVersions (used in EwsEndpointInformation) to 
        /// System.ServiceModel.Channels.MessageVersion used by the reference client
        /// </summary>
        /// <param name="SOAPVersion">SOAPVersion</param>
        /// <returns>MessageVersion</returns>
        public static MessageVersion SOAPVersionToMessageVersion(SOAPVersions SOAPVersion)
        {
            switch (SOAPVersion)
            {
                case SOAPVersions.Soap11: return MessageVersion.Soap11;
                case SOAPVersions.Soap11WSAddressing10: return MessageVersion.Soap11WSAddressing10;
                case SOAPVersions.Soap11WSAddressingAugust2004: return MessageVersion.Soap11WSAddressingAugust2004;
                case SOAPVersions.Soap12: return MessageVersion.Soap12;
                case SOAPVersions.Soap12WSAddressing10: return MessageVersion.Soap12WSAddressing10;
                case SOAPVersions.Soap12WSAddressingAugust2004: return MessageVersion.Soap12WSAddressingAugust2004;
                default: return MessageVersion.Soap12;
            }
        }
    }
}

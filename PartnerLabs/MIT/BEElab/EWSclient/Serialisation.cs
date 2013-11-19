using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace CommonFunctions.Serialisation
{
    /// <summary>
    /// This class includes seralisiation operations (serialisation/deserialisation)
    /// needed in different parts of the software
    /// </summary>
    public static class SerializationContext
    {
        #region Methods
        /// <summary>
        /// This method serialised objects of a given type to a text
        /// </summary>
        /// <param name="obj">Type object | A serialisable object</param>
        /// <param name="type">Type Type | Type of the object</param>
        /// <returns>Type string | Text including a serialisable object</returns>
        public static string Serialize(object obj, Type type)
        {
            // A text writer will be created...
            using (TextWriter tex = new StringWriter())
            {
                // ... using a serialiser...
                var serializer1 = new XmlSerializer(type);
                //... to serialise an object...
                serializer1.Serialize(tex, obj);
                // ... and retrun is as string
                return tex.ToString();
            }

            //ToDo: Exception handling missing
            //Optional: Reduce parameter to 1 (object obj) by using typeof)
        }

        /// <summary>
        /// This method deserialise a given serialised object (text)
        /// into an object.
        /// </summary>
        /// <param name="serialisedObject">Type string | A serialised object</param>
        /// <param name="type">Type Type | Original type of the serialised object</param>
        /// <returns>Type Object | The original object</returns>
        public static object Deserialize(string serialisedObject, Type type)
        {
            // First create a return value
            object returnValue = null;

            // If the string includes information...
            if (String.IsNullOrEmpty(serialisedObject) == false)
            {
                // ... a text reader will be used...
                using (TextReader reader = new StringReader(serialisedObject))
                {
                    // ... to deserialise an object...
                    var serializer = new XmlSerializer(type);
                    // ... and store it into the returnValue object
                    returnValue = serializer.Deserialize(reader);
                }
            }

            // Return null or the deserialised object
            return returnValue;

            //Todo: Exception Handling is missing
            //Optional: Perhaps it is possible to use a <T> as return value instead of an object
        }

        /// <summary>
        /// Simple method to change a string into an byte array
        /// </summary>
        /// <param name="str">Type String | The string content for creating the byte array</param>
        /// <returns>Type byte[] | An ASCII encoded byte array</returns>
        public static byte[] StringToByteArray(string str)
        {
            // The ASCII encoder is choosen...
            var enc = new System.Text.ASCIIEncoding();
            // ...to encode the given string into a byte[]
            return enc.GetBytes(str);

            //Todo: Exception Handling is missing
        }

        /// <summary>
        /// Simple method to create a string from a given byte[].
        /// </summary>
        /// <param name="arr">Type byte[] | The byte[] for transformation</param>
        /// <returns>Byte array in string format.</returns>
        public static string ByteArrayToString(byte[] arr)
        {
            // The ASCII encoder is choosen...
            var enc = new System.Text.ASCIIEncoding();
            // to decode the bytearray into a string
            return enc.GetString(arr);

            //Todo: Exception Handling is missing
        }
        #endregion
    }
}

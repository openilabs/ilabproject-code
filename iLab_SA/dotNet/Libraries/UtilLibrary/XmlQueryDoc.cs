using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace iLabs.UtilLib
{
	/// <summary>
	/// A wrapped XmlDocument.
	/// </summary>
	public class XmlQueryDoc
	{
		XPathDocument pathDocument = null;
		public XPathNavigator pathNavigator = null;


		public XmlQueryDoc()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public XmlQueryDoc(String input)
		{
			UTF8Encoding utf8 = new UTF8Encoding();
 			MemoryStream mStream = new MemoryStream(utf8.GetBytes(input),false);

			pathDocument = new XPathDocument(mStream);
            //pathDocument.Load(mStream);
			pathNavigator = pathDocument.CreateNavigator();
		}

		// Get all the book prices
		//Query( "descendant::book/price");

		// Get the ISBN of the last book
		// "bookstore/book[3]/@ISBN");
        /// <summary>
        /// Returns the values within the queried element, only should be used with a single element without children as only the text is returned.
        /// </summary>
        /// <param name="xpathexpr"></param>
        /// <returns></returns>
		public string Query( string xpathexpr )
		{
			StringBuilder buf = new StringBuilder();
			try
			{
				// Create a node interator to select nodes and move through them (read-only)
				XPathNodeIterator pathNodeIterator =  pathNavigator.Select (xpathexpr);

				while (pathNodeIterator.MoveNext())
				{
					buf.Append(pathNodeIterator.Current.Value);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("Exception: {0}", e.ToString());
			}
			return buf.ToString();
		}

        /// <summary>
        /// Test to see if the requested node is found.
        /// </summary>
        /// <param name="xpathexpr"></param>
        /// <returns></returns>
        public bool Found(string xpathexpr)
        {
            bool status = false;
            try
            {
                // Create a node interator to select nodes, if found set true
                XPathNodeIterator pathNodeIterator = pathNavigator.Select(xpathexpr);
                if (pathNodeIterator != null && pathNodeIterator.Count > 0)
                {
                    status = true;
                }
            }
            catch (XPathException e)
            {
                status = false;
            }
            return status;
        }
        //public bool Insert(string xpathexpr, string item, string value)
        //{
        //    bool status = false;
        //    try
        //    {
        //        // Create a node interator to select nodes and move through them (read-only)
        //        XPathNodeIterator pathNodeIterator = pathNavigator.Select(xpathexpr);
        //        if (pathNodeIterator != null && pathNodeIterator.Count > 0)
        //        {
                     
        //            pathNavigator.InsertAfter("<" + item + ">" + value + "</" + item + ">");
        //             //pathNavigator.AppendChildElement(null,item,null,value);
        //            //pathNodeIterator.Current.AppendChild( "<" +item + ">" + value + "</" +item + ">");
        //            //pathNavigator.AppendChild( "<" +item + ">" + value + "</" +i tem + ">");
                   
        //            status = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception: {0}", e.ToString());
        //    }
        //    return status;
        //}

        //public bool Replace(string xpathexpr,  string value)
        //{
        //    bool status = false;
        //    try
        //    {
        //        // Create a node interator to select nodes and move through them (read-only)
        //        XPathNodeIterator pathNodeIterator = pathNavigator.Select(xpathexpr);
        //        if (pathNodeIterator != null && pathNodeIterator.Count > 0)
        //        {
        //            pathNavigator.SetValue(value);
        //            status = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception: {0}", e.ToString());
        //    }
        //    return status;
        //}


        /// <summary>
        /// Create a node interator to select nodes and move through them (read-only)
        /// </summary>
        /// <param name="xpathexpr"></param>
        /// <returns></returns>
        public XPathNodeIterator Select(string xpathexpr)
        {
            XPathNodeIterator pathNodeIterator = null;
            try
            {
                // Create a node interator to select nodes and move through them (read-only)
                pathNodeIterator = pathNavigator.Select(xpathexpr);
                
            }
            catch (XPathException e)
            {
                Trace.WriteLine("XPath not found: " + xpathexpr); 
            }
            return pathNodeIterator;
        }

        //public string Select(string xpathexpr)
        //{
        //    StringBuilder buf = new StringBuilder();
        //    try
        //    {

        //        // Create a node interator to select all child nodes
        //        XPathNodeIterator pathNodeIterator = pathNavigator.Select(xpathexpr);
        //        buf.Append(pathNodeIterator.Current.InnerXml);
                
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception: {0}", e.ToString());
        //    }
        //    return buf.ToString();
        //}

        /// <summary>
        /// This does not work
        /// </summary>
        /// <returns></returns>
        public string GetTopName(){
            string str;
            pathNavigator.MoveToRoot();
            pathNavigator.MoveToFirstChild();
            str = pathNavigator.Name;
           return str;
        }

       

        /// <summary>
        /// This does not work
        /// </summary>
        /// <param name="xpathexpr"></param>
        /// <returns></returns>
        public string LocalName(string xpathexpr)
        {
            string name = null;
            try
            {
                // Create a node interator to select nodes and move through them (read-only)
                XPathNodeIterator iter = pathNavigator.Select(xpathexpr);
                name = iter.Current.LocalName;
            
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
            return name;

        }

        public string ToXML()
        {
            if (pathDocument == null)
                return null;
            else{
                 pathNavigator.MoveToRoot();
            pathNavigator.MoveToFirstChild();
            return pathNavigator.OuterXml;
             
                //return pathDocument.DocumentElement.OuterXml;
            }
        }
        
        /// <summary>
        /// Walks the XPathNavigator tree recursively
        /// </summary>
        /// <param name="myXPathNavigator"></param>
 
        public static void DisplayTree(XPathNavigator myXPathNavigator)
        {
            if (myXPathNavigator.HasChildren)
            {
                myXPathNavigator.MoveToFirstChild();

                Format(myXPathNavigator);
                DisplayTree(myXPathNavigator);

                myXPathNavigator.MoveToParent();
            }
            while (myXPathNavigator.MoveToNext())
            {
                Format(myXPathNavigator);
                DisplayTree(myXPathNavigator);
            }
        }


        /// <summary>
        /// Format the output
        /// </summary>
        /// <param name="myXPathNavigator"></param>
        private static void Format(XPathNavigator myXPathNavigator)
        {
            if (!myXPathNavigator.HasChildren)
            {
                if (myXPathNavigator.NodeType == XPathNodeType.Text)
                    Console.WriteLine(myXPathNavigator.Value);
                else if (myXPathNavigator.Name != String.Empty)
                    Console.WriteLine("<" + myXPathNavigator.Name + ">");
                else
                    Console.WriteLine();
            }
            else
            {
                Console.WriteLine("<" + myXPathNavigator.Name + ">");

                // Show the attributes if there are any
                if (myXPathNavigator.HasAttributes)
                {
                    Console.WriteLine("Attributes of <" + myXPathNavigator.Name + ">");

                    while (myXPathNavigator.MoveToNextAttribute())
                        Console.Write("<" + myXPathNavigator.Name + "> " + myXPathNavigator.Value + " ");

                    // Return to the 'Parent' node of the attributes
                    myXPathNavigator.MoveToParent();
                }
            }
        }



	}
}

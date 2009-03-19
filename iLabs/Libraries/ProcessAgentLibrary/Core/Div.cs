
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace iLabs.Core
{
    public class Div : System.Web.UI.Control
    {
        private string m_KeyCssClass = "CssClass";
        private string m_KeyDivId = "Id";
        private string m_KeyHeight = "Height";
        private string m_KeyText = "Text";
        private string m_KeyWidth = "Width";

        public Div()
        {          
        }

        public string CssClass
        {
            get
            {
                if ((object)ViewState[m_KeyCssClass] == null) return "";
                return (string)ViewState[m_KeyCssClass];
            }

            set { ViewState[m_KeyCssClass] = value; }
        }

        public string DivId
        {
            get
            {
                if ((object)ViewState[m_KeyDivId] == null) return "";
                return (string)ViewState[m_KeyDivId];
            }

            set { ViewState[m_KeyDivId] = value; }
        }

        public int Height
        {
            get
            {
                if ((object)ViewState[m_KeyHeight] == null) return 0;
                return (int)ViewState[m_KeyHeight];
            }

            set { ViewState[m_KeyHeight] = value; }
        } 

        public string Text
        {
            get
            {
                if ((object)ViewState[m_KeyText] == null) return "";
                return (string)ViewState[m_KeyText];
            }

            set { ViewState[m_KeyText] = value; }
        }

        public int Width
        {
            get
            {
                if ((object)ViewState[m_KeyWidth] == null) return 0;
                return (int)ViewState[m_KeyWidth];
            }

            set { ViewState[m_KeyWidth] = value; }
        } 

        public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
        {
            HtmlGenericControl DivObj;

            DivObj = new HtmlGenericControl("div");          
            if( CssClass.Length > 0 ) DivObj.Attributes.Add("class", CssClass);
            if( Height > 0 ) DivObj.Attributes.Add("height", "" + Height);
            if( Width > 0 ) DivObj.Attributes.Add("width", "" + Width);
            if (Text.Length > 0) DivObj.InnerText = Text;
            Controls.Add(DivObj);
            base.RenderControl(writer); 	                 
        }
    }

}

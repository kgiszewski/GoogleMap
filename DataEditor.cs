using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml;

using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.BusinessLogic;

namespace uNews.GoogleMap
{
    /// <summary>
    /// This class is used for the actual datatype dataeditor, i.e. the control you will get in the content section of umbraco. 
    /// </summary>
    public class DataEditor : System.Web.UI.UpdatePanel, umbraco.interfaces.IDataEditor
    {

        private umbraco.interfaces.IData savedData;
        private Options savedOptions;
        private XmlDocument savedXML = new XmlDocument();
        private TextBox saveBox, searchBox;
        private HtmlGenericControl mapDiv = new HtmlGenericControl("div");
        private HtmlGenericControl searchDiv = new HtmlGenericControl("div");
        public string CurrentData = "";

        public DataEditor(umbraco.interfaces.IData Data, Options Configuration)
        {
            //load the prevalues
            savedOptions = Configuration;

            //ini the savedData object
            savedData = Data;
        }

        public virtual bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public bool ShowLabel
        {
            get { return true; }
        }

        public Control Editor { get { return this; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/uNewsGoogleMap/dataeditor.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "uNewsGoogleMapCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/uNewsGoogleMap/dataeditor.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "uNewsGoogleMapJS", js, false);

            string js2 = string.Format("<script src=\"{0}\" ></script>", "http://maps.googleapis.com/maps/api/js?sensor=true");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "uNewsGoogleMapAPI", js2, false);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "uNewsGoogleMapSaveBox";
            ContentTemplateContainer.Controls.Add(saveBox);

            searchDiv.Attributes["class"] = "uNewsGoogleMapControls";
            ContentTemplateContainer.Controls.Add(searchDiv);
            searchBox = new TextBox();
            searchBox.TextMode = TextBoxMode.MultiLine;
            searchBox.Attributes["class"] = "uNewsGoogleMapSearchBox";
            searchDiv.Controls.Add(searchBox);

            HtmlGenericControl searchButton = new HtmlGenericControl("input");
            searchDiv.Controls.Add(searchButton);
            searchButton.Attributes["type"] = "button";
            searchButton.Attributes["class"] = "uNewsGoogleMapSearchButton";
            searchButton.Attributes["value"] = "Search";

            ContentTemplateContainer.Controls.Add(mapDiv);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            buildControls();
        }

        protected void buildControls()
        {
            string data;

            //get the data based on action
            if (Page.IsPostBack)
            {
                data = saveBox.Text;
            }
            else
            {
                data = savedData.Value.ToString();
            }

            //this overrides everything
            if (CurrentData != "")
            {
                data = CurrentData;
            }

            //load the data into an xml doc
            XmlDocument xd = new XmlDocument();

            try
            {
                xd.LoadXml(data);
            }
            catch (Exception e)
            {
                xd.LoadXml(DefaultData.defaultXML);
            }

            //Log.Add(LogTypes.Custom, 0, "mapData=>" + xd.OuterXml);
            XmlNode googleMapXml = xd.SelectSingleNode("//googleMap");

            mapDiv.Attributes["class"] = "uNewsGoogleMapDiv";
            mapDiv.Attributes["lat"] = googleMapXml.SelectSingleNode("coords/@lat").Value;
            mapDiv.Attributes["lng"] = googleMapXml.SelectSingleNode("coords/@lng").Value;
            mapDiv.Attributes["zoom"] = googleMapXml.SelectSingleNode("zoom").InnerText;

            searchBox.Text = HttpUtility.HtmlDecode(googleMapXml.SelectSingleNode("address").InnerXml).Replace("<br />","\n");
        }

        public void Save()
        {
            savedData.Value = saveBox.Text;
        }

    }
}
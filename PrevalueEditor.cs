using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Xml;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;

using System.Web.Script.Serialization;

namespace uNews.GoogleMap
{
    /// <summary>
    /// This class is used to setup the datatype settings. 
    /// On save it will store these values (using the datalayer) in the database
    /// </summary>
    public class PrevalueEditor : System.Web.UI.UpdatePanel, IDataPrevalue
    {
        // referenced datatype
        private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;

        private TextBox saveBox, address, lat, lng, zoom;//, width, height;
        HtmlGenericControl settingsTable;

        private JavaScriptSerializer jsonSerializer;
        private Options savedOptions;

        public PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
        {
            _datatype = DataType;
            jsonSerializer = new JavaScriptSerializer();
            savedOptions = Configuration;
        }

        public Control Editor
        {
            get
            {
                return this;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            saveBox = new TextBox();
            saveBox.TextMode = TextBoxMode.MultiLine;
            saveBox.CssClass = "saveBox";
            ContentTemplateContainer.Controls.Add(saveBox);

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/uNewsGoogleMap/prevalue.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "DataTypePrevalueCSS", css, false);

            string js = string.Format("<script src=\"{0}\" ></script>", "/umbraco/plugins/uNewsGoogleMap/prevalue.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataEditor), "DataTypePrevalueJS", js, false);

            settingsTable = new HtmlGenericControl("table");
            settingsTable.Attributes["class"] = "settingsTable";
            ContentTemplateContainer.Controls.Add(settingsTable);

            HtmlGenericControl tr, th, td;

            tr = new HtmlGenericControl("tr");
            settingsTable.Controls.Add(tr);

            th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Address";
            th.Attributes["colspan"] = "2";
            th.InnerHtml = "Default Settings";

            //address
            tr = new HtmlGenericControl("tr");
            settingsTable.Controls.Add(tr);
            
            th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Address";

            address = new TextBox();
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Controls.Add(address);
            address.Attributes["class"] = "address";

            //lat
            tr = new HtmlGenericControl("tr");
            settingsTable.Controls.Add(tr);

            th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Lat";

            lat = new TextBox();
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Controls.Add(lat);
            lat.Attributes["class"] = "lat";

            //lng
            tr = new HtmlGenericControl("tr");
            settingsTable.Controls.Add(tr);

            th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Lng";

            lng = new TextBox();
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Controls.Add(lng);
            lng.Attributes["class"] = "lng";

            //zoom
            tr = new HtmlGenericControl("tr");
            settingsTable.Controls.Add(tr);

            th = new HtmlGenericControl("th");
            tr.Controls.Add(th);
            th.InnerHtml = "Zoom";

            zoom = new TextBox();
            td = new HtmlGenericControl("td");
            tr.Controls.Add(td);
            td.Controls.Add(zoom);
            zoom.Attributes["class"] = "zoom";

            //height
            //tr = new HtmlGenericControl("tr");
            //settingsTable.Controls.Add(tr);

            //th = new HtmlGenericControl("th");
            //tr.Controls.Add(th);
            //th.InnerHtml = "Height";

            //height = new TextBox();
            //td = new HtmlGenericControl("td");
            //tr.Controls.Add(td);
            //td.Controls.Add(height);
            //height.Attributes["class"] = "height";

            ////width
            //tr = new HtmlGenericControl("tr");
            //settingsTable.Controls.Add(tr);

            //th = new HtmlGenericControl("th");
            //tr.Controls.Add(th);
            //th.InnerHtml = "Width";

            //width = new TextBox();
            //td = new HtmlGenericControl("td");
            //tr.Controls.Add(td);
            //td.Controls.Add(width);
            //width.Attributes["class"] = "width";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Options renderingOptions;

            //test for postback, decide to use db or saveBox for rendering
            if (Page.IsPostBack)
            {
                //test for saveBox having a value, default if not
                if (saveBox.Text != "")
                {
                    renderingOptions = jsonSerializer.Deserialize<Options>(saveBox.Text);
                }
                else
                {
                    renderingOptions = new Options();
                }
            }
            else
            {
                renderingOptions = savedOptions;
            }

            address.Text = HttpUtility.UrlDecode(renderingOptions.address);
            lat.Text = renderingOptions.lat;
            lng.Text = renderingOptions.lng;
            zoom.Text = renderingOptions.zoom.ToString();
            //height.Text = renderingOptions.height.ToString();
            //width.Text = renderingOptions.width.ToString();            

        }

        public void Save()
        {
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), DBTypes.Ntext.ToString(), true);

            SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
            SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", saveBox.Text));
        }

        public Options Configuration
        {
            get
            {
                string dbValue = "";
                try
                {
                    object conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                    dbValue = conf.ToString();
                }
                catch (Exception e)
                {
                }

                if (dbValue.ToString() != "")
                {
                    return jsonSerializer.Deserialize<Options>(dbValue.ToString());
                }
                else
                {
                    return new Options();
                }
            }
        }

        public static ISqlHelper SqlHelper
        {
            get
            {
                return Application.SqlHelper;
            }
        }
    }
}
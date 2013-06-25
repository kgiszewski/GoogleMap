using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using umbraco.BusinessLogic;

namespace uNews.GoogleMap
{
    public class DefaultData : umbraco.cms.businesslogic.datatype.DefaultData
    {
        public static string defaultXML = "<googleMap><coords lat='40.718119' lng='-74.004135'/><zoom>16</zoom><address>Times Square, New York, NY</address></googleMap>";

        public DefaultData(umbraco.cms.businesslogic.datatype.BaseDataType DataType) : base(DataType) { }

        public override System.Xml.XmlNode ToXMl(System.Xml.XmlDocument data)
        {

            XmlDocument xd = new XmlDocument();
            try
            {
                xd.LoadXml(this.Value.ToString());
            }
            catch (Exception e)
            {
                xd.LoadXml(defaultXML);
            }

            return data.ImportNode(xd.DocumentElement, true);
        }
    }
}


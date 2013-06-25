using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using umbraco.cms.businesslogic.datatype;

namespace uNews.GoogleMap
{

    public class GoogleMap : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType
    {
        private umbraco.interfaces.IDataEditor _Editor;
        private umbraco.interfaces.IData _baseData;
        private PrevalueEditor _prevalueeditor;

        // Instance of the Datatype
        public override umbraco.interfaces.IDataEditor DataEditor
        {
            get
            {
                if (_Editor == null)
                    _Editor = new DataEditor(Data, ((PrevalueEditor)PrevalueEditor).Configuration);
                return _Editor;
            }
        }

        //this is what the cache will use when getting the data
        public override umbraco.interfaces.IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new DefaultData(this);
                return _baseData;
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get
            {
                return new Guid("b407d997-8d08-4eb8-8457-d70b1d4a9fa1");
            }
        }

        /// <summary>
        /// Gets the datatype unique id.
        /// </summary>
        /// <value>The id.</value>
        public override string DataTypeName
        {
            get
            {
                return "uNews Google Map";
            }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueeditor == null)
                {
                    _prevalueeditor = new PrevalueEditor(this);
                }
                return _prevalueeditor;
            }
        }
    }
}
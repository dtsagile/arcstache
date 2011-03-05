using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public abstract class InMemoryGISLayer: IGISLayer, IGISEditableLayer
    {
        List<InMemoryGISFeature> _features = new List<InMemoryGISFeature>();
        string _layerName;
        string _keyFieldName;
        int _index = -1;
        InMemoryGISFeature _current;

        public InMemoryGISLayer(string layerName, string keyFieldName)
        {
            _layerName = layerName;
            _keyFieldName = keyFieldName;
        }

        public InMemoryGISLayer(IEnumerable<InMemoryGISFeature> features, string layerName, string keyFieldName)
        {
            _features = new List<InMemoryGISFeature>(features);
            _layerName = layerName;
            _keyFieldName = keyFieldName;
        }
        public abstract InMemoryGISFeature CreateFeature();

        #region IGISLayer Members

        public string LayerName
        {
            get { return _layerName; }
        }

        public string KeyFieldName
        {
            get { return _keyFieldName; }
        }

        public IGISFeature Current
        {
            get
            {
                return _current;
            }
        }

        public bool MoveNext()
        {
            if (++_index >= _features.Count)
            {
                return false;
            }
            else
            {
                _current = _features[_index];
                return true;
            }
        }

        public void Search(string queryString)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGISXMLSerializable Members

        public void ToXML(System.Xml.XmlWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FromXML(System.Xml.XmlReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGISJSONSerializable Members

        public void ToJSON(Jayrock.Json.JsonTextWriter jwriter)
        {
            GeoJSONWriter.Write(this, jwriter);
        }

        public void FromJSON(Jayrock.Json.JsonTextReader jreader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGISEditableLayer Members

        public void Add(IGISFeature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature", "A valid feature is required to be added.");

            if (_features == null)
                throw new NullReferenceException("Cannot add feature to the InMemoryLayer. The internal list reference is NULL.");

            if(FindByKeyValue(_features, feature.Attributes.GetValue(KeyFieldName)) != null)
                throw new ArgumentException("Cannot add feature. A feature with the same key already exists.");

            InMemoryGISFeature newFeature = CreateFeature();
            _features.Add(newFeature);

            newFeature.Shape = feature.Shape;

            foreach (string key in feature.Attributes.GetKeys())
            {
                newFeature.Attributes.SetValue(key, feature.Attributes.GetValue(key));
            }
        }

        public void Update(IGISFeature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature", "A valid feature is required to be added.");

            if (_features == null)
                throw new NullReferenceException("Cannot update feature to the InMemoryLayer. The internal list reference is NULL.");

            if (FindByKeyValue(_features, feature.Attributes.GetValue(KeyFieldName)) != null)
                throw new ArgumentException("Cannot update feature. A feature with the same key already exists.");

            InMemoryGISFeature updateFeature = FindByKeyValue(_features, feature.Attributes.GetValue(KeyFieldName));
            if (updateFeature == null)
                throw new ArgumentException("Could not update the InMemoryLayer. The feature was not found in the internal list.");

            updateFeature.Shape = feature.Shape;

            foreach (string key in feature.Attributes.GetKeys())
            {
                updateFeature.Attributes.SetValue(key, feature.Attributes.GetValue(key));
            }
        }

        public void Delete(IGISFeature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature", "A valid feature is required to be added.");

            if (_features == null)
                throw new NullReferenceException("Cannot delete feature from the InMemoryLayer. The internal list reference is NULL.");

            InMemoryGISFeature mc = FindByKeyValue(_features, feature.Attributes.GetValue(KeyFieldName));

            if (mc != null)
            {
                _features.Remove(mc);
            }
        }

        #endregion

        private InMemoryGISFeature FindByKeyValue(List<InMemoryGISFeature> features, object id)
        {
            foreach (InMemoryGISFeature item in features)
            {
                if (id == item.Attributes.GetValue(KeyFieldName))
                    return item;
            }
            return null;
        }
    }
}

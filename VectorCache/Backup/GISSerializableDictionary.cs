using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DTS.Common;
using System.Xml;
using System.IO;
using Jayrock.Json;
using System.ComponentModel;

namespace DTS.Common.GIS
{
    public class GISSerializableDictionary: SerializableDictionary, IGISAttributes
    {
        #region IGISAttributes Members

        public object GetValue(string attribute)
        {
            return this.Dictionary[attribute];
        }

        public void SetValue(string attribute, object value)
        {
            this.Dictionary[attribute] = value;
        }

        public IEnumerable<string> GetKeys()
        {
            string[] keys = new string[this.Dictionary.Keys.Count];

            this.Dictionary.Keys.CopyTo(keys, 0);

            return keys;
        }

        #endregion

        #region IGISXMLSerializable Members

        public void ToXML(XmlWriter writer)
        {
            writer.WriteStartElement("Attributes");

            foreach (DictionaryEntry item in this.Dictionary)
            {
                writer.WriteStartElement("item");
                    //write the key
                    writer.WriteElementString("key", item.Key.ToString());
                    //write the qualified type name of the value
                    //write the value
                    if (item.Value == null)
                    {
                        writer.WriteElementString("type", null);
                        writer.WriteElementString("value", null);
                    }
                    else
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(item.Value.GetType());
                        writer.WriteElementString("type", item.Value.GetType().AssemblyQualifiedName);
                        writer.WriteElementString("value", typeConverter.ConvertToString(item.Value));
                    }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void FromXML(XmlReader reader)
        {
            Dictionary.Clear();
            if (reader.IsStartElement("Attributes"))
            {
                reader.ReadStartElement("Attributes");

                DictionaryEntry? de = null;
                while(reader.IsStartElement("item"))
                {
                    de = GetItem(reader);
                    if (de.HasValue)
                        Dictionary.Add(de.Value.Key, de.Value.Value);
                }

                reader.ReadEndElement();
            }
        }

        private DictionaryEntry? GetItem(XmlReader reader)
        {
            if (reader.IsStartElement("item"))
            {
                reader.ReadStartElement("item");

                string key = null;
                string type = null;
                string stringValue = null;
                object value = null;

                if (reader.IsStartElement("key"))
                {
                    reader.ReadStartElement("key");
                    key = reader.ReadString();
                    reader.ReadEndElement();

                    if (reader.IsStartElement("type"))
                    {
                        reader.ReadStartElement("type");
                        type = reader.ReadString();
                        reader.ReadEndElement();
                        if (reader.IsStartElement("value"))
                        {
                            reader.ReadStartElement("value");
                            stringValue = reader.ReadString();
                            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(stringValue))
                            {
                                Type t = Type.GetType(type);
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(t);
                                value = typeConverter.ConvertFromString(stringValue);
                            }
                            reader.ReadEndElement();
                        }
                    }
                }

                reader.ReadEndElement();

                if (key != null)
                    return new DictionaryEntry(key, stringValue);
                else
                    return null;
            }
            return null;
        }

        #endregion

        #region IGISJSONSerializable Members

        public void ToJSON(JsonTextWriter jwriter)
        {
            jwriter.WriteStartArray();

                foreach (DictionaryEntry item in this.Dictionary)
                {
                    jwriter.WriteStartObject();

                        jwriter.WriteMember("key");
                        jwriter.WriteString(item.Key.ToString());

                        //write the qualified type name of the value
                        //write the value
                        if (item.Value == null)
                        {
                            jwriter.WriteMember("type");
                            jwriter.WriteNull();
                            jwriter.WriteMember("value");
                            jwriter.WriteNull();
                        }
                        else
                        {
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(item.Value.GetType());
                            jwriter.WriteMember("type");
                            jwriter.WriteString(item.Value.GetType().AssemblyQualifiedName);
                            jwriter.WriteMember("value");
                            jwriter.WriteString(typeConverter.ConvertToString(item.Value));
                        }

                    jwriter.WriteEndObject();
                }

            jwriter.WriteEndArray();
        }

        public void FromJSON(JsonTextReader reader)
        {
            Dictionary.Clear();

            if (reader.MoveToContent() && reader.TokenClass == JsonTokenClass.Array)
            {
                reader.ReadToken(JsonTokenClass.Array);

                while(reader.TokenClass == JsonTokenClass.Object)
                {
                    reader.ReadToken(JsonTokenClass.Object);

                    //read the 'key'
                    reader.ReadMember();
                    string key = reader.ReadString();

                    string type = null;
                    string stringValue = null;
                    object value = null;

                    //read the 'type'
                    reader.ReadMember();
                    if (reader.TokenClass == JsonTokenClass.Null)
                    {
                        reader.ReadNull();
                        //read the value (should be NULL)
                        reader.ReadMember();
                        if (reader.TokenClass == JsonTokenClass.Null)
                        {
                            reader.ReadNull();
                        }
                        else
                        {
                            stringValue = reader.ReadString();
                        }
                    }
                    else
                    {
                        //read the type
                        type = reader.ReadString();
                        //read the 'value'
                        reader.ReadMember();
                        stringValue = reader.ReadString();
                    }

                    if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(stringValue))
                    {
                        Type t = Type.GetType(type);
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(t);
                        value = typeConverter.ConvertFromString(stringValue);
                    }

                    Dictionary.Add(key, value);

                    reader.ReadToken(JsonTokenClass.EndObject);
                }

                reader.ReadToken(JsonTokenClass.EndArray);
            }
        }

        #endregion
    }
}

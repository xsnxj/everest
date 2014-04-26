﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.Everest.Attributes;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using MARC.Everest.Connectors;
using MARC.Everest.DataTypes.StructDoc;

namespace MARC.Everest.DataTypes
{
    /// <summary>
    /// Represents a structured document type
    /// </summary>
    [Structure(Name = "SD", StructureType = StructureAttribute.StructureAttributeType.DataType)]
    [XmlType("SD", Namespace = "urn:hl7-org:v3")]
#if !WINDOWS_PHONE
    [Serializable]
#endif
    public class SD : ANY, IEquatable<SD>
    {

        /// <summary>
        /// Default constructor for SD
        /// </summary>
        public SD()
        {
            this.MediaType = "text/x-hl7-title+xml";
            this.Content = new List<StructDoc.StructDocNode>();
        }

        /// <summary>
        /// Creates the SD instance with the specified data
        /// </summary>
        /// <param name="data">The data being specified</param>
        public SD(params StructDoc.StructDocNode[] documentContent) : this()
        {
            this.Content = new List<StructDoc.StructDocNode>(documentContent);
        }

        /// <summary>
        /// Structured document element node
        /// </summary>
        public List<StructDoc.StructDocNode> Content { get; set; }

        /// <summary>
        /// Creates a text node
        /// </summary>
        public static StructDocTextNode CreateText(String text)
        {
            return new StructDocTextNode(text);
        }

        /// <summary>
        /// Creates an element node
        /// </summary>
        public static StructDocElementNode CreateElement(String elementName, String value = null, String namespaceUri = null)
        {
            return new StructDocElementNode(elementName, value) { NamespaceUri = namespaceUri };
        }

        /// <summary>
        /// Creates an attribute node
        /// </summary>
        public static StructDocAttributeNode CreateAttribute(String attributeName, String value)
        {
            return new StructDocAttributeNode(attributeName, value);
        }

        /// <summary>
        /// The human language of the content. Valid codes are taken from the IETF. 
        /// </summary>
        [Property(Name = "language", Conformance = PropertyAttribute.AttributeConformanceType.Optional, PropertyType = PropertyAttribute.AttributeAttributeType.Structural)]
        public virtual string Language { get; set; }

        /// <summary>
        /// The IDREF for this SD instance
        /// </summary>
        [Property(Name = "ID", Conformance = PropertyAttribute.AttributeConformanceType.Optional, PropertyType = PropertyAttribute.AttributeAttributeType.Structural)]
        public virtual string Id { get; set; }

        /// <summary>
        /// A style code applied to the SD
        /// </summary>
        [Property(Name = "styleCode", Conformance = PropertyAttribute.AttributeConformanceType.Optional, PropertyType = PropertyAttribute.AttributeAttributeType.Structural)]
        public virtual string StyleCode { get; set; }

        /// <summary>
        /// A style code applied to the SD
        /// </summary>
        [Property(Name = "mediaType", Conformance = PropertyAttribute.AttributeConformanceType.Mandatory, PropertyType = PropertyAttribute.AttributeAttributeType.Structural, FixedValue = "text/x-hl7-title+xml")]
        public virtual string MediaType { get; set; }

        /// <summary>
        /// Determine equality
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is SD)
                return this.Equals((SD)obj);
            else
                return base.Equals(obj);
        }

        #region IEquatable<SD> Members

        /// <summary>
        /// Determine equality
        /// </summary>
        public bool Equals(SD other)
        {
            var equals = base.Equals(other);
            equals &= this.MediaType == other.MediaType;
            equals &= this.Language == other.Language;
            equals &= this.StyleCode == other.StyleCode;
            equals &= this.Id == other.Id;

            if (this.Content != null && other.Content != null &&
                this.Content.Count == other.Content.Count)
                for (int i = 0; i < this.Content.Count; i++)
                    equals &= this.Content[i].Equals(other.Content[i]);
            else
                equals = false;
            return equals;
        }

        #endregion

        /// <summary>
        /// Validate this object
        /// </summary>
        public override IEnumerable<Connectors.IResultDetail> ValidateEx()
        {
            var retVal = base.ValidateEx() as List<IResultDetail>;
            if (this.MediaType != "text/x-hl7-title+xml")
                retVal.Add(new DatatypeValidationResultDetail(ResultDetailType.Warning, "SD", String.Format(ValidationMessages.MSG_INVALID_VALUE, this.MediaType, "MediaType"), null));
            else if (!(this.IsNull ^ (this.Content.Count > 0)))
                retVal.Add(new DatatypeValidationResultDetail(ResultDetailType.Error, "SD", ValidationMessages.MSG_NULLFLAVOR_WITH_VALUE, null));
            return retVal;
        }

        /// <summary>
        /// Validate the instance
        /// </summary>
        public override bool Validate()
        {
            return base.Validate() && (this.IsNull ^ (this.Content.Count > 0)) &&
                this.MediaType == "text/x-hl7-title+xml";
        }

        /// <summary>
        /// Cast a string to an SD instance
        /// </summary>
        public static implicit operator SD(String text)
        {
            return new SD(new MARC.Everest.DataTypes.StructDoc.StructDocTextNode(text));
        }

    }
}
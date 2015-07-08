﻿/**
 * Copyright 2008-2014 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 20-4-2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace MARC.Everest.Sherpas.Templating.Format
{
    /// <summary>
    /// Represents a template definition file
    /// </summary>
    [XmlType("TemplateProjectDefinition", Namespace = "urn:marc-hi:everest/sherpas/template")]
    [XmlRoot("Template", Namespace = "urn:marc-hi:everest/sherpas/template")]
    public class TemplateProjectDefinition
    {

        /// <summary>
        /// CTOR for template project
        /// </summary>
        public TemplateProjectDefinition()
        {
            this.Templates = new List<ArtifactTemplateBase>();
        }

        /// <summary>
        /// Represents project information
        /// </summary>
        [XmlElement("projectInfo")]
        public ProjectInfoDefinition ProjectInfo { get; set; }
        /// <summary>
        /// Get the templated classes
        /// </summary>
        [XmlElement("classTemplate", typeof(ClassTemplateDefinition))]
        [XmlElement("enumerationTemplate", typeof(EnumerationTemplateDefinition))]
        [XmlElement("nullTemplate", typeof(NullArtifactTemplate))]
        public List<ArtifactTemplateBase> Templates { get; set; }

        /// <summary>
        /// Merges this template with another
        /// </summary>
        public void Merge(TemplateProjectDefinition template)
        {
            
            foreach (var tpl in template.Templates)
            {
                if (this.Templates.Exists(o => o.Name == tpl.Name && o.GetType() == tpl.GetType()))
                    throw new InvalidOperationException(String.Format("Template '{0}' is already defined!", tpl.Name));
                this.Templates.Add(tpl);
            }
            
            // Merge project information
            if (this.ProjectInfo == null)
                this.ProjectInfo = new ProjectInfoDefinition();
            if (this.ProjectInfo.Copyright == null)
                this.ProjectInfo.Copyright = new XmlElement[0];
            if (this.ProjectInfo.Documentation == null)
                this.ProjectInfo.Documentation = new XmlNode[0];

            // Merge copyright
            List<XmlElement> copyright = new List<XmlElement>(this.ProjectInfo.Copyright);
            List<XmlNode> node = new List<XmlNode>(this.ProjectInfo.Documentation);
            if(template.ProjectInfo.Copyright != null)
                copyright.AddRange(template.ProjectInfo.Copyright);
            if (template.ProjectInfo.Documentation != null)
                node.AddRange(template.ProjectInfo.Documentation);


            this.ProjectInfo.Copyright = copyright.ToArray();
            this.ProjectInfo.Documentation = node.ToArray();
            // Original Author
            if (this.ProjectInfo.OriginalAuthor == null)
                this.ProjectInfo.OriginalAuthor = new List<string>();
            if(template.ProjectInfo.OriginalAuthor != null)
                this.ProjectInfo.OriginalAuthor.AddRange(template.ProjectInfo.OriginalAuthor);

            this.ProjectInfo.Name = this.ProjectInfo.Name ?? template.ProjectInfo.Name;
            this.ProjectInfo.Version = this.ProjectInfo.Version ?? template.ProjectInfo.Version;

        }

        /// <summary>
        /// Save this template to the specified location
        /// </summary>
        public void Save(string fileName)
        {
            using (FileStream fs = File.Create(fileName))
            using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true }))
            {
                XmlSerializer xsz = new XmlSerializer(typeof(TemplateProjectDefinition));
                xsz.Serialize(xw, this);

            }
        }
    }
}
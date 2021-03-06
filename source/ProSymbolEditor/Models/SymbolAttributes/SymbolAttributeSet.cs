﻿/*******************************************************************************
 * Copyright 2016 Esri
 * 
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Contracts;
using MilitarySymbols;
using System.Web.Script.Serialization;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ProSymbolEditor
{
    public class SymbolAttributeSet : PropertyChangedBase
    {
        private BitmapImage _symbolImage = null;

        public SymbolAttributeSet()
        {
            DisplayAttributes = new DisplayAttributes();
            DisplayAttributes.PropertyChanged += Attributes_PropertyChanged;

            LabelAttributes = new LabelAttributes();
        }

        public SymbolAttributeSet(Dictionary<string, string> fieldValues)
        {
            //Used to make a SymbolAttributeSet from field data in a feature
            DisplayAttributes = new DisplayAttributes();
            DisplayAttributes.PropertyChanged += Attributes_PropertyChanged;

            LabelAttributes = new LabelAttributes();

            if (fieldValues.ContainsKey("identity"))
            {
                DisplayAttributes.Identity = fieldValues["identity"];
            }

            if (fieldValues.ContainsKey("symbolset"))
            {
                DisplayAttributes.SymbolSet = fieldValues["symbolset"];
            }

            if (fieldValues.ContainsKey("symbolentity"))
            {
                DisplayAttributes.SymbolEntity = fieldValues["symbolentity"];
            }

            if (fieldValues.ContainsKey("indicator"))
            {
                DisplayAttributes.Indicator = fieldValues["indicator"];
            }

            if (fieldValues.ContainsKey("echelon"))
            {
                DisplayAttributes.Echelon = fieldValues["echelon"];
            }

            if (fieldValues.ContainsKey("mobility"))
            {
                DisplayAttributes.Mobility = fieldValues["mobility"];
            }

            if (fieldValues.ContainsKey("operationalcondition"))
            {
                DisplayAttributes.OperationalCondition = fieldValues["operationalcondition"];
            }

            if (fieldValues.ContainsKey("status"))
            {
                DisplayAttributes.Status = fieldValues["status"];
            }

            if (fieldValues.ContainsKey("context"))
            {
                DisplayAttributes.Context = fieldValues["context"];
            }

            if (fieldValues.ContainsKey("modifier1"))
            {
                DisplayAttributes.Modifier1 = fieldValues["modifier1"];
            }

            if (fieldValues.ContainsKey("modifier2"))
            {
                DisplayAttributes.Modifier2 = fieldValues["modifier2"];
            }

            //LABELS
            if (fieldValues.ContainsKey("datetimevalid"))
            {
                LabelAttributes.DateTimeValid = DateTime.Parse(fieldValues["datetimevalid"]);
            }

            if (fieldValues.ContainsKey("datetimeexpired"))
            {
                LabelAttributes.DateTimeExpired = DateTime.Parse(fieldValues["datetimeexpired"]);
            }

            if (fieldValues.ContainsKey("uniquedesignation"))
            {
                LabelAttributes.UniqueDesignation = fieldValues["uniquedesignation"];
            }

            if (fieldValues.ContainsKey("staffcomment"))
            {
                LabelAttributes.StaffComments = fieldValues["staffcomment"];
            }

            if (fieldValues.ContainsKey("additionalinformation"))
            {
                LabelAttributes.AdditionalInformation = fieldValues["additionalinformation"];
            }

            if (fieldValues.ContainsKey("type"))
            {
                LabelAttributes.Type = fieldValues["type"];
            }

            if (fieldValues.ContainsKey("commonidentifier"))
            {
                LabelAttributes.CommonIdentifier = fieldValues["commonidentifier"];
            }

            if (fieldValues.ContainsKey("speed"))
            {
                LabelAttributes.Speed = short.Parse(fieldValues["speed"]);
            }

            if (fieldValues.ContainsKey("higherFormation"))
            {
                LabelAttributes.HigherFormation = fieldValues["higherFormation"];
            }

            if (fieldValues.ContainsKey("reinforced"))
            {
                LabelAttributes.Reinforced = fieldValues["reinforced"];
            }

            if (fieldValues.ContainsKey("credibility"))
            {
                LabelAttributes.Credibility = fieldValues["credibility"];
            }

            if (fieldValues.ContainsKey("reliability"))
            {
                LabelAttributes.Reliability = fieldValues["reliability"];
            }

            if (fieldValues.ContainsKey("countrycode"))
            {
                LabelAttributes.CountryCode = fieldValues["countrycode"];
            }
        }

        #region Getters/Setters

        [ExpandableObject]
        public DisplayAttributes DisplayAttributes { get; set; }

        [ExpandableObject]
        public LabelAttributes LabelAttributes { get; set; }

        public string FavoriteId { get; set; }

        public string StandardVersion { get; set; }

        public string SymbolTags { get; set; }

        [ScriptIgnore]
        public BitmapImage SymbolImage
        {
            get
            {
                return _symbolImage;
            }
        }

        #endregion

        public void GeneratePreviewSymbol()
        {
            // Step 1: Create a dictionary/map of well known attribute names to values
            Dictionary<string, string> attributeSet = GenerateAttributeSetDictionary();

            // Step 2: Set the SVG Home Folder
            // This should be within the git clone of joint-military-symbology-xml 
            // ex: C:\Github\joint-military-symbology-xml\svg\MIL_STD_2525D_Symbols

            // This is called in CheckSettings below, but you should call yourself if
            // reusing this method 
            //Utilities.SetImageFilesHome(@"C:\Projects\Github\joint-military-symbology-xml\svg\MIL_STD_2525D_Symbols");

            string militarySymbolsPath = System.IO.Path.Combine(ProSymbolUtilities.AddinAssemblyLocation(), "Images", "MIL_STD_2525D_Symbols");
            bool pathExists = Utilities.SetImageFilesHome(militarySymbolsPath);

            if (!Utilities.CheckImageFilesHomeExists())
            //if (!CheckSettings())
            {
                Console.WriteLine("No SVG Folder, can't continue.");
                return;
            }

            // Step 3: Get the Layered Bitmap from the Library
            const int width = 256, height = 256;
            Size exportSize = new Size(width, height);

            System.Drawing.Bitmap exportBitmap;

            bool success = Utilities.ExportSymbolFromAttributes(attributeSet, out exportBitmap, exportSize);

            if (success && exportBitmap != null)
            {
                _symbolImage = ProSymbolUtilities.BitMapToBitmapImage(exportBitmap);
                NotifyPropertyChanged(() => SymbolImage);
            }

            if (!success || (exportBitmap == null))
            {
                Console.WriteLine("Export failed!");
                _symbolImage = null;
                NotifyPropertyChanged(() => SymbolImage);
                return;
            }
        }


        public Dictionary<string, string> GenerateAttributeSetDictionary()
        {
            Dictionary<string, string> attributeSet = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(DisplayAttributes.Identity))
            {
                attributeSet["identity"] = DisplayAttributes.Identity;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolSet))
            {
                attributeSet["symbolset"] = DisplayAttributes.SymbolSet;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolEntity))
            {
                attributeSet["symbolentity"] = DisplayAttributes.SymbolEntity;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Indicator))
            {
                attributeSet["indicator"] = DisplayAttributes.Indicator;
            }

            //Echelon or Mobility

            if (!string.IsNullOrEmpty(DisplayAttributes.Echelon))
            {
                attributeSet["echelon"] = DisplayAttributes.Echelon;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Mobility))
            {
                attributeSet["echelon"] = DisplayAttributes.Mobility;
            }

            //Statuses or Operation

            if (!string.IsNullOrEmpty(DisplayAttributes.OperationalCondition))
            {
                attributeSet["operationalcondition"] = DisplayAttributes.OperationalCondition;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Status))
            {
                attributeSet["operationalcondition"] = DisplayAttributes.Status;
            }

            //Delta attributes
            if (!string.IsNullOrEmpty(DisplayAttributes.Context))
            {
                attributeSet["context"] = DisplayAttributes.Context;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier1))
            {
                attributeSet["modifier1"] = DisplayAttributes.Modifier1;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier2))
            {
                attributeSet["modifier2"] = DisplayAttributes.Modifier2;
            }

            return attributeSet;
        }

        public void PopulateRowBufferWithAttributes(ref RowBuffer rowBuffer)
        {
            if (!string.IsNullOrEmpty(DisplayAttributes.Identity))
            {
                rowBuffer["identity"] = DisplayAttributes.Identity;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolSet))
            {
                rowBuffer["symbolset"] = Convert.ToInt32(DisplayAttributes.SymbolSet);
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolEntity))
            {
                rowBuffer["symbolentity"] = Convert.ToInt32(DisplayAttributes.SymbolEntity);
            }

            //Indicator / HQTFFD /

            if (!string.IsNullOrEmpty(DisplayAttributes.Indicator))
            {
                rowBuffer["indicator"] = DisplayAttributes.Indicator;
            }

            //Echelon or Mobility

            if (!string.IsNullOrEmpty(DisplayAttributes.Echelon))
            {
                rowBuffer["echelon"] = DisplayAttributes.Echelon;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Mobility))
            {
                rowBuffer["mobility"] = DisplayAttributes.Mobility;
            }

            //Statuses or Operation

            if (!string.IsNullOrEmpty(DisplayAttributes.OperationalCondition))
            {
                rowBuffer["operationalcondition"] = DisplayAttributes.OperationalCondition;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Status))
            {
                rowBuffer["status"] = DisplayAttributes.Status;
            }

            //Delta attributes
            if (!string.IsNullOrEmpty(DisplayAttributes.Context))
            {
                rowBuffer["context"] = DisplayAttributes.Context;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier1))
            {
                rowBuffer["modifier1"] = DisplayAttributes.Modifier1;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier2))
            {
                rowBuffer["modifier2"] = DisplayAttributes.Modifier2;
            }

            //LABELS
            if (LabelAttributes.DateTimeValid != null)
            {
                rowBuffer["datetimevalid"] = LabelAttributes.DateTimeValid.ToString();
            }

            if (LabelAttributes.DateTimeExpired != null)
            {
                rowBuffer["datetimeexpired"] = LabelAttributes.DateTimeExpired.ToString();
            }
            
            if (!string.IsNullOrEmpty(LabelAttributes.UniqueDesignation))
            {
                rowBuffer["uniquedesignation"] = LabelAttributes.UniqueDesignation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.StaffComments))
            {
                rowBuffer["staffcomment"] = LabelAttributes.StaffComments;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.AdditionalInformation))
            {
                rowBuffer["additionalinformation"] = LabelAttributes.AdditionalInformation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Type))
            {
                rowBuffer["type"] = LabelAttributes.Type;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.CommonIdentifier))
            {
                rowBuffer["commonidentifier"] = LabelAttributes.CommonIdentifier;
            }

            if (LabelAttributes.Speed != null)
            {
                //Short
                rowBuffer["speed"] = LabelAttributes.Speed;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.HigherFormation))
            {
                rowBuffer["higherFormation"] = LabelAttributes.HigherFormation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Reinforced))
            {
                rowBuffer["reinforced"] = LabelAttributes.Reinforced;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Credibility))
            {
                rowBuffer["credibility"] = LabelAttributes.Credibility;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Reliability))
            {
                rowBuffer["reliability"] = LabelAttributes.Reliability;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.CountryCode))
            {
                rowBuffer["countrycode"] = LabelAttributes.CountryCode;
            }
        }

        public void PopulateFeatureWithAttributes(ref Feature feature)
        {
            if (!string.IsNullOrEmpty(DisplayAttributes.Identity))
            {
                feature["identity"] = DisplayAttributes.Identity;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolSet))
            {
                feature["symbolset"] = Convert.ToInt32(DisplayAttributes.SymbolSet);
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.SymbolEntity))
            {
                feature["symbolentity"] = Convert.ToInt32(DisplayAttributes.SymbolEntity);
            }

            //Indicator / HQTFFD /

            if (!string.IsNullOrEmpty(DisplayAttributes.Indicator))
            {
                feature["indicator"] = DisplayAttributes.Indicator;
            }

            //Echelon or Mobility

            if (!string.IsNullOrEmpty(DisplayAttributes.Echelon))
            {
                feature["echelon"] = DisplayAttributes.Echelon;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Mobility))
            {
                feature["mobility"] = DisplayAttributes.Mobility;
            }

            //Statuses or Operation

            if (!string.IsNullOrEmpty(DisplayAttributes.OperationalCondition))
            {
                feature["operationalcondition"] = DisplayAttributes.OperationalCondition;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Status))
            {
                feature["status"] = DisplayAttributes.Status;
            }

            //Delta attributes
            if (!string.IsNullOrEmpty(DisplayAttributes.Context))
            {
                feature["context"] = DisplayAttributes.Context;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier1))
            {
                feature["modifier1"] = DisplayAttributes.Modifier1;
            }

            if (!string.IsNullOrEmpty(DisplayAttributes.Modifier2))
            {
                feature["modifier2"] = DisplayAttributes.Modifier2;
            }

            //LABELS
            if (LabelAttributes.DateTimeValid != null)
            {
                feature["datetimevalid"] = LabelAttributes.DateTimeValid.ToString();
            }

            if (LabelAttributes.DateTimeExpired != null)
            {
                feature["datetimeexpired"] = LabelAttributes.DateTimeExpired.ToString();
            }

            if (!string.IsNullOrEmpty(LabelAttributes.UniqueDesignation))
            {
                feature["uniquedesignation"] = LabelAttributes.UniqueDesignation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.StaffComments))
            {
                feature["staffcomment"] = LabelAttributes.StaffComments;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.AdditionalInformation))
            {
                feature["additionalinformation"] = LabelAttributes.AdditionalInformation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Type))
            {
                feature["type"] = LabelAttributes.Type;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.CommonIdentifier))
            {
                feature["commonidentifier"] = LabelAttributes.CommonIdentifier;
            }

            if (LabelAttributes.Speed != null)
            {
                //Short
                feature["speed"] = LabelAttributes.Speed;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.HigherFormation))
            {
                feature["higherFormation"] = LabelAttributes.HigherFormation;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Reinforced))
            {
                feature["reinforced"] = LabelAttributes.Reinforced;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Credibility))
            {
                feature["credibility"] = LabelAttributes.Credibility;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.Reliability))
            {
                feature["reliability"] = LabelAttributes.Reliability;
            }

            if (!string.IsNullOrEmpty(LabelAttributes.CountryCode))
            {
                feature["countrycode"] = LabelAttributes.CountryCode;
            }
        }

        public void ResetAttributes()
        {
            //Reset attributes
            DisplayAttributes.SymbolSet = "";
            DisplayAttributes.SymbolEntity = "";
            DisplayAttributes.Echelon = "";
            DisplayAttributes.Identity = "";
            DisplayAttributes.OperationalCondition = "";
            DisplayAttributes.Status = "";
            DisplayAttributes.Mobility = "";
            DisplayAttributes.Indicator = "";
            DisplayAttributes.Context = "";
            DisplayAttributes.Modifier1 = "";
            DisplayAttributes.Modifier2 = "";

            //Reset label text attributes
            LabelAttributes.DateTimeValid = null;
            LabelAttributes.DateTimeExpired = null;
            LabelAttributes.UniqueDesignation = "";
            LabelAttributes.StaffComments = "";
            LabelAttributes.AdditionalInformation = "";
            LabelAttributes.Type = "";
            LabelAttributes.CommonIdentifier = "";
            LabelAttributes.Speed = null;
            LabelAttributes.HigherFormation = "";
            LabelAttributes.Reinforced = "";
            LabelAttributes.Credibility = null;
            LabelAttributes.Reliability = "";
            LabelAttributes.CountryCode = "";

            SymbolTags = "";
        }

        private void Attributes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GeneratePreviewSymbol();
        }
    }
}

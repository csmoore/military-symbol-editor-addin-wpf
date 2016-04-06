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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProSymbolEditor;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Hosting;

namespace SymbolEditorUnitTests
{
    [TestClass]
    public class SymbolEditorTests
    {
        [TestMethod]
        public void CheckLayerForSymbolSetTest()
        {
            SymbolSetMappings symbolSetMappings = new SymbolSetMappings();
            string featureClassName = symbolSetMappings.GetFeatureClassFromMapping("01", StyleItemType.PointSymbol);
            Assert.IsTrue(featureClassName == "Air", "Feature Class from mapping is incorrect");

            featureClassName = symbolSetMappings.GetFeatureClassFromMapping("40", StyleItemType.PointSymbol);
            Assert.IsTrue(featureClassName == "Activities", "Feature Class from mapping is incorrect");

            featureClassName = symbolSetMappings.GetFeatureClassFromMapping("60", StyleItemType.PointSymbol);
            Assert.IsTrue(featureClassName == "Cyberspace", "Feature Class from mapping is incorrect");
        }
    }
}

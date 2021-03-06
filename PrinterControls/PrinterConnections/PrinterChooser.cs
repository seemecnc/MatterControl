﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SettingsManagement;

namespace MatterHackers.MatterControl
{
    public class PrinterChooser : GuiWidget
    {
        public StyledDropDownList ManufacturerDropList;

        public PrinterChooser(string selectedMake = null)
        {
            string defaultManufacturerLabel = LocalizedString.Get("Select Make");
            string defaultManufacturerLabelFull = string.Format("- {0} -", defaultManufacturerLabel);
            ManufacturerDropList = new StyledDropDownList(defaultManufacturerLabelFull, maxHeight: 300);
            bool addOther = false;
            string[] printerWhiteListStrings = OemSettings.Instance.PrinterWhiteList.ToArray();
            string pathToManufacturers = Path.Combine(ApplicationDataStorage.Instance.ApplicationStaticDataPath, "PrinterSettings");
            if (Directory.Exists(pathToManufacturers))
            {
                int index = 0;
                int preselectIndex = -1;
                foreach (string manufacturerDirectory in Directory.EnumerateDirectories(pathToManufacturers))
                {
                    string folderName = new System.IO.DirectoryInfo(manufacturerDirectory).Name;
                    if (printerWhiteListStrings.Contains(folderName))
                    {
                        string manufacturer = Path.GetFileName(manufacturerDirectory);
                        if (manufacturer == "Other")
                        {
                            addOther = true;
                        }
                        else
                        {
                            ManufacturerDropList.AddItem(manufacturer);
                            if (selectedMake != null)
                            {
                                if (manufacturer == selectedMake)
                                {
                                    preselectIndex = index;
                                }
                            }
                        
                            index++;

                        }
                    }
                }
                if (addOther)
                {
                    if (selectedMake != null && preselectIndex == -1)
                    {
                        preselectIndex = index;
                    }
					ManufacturerDropList.AddItem(LocalizedString.Get("Other"));
                }
                if (preselectIndex != -1)
                {
                    ManufacturerDropList.SelectedIndex = preselectIndex;
                }

            }

            AddChild(ManufacturerDropList);

            HAnchor = HAnchor.FitToChildren;
            VAnchor = VAnchor.FitToChildren;
        }
    }

    public class ModelChooser : GuiWidget
    {
        public StyledDropDownList ModelDropList;

        public ModelChooser(string manufacturer)
        {
            string defaultModelDropDownLabel = LocalizedString.Get("Select Model");
            string defaultModelDropDownLabelFull = string.Format("- {0} -", defaultModelDropDownLabel);
            ModelDropList = new StyledDropDownList(defaultModelDropDownLabelFull);
            string pathToModels = Path.Combine(ApplicationDataStorage.Instance.ApplicationStaticDataPath, "PrinterSettings", manufacturer);
            if (Directory.Exists(pathToModels))
            {
                foreach (string manufacturerDirectory in Directory.EnumerateDirectories(pathToModels))
                {
                    string model = Path.GetFileName(manufacturerDirectory);
                    ModelDropList.AddItem(model);
                }
            }
			ModelDropList.AddItem(LocalizedString.Get("Other"));

            AddChild(ModelDropList);

            HAnchor = HAnchor.FitToChildren;
            VAnchor = VAnchor.FitToChildren;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;



using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.Font;
using MatterHackers.VectorMath;

using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
    public class ApplicationMenuRow : FlowLayoutWidget
    {
        static FlowLayoutWidget rightElement;
        
        public ApplicationMenuRow()
            :base(FlowDirection.LeftToRight)
        {
            LinkButtonFactory linkButtonFactory = new LinkButtonFactory();
            linkButtonFactory.textColor = ActiveTheme.Instance.PrimaryTextColor;
            linkButtonFactory.fontSize = 8;
            
            Button signInLink = linkButtonFactory.Generate("(Sign Out)");
            signInLink.VAnchor = Agg.UI.VAnchor.ParentCenter;            
            signInLink.Margin = new BorderDouble(top: 0);
            
            this.HAnchor = HAnchor.ParentLeftRight;
            this.BackgroundColor = ActiveTheme.Instance.PrimaryBackgroundColor;

            MenuOptionFile menuOptionFile = new MenuOptionFile();         

            //TextWidget menuOptionFile = new TextWidget("FILE", pointSize: 10);
            
            //menuOptionFile.TextColor = ActiveTheme.Instance.PrimaryTextColor;

            MenuOptionHelp menuOptionHelp = new MenuOptionHelp();

            rightElement = new FlowLayoutWidget(FlowDirection.LeftToRight);
            rightElement.Height = 24;
            rightElement.Margin = new BorderDouble(bottom: 4);
            //rightElement.VAnchor = Agg.UI.VAnchor.ParentCenter;

            this.AddChild(menuOptionFile);
            this.AddChild(menuOptionHelp);
            this.AddChild(new HorizontalSpacer());
            this.AddChild(rightElement);

            this.Padding = new BorderDouble(0, 0, 6, 0);

            if (privateAddRightElement != null)
            {
                privateAddRightElement(rightElement);
            }
        }

        public delegate void AddRightElementDelegate(GuiWidget iconContainer);
        private static event AddRightElementDelegate privateAddRightElement;
        public static event AddRightElementDelegate AddRightElement
        {
            add
            {
                privateAddRightElement += value;
                // and call it right away
                value(rightElement);
            }

            remove
            {
                privateAddRightElement -= value;
            }
        }
    }

    public class MenuOptionFile : GuiWidget
    {
        public DropDownMenu MenuDropList;
        private TupleList<string, Func<bool>> menuItems;

        public MenuOptionFile()
        {
            MenuDropList = new DropDownMenu("File".Localize().ToUpper(), Direction.Down,pointSize:10);        
            MenuDropList.MenuItemsPadding = new BorderDouble(0);
            MenuDropList.Margin = new BorderDouble(0);
            MenuDropList.Padding = new BorderDouble(0);

            SetMenuItems();

            AddChild(MenuDropList);
            this.Width = 44;
            this.Height = 22;
            this.Margin = new BorderDouble(0);
            this.Padding = new BorderDouble(0);
            this.VAnchor = Agg.UI.VAnchor.ParentCenter;
            this.MenuDropList.SelectionChanged += new EventHandler(MenuDropList_SelectionChanged);
            this.MenuDropList.OpenOffset = new Vector2(0, 0);
        }

        void MenuDropList_SelectionChanged(object sender, EventArgs e)
        {
            string menuSelection = ((DropDownMenu)sender).SelectedValue;
            foreach (Tuple<string, Func<bool>> item in menuItems)
            {
                if (item.Item1 == menuSelection)
                {
                    if (item.Item2 != null)
                    {
                        item.Item2();
                    }
                }
            }
        }

        void SetMenuItems()
        {
            menuItems = new TupleList<string, Func<bool>> 
            {                
                {LocalizedString.Get("Add File"), importFile_Click},
                {LocalizedString.Get("Exit"), exit_Click},
            };

            BorderDouble padding = MenuDropList.MenuItemsPadding;
            //Add the menu items to the menu itself
            foreach (Tuple<string, Func<bool>> item in menuItems)
            {
                MenuDropList.MenuItemsPadding = new BorderDouble(8,4,8,4);
                MenuDropList.AddItem(item.Item1,pointSize:10);
            }            
            MenuDropList.Padding = padding;
        }

        bool importFile_Click()
        {
            UiThread.RunOnIdle((state) =>
            {  
                OpenFileDialogParams openParams = new OpenFileDialogParams("Select an STL file, Select a GCODE file|*.stl;*.gcode", multiSelect: true);
                openParams.ActionButtonLabel = "Add to Queue";
                openParams.Title = "MatterControl: Select A File";

                FileDialog.OpenFileDialog(ref openParams);
                if (openParams.FileNames != null)
                {
                    foreach (string loadedFileName in openParams.FileNames)
                    {
                        QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(loadedFileName), Path.GetFullPath(loadedFileName))));
                    }
                }
            });
            return true;
        }

        bool exit_Click()
        {
            UiThread.RunOnIdle((state) =>
            {                
                GuiWidget parent = this;
                while (parent as MatterControlApplication == null)
                {
                    parent = parent.Parent;
                }
                MatterControlApplication app = parent as MatterControlApplication;
                app.RestartOnClose = false;
                app.Close();
            });
            return true;
        }    
    }

    public class MenuOptionHelp : GuiWidget
    {
        public DropDownMenu MenuDropList;
        private TupleList<string, Func<bool>> menuItems;

        public MenuOptionHelp()
        {
            MenuDropList = new DropDownMenu("Help".Localize().ToUpper(), Direction.Down, pointSize: 10);
            MenuDropList.MenuItemsPadding = new BorderDouble(0);
            MenuDropList.Margin = new BorderDouble(0);
            MenuDropList.Padding = new BorderDouble(0);

            SetMenuItems();

            AddChild(MenuDropList);
            this.Width = 48;
            this.Height = 22;
            this.Margin = new BorderDouble(0);
            this.Padding = new BorderDouble(0);
            this.VAnchor = Agg.UI.VAnchor.ParentCenter;
            this.MenuDropList.SelectionChanged += new EventHandler(MenuDropList_SelectionChanged);
            this.MenuDropList.OpenOffset = new Vector2(0, 0);
        }

        void MenuDropList_SelectionChanged(object sender, EventArgs e)
        {
            string menuSelection = ((DropDownMenu)sender).SelectedValue;
            foreach (Tuple<string, Func<bool>> item in menuItems)
            {
                if (item.Item1 == menuSelection)
                {
                    if (item.Item2 != null)
                    {
                        item.Item2();
                    }
                }
            }
        }

        void SetMenuItems()
        {
            menuItems = new TupleList<string, Func<bool>> 
            {                
                {LocalizedString.Get("Getting Started"), gettingStarted_Click},
                {LocalizedString.Get("View Help"), help_Click},
                {LocalizedString.Get("About"), about_Click},
            };

            BorderDouble padding = MenuDropList.MenuItemsPadding;
            //Add the menu items to the menu itself
            foreach (Tuple<string, Func<bool>> item in menuItems)
            {
                MenuDropList.MenuItemsPadding = new BorderDouble(8, 4, 8, 4);
                MenuDropList.AddItem(item.Item1, pointSize: 10);
            }
            MenuDropList.Padding = padding;
        }

        bool help_Click()
        {
            UiThread.RunOnIdle((state) =>
            {   
                System.Diagnostics.Process.Start("http://www.mattercontrol.com/articles");
            });
            return true;
        }

        bool about_Click()
        {
            UiThread.RunOnIdle((state) =>
            {
                System.Diagnostics.Process.Start("http://www.mattercontrol.com/");
            });
            return true;
        }

        bool gettingStarted_Click()
        {
            UiThread.RunOnIdle((state) =>
            {
                System.Diagnostics.Process.Start("http://www.mattercontrol.com/articles/mattercontrol-getting-started");
            });
            
            return true;
        }
    }
}

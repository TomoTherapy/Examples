﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using Size = System.Drawing.Size;

namespace WpfAppWithNoti
{
    public class Class1
    {
        private NotifyIcon Noti;

        public Class1()
        {
            GenerateNotifyIcon();
        }

        private void GenerateNotifyIcon()
        {
            if (Noti != null) Noti.Dispose();

            ContextMenu Menu = new ContextMenu();
            Noti = new NotifyIcon
            {
                Icon = new Icon(@"Resources\icon.ico", new Size(10, 10)),
                Visible = true,
                Text = "노티파이아이콘",
                ContextMenu = Menu
            };

            MenuItem menu1 = new MenuItem
            {
                Text = "Menu1"
            };
            menu1.Click += (object o, EventArgs e) =>
            {
                //event here
            };


            MenuItem menu2 = new MenuItem()
            {
                Text = "Menu2"
            };

            MenuItem menu2submenu1 = new MenuItem()
            {
                Text = "sub1"
            };
            menu2submenu1.Click += (object o, EventArgs e) =>
            {
                //event here
            };
            menu2.MenuItems.Add(menu2submenu1);

            MenuItem menu2submenu2 = new MenuItem()
            {
                Text = "sub2"
            };
            menu2submenu2.Click += (object o, EventArgs e) =>
            {
                //event here
            };
            menu2.MenuItems.Add(menu2submenu2);

            MenuItem ExitItem = new MenuItem()
            {
                Text = "나가기"
            };
            ExitItem.Click += (object o, EventArgs e) =>
            {
                ((App)System.Windows.Application.Current).Shutdown();
            };

            Menu.MenuItems.Add(menu1);
            Menu.MenuItems.Add(menu2);
            Menu.MenuItems.Add(ExitItem);
            Noti.ContextMenu = Menu;
        }
    }
}
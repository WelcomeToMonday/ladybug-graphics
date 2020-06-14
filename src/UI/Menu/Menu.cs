using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Linq;

using Ladybug.Graphics.UI.Controls;

namespace Ladybug.Graphics.UI
{
	public class Menu : MenuPanel
	{
		

		public Menu()
		{

			AddAlias("Button", typeof(ButtonControl));
			AddAlias("Menu", typeof(Menu));
			AddAlias("Panel", typeof(MenuPanel));
			
		}

		public void BuildMenu(string filePath)
		{
			using (XmlReader reader = XmlReader.Create(filePath))
			{
				BuildFromXml(reader);
			}
		}

		public void BuildMenu(XmlReader reader)
		{
			BuildFromXml(reader);
		}

		internal static Type LocateType(string typeName)
		{
			Type res = null;
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] assemblyTypes = a.GetTypes();

				if (res != null)
				{
					break;
				}
				else
				{
					res = assemblyTypes.FirstOrDefault(t =>
					(
						t.Name == typeName &&
						(t.IsSubclassOf(typeof(MenuControl)) || (t == typeof(MenuPanel) || t.IsSubclassOf(typeof(MenuPanel))))
					));
				}
			}
			return res;
		}
	}
}
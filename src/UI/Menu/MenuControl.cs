using System;
using System.Collections.Generic;

using System.Xml;

namespace Ladybug.Graphics.UI
{
	public abstract class MenuControl
	{
		public string ID
		{
			get
			{
				return Attributes.ContainsKey("id") ? Attributes["id"] : "";
			}
		}

		public Dictionary<string, string> Attributes { get; protected set; } = new Dictionary<string, string>();

		public MenuControl()
		{

		}

		public MenuControl(Dictionary<string, string> attributes)
		{
			Attributes = attributes;
			PostBuild();
		}

		public event EventHandler GainFocus;
		protected virtual void OnGainFocus(EventArgs e)
		{
			var handler = GainFocus;
			handler?.Invoke(this, e);
		}

		public event EventHandler LoseFocus;
		protected virtual void OnLoseFocus(EventArgs e)
		{
			var handler = LoseFocus;
			handler?.Invoke(this, e);
		}

		public event EventHandler Activate;
		protected virtual void OnActivate(EventArgs e)
		{
			var handler = Activate;
			handler?.Invoke(this, e);
		}

		public virtual void BuildFromXml(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						Attributes.Add(reader.Name, reader.Value);
					}
				}
			}
			PostBuild();
		}

		protected virtual void PostBuild()
		{

		}
	}
}
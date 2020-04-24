using System;
using System.Reflection;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace Ladybug.Graphics.UI
{
	public class MenuPanel
	{
		private Dictionary<string, MenuControl> _controls = new Dictionary<string, MenuControl>();
		private Dictionary<string, MenuPanel> _panels = new Dictionary<string, MenuPanel>();
		protected Dictionary<string, Type> _typeAliases = new Dictionary<string, Type>();
		protected Dictionary<string, string> Attributes = new Dictionary<string, string>();

		public Dictionary<string, MenuPanel> Panels { get => _panels; }
		public Dictionary<string, MenuControl> Controls { get => _controls; }

		public MenuPanel() : base(){}
		public MenuPanel(Dictionary<string, string> attributes)
		{
			Attributes = attributes;
			PostBuild();
		}

		public string ID
		{
			get
			{
				return Attributes.ContainsKey("id") ? Attributes["id"] : "";
			}
		}

		public void ImportAliases(Dictionary<string, Type> aliases)
		{
			aliases.ToList().ForEach(x =>
			{
				if (!_typeAliases.ContainsKey(x.Key)) _typeAliases.Add(x.Key, x.Value);
			});
		}

		public void AddAlias(string key, Type value)
		{
			if (_typeAliases.ContainsKey(key))
			{
				_typeAliases[key] = value;
			}
			else
			{
				_typeAliases.Add(key, value);
			}
		}

		public void BuildFromXml(XmlReader rreader)
		{

			while (rreader.Read())
			{
				rreader.MoveToElement();
				if (rreader.HasAttributes)
				{
					while (rreader.MoveToNextAttribute())
					{
						Attributes.Add(rreader.Name, rreader.Value);
					}
				}

				rreader.MoveToElement();

				if (rreader.NodeType == XmlNodeType.Element)
				{
					using (var reader = rreader.ReadSubtree())
					{
						reader.Read(); //skip ahead a line
						while (reader.Read())
						{
							if (reader.Name == "Alias")
							{
								using (var sub = reader.ReadSubtree())
								{
									while (sub.Read())
									{
										if (sub.NodeType == XmlNodeType.Element)
										{
											var aliasName = sub.Name;

											sub.MoveToAttribute("type");
											var typeName = sub.Value;
											Type type = Menu.LocateType(typeName);

											if (type != null)
											{
												if (_typeAliases.ContainsKey(aliasName))
												{
													_typeAliases[aliasName] = type;
												}
												else
												{
													_typeAliases.Add(aliasName, type);
												}
											}
										}
									}
								}
							}
							else
							{
								if (reader.NodeType == XmlNodeType.Element)
								{
									var typename = reader.Name;

									Type t = _typeAliases.ContainsKey(typename) ? _typeAliases[typename] : Menu.LocateType(typename);
									if (t != null && t.IsSubclassOf(typeof(MenuControl)))
									{
										var newControl = (MenuControl)Activator.CreateInstance(t);
										using (var sub = reader.ReadSubtree())
										{
											newControl.BuildFromXml(sub);
											//newControl.ProcessAttributes();
										}
										AddControl(newControl);
									}
									else if (t != null && (t == typeof(MenuPanel) || t.IsSubclassOf(typeof(MenuPanel))))
									{
										var newPanel = (MenuPanel)Activator.CreateInstance(t);
										if (reader.NodeType == XmlNodeType.Element)
										{
											using (var sub = reader.ReadSubtree())
											{
												newPanel.ImportAliases(_typeAliases);
												newPanel.BuildFromXml(sub);
												//newPanel.ProcessAttributes();
											}
										}
										AddPanel(newPanel);
									}
								}
							}
						}
					}
				}
			}
			PostBuild();
		}

		public void AddControl(MenuControl control)
		{
			_controls.Add(control.ID, control);
		}

		public void AddPanel(MenuPanel panel)
		{
			_panels.Add(panel.ID, panel);
		}

		protected virtual void PostBuild()
		{

		}

		public T FindPanel<T>(string id, bool recurse = true) where T : MenuPanel
		{
			var res = Panels.Values.OfType<T>().Where(panel => panel.ID == id).FirstOrDefault();

			if (recurse && res == null)
			{
				foreach (var panel in Panels.Values)
				{
					var subRes = panel.FindPanel<T>(id, recurse);
					if (subRes != null)
					{
						res = subRes;
						break;
					}
				}
			}

			return res;
		}

		public T FindControl<T>(string id, bool recurse = true) where T : MenuControl
		{
			var res = Controls.Values.OfType<T>().Where(control => control.ID == id).FirstOrDefault();

			if (recurse && res == null)
			{
				foreach (var panel in Panels.Values)
				{
					var subRes = panel.FindControl<T>(id, recurse);
					if (subRes != null)
					{
						res = subRes;
						break;
					}
				}
			}

			return res;
		}

		public List<T> FindPanels<T>(bool includeSelf = true, bool recurse = true) where T : MenuPanel
		{
			var res = Panels.Values.OfType<T>().Where(panel => panel.GetType() == typeof(T)).ToList();

			if (includeSelf && (this.GetType().IsSubclassOf(typeof(T)) || this.GetType() == typeof(T)))
			{
				res.Add(this as T);
			}

			if (recurse)
			{
				Panels.Values.ToList().ForEach(
					panel =>
					{
						var subRes = panel.FindPanels<T>(recurse);
						subRes.ForEach(item => {
							if (!res.Contains(item))
							{
								res.Add(item);
							}
						});
					}
				);
			}

			return res;
		}

		public List<T> FindControls<T>(bool strict = false, bool recurse = true) where T : MenuControl
		{
			var res = strict
			? Controls.Values.OfType<T>().Where(control => control.GetType() == typeof(T)).ToList()
			: Controls.Values.OfType<T>().ToList();

			if (recurse)
			{
				_panels.Values.ToList().ForEach(
					panel =>
					{
						var subRes = panel.FindControls<T>(strict, recurse);
						subRes.ForEach(item => res.Add(item));
					}
				);
			}

			return res;
		}
	}
}
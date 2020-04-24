using System;
using System.Collections.Generic;

namespace Ladybug.Graphics.UI.Controls
{
	public class ButtonControl : MenuControl
	{

		public ButtonControl() : base(){}
		public ButtonControl(Dictionary<string, string> attributes) : base(attributes){}

		public virtual void Click()
		{
			OnActivate(new EventArgs());
		}
	}
}
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Ladybug.Graphics.BoxModel
{
	public enum BoxHandle {TOPLEFT, TOPRIGHT, BOTTOMLEFT, BOTTOMRIGHT, CENTER}

	public enum PositioningMode {ABSOLUTE, RELATIVE, INHERIT}

	public class Box
	{
		protected List<Box> children;

		private Box _parent;

		private PositioningMode m_positionMode;
		
		public Rectangle Bounds { get; protected set; }
		
		public Rectangle Margin { get; protected set; }

		public PositioningMode PositionMode 
		{
			get
			{
				PositioningMode mode = PositioningMode.ABSOLUTE;
				switch (m_positionMode)
				{
					default:
					case PositioningMode.ABSOLUTE:
						mode = m_positionMode;
						break;
					
					case PositioningMode.RELATIVE:
						
						if (_parent != null)
						{
							mode = m_positionMode;
						}
						else
						{
							mode = PositioningMode.ABSOLUTE;
						}
						break;
					
					case PositioningMode.INHERIT:
						
						if (_parent != null)
						{
							mode = _parent.PositionMode;
						}
						else
						{
							mode = PositioningMode.ABSOLUTE;
						}
						break;
				}
				return mode;
			}
			protected set => m_positionMode = value;
		} 

		public void AddChild(Box child)
		{
			if (children == null)
			{
				children = new List<Box>();
			}
			child._parent = this;
			child.PositionMode = PositioningMode.INHERIT;
			children.Add(child);
		}

		public Vector2 GetAbsolutePosition() => new Vector2((int)Bounds.X, (int)Bounds.Y);
		
		public Vector2 GetRelativePosition()
		{
			if (_parent == null)
			{
				return GetAbsolutePosition();		
			}
			else
			{
				return new Vector2
				(
					(int)(Bounds.X - _parent.Bounds.X),
					(int)(Bounds.Y - _parent.Bounds.Y)
				);
			}
		}

		public void SetPosition(Vector2 newPos, BoxHandle handle = BoxHandle.TOPLEFT)
		{
			switch (PositionMode)
			{
				default:
				case PositioningMode.ABSOLUTE:
					Bounds.CopyAtPosition(newPos, handle);
					break;

				case PositioningMode.RELATIVE:
					Bounds.CopyAtPosition
					(
						new Vector2(
							(int)(_parent.Bounds.X + newPos.X),
							(int)(_parent.Bounds.Y + newPos.Y)
						),
						handle
					);
					break;
			}
		}
	}
}
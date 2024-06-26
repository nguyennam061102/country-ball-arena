using UnityEngine;

namespace RootMotion
{
	public class InspectorComment : PropertyAttribute
	{
		public string name;

		public string color = "white";

		public InspectorComment(string name)
		{
			this.name = name;
			color = "white";
		}

		public InspectorComment(string name, string color)
		{
			this.name = name;
			this.color = color;
		}
	}
}

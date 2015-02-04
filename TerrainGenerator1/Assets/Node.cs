
using System;

		public class Node : IComparable
		{
		//N,NE,E,SE,S,SW,W,NW
		public Node[] neibors;
		public int x,y;
		public float height;
		public bool isRidge;
				public Node ()
				{
			this.isRidge = false;
		this.neibors = new Node[8];
				

				}
	public int CompareTo(object obj) {
		if (obj == null) return 1;
		
		Node otherNode = obj as Node;
		if (otherNode != null) 
			return this.height.CompareTo(otherNode.height);
		else 
			throw new ArgumentException("Object is not a Node");
	}
		}



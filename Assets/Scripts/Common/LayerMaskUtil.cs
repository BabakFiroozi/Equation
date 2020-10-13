using System;
using UnityEngine;

namespace Equation
{
	public static class LayerMaskUtil
	{
		public static int GetLayerMask(int layer)
		{
			int layerMask = 1 << layer;
			return layerMask;
		}

		public static int GetLayerMask(string layerName)
		{
			int layer = LayerMask.NameToLayer(layerName);
			int layerMask = 1 << layer;
			return layerMask;
		}
	}

}
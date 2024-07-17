using System;
using UnityEngine;

namespace TKSRPlayables
{
	[Serializable]
	public class FaceParam
	{
		public FaceType faceType;

		public Vector2 destination;
		public Transform target;
		public float animSpeed = 1f;

		public FaceParam(FaceType ty, Vector2 v, Transform ta, float speed)
		{
			faceType = ty;
			destination = v;
			target = ta;
			animSpeed = speed;
		}

		public FaceParam(FaceType ty, Vector2 v)
		{
			faceType = ty;
			destination = v;
		}

		public FaceParam(FaceType ty, Transform ta)
		{
			faceType = ty;
			target = ta;
		}

		public FaceParam(FaceType ty)
		{
			faceType = ty;
		}

		public enum FaceType
		{
			ToPosition,
			ToTransform
		}
	}
}
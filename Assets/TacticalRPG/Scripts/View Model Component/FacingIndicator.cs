using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class FacingIndicator : MonoBehaviour 
	{
#if OLD_TRPG
		[SerializeField] Renderer[] directions;
		[SerializeField] Material normal;
		[SerializeField] Material selected;
		
		public void SetDirection (Directions dir)
		{
			int index = (int)dir;
			for (int i = 0; i < 4; ++i)
				directions[i].material = (i == index) ? selected : normal;
		}
#else
		[SerializeField]
		private SpriteRenderer[] directions;
		[SerializeField] Sprite[] normal;
		[SerializeField] Sprite[] selected;
		
		private void SetDirection(Directions dir)
		{
			int index = (int)dir;
			for (int i = 0; i < 4; ++i)
			{
				directions[i].sprite = (i == index) ? selected[i] : normal[i];
			}
		}
		
		public void SetDirection (Unit unit)
		{
			transform.position = unit.transform.position;
			var dir = unit.dir;
			int index = (int)dir;
			for (int i = 0; i < 4; ++i)
			{
				directions[i].sprite = (i == index) ? selected[i] : normal[i];
			}
	
			m_OldIndex = index;
		}
	
		private int m_OldIndex = -1;
		private float last_time;
		
		/// <summary>
		/// 点击方向指示符来选择朝向
		/// </summary>
		/// <param name="index"></param>
		public void OnClickArrow(int index)
		{
			// 通过双击来确定保存朝向
			if (m_OldIndex == index)
			{
				if (Time.time - last_time < 0.5f)
				{
					Debug.Log("Double Click");
					InputController.Instance.SimOnFireEvent();
				}
				last_time = Time.time;
				return;
			}
			
			Debug.Log($"[TKSR] Click Arrow by index = {index}");
			Point pt;
			if (index == 0)
			{
				pt = new Point(0, 1);
			}
			else if (index == 1)
			{
				pt = new Point(1, 0);
			}
			else if (index == 2)
			{
				pt = new Point(0, -1);
			}
			else
			{
				pt = new Point(-1, 0);
			}
			InputController.Instance.SimOnMoveEvent(pt);
		}
#endif
	}
}

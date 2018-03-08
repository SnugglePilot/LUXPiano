using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
	public class MouseInput : MonoBehaviour
	{
		[Tooltip("The camera we will raycast from. If blank, will use Camera.main")]
		public Camera camera;

		protected Keys luxKeys;

		protected void Start()
		{
			if (camera == null)
			{
				camera = Camera.main;
			}

			luxKeys = gameObject.GetComponentInChildren<Keys>();
		}
		
		protected void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnClick(true);
			}

			if (Input.GetMouseButtonDown(1))
			{
				OnClick(false);
			}

			if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			{
				OnRelease();
			}
		}

		protected void OnClick(bool precisionClick)
		{
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
			if (Physics.Raycast(ray, out hit))
			{
				Key hitKey = hit.transform.gameObject.GetComponentInParent<Key>();
				if (hitKey != null)
				{

					if (precisionClick)
					{
						hitKey.OnPressed(1);
					}
					else
					{
						luxKeys.OnClumsyKeyPressed(hitKey, 1);
					}
					
				}
			}
		}

		protected void OnRelease()
		{
			luxKeys.ReleaseAll();
		}
	}
}

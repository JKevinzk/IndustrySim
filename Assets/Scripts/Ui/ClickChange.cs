using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickChange : MonoBehaviour
{
	private bool flag = true;
	public Button btn;
	public Image runImage;
	public Image pauseImage;
	// Start is called before the first frame update
	void Start()
	{
		runImage.transform.gameObject.SetActive(true);
		pauseImage.transform.gameObject.SetActive(true);
		pauseImage.enabled = false;
		transform.GetComponent<Button>().onClick.AddListener(OnClick);
	}


	void OnClick()
	{
		if (flag)
		{
			runImage.enabled = false;
			pauseImage.enabled = true;
			btn.targetGraphic = pauseImage;
		}
		else
		{
			runImage.enabled = true;
			pauseImage.enabled = false;
			btn.targetGraphic = runImage;
		}
		flag = !flag;
	}
}

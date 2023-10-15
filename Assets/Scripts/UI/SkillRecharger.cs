using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillRecharger : MonoBehaviour
{
	public GameObject RechargingObject;

	private float chargingTime;
	private float currentTime;
	private bool recharging;

	private Image image;

	void Start()
	{
		image = RechargingObject.GetComponent<Image>();
		RechargingObject.SetActive(false);
	}

	public void ChargeSkill(float chargingTime)
	{
		RechargingObject.SetActive(true);
		this.chargingTime = chargingTime;
		recharging = true;
	}

    public void Update()
    {
		if (!recharging)
			return;

		currentTime += Time.deltaTime;
		float percentage = currentTime / chargingTime;
		float lerp = Mathf.Lerp(1, 0, percentage);
		image.fillAmount = lerp;

		if(lerp <= 0)
		{
			RechargingObject.SetActive(false);
			recharging = false;
			currentTime = 0;
		}
    }
}


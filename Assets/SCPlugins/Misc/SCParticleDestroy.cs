using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]

public class SCParticleDestroy : MonoBehaviour {
	public bool OnlyDeactivate;
	public bool ActivatePlayOnAwake = false;

	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}

	IEnumerator CheckIfAlive ()
	{
		ParticleSystem ps = this.GetComponent<ParticleSystem>();
		
		while(true && ps != null)
		{
			yield return new WaitForSeconds(0.5f);
			if(!ps.IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					this.gameObject.SetActive(false);
					if (ActivatePlayOnAwake) {
						AudioSource aSource = GetComponent<AudioSource>();
						if (aSource != null) {
							aSource.playOnAwake = true;
						}
					}
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}

}

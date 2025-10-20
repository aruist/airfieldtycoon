// UI Pack : Toony PRO
// Version: 1.0.5
// Author: Gold Experience Team (http://ge-team.com/pages/unity-3d/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to support e-mail (geteamdev@gmail.com)
#if UI_PACK
#region Namespaces

	using UnityEngine;
	using System.Collections;

#endregion

// ######################################################################
// This class is Singleton pattern class.
// It creates ParticleSystem object in the scene when CreateParticle function is called.
// ######################################################################

public class ParticleController : MonoBehaviour
{
	#region Variables

		// Private reference which can be accessed by this class only
		private static ParticleController instance;

		// Public static reference that can be accesd from anywhere
		public static ParticleController Instance
		{
			get
			{
				// Check if instance has not been set yet and set it it is not set already
				// This takes place only on the first time usage of this reference
				if(instance==null)
				{
					instance = GameObject.FindObjectOfType<ParticleController>();
					DontDestroyOnLoad(instance.gameObject);
				}
				return instance;
			}
		}

		// ParticleSystem prefabs
		public ParticleSystem	m_PrefabButton_01				= null;
		public ParticleSystem	m_PrefabButton_02				= null;
		public ParticleSystem	m_PrefabUseItem					= null;
		public ParticleSystem	m_PrefabFullFIll				= null;

	#endregion //Variables

	// ######################################################################
	// MonoBehaviour functions
	// ######################################################################

	#region MonoBehaviour functions

		// Awake is called when the script instance is being loaded.
		void Awake()
		{
			if(instance == null)
			{
				// Make the current instance as the singleton
				instance = this;

				// Make it persistent
				DontDestroyOnLoad(this);
			}
			else
			{
				// If more than one singleton exists in the scene find the existing reference from the scene and destroy it
				if(this != instance)
				{
					Destroy(this.gameObject);
				}
			}
		}

		// Use this for initialization
		void Start ()
		{
		}

	#endregion //MonoBehaviour functions

	// ######################################################################
	// Functions
	// ######################################################################

	#region Functions

		// Create particle on a UI object
		public void CreateParticle(GameObject goParent, ParticleSystem goOriginal)
		{
        /*
			// Instantiate a ParticleSystem object
			ParticleSystem pParticle = (ParticleSystem) Instantiate(goOriginal);
			if(pParticle!=null)
			{
				// Set parent
				pParticle.transform.SetParent(goParent.transform);

				// Set local position
				pParticle.transform.localPosition = new Vector3(0,0,-1.0f);
				if(pParticle.playOnAwake==false)
				{
					// Play particle
					pParticle.Play();
				}

				// Destroy when it finished
				Destroy(pParticle.gameObject, pParticle.duration + 1.5f);
			}*/
		}

	#endregion //Utilitie functions
}
#endif
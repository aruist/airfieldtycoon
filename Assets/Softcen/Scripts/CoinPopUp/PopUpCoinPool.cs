using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUpCoinPool : MonoBehaviour {
	public GameObject pooledObject;
	public int pooledAmount = 10;
	public Transform trCoinTarget;

	public int maxCount;
	
	protected List<PopupCoin> pooledObjects;
	private int i1;
	
	
	virtual protected void Start () {
		//Debug.Log("ObjectPooler Start " + gameObject.name);
		maxCount = 0;
		pooledObjects = new List<PopupCoin>();
		CreatePool();
	}
	
	private void CreatePool()
	{
		for (int i=0; i < pooledAmount; i++) {
			GameObject go = (GameObject)Instantiate (pooledObject);
			go.transform.SetParent(this.transform, false);
			PopupCoin pc = go.GetComponent<PopupCoin>();
			pc.popUpCoinPool = this;
			pc.trTarget1 = trCoinTarget;
			pooledObjects.Add(pc);
            pc.startInit = true;
            pc.gameObject.transform.position = new Vector3(100, 100, 0);
            pc.gameObject.SetActive(true);
		}
		maxCount = Mathf.Max(maxCount, pooledObjects.Count);
		
	}

	public void AddToList(PopupCoin pc) {
		pooledObjects.Add(pc);
		maxCount = Mathf.Max(maxCount, pooledObjects.Count);
	}

	public void ActivateCoin(Vector3 pos, double money) {
		PopupCoin pc;
		int index = pooledObjects.Count-1;
		if (index >= 0) {
			pc = pooledObjects[index];
			pooledObjects.RemoveAt(index);
		}
		else {
			GameObject go = (GameObject)Instantiate (pooledObject);
			pc = go.GetComponent<PopupCoin>();
			pc.popUpCoinPool = this;
			pc.trTarget1 = trCoinTarget;
		}
		if (pc != null) {
			pc.ActivateCoin(pos, money);
		}
	}
	
	/*
	public GameObject GetPooledObject()
	{
		if (pooledObjects == null) {
			pooledObjects = new List<GameObject>();
		}
		for (i1=0; i1 < pooledObjects.Count; i1++) {
			if (!pooledObjects[i1].activeSelf) {
				return pooledObjects[i1];
			}
		}
		
		if (willGrow) {
			GameObject go = (GameObject)Instantiate(pooledObject);
			go.transform.SetParent(this.transform, false);
			pooledObjects.Add(go);
			maxCount = Mathf.Max(maxCount, pooledObjects.Count);
			return go;
		}
		
		return null;
	}
	
	virtual public void DisableAllObjects() {
		if (pooledObjects == null) {
			pooledObjects = new List<GameObject>();
		}
		for (int i=0; i < pooledObjects.Count; i++) {
			if (pooledObjects[i] != null) {
				if (pooledObjects[i].activeSelf) {
					pooledObjects[i].SetActive(false);
				}
			}
			
		}
	}*/
	public void ClearPool()
	{
		//Debug.Log("ClearPool " + gameObject.name + ", count: " + pooledObjects.Count);
		if (pooledObjects != null)
		{
			for (int i=0; i < pooledObjects.Count; i++)
			{
				if (pooledObjects[i] != null)
				{
					Destroy(pooledObjects[i].gameObject);
				}
			}
			pooledObjects.Clear();
		}
	}
	
	public void InitPool(GameObject go, int startCount)
	{
		pooledObject = go;
		pooledAmount = startCount;
		// Clear old ones
		ClearPool();
		CreatePool();
	}
}

using UnityEngine;
using System.Collections;

public class EventData {
	public int Id;
	public string Name;
	public string Description;

	public double Value;
	public float Duration;
    public int Timantit;

	public EventManager.boostType Type;

    public EventData(int id, EventManager.boostType type, string name, string desc, double value, float duration, int t) {
		Id = id;
		Type = type;
		Name = name;
		Description = desc;
		Value = value;
		Duration = duration;
        Timantit = t;
	}

}

using System;
using UnityEngine;

[Serializable]
public class ArKKu {
    [SerializeField]
    private bool _lukittu = true;
    public bool Lukittu {
        get { return _lukittu; }
        set { _lukittu = value; }
    }
	public ArKKu() {
        _tyyppi = ArKKuTyyPPi.tyyppi.TYHJA;
	}
    public ArKKu(ArKKuTyyPPi.tyyppi t) {
		_tyyppi = t;
        KaynnistaAika ();
	}
    public ArKKu(ArKKuTyyPPi.tyyppi t, long a) {
        _tyyppi = t;
        _avaamisAika = a;
    }

    public void asetaAika(long a) {
        //Debug.Log ("ArKKu asetaAika " + a);
        _avaamisAika = a;
    }

    public String aika() {
        // 1d 12h 17min
        // 23d 13h 59min
        // 1h 32min
        // 21min 32sec
        String ret;
        long ero = _avaamisAika - DateTime.UtcNow.Ticks;
        if (ero > 0) {
            ts = TimeSpan.FromTicks (ero);
            if (ts.Days > 0) {
                ret = ts.Days.ToString () + "d " + ts.Hours + "h " + ts.Minutes + "m";    
            } else if (ts.Hours > 0) {
                ret = ts.Hours + "h " + ts.Minutes + "m"; 
            } else if (ts.Minutes > 0) {
                ret = ts.Minutes + "m " + ts.Seconds + "s";
            } else {
                ret = ts.Seconds + "s";
            }
        } else {
            return "Open";
        }
        return ret;
    }
    [SerializeField]
    private ArKKuTyyPPi.tyyppi _tyyppi = ArKKuTyyPPi.tyyppi.TYHJA;
    public ArKKuTyyPPi.tyyppi Tyyppi {
		get { return _tyyppi; }
		set { _tyyppi = value; }
	}

	private TimeSpan ts = new TimeSpan();

	//long _currentTicks = DateTime.UtcNow.Ticks;
	long _avaamisAika;
	public long AvaamisAika {
		get { return _avaamisAika; }
		set { _avaamisAika = value; }
	}

	public void KaynnistaAika() {
        _avaamisAika = DateTime.UtcNow.Ticks + ArKKuTyyPPi.aika (_tyyppi);
	}

	public bool voikoAvata() {
        //Debug.Log ("voikoAvata utc: " + DateTime.UtcNow.Ticks + ", aa: " + _avaamisAika + ", compare: " + (DateTime.UtcNow.Ticks >= _avaamisAika) );
		return DateTime.UtcNow.Ticks >= _avaamisAika;
	}

	public long aikaaJaljella() {
		return _avaamisAika - DateTime.UtcNow.Ticks;
	}

	public long aikaaJaljellaMinuutteina() {
		long ero = _avaamisAika - DateTime.UtcNow.Ticks;
		ts = TimeSpan.FromTicks (ero);
		return (ts.Days * 1440) + (ts.Hours * 60) + ts.Minutes;
	}

	public long aikaaJaljellaSekuntteina() {
		long ero = _avaamisAika - DateTime.UtcNow.Ticks;
		ts = TimeSpan.FromTicks (ero);
		return (ts.Days * 86400) + (ts.Hours * 3600) + (ts.Minutes * 60) + ts.Seconds;
	}
		
}

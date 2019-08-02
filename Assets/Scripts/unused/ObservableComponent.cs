using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ObservableComponent : MonoBehaviour {
    List<Observer> observers = new List<Observer>();
    
    public void NotifySubscribers() {
        foreach(Observer o in observers) {
            o.ObserverUpdate();
        }
    }
        
    public void AddObserver(Observer observer) {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer) {
        observers.Remove(observer);
    }
}
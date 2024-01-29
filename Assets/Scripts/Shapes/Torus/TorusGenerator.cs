using UnityEngine;

public class TorusGenerator : MonoBehaviour {

    /**
     *
     */
    public Torus CreateTorus(string torusName) {
        GameObject torusGameObject = new(torusName);
        Torus torus = torusGameObject.AddComponent<Torus>();
        torus.UpdateTorus();
        return torus;
    }
}
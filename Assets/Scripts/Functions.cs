using System.Collections;
using UnityEngine;

public class Functions : MonoBehaviour {
    // private static Functions instance;

    // void Start() {
    //     instance = this;
    // }

    // public static void Stop(MonoBehaviour script) {
    //     script.StopAllCoroutines();
    // }

    public static Quaternion GetQuaternionFromVectors(Vector3 a, Vector3 b) {
        return Quaternion.LookRotation(a - b, Vector3.up);
    }

    public static void LerpTurn(MonoBehaviour script, Quaternion startRotation, Quaternion targetRotation, float turnDuration) {
        script.StartCoroutine(QuaternionLerp(startRotation, targetRotation, turnDuration));
    }

    private static IEnumerator QuaternionLerp(Quaternion startRotation, Quaternion targetDuration, float turnDuration) {
        float timestamp = Time.time;
        float timeTaken = 0;
        
        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            // tf.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
    }

    public static void LerpMove() { 

    }
}
using UnityEngine;

public class Functions {

    public static Quaternion GetQuaternionFromVectors(Vector3 a, Vector3 b) {
        return Quaternion.LookRotation(a - b, Vector3.up);
    }

    public static void TurnTransform(Transform tf, Quaternion targetRotation, float turnDuration) {
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = tf.rotation;

        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            // yield return new WaitForFixedUpdate();
            tf.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
    }
}
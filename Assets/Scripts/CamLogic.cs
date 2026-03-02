using NUnit.Framework.Constraints;
using UnityEngine;

public class CamLogic : MonoBehaviour
{
    public Transform playerTranform;

    public float fixedDistance;
    public float height;
    public float heightFactor;
    public float rotationFactor;

    float startRotationAngle; // This camera's rotation angle around the y-axis.
    float endRotationAngle; // The player's rotation angle around the y-axis.
    float finalRotationAngle; // The final smoothed out(interpolated) rotation angle of the camera around the y-axis.

    float currentHeight;
    float wantedHeight;

    // Update is called once per frame
    void LateUpdate()
    {
        currentHeight = this.transform.position.y;
        wantedHeight = playerTranform.position.y + height;
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightFactor * Time.deltaTime);

        startRotationAngle = this.transform.eulerAngles.y;
        endRotationAngle = playerTranform.eulerAngles.y;
        finalRotationAngle = Mathf.LerpAngle(startRotationAngle, endRotationAngle, Time.deltaTime * rotationFactor);

        Quaternion finalRotation = Quaternion.Euler(0, finalRotationAngle, 0); // Convert angle value into actual rotation.
        this.transform.position = playerTranform.position;
        this.transform.position -= finalRotation * Vector3.forward * fixedDistance; // Same as: new Vector3(sin(finalRotationAngle) * fixedDistance, 0, cos(finalRotationAngle) * fixedDistance).

        this.transform.position = new Vector3(this.transform.position.x, currentHeight, this.transform.position.z);
        this.transform.LookAt(playerTranform);
    }
}

using UnityEngine;

public class Algorithm
{
    public Quaternion Qs2w = Quaternion.identity; //quaternion of transformation sensor -> world basis
    public Quaternion Q0 = Quaternion.identity; //default quaternion of rotation

    private (Vector3 A, float gamma) GetAxisAngleFrom(Quaternion q)
    {
        float cosHalfGamma = q.w; //by definition of quaternion
        float cosGamma = (2.0F * Mathf.Pow(cosHalfGamma, 2.0F)) - 1.0F; //by trigonometry formulas of half/double angles - cos(x)=(2*(cos(x/2)^2))–1
        float sinGamma = Mathf.Sqrt(1.0F - Mathf.Pow(cosGamma, 2.0F)); //by trigonometry law - (sin(x)^2)+(cos(x)^2)=1
        float gamma = Mathf.Atan2(sinGamma, cosGamma); // this is most proper way to calculate gamma!
        float sinHalfGamma = Mathf.Sin(gamma / 2.0F);
        Vector3 A = Vector3.Normalize(new Vector3(q.x / sinHalfGamma, q.y / sinHalfGamma, q.z / sinHalfGamma));  //by definition of quaternion
        return (A, gamma* Mathf.Rad2Deg % 180.0F);
    }

    private Vector3 S2W(Vector3 v)
    {
        return Qs2w * v;
    }

    public (Vector3, float) CalculateRotation(Vector3 N, Vector3 G)
    {
        //sensor axis
        Vector3 Xs = Vector3.right; //(1,0,0), sensor X axis
        Vector3 Ys = Vector3.up; //(0,1,0), sensor Y axis

        Vector3 Xw = N; //world X axis (to "magnetic north")
        Vector3 Yw = -G; //world Y axis (inversed gravity direction)

        Vector3 H = Vector3.Normalize(Vector3.ProjectOnPlane(Xw + Yw, Ys)); //"handle" vector

        Quaternion Q1 = Quaternion.Normalize(Quaternion.FromToRotation(S2W(Xs), S2W(H)));  //current quaternion of rotation
        Quaternion dQ = Q1 * Quaternion.Inverse(Q0);  //quaternion of difference

        (Vector3 axis, float angle) = GetAxisAngleFrom(dQ); //axis of rotation and current angle

        return (axis,angle);
    }

    public void Calibrate(Vector3 N, Vector3 G)
    {
        //sensor axis
        Vector3 Xs = Vector3.right; //(1,0,0), sensor X axis
        Vector3 Ys = Vector3.up; //(0,1,0), sensor Y axis
        Vector3 Zs = Vector3.forward; //(0,0,1), sensor Z axis
        Vector3 Cs = Vector3.one; //(1,1,1), "constant" axis, rotation around which is less probable

        Quaternion Qc = Quaternion.Normalize(Quaternion.FromToRotation(Zs, Cs)); //Quaternion of rotation from sensor Z axis to "constant" axis

        //world axis
        Vector3 Xw = N; //world X axis (to "magnetic north")
        Vector3 Yw = -G; //world Y axis (inversed gravity direction)
        Vector3 Zw = Vector3.Normalize(Vector3.Cross(Xw, Yw)); //world Z axis

        Vector3 Cw = Qc * Zw; //world "constant" axis

        Qs2w = Quaternion.Normalize(Quaternion.FromToRotation(Cs, Cw)); //quaternion of transformation sensor -> world basis

        Vector3 H = Vector3.Normalize(Vector3.ProjectOnPlane(Xw + Yw, Ys)); //"handle" vector

        Q0 = Quaternion.Normalize(Quaternion.FromToRotation(S2W(Xs), S2W(H))); //default quaternion of rotation
    }
}
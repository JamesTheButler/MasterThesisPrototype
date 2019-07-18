using UnityEngine;

public class SimpleMovement : MonoBehaviour {
    public float power;
	
	// Update is called once per frame
	void Update () {
        Vector3 inputVector = new Vector3();
        if (Input.GetKey(KeyCode.UpArrow))
            inputVector += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.DownArrow))
            inputVector += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.RightArrow))
            inputVector += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            inputVector += new Vector3(-1, 0, 0);

        transform.position += Vector3.Normalize(inputVector) * power * Time.deltaTime;
    }
}

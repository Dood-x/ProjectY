using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public float size = 0.7f;

    public void ButtonPressed()
    {
        transform.localScale = new Vector3(size, size, size);
        StartCoroutine(ScaleButton());
    }

    public IEnumerator ScaleButton()
    {
        do
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.1f);
        } while (transform.localScale.x < 1f);

    }
}

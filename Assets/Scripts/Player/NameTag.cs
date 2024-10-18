using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    public Transform target; // El transform del personaje
    public Vector3 offset;   // Offset para la posición del nombre

    private Text nameText;

    void Start()
    {
        nameText = GetComponent<Text>();
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);
            transform.position = screenPosition;
        }
    }

    public void SetName(string name)
    {
        if (nameText != null)
        {
            nameText.text = name;
        }
    }
}

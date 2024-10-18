using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{

    public Transform puntoInicial;
    public GameObject jpersonaje;

    // Start is called before the first frame update
    void Start()
    {
        jpersonaje = GameObject.FindGameObjectWithTag("Player");
        puntoInicial = GameObject.FindGameObjectWithTag("PuntoInicial").transform;
        MoverAPuntoInicial();

        
    }

    // Update is called once per frame
    void Update()
    {
        ProbarCambioDeEscena();
    }

    public void MoverAPuntoInicial(){
        jpersonaje.transform.position = puntoInicial.position;
    }


    public void ProbarCambioDeEscena()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}

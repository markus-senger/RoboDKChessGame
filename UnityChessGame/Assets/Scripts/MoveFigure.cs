using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFigure : MonoBehaviour
{
    [SerializeField]
    private GameObject sceneHandler;

    [SerializeField]
    public GameObject currentField;

    private Color orgColor;
    private MoveHandler moveHandler;
    private bool mouseOver;

    private TCPIPClient tcpIPClient;

    private void Awake()
    {
        orgColor = GetComponent<Renderer>().material.color;
        moveHandler = sceneHandler.GetComponent<MoveHandler>();
        currentField.GetComponent<PlaceField>().currentFigureOnField = gameObject.transform.parent.gameObject;
        tcpIPClient = sceneHandler.GetComponent<TCPIPClient>();
    }

    private void OnMouseOver()
    {
        if (tcpIPClient.serverReady)
        {
            mouseOver = true;
            GetComponent<Renderer>().material.color = Color.yellow;

            if (Input.GetMouseButtonDown(0))
            {
                moveHandler.isMoving = true;
                moveHandler.movingFigure = gameObject;
            }
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }

    private void Update()
    {
        if(moveHandler.movingFigure != gameObject && !mouseOver)
            GetComponent<Renderer>().material.color = orgColor;
    }
}
